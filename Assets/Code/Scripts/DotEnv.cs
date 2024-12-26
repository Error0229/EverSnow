using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class DotEnv : Singleton<DotEnv>
{
    private readonly Dictionary<string, string> env = new Dictionary<string, string>();
    protected override void Init()
    {
        if (!File.Exists(".env"))
        {
            Debug.Log(".env file not found");
            return;
        }

        foreach (var line in File.ReadAllLines(".env"))
        {
            // find first equal sign and split the string
            var parts = line.Split(new[]
            {
                '='
            }, 2);
            env.Add(parts[0], parts[1]);
        }
    }
    public string Get(string key)
    {
        return env[key] ?? string.Empty;
    }
}
