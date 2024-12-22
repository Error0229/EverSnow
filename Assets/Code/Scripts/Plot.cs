using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MongoDB.Bson;
public class Plot
{
    private PlotDialog currentDialog;

    private int? currentDialogId;
    public ObjectId Id { get; set; }

    public IList<PlotDialog> Dialogs { get; set; } = new List<PlotDialog>();

    public string? Label { get; set; }

    public string? NPCName { get; set; }

    public string? NPCState { get; set; }

    public string? PlayerState { get; set; }
    public PlotDialog CurrentDialog
    {
        get => currentDialog ??= Dialogs.First();
    }
    public void NextDialog([CanBeNull] string option)
    {
        if (option != null)
        {
            currentDialogId = CurrentDialog.Options.First(o => o.Text == option).NextDialog;
        }
        else
        {
            currentDialogId = CurrentDialog.NextDialogId ?? currentDialogId + 1;
        }
        currentDialog = Dialogs
            .FirstOrDefault(dialog => dialog.DialogId == currentDialogId);
    }
    public void StartDialog()
    {
        // find min dialog id
        currentDialogId = Dialogs
            .Where(dialog => dialog.DialogId != null)
            .Select(dialog => dialog.DialogId)
            .Min();
        currentDialog = Dialogs
            .FirstOrDefault(dialog => dialog.DialogId == currentDialogId);
    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Plot Id: {Id}");
        sb.AppendLine($"Label: {Label}");
        sb.AppendLine($"NPC Name: {NPCName}");
        sb.AppendLine($"NPC State: {NPCState}");
        sb.AppendLine($"Player State: {PlayerState}");
        sb.AppendLine("Dialogs:");
        foreach (var dialog in Dialogs)
        {
            sb.AppendLine(dialog.ToString());
        }
        return sb.ToString();
    }
}

public class PlotDialog
{
    public IList<PlotDialogCharacter> Characters { get; set; } = new List<PlotDialogCharacter>();

    public int? DialogId { get; set; }

    public PlotDialogEnd? EndDialog { get; set; }

    public int? NextDialogId { get; set; }

    public IList<PlotDialogOption> Options { get; set; } = new List<PlotDialogOption>();

    public string? Position { get; set; }

    public string? Speaker { get; set; }

    public string? Text { get; set; }

    [CanBeNull] public string DialogImage { get; set; }

    public bool IsEndDialog
    {
        get => EndDialog != null;
    }
    public bool IsOption
    {
        get => Options.Count > 0;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"  Dialog ID: {DialogId}");
        sb.AppendLine($"  Speaker: {Speaker}");
        sb.AppendLine($"  Position: {Position}");
        sb.AppendLine($"  Text: {Text}");
        sb.AppendLine("  Characters:");
        foreach (var character in Characters)
        {
            sb.AppendLine($"    {character}");
        }
        sb.AppendLine("  Options:");
        foreach (var option in Options)
        {
            sb.AppendLine($"    {option}");
        }
        if (EndDialog != null)
        {
            sb.AppendLine($"  End Dialog: {EndDialog}");
        }
        return sb.ToString();
    }
}

public class PlotDialogCharacter
{
    public string? Animation { get; set; }

    public string? Image { get; set; }

    public string? Name { get; set; }

    public string? Position { get; set; }

    public override string ToString()
    {
        return $"Character - Name: {Name}, Animation: {Animation}, Image: {Image}, Position: {Position}";
    }
}

public class PlotDialogEnd
{
    public IList<PlotDialogEndState> NextState { get; set; } = new List<PlotDialogEndState>();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("    End Dialog Next States:");
        foreach (var state in NextState)
        {
            sb.AppendLine($"      {state}");
        }
        return sb.ToString();
    }
}

public class PlotDialogEndState
{
    public string? Name { get; set; }

    public string? State { get; set; }

    public override string ToString()
    {
        return $"Next State - Name: {Name}, State: {State}";
    }
}

public class PlotDialogOption
{
    public int? NextDialog { get; set; }

    public string? Text { get; set; }

    public override string ToString()
    {
        return $"Option - Next Dialog: {NextDialog}, Text: {Text}";
    }
}
