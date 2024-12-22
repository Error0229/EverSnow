using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogBubble : MonoBehaviour
{
    private static readonly Dictionary<string, int> DialogIndexMapping = new Dictionary<string, int>
    {
        ["Common"] = 0, ["Thinking"] = 1, ["Surprised"] = 2, ["Option"] = 3
    };
    [SerializeField]
    private GameObject optionPanel;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private TextMeshPro uiText;
    [SerializeField]
    private GameObject optionPrefab;

    private readonly IList<Sprite> dialogBubbleSprites = new List<Sprite>();

    public string Speaker { get; set; }
    public string Text { get; set; }

    public void SetUp(PlotDialog dialogData)
    {
        Speaker = dialogData.Speaker;
        Text = dialogData.Text;
        uiText.text = dialogData.Text;
        if (dialogData.Options.Any())
        {
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Option"]];
            foreach (var opt in dialogData.Options)
            {
                var option = Instantiate(optionPrefab, optionPanel.transform, true);
                var oph = option.GetComponent<OptionHandler>();
                oph.Init(opt);
            }
        }
        backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping[dialogData.DialogImage ?? "Common"]];
        // where should I add this?
        var typewriter = GetComponentInChildren<TypewriterEffect>();
        if (typewriter)
        {
            typewriter.enabled = true;
            typewriter.StartTyping(dialogData.Text);
        }
    }
}
