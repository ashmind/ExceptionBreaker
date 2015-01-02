using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using ExceptionBreaker.Breakpoints;
using ExceptionBreaker.Core;
using ExceptionBreaker.Core.VersionSpecific;
using ExceptionBreaker.Options;
using ExceptionBreaker.Toolbar;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.Debugging)]
    [ProvideMenuResource("Menus.2012.ctmenu", 1)]
    [ProvideOptionPageInExistingCategory(typeof(OptionsPageData), "Debugger", "ExceptionBreaker", 110)]
    [Guid(GuidList.PackageString)]
    public sealed class ExceptionBreakerPackage : Package {
        private DTE _dte;
        private ToggleBreakOnAllController _toolbarController;
        private BreakpointSetupExceptionsController _breakpointController;
        private BreakpointEventProcessor _breakpointEventProcessor;

        public IDiagnosticLogger Logger { get; private set; }
        public ExceptionBreakManager ExceptionBreakManager { get; private set; }

        public static ExceptionBreakerPackage Current { get; private set; }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ExceptionBreakerPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
            Current = this;
        }
        
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();
            Logger = new ExtensionLogger("ExceptionBreaker", paneCaption => GetOutputPane(GuidList.OutputPane, paneCaption));

            _dte = (DTE)GetService(typeof(DTE));
            SetupCoreManager();

            SetupToolbar();
            SetupBreakpoints();
        }

        private void SetupCoreManager() {
            var versionSpecificFactory = new VersionSpecificAdapterFactory(_dte);
            var debugger = GetDebugger();
            var sessionManager = new DebugSessionManager(versionSpecificFactory.AdaptDebuggerInternal(debugger), Logger);
            var optionsPage = new Lazy<OptionsPageData>(() => (OptionsPageData)GetDialogPage(typeof (OptionsPageData)));
            ExceptionBreakManager = new ExceptionBreakManager(
                sessionManager,
                name => optionsPage.Value.Ignored.Any(p => p.Matches(name)),
                Logger
            );
        }

        private void SetupToolbar() {
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var monitorSelection = (IVsMonitorSelection)GetService(typeof(IVsMonitorSelection));
            _toolbarController = new ToggleBreakOnAllController(
                _dte,
                new CommandInitializer(CommandIDs.ToggleBreakOnAll, menuCommandService),
                monitorSelection,
                ExceptionBreakManager,
                Logger
            );
        }

        private void SetupBreakpoints() {
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var debugger = GetDebugger();
            var store = new ExceptionBreakChangeStore(Logger);
            _breakpointEventProcessor = new BreakpointEventProcessor(debugger, store, ExceptionBreakManager, Logger);
            _breakpointController = new BreakpointSetupExceptionsController(
                new CommandInitializer(CommandIDs.BreakpointToggleExceptions, menuCommandService),
                new BreakpointFinder(_dte),
                store,
                Logger
            );
        }

        private static IVsDebugger GetDebugger() {
            return (IVsDebugger)GetGlobalService(typeof(SVsShellDebugger));
        }
    }
}
