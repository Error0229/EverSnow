using System;
using System.Linq;
using System.Threading.Tasks;
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
        return result.Any(p => p.PlayerState == playerState)
            ? result.First(p => p.PlayerState == playerState)
            : result.FirstOrDefault();
    }

    public async Task<Plot> GetPlotByLabelAsync(string label)
    {
        return await (await Plots.FindAsync(plot => plot.Label == label)).FirstOrDefaultAsync();
    }

    public async Task<Plot> GetPlotByPlayerStateAsync(string playerState)
    {
        return await (await Plots.FindAsync(plot => plot.PlayerState == playerState)).FirstOrDefaultAsync();
    }
}
