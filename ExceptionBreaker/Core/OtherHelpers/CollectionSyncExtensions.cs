using System;
using System.Collections.Generic;

namespace ExceptionBreaker.Core.OtherHelpers {
    public static class CollectionSyncExtensions {
        public static void SyncToWhere<T>(this IList<T> source, IList<T> target, Func<T, bool> predicate) {
            var offset = 0;
            for (var i = 0; i < target.Count; i++) {
                var targetItem = target[i];
                var shouldContinue = false;
                while (!Equals(source[i + offset], targetItem)) {
                    var notPreviouslyMatched = source[i + offset];
                    if (predicate(notPreviouslyMatched)) {
                        target.Insert(i, notPreviouslyMatched);
                        shouldContinue = true;
                        break;
                    }

                    offset += 1;
                }

                if (shouldContinue)
                    continue;

                if (!predicate(targetItem)) {
                    target.RemoveAt(i);
                    i -= 1;
                    offset += 1;
                }
            }

            for (var i = target.Count; i < source.Count - offset; i++) {
                var sourceItem = source[i + offset];
                if (predicate(sourceItem))
                    target.Add(sourceItem);
            }
        }
    }
}
