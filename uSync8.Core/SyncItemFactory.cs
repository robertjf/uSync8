﻿using System.Collections.Generic;
using System.Xml.Linq;

using uSync8.Core.Cache;
using uSync8.Core.Dependency;
using uSync8.Core.Models;
using uSync8.Core.Serialization;
using uSync8.Core.Tracking;

namespace uSync8.Core
{
    public class SyncItemFactory : ISyncItemFactory
    {
        private readonly SyncTrackerCollection syncTrackers;
        private readonly SyncDependencyCollection syncCheckers;

        private readonly SyncEntityCache entityCache;


        public SyncItemFactory(
            SyncEntityCache entityCache,
            SyncTrackerCollection syncTrackers,
            SyncDependencyCollection syncCheckers)
        {
            this.syncTrackers = syncTrackers;
            this.syncCheckers = syncCheckers;
            this.entityCache = entityCache;
        }

        public SyncEntityCache EntityCache => entityCache;

        public IEnumerable<ISyncTracker<TObject>> GetTrackers<TObject>()
            => syncTrackers.GetTrackers<TObject>();

        public IEnumerable<uSyncChange> GetChanges<TObject>(XElement node, SyncSerializerOptions options)
            => syncTrackers.GetChanges<TObject>(node, options);
        public IEnumerable<uSyncChange> GetChanges<TObject>(XElement node, XElement currentNode, SyncSerializerOptions options)
        {
            if (currentNode == null)
                return syncTrackers.GetChanges<TObject>(node, options);
            else
                return syncTrackers.GetChanges<TObject>(node, currentNode, options);
        }


        public IEnumerable<ISyncDependencyChecker<TObject>> GetCheckers<TObject>()
            => syncCheckers.GetCheckers<TObject>();

        public IEnumerable<uSyncDependency> GetDependencies<TObject>(TObject item, DependencyFlags flags)
        {
            var dependencies = new List<uSyncDependency>();
            foreach (var checker in syncCheckers.GetCheckers<TObject>())
            {
                dependencies.AddRange(checker.GetDependencies(item, flags));
            }
            return dependencies;
        }
    }
}
