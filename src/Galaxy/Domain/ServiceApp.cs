﻿using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class ServiceApp
    {
        public ObjectId Id { get; internal set; }
        public string AssemblyQualifiedType { get; set; }
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string FeedName { get; set; }
        public ObjectId FeedId { get; internal set; }
        public string PackageName { get; set; }
        public string Status { get; set; }
    }
}
