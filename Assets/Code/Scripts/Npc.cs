﻿using UnityEngine;
public class Npc : MonoBehaviour
{
    [SerializeField]
    private string characterName;
    [SerializeField]
    private Story story;

    [SerializeField]
    private string realName;


    public string StoryState
    {
        get => story.State;
        set => story.State = value;
    }

    public string RealName
    {
        get => realName;
    }

    private void Awake()
    {
        GameManager.Instance.RegisterNpc(this);
    }
}