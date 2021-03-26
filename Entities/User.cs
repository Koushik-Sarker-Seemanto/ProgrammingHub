using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities
{
    [Serializable]
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonRequired]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string ProfileImage { get; set; }
        [BsonRequired]
        public string Username { get; set; }
        [BsonRequired]
        public string Password { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
