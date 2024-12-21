using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
public class Plot
{
    public ObjectId Id { get; set; }

    public IList<Plot_Dialogs> Dialogs { get; set; } = new List<Plot_Dialogs>();

    public string? Label { get; set; }

    public string? NPCName { get; set; }

    public string? NPCState { get; set; }

    public string? PlayerState { get; set; }

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

public class Plot_Dialogs
{
    public IList<Plot_Dialogs_Characters> Characters { get; set; } = new List<Plot_Dialogs_Characters>();

    public int? DialogId { get; set; }

    public Plot_Dialogs_EndDialog? EndDialog { get; set; }

    public int? NextDialogId { get; set; }

    public IList<Plot_Dialogs_Options> Options { get; set; } = new List<Plot_Dialogs_Options>();

    public string? Position { get; set; }

    public string? Speaker { get; set; }

    public string? Text { get; set; }

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

public class Plot_Dialogs_Characters
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

public class Plot_Dialogs_EndDialog
{
    public IList<Plot_Dialogs_EndDialog_NextState> NextState { get; set; } = new List<Plot_Dialogs_EndDialog_NextState>();

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

public class Plot_Dialogs_EndDialog_NextState
{
    public string? Name { get; set; }

    public string? State { get; set; }

    public override string ToString()
    {
        return $"Next State - Name: {Name}, State: {State}";
    }
}

public class Plot_Dialogs_Options
{
    public int? NextDialog { get; set; }

    public string? Text { get; set; }

    public override string ToString()
    {
        return $"Option - Next Dialog: {NextDialog}, Text: {Text}";
    }
}
