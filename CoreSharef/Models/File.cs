using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreSharef.Models;

public class File
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string? Name { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public string? Code { get; set; }
}