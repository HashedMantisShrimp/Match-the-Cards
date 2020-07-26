using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using UnityEngine;
//TODO: Find way to install mongodb.legacy.dll & DotEnv dll/plugin files

public class Database
{
    #region Variables
    
    private const string dbName = SData.DB_NAME; //TODO: Substitute later with actual DotEnv files
    private const string dbUser = SData.DB_USER;
    private const string dbPass = SData.DB_PASS;
    private const string dbCluster = SData.DB_CLUSTER;
    private const string dbLPort = SData.DB_LOCAL_PORT;
    private const string lbDocID = "lb00";
    private static string lbColl = "leaderboard";
    private static string lbCollTest = "leaderboard_copy";//Back-up collection, used for testing purposes
    private static string gameStateColl = "gamestate";
    private static Database instance;
    private static IMongoDatabase db;
    private static IMongoCollection<LeaderBoardSchema> lbCollection; 
    private static IMongoCollection<BsonDocument> gameStateCollection;
    private static string remoteDB = $"mongodb+srv://{dbUser}:{dbPass}@{dbCluster}.mongodb.net/test?retryWrites=true&w=majority";
    private static string localDB = $"mongodb://127.0.0.1:{dbLPort}";
    private static TimeSpan timeOut = TimeSpan.FromSeconds(2);
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Init Functions

    static MongoClientSettings settings = new MongoClientSettings()
    {
        ConnectTimeout = timeOut,
        ServerSelectionTimeout = TimeSpan.FromSeconds(1),
        MaxConnectionPoolSize = 10
    };
    
    private Database()
    {
        db = newClient.GetDatabase(dbName);
        lbCollection = db.GetCollection<LeaderBoardSchema>(lbColl);
        gameStateCollection = db.GetCollection<BsonDocument>(gameStateColl);
    }

    internal static Database GetInstance()
    {
        if (instance == null)
        {
            instance = new Database();
        }
        return instance;
    }

    static MongoClient newClient = new MongoClient(remoteDB);
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region LeaderBoard Functions

    internal static async Task SaveLeaderBoard(string playerListJSON)//Saves leaderboard data into db 
    {
        try
        {
            var data = BsonDocument.Parse(playerListJSON);

            var queryFilter = Builders<LeaderBoardSchema>.Filter.Eq("_id", lbDocID);
            var queryUpdate = Builders<LeaderBoardSchema>.Update.Set("leaderBList", data.GetValue("leaderBList"));
            var queryOptions = new UpdateOptions { IsUpsert = true };

            UpdateResult result = await lbCollection.UpdateOneAsync(queryFilter, queryUpdate, queryOptions);

            Debug.Log($"<color=yellow> Operation {nameof(lbCollection.UpdateOneAsync)}()</color> returned result: {result}");
            Debug.Log($"<color=yellow>{nameof(SaveLeaderBoard)}</color> called. {nameof(playerListJSON)} updated & saved.");
        }
        catch (Exception e)
        {
            Misc.HandleException(e, GameData.GetExcDBSaveLeaderBoard());
        }
    }

    internal static async Task<string> LoadLeaderBoardData()//Loads leaderboard data from db, returns empty string if error occurs or if collection is empty
    {
        try
        {
            var JSONdata = string.Empty;
            if (!await IsCollEmpty(lbCollection))
            {
                BsonDocument docFound = null;
                var newProjection = Builders<LeaderBoardSchema>.Projection.Exclude(x=> x._id);
                var filter = Builders<LeaderBoardSchema>.Filter.Eq("_id", lbDocID);
                var options = new FindOptions<LeaderBoardSchema, BsonDocument>
                {
                    MaxAwaitTime = timeOut,
                    Limit = 1,
                    Projection = newProjection,
                };

                using (IAsyncCursor<BsonDocument> cursor = await lbCollection.FindAsync(filter, options))
                {//TODO: adjust code for case where there is no lbDocID but there are other docs
                    while (await cursor.MoveNextAsync())
                    {
                        IEnumerable<BsonDocument> batch = cursor.Current;

                        foreach (BsonDocument document in batch)
                        {
                            docFound = document;
                            Debug.Log($"<color=yellow>Document</color> found: {docFound}");
                        }
                    }
                }
                
                JSONdata = docFound.ToJson();
            }
            else
            {
                Debug.Log($"<color=red>Collection {nameof(lbCollection)}</color> is empty.");
            }
            
            return JSONdata;
        }
        catch (Exception e)
        {
            Misc.HandleException(e, GameData.GetExcDBLoadLeaderBoard());
        }

        Debug.Log($"<color=red>ERROR:</color> out of try-catch block, returning empty string for {nameof(LoadLeaderBoardData)}()");
        return string.Empty;
    }
    #endregion

    //Insert Game State saving here

    //---------------------------------------------------------------------------------------------------

    #region Miscellaneous

    internal static async Task TestConnection()
    {
        try
        {
            await GetDBNames(); //TODO: Implement a more efficient way to check connection, use server.ping() method
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>Error</color> found while testing db connection:");
            Debug.Log(e.Message);
        }
    }

    private static async Task<bool> IsCollEmpty(IMongoCollection<LeaderBoardSchema> collection)//Checks if there is at least one doc present in collection
    {//TODO: Can this method be modified to check game state collection as well?
        var docType = FilterDefinition<LeaderBoardSchema>.Empty;
        var countOptions = new CountOptions { MaxTime = timeOut };
        var collIsEmpty = true;
        
        if (await collection.CountDocumentsAsync(docType, countOptions) > 0)
        {
            Debug.Log($"<Color=blue>Attention</color> Collection {collection.GetType()} is NOT empty");
            collIsEmpty = false;
        }

        Debug.Log($"<Color=blue>Attention</color> {nameof(IsCollEmpty)}() Returning {collIsEmpty}.");
        return collIsEmpty;
    }

    private static async Task<bool> IsDocumentPresent(string _id, IMongoCollection<LeaderBoardSchema> collection)//TODO: Modify this to check for Game State docs
    {
        LeaderBoardSchema docFound = null;
        var docType = FilterDefinition<LeaderBoardSchema>.Empty;
        var countOptions = new CountOptions { MaxTime = timeOut};
        var newProjection = Builders<LeaderBoardSchema>.Projection.Exclude(x => x.list);
        var searchOptions = new FindOptions<LeaderBoardSchema> { Limit = 1, MaxAwaitTime = timeOut, Projection = newProjection };
        var searchFilter = Builders<LeaderBoardSchema>.Filter.Eq("_id", _id);


        if (await collection.CountDocumentsAsync(docType, countOptions) > 0) 
        {
            
            using (IAsyncCursor<LeaderBoardSchema> cursor = await collection.FindAsync(searchFilter, searchOptions))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<LeaderBoardSchema> batch = cursor.Current;

                    foreach (LeaderBoardSchema document in batch)
                    {
                        docFound = document;
                        Debug.Log($"isDocPresent: {document}");
                    }
                }
            }

            Debug.Log($"<Color=blue>Attention</color> docFound {docFound}");

            return (docFound != null) ? true: false;
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
