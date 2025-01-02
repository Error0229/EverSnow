using System;
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
        var result = Plots.Find(plot => plot.PlayerState == playerState && plot.NPCState == npcState && plot.NPCName == npcName).FirstOrDefault();
        print(result);
        return result;
    }

    public Plot GetPlotByLabel(string label)
    {
        return Plots.Find(plot => plot.Label == label).FirstOrDefault();
    }
}
