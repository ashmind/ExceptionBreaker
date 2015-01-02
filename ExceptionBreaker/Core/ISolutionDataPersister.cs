using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExceptionBreaker.Core {
    public interface ISolutionDataPersister {
        string Key { get; }
        void SaveTo(Stream stream);
        void LoadFrom(Stream stream);
    }
}
