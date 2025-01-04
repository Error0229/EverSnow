using System;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class Quest
{
    protected string questName;
    protected string description;
    protected bool isCompleted;
    protected State state;
    protected Dictionary<string, int> goals = new Dictionary<string, int>();
    protected Dictionary<string, int> progressions = new Dictionary<string, int>();

    public virtual void Finish()
    {
        state = State.Finished;
    }
    public virtual void Reset()
    {
        state = State.None;
        isCompleted = false;
    }

    public virtual void Accept()
    {
        state = State.Accepted;
    }
    protected virtual void AddGoal(string goal, int amount)
    {
        goals.Add(goal, amount);
        progressions.Add(goal, 0);
    }

    public virtual string GetStatus()
    {
        string progression = "";
        foreach (var goal in goals)
        {
            progression += $"{progressions[goal.Key]}/{goal.Value}\n";
        }
        return progression;
    }

    public virtual void AddProgression(string goal, int amount)
    {
        if (goals.ContainsKey(goal))
        {
            progressions[goal] += amount;
        }
    }

    public virtual void SyncProgressions()
    {
    }


    public string QuestName
    {
        get => questName;
    }

    public string Description
    {
        get => description;
    }

    public bool IsCompleted
    {
        get => isCompleted;
    }


    public virtual void Complete()
    {
        AudioManager.Instance.PlaySFX("UseItem");
        isCompleted = true;
    }

    public enum State { None, Accepted, Completed, Finished, Failed }
}
