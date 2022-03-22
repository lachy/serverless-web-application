using MongoDB.Bson.Serialization.Attributes;

namespace ToDoFunctionApp.Models;

public class TodoItem
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("owner")]
    public string Owner { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("status")]
    public bool Status { get; set; }
}