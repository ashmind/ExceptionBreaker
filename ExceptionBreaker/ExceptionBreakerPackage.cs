using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using ExceptionBreaker.Breakpoints;
using ExceptionBreaker.Core;
using ExceptionBreaker.Core.VersionSpecific;
using ExceptionBreaker.Options;
using ExceptionBreaker.Toolbar;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker {
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
        // ReSharper disable NotAccessedField.Local
        private ToggleBreakOnAllController _toolbarController;
        private BreakpointSetupExceptionsController _breakpointController;
        private BreakpointEventProcessor _breakpointEventProcessor;
        // ReSharper restore NotAccessedField.Local

        private readonly SolutionDataPersisterCollection _solutionDataPersisters = new SolutionDataPersisterCollection();

        public ExportProvider Mef { get; private set; }
        public ExceptionBreakManager ExceptionBreakManager { get; private set; }
        public IDiagnosticLogger Logger { get; private set; }

        public static ExceptionBreakerPackage Current { get; private set; }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ExceptionBreakerPackage() {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
            Current = this;
        }
        
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();

            Mef = ((IComponentModel)GetGlobalService(typeof(SComponentModel))).DefaultExportProvider;

            Logger = Mef.GetExportedValue<IDiagnosticLogger>();
            _dte = Mef.GetExportedValue<DTE>();
            SetupCoreManager();

            SetupToolbar();
            SetupBreakpoints();

            foreach (var persister in _solutionDataPersisters) {
                AddOptionKey(persister.Key);
            }
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

            var finder = Mef.GetExportedValue<BreakpointFinder>();
            var extraDataStore = Mef.GetExportedValue<BreakpointExtraDataStore>();
            _solutionDataPersisters.Add(extraDataStore);

            _breakpointEventProcessor = new BreakpointEventProcessor(debugger, extraDataStore, ExceptionBreakManager, Logger);
            _breakpointController = new BreakpointSetupExceptionsController(
                new CommandInitializer(CommandIDs.BreakpointToggleExceptions, menuCommandService),
                finder, extraDataStore, Logger
            );
        }

        private static IVsDebugger GetDebugger() {
            return (IVsDebugger)GetGlobalService(typeof(SVsShellDebugger));
        }

        protected override void OnLoadOptions(string key, Stream stream) {
            try {
                _solutionDataPersisters[key].LoadFrom(stream);
            }
            catch (Exception ex) {
                Logger.WriteLine("Exception while loading solution data for '{0}': {1}", key, ex);
            }
        }

        protected override void OnSaveOptions(string key, Stream stream) {
            try {
                _solutionDataPersisters[key].SaveTo(stream);
            }
            catch (Exception ex) {
                Logger.WriteLine("Exception while saving solution data for '{0}': {1}", key, ex);
            }
        }

        #region SolutionDataPersisterCollection class
        private class SolutionDataPersisterCollection : KeyedCollection<string, ISolutionDataPersister> {
            protected override string GetKeyForItem(ISolutionDataPersister item) {
                return item.Key;
            }
        }
        #endregion
    }
}
