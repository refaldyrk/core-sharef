using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreSharef.Models;

public class Cache
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Code { get; set; }
}