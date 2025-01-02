using System;
using System.Linq;
using MongoDB.Driver;
using UnityEngine;
public class MongoManager : Singleton<MongoManager>
{
    private IMongoCollection<Plot> Plots { get; set; }
    protected override void Init()
    {
        var connectionUri = DotEnv.Instance.Get("MONGO_URI");
        if (connectionUri == string.Empty)
        {
            Debug.Log("MONGO_URI environment variable not found");
            return;
        }
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        try
        {
            Plots = client.GetDatabase("EverSnow").GetCollection<Plot>("Plot");
            Debug.Log("Connected to MongoDB");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    public Plot GetPlotByStates(string playerState, string npcState, string npcName)
    {
        var filter = Builders<Plot>.Filter.Or(
            // Original condition - exact match
            Builders<Plot>.Filter.And(
                Builders<Plot>.Filter.Eq(p => p.PlayerState, playerState),
                Builders<Plot>.Filter.Eq(p => p.NPCState, npcState),
                Builders<Plot>.Filter.Eq(p => p.NPCName, npcName)
            ),
            // Alternative condition - empty PlayerState
            Builders<Plot>.Filter.And(
                Builders<Plot>.Filter.Eq(p => p.PlayerState, ""),
                Builders<Plot>.Filter.Eq(p => p.NPCState, npcState),
                Builders<Plot>.Filter.Eq(p => p.NPCName, npcName)
            )
        );
        var result = Plots.Find(filter).ToList();
        if (result.Any(p => p.PlayerState == playerState))
        {
            return result.First(p => p.PlayerState == playerState);
        }
        return result.FirstOrDefault();
    }

    public Plot GetPlotByLabel(string label)
    {
        return Plots.Find(plot => plot.Label == label).FirstOrDefault();
    }
}
