using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker.Core {
    public class CommandInitializer {
        private readonly CommandID _commandId;
        private readonly IMenuCommandService _menuCommandService;

        public CommandInitializer(CommandID commandId, IMenuCommandService menuCommandService) {
            _commandId = commandId;
            _menuCommandService = menuCommandService;
        }

        // a bit complicated, but the idea is that I can not have callback until I am in the constructor
        // of a command Controller, and command Controller constructors should require command (or a way to get one).
        //
        // also I do not want to let Controllers create command themselves, which is probably just my architectural
        // inertia
        public MenuCommand InitializeCommand(EventHandler callback, EventHandler beforeQueryStatus = null) {
            var command = new OleMenuCommand(id: _commandId, invokeHandler: callback, beforeQueryStatus: beforeQueryStatus, changeHandler: null);
            _menuCommandService.AddCommand(command);

            return command;
        }
    }
}
