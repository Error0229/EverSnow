using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UnityEngine;

public class MongoManager : Singleton<MongoManager>
{
    private readonly System.Random _random = new System.Random();
    private IMongoCollection<Plot> Plots { get; set; }
    private IList<Plot> localPlots = new List<Plot>();
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
            localPlots = Plots.AsQueryable().ToList();
            Debug.Log("Connected to MongoDB");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public Plot GetPlotByStates(string playerState, string npcState, string npcName)
    {
        var result = localPlots.Where(p =>
            (p.PlayerState == playerState || p.PlayerState == "") &&
            p.NPCState == npcState &&
            p.NPCName == npcName
        ).ToList();

        var exactMatches = result.Where(p => p.PlayerState == playerState).ToList();

        if (exactMatches.Any())
        {
            return exactMatches[_random.Next(exactMatches.Count)];
        }

        return result.Any() ? result[_random.Next(result.Count)] : null;
    }

    public Plot GetPlotByLabel(string label)
    {
        var playerState = GameManager.Instance.PlayerInstance.StoryState;
        var result = localPlots.Where(p =>
            (p.PlayerState == playerState || p.PlayerState == "") &&
            p.Label == label
        ).ToList();

        var exactMatches = result.Where(p => p.PlayerState == playerState).ToList();
        if (exactMatches.Any())
        {
            return exactMatches[_random.Next(exactMatches.Count)];
        }
        return result.Any() ? result[_random.Next(result.Count)] : null;
    }

    public Plot GetPlotByPlayerState(string playerState)
    {
        return localPlots.FirstOrDefault(plot => plot.PlayerState == playerState);
    }
}
