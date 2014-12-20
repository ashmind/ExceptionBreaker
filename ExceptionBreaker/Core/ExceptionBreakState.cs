using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceptionBreaker.Core {
    public enum ExceptionBreakState {
        /// <summary>
        /// Either the session was not available yet, or the session does not have any exceptions *set*,
        /// whatever this means (not the same thing as *set to do not break*).
        /// </summary>
        Unknown,

        /// <summary>
        /// Some exception breaks are set, but not on all of them. This state is inferred, but can not be
        /// set by the user.
        /// </summary>
        Inconclusive,

        /// <summary>
        /// Break on all managed exceptions. This state can be set by the user.
        /// </summary>
        BreakOnAll,

        /// <summary>
        /// Do not break on any managed exceptions. This state can be st by the user.
        /// </summary>
        BreakOnNone
    }
}
