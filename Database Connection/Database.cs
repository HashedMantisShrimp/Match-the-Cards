using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
//TODO: Find way to install mongodb.legacy.dll & DotEnv dll/plugin files

public class Database
{
    internal static bool connectedToServer { get; private set; } = false; //TODO: If no use is found, delete it
    private const string dbName = SData.DB_NAME; //TODO: Substitute later with actual DotEnv files
    private const string dbUser = SData.DB_USER;
    private const string dbPass = SData.DB_PASS;
    private const string dbCluster = SData.DB_CLUSTER;
    private const string dbLPort = SData.DB_LOCAL_PORT;
    private const string lbDocID = "lb00";
    private static string lbColl = "leaderboard";
    private static string gameStateColl = "gamestate";
    private static IMongoDatabase db;
    private static IMongoCollection<LeaderBoardSchema> lbCollection; //TODO: Create a Schema/Model script for collections
    private static IMongoCollection<BsonDocument> gameStateCollection;
    private static string remoteDB = $"mongodb+srv://{dbUser}:{dbPass}@{dbCluster}.mongodb.net/test?retryWrites=true&w=majority";
    private static string localDB = $"mongodb://127.0.0.1:{dbLPort}";

    #region Init Functions
    static MongoClient newClient = new MongoClient(localDB);
    private static Database instance = new Database();

    private Database()
    {
        db = newClient.GetDatabase(dbName);
        lbCollection = db.GetCollection<LeaderBoardSchema>(lbColl);
        gameStateCollection = db.GetCollection<BsonDocument>(gameStateColl);
        connectedToServer = true;
    }

    internal static Database GetInstance()
    {
        return instance;
    }
    #endregion

    #region LeaderBoard
    internal static async Task SaveLeaderBoard(string playerListJSON)
    {
        var data = BsonDocument.Parse(playerListJSON);

        var filter = Builders<LeaderBoardSchema>.Filter.Eq("_id", lbDocID);
        var update = Builders<LeaderBoardSchema>.Update.Set("list", data);

        var options = new UpdateOptions { IsUpsert = true };

        UpdateResult result = await lbCollection.UpdateOneAsync(filter, update, options);

        Debug.Log($"<color=yellow> Operation {nameof(lbCollection.UpdateOneAsync)}()</color> returned result: {result}");
        Debug.Log($"<color=yellow>{nameof(SaveLeaderBoard)}</color> called. {nameof(playerListJSON)} updated & saved.");
    }

    //Insert new features below

    #endregion

    #region Miscellaneous

    internal static async Task TestConnection()
    {
        try
        {
            await GetDBNames(); //TODO: Implement a more efficient way to check connection
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>Error</color> found while testing db connection:");
            Debug.Log(e.Message);
        }
    }
    
    private static async Task<bool> IsDocumentPresent(string _id, IMongoCollection<BsonDocument> collection)
    {
        var docType = FilterDefinition<BsonDocument>.Empty;

        var docIDFilter = new BsonDocument()
        {
            {"_id", _id }
        };


        if (await collection.CountDocumentsAsync(docType) > 0)
        {
            var docfound = await collection.FindAsync(docIDFilter);

            return (docfound != null) ? true: false;
        }

        Debug.Log($"<Color=red>Attention</color> Collection {collection.ToString()} is empty. Returning false.");
        return false;
    }

    private static async Task<List<string>> GetDBNames()
    {
        List<string> dbNames = new List<string>();

        var dbs = await newClient.ListDatabaseNamesAsync();
        dbNames = dbs.ToList();

        if (dbNames.Count > 0)
        {
            Debug.Log($"Databases in {nameof(dbNames)}: ");

            for (int i = 0; i < dbNames.Count; i++)
            {
                Debug.Log(dbNames[i]);
            }
            
            return dbNames;
        }

        Debug.Log($"<color=red>{nameof(GetDBNames)}</color> Returning null. No Dbs found.");
        return null;
    }
    #endregion
}
