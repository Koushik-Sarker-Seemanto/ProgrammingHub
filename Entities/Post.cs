using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Entities
{
    public class Post
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string Username { get; set; }

        [BsonRequired]
        public string Title { get; set; }
        [BsonRequired]
        public string PostType { get; set; }
        public string ExpertLevel { get; set; }
        public string PostCoverBig { get; set; }
        public string PostCoverSmall { get; set; }
        [BsonRequired]
        public string Description { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<Comment> Comments { get; set; }

    }
}