using System;
using System.Linq;
using MongoDB.Driver;
using UnityEngine;
public class MongoManager : Singleton<MongoManager>
{
    private IQueryable<Plot> Plots { get; set; }
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
            Plots = client.GetDatabase("EverSnow").GetCollection<Plot>("Plot").AsQueryable();
            Debug.Log(Plots.FirstOrDefault());
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
