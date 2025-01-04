using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UnityEngine;

public class MongoManager : Singleton<MongoManager>
{
    private readonly System.Random _random = new System.Random();
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

    public async Task<Plot> GetPlotByStatesAsync(string playerState, string npcState, string npcName)
    {
        var filter = Builders<Plot>.Filter.Or(
            Builders<Plot>.Filter.And(
                Builders<Plot>.Filter.Eq(p => p.PlayerState, playerState),
                Builders<Plot>.Filter.Eq(p => p.NPCState, npcState),
                Builders<Plot>.Filter.Eq(p => p.NPCName, npcName)
            ),
            Builders<Plot>.Filter.And(
                Builders<Plot>.Filter.Eq(p => p.PlayerState, ""),
                Builders<Plot>.Filter.Eq(p => p.NPCState, npcState),
                Builders<Plot>.Filter.Eq(p => p.NPCName, npcName)
            )
        );

        var result = await (await Plots.FindAsync(filter)).ToListAsync();
        var exactMatches = result.Where(p => p.PlayerState == playerState).ToList();

        if (exactMatches.Any())
        {
            return exactMatches[_random.Next(exactMatches.Count)];
        }

        return result.Any() ? result[_random.Next(result.Count)] : null;
    }

    public async Task<Plot> GetPlotByLabelAsync(string label)
    {
        var playerState = GameManager.Instance.PlayerInstance.StoryState;
        return await (await Plots.FindAsync(plot => plot.Label == label && plot.PlayerState == playerState)).FirstOrDefaultAsync();
    }

    public async Task<Plot> GetPlotByPlayerStateAsync(string playerState)
    {
        return await (await Plots.FindAsync(plot => plot.PlayerState == playerState)).FirstOrDefaultAsync();
    }
}
