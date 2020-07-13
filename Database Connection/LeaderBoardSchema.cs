using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public struct LeaderBoardSchema
{
    [BsonId] string _id { get; set; }

    BsonDocument list { get; set; }
}
