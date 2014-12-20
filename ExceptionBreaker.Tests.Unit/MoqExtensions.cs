using System;
using System.Collections.Generic;
using System.Reflection;
using Moq.Language;
using Moq.Language.Flow;

namespace ExceptionBreaker.Tests.Unit {
    public static class MoqExtensions {
        public delegate void ActionOut2<T1, T2>(T1 arg1, out T2 arg2);

        public static IReturnsThrows<TMock, TReturn> Callback<T1, T2, TMock, TReturn>(this ICallback<TMock, TReturn> mock, ActionOut2<T1, T2> action) 
            where TMock : class
        {
            return mock.Callback((Delegate)action);
        }

        public static IReturnsThrows<TMock, TReturn> Callback<TMock, TReturn>(this ICallback<TMock, TReturn> mock, Delegate @delegate)
            where TMock : class {
            mock.GetType()
                .Assembly.GetType("Moq.MethodCall")
                .InvokeMember("SetCallbackWithArguments",
                    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, mock, new object[] { @delegate }
                );
            return (IReturnsThrows<TMock, TReturn>)mock;
        }
    }
}
