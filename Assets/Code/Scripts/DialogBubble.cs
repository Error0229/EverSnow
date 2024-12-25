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
    private TextMeshProUGUI uiText;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private GameObject optionPrefab;

    [SerializeField]
    private List<Sprite> dialogBubbleSprites = new List<Sprite>();

    public string Speaker { get; set; }
    public string Text { get; set; }

    public void SetUp(PlotDialog dialogData)
    {
        Text = dialogData.Text;
        uiText.text = dialogData.Text;
        nameText.text = dialogData.Speaker;
        if (dialogData.Options.Any())
        {
            optionPanel.SetActive(true);
            Speaker = dialogData.Speaker ?? GameManager.Instance.PlayerInstance.RealName;
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Option"]];
            foreach (var opt in dialogData.Options)
            {
                var option = Instantiate(optionPrefab, optionPanel.transform);
                var oph = option.GetComponent<OptionHandler>();
                oph.Init(opt);
            }
        }
        else
        {
            Speaker = dialogData.Speaker;
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping[dialogData.DialogImage ?? "Common"]];
        }
        // where should I add this?
        var typewriter = GetComponentInChildren<TypewriterEffect>();
        if (typewriter)
        {
            typewriter.enabled = true;
            typewriter.StartTyping(dialogData.Text);
        }
    }

    public void OptionApply(string reply)
    {
        optionPanel.SetActive(false);
        backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Common"]];
        uiText.text = reply;
    }
}
