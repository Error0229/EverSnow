using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class StoryUI : Singleton<StoryUI>
{

    // define a const dict for position and index mapping
    private static readonly Dictionary<string, int> PositionIndexMapping = new Dictionary<string, int>
    {
        ["Left"] = 0, ["Right"] = 1, ["Center"] = 2, ["Top"] = 3
    };
    [SerializeField] private GameObject plotPanel;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private GameObject dialogBubblePrefab;
    public UnityEvent<string> evtOptionClick = new UnityEvent<string>();
    [SerializeField]
    private List<Image> characterImages = new List<Image>();
    private IList<PlotDialog> dialogs = new List<PlotDialog>();
    private List<UIImageFader> faders = new List<UIImageFader>();
    private void Start()
    {
        faders = GetComponentsInChildren<UIImageFader>().ToList();
    }
    public void ShowCharacter(PlotDialogCharacter character)
    {
        var index = PositionIndexMapping[character.Position ?? "Left"];
        var characterImage = characterImages[index];
        var spritePath = character.Name + character.Image;
        var sprite = Resources.Load<Sprite>(spritePath);
        characterImage.enabled = true;
        characterImage.sprite = sprite;
        characterImage.preserveAspect = true;
        faders.ForEach(f => f.TriggerFade(true, true));
    }



    public void ShowDialog(PlotDialog dialogData, bool visibility = true)
    {
        foreach (var character in dialogData.Characters)
        {
            ShowCharacter(character);
        }
        dialogPanel.SetActive(visibility);
        var dialog = Instantiate(dialogBubblePrefab, dialogPanel.transform, true);
        dialog.GetComponent<DialogBubble>().SetUp(dialogData);
    }
    public void StartPlot(Plot plotData)
    {
        plotPanel.SetActive(true);
    }
    public void EndPlot()
    {
        plotPanel.SetActive(false);
        foreach (Transform dialogBubble in dialogPanel.transform)
        {
            foreach (Transform option in dialogBubble.transform)
            {
                Destroy(option.gameObject);
            }
            Destroy(dialogBubble.gameObject);
        }
    }

    public void OnOptionClick(string option)
    {
        evtOptionClick.Invoke(option);
    }
}
