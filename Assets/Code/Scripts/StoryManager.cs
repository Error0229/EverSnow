﻿using UnityEngine.Events;
public class StoryManager : Singleton<StoryManager>
{
    public UnityEvent evtEnterDialog;
    public UnityEvent evtLeaveDialog;
    private Plot currentPlot;

    private string selectedOption;
    private State state;
    private bool triggerEnter;
    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Ongoing:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    StoryUI.Instance.StartPlot(currentPlot);
                    currentPlot.StartDialog();
                    evtEnterDialog?.Invoke();
                }
                break;
            case State.Finished:
                if (triggerEnter)
                {
                    StoryUI.Instance.EndPlot();
                    triggerEnter = false;
                    evtLeaveDialog?.Invoke();
                }
                GotoState(State.Idle);
                break;
        }

    }

    private void OnOptionClick(string option)
    {
        currentPlot.NextDialog(option);
        StoryUI.Instance.ShowDialog(currentPlot.CurrentDialog);
    }
    protected override void Init()
    {
        StoryUI.Instance.evtOptionClick.AddListener(OnOptionClick);
        PlayerInputManager.Instance.evtNextDialog.AddListener(OnDialogClicked);
        GotoState(State.Idle);
    }

    public void InvokePlot(Npc talker)
    {
        var player = GameManager.Instance.PlayerInstance;
        var plot = MongoManager.Instance.GetPlotByStates(player.StoryState, talker.StoryState);
        if (plot == null) return;
        currentPlot = plot;
        GotoState(State.Ongoing);
    }

    private void GotoState(State nextState)
    {
        state = nextState;
        triggerEnter = true;
    }

    private void OnDialogClicked()
    {
        if (currentPlot.CurrentDialog.IsEndDialog)
        {
            GameManager.Instance.UpdateStoryState(currentPlot.CurrentDialog.EndDialog.NextState);
            GotoState(State.Finished);
            return;
        }
        if (!currentPlot.CurrentDialog.IsOption)
        {
            currentPlot.NextDialog(null);
            StoryUI.Instance.ShowDialog(currentPlot.CurrentDialog);
        }
    }

    private enum State
    {
        Idle,
        Ongoing,
        Finished
    }
}