using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson;

namespace Entities
{
    public class Comment
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonRequired]
        public Guid UserId { get; set; }
        [BsonRequired]
        public string Username { get; set; }
        [BsonRequired]
        public string Description { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}