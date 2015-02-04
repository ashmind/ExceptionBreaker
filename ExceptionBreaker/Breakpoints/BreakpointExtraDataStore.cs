using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using EnvDTE80;
using ExceptionBreaker.Core;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Debugger.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExceptionBreaker.Breakpoints {
    [Export]
    public class BreakpointExtraDataStore : ISolutionDataPersister {
        private readonly BreakpointKeyProvider _keyProvider;
        private readonly BreakpointFinder _finder;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IDiagnosticLogger _logger;
        private readonly ConcurrentDictionary<string, BreakpointExtraData> _store = new ConcurrentDictionary<string, BreakpointExtraData>(StringComparer.InvariantCultureIgnoreCase);

        public event EventHandler<BreakpointExtraDataChangedEventArgs> DataChanged = delegate {};

        [ImportingConstructor]
        public BreakpointExtraDataStore(BreakpointKeyProvider keyProvider, BreakpointFinder finder, IDiagnosticLogger logger) {
            _keyProvider = keyProvider;
            _finder = finder;
            _jsonSerializer = new JsonSerializer {
                Converters = { new StringEnumConverter() },
                Formatting = Formatting.None
            };
            _logger = logger;
        }

        [NotNull]
        public BreakpointExtraData GetData([NotNull] Breakpoint2 breakpoint) {
            return GetData(_keyProvider.GetKey(breakpoint));
        }

        [CanBeNull]
        public BreakpointExtraData GetData([NotNull] IDebugBoundBreakpoint2 breakpoint) {
            var key = _keyProvider.GetKey(breakpoint);
            if (key == null)
                return null;

            return GetData(key);
        }

        [NotNull]
        private BreakpointExtraData GetData([NotNull] string key) {
            return _store.GetOrAdd(key, k => new BreakpointExtraData());
        }

        public void NotifyDataChanged([NotNull] Breakpoint2 breakpoint) {
            DataChanged(this, new BreakpointExtraDataChangedEventArgs(breakpoint));
        }

        string ISolutionDataPersister.Key {
            // due to VS limitation, this has to be shorted than 31 char and contain no '.'
            get { return "XB-BreakpointExtraData"; }
        }

        void ISolutionDataPersister.SaveTo(Stream stream) {
            _logger.WriteLine("Breakpoints: saving extra data.");
            var existingBreakpointKeys = _finder.GetAllBreakpoints().Select(b => _keyProvider.GetKey(b));
            foreach (var key in _store.Keys.Except(existingBreakpointKeys).ToArray()) {
                BreakpointExtraData _;
                _store.TryRemove(key, out _);
                _logger.WriteLine("  Skipped '{0}' — no corresponding breakpoint.", key);
            }

            using (var writer = new StreamWriter(stream)) {
                _jsonSerializer.Serialize(writer, _store.ToDictionary(p => p.Key, p => new BreakpointExtraDataSerializable(p.Value)));
            }
            _logger.WriteLine("  Saved {0} extra data entries to the solution.", _store.Count);
        }

        void ISolutionDataPersister.LoadFrom(Stream stream) {
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader)) {
                var loaded = _jsonSerializer.Deserialize<IDictionary<string, BreakpointExtraDataSerializable>>(jsonReader);
                _store.Clear();
                _logger.WriteLine("Breakpoints: loading extra data.");
                foreach (var pair in loaded) {
                    _store[pair.Key] = pair.Value.ToData();
                    _logger.WriteLine("  Loaded '{0}': change = {1}.", pair.Key, pair.Value.ExceptionBreakChange);
                }
            }
        }

        #region BreakpointExtraDataSerialized Class
        private class BreakpointExtraDataSerializable {
            public BreakpointExtraDataSerializable(BreakpointExtraData data) {
                Version = 1;
                ExceptionBreakChange = data.ExceptionBreakChange.Value;
            }

            public BreakpointExtraData ToData() {
                return new BreakpointExtraData {
                    ExceptionBreakChange = {Value = ExceptionBreakChange}
                };
            }

            public int Version { get; set; }
            public ExceptionBreakChange ExceptionBreakChange { get; set; }
        }
        #endregion
    }
}
