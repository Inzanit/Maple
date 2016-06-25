﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace InsireBotCore
{
    public static class LinqExtensions
    {
        public static T Random<T>(this IEnumerable<T> items)
        {
            if (items?.Any() == false)
                throw new ArgumentNullException("Random can't generate a result. There were no valid parameters", nameof(items));


            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new Random();
            var list = items as IList<T> ?? items.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }

        /// <summary>
        /// determines if some of the submitted items can be added to the collection and returns a collection of these items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<IIdentifier> CanAddRange(this IList<IIdentifier> baseCollection, IList<IIdentifier> newItems)
        {
            if (newItems?.Any() != true)
                throw new ArgumentNullException("CanAddRange can't return a result. There were no valid parameters", nameof(newItems));

            if (baseCollection == null)
                throw new ArgumentNullException("CanAddRange can't return a result. There were no valid parameters", nameof(baseCollection));

            var excludedIDs = new HashSet<Guid>(baseCollection.Select(p => p.ID));
            return newItems.Where(p => !excludedIDs.Contains(p.ID));
        }

        /// <summary>
        /// determines if some of the submitted items can be added to the collection and returns a collection of these items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<IMediaItem> CanAddRange(this IList<IMediaItem> baseCollection, IList<IMediaItem> newItems)
        {
            if (newItems?.Any() != true)
                throw new ArgumentNullException("CanAddRange can't return a result. There were no valid parameters", nameof(newItems));

            if (baseCollection == null)
                throw new ArgumentNullException("CanAddRange can't return a result. There were no valid parameters", nameof(baseCollection));

            var excludedIDs = new HashSet<Guid>(baseCollection.Select(p => p.ID));
            return newItems.Where(p => !excludedIDs.Contains(p.ID));
        }
    }
}
