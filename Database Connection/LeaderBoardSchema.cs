using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class LeaderBoardSchema
{
    [BsonId] public string _id { get; set; }

    public BsonDocument list { get; set; }
}
