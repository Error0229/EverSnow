using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    [SerializeField] List<Sprite> endingSprites;
    [SerializeField] Button endButton;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image endingImage;

    private Dictionary<string, (Sprite sprite, string text, string bgm)> endingContent;

    protected void Awake()
    {
        endingContent = new Dictionary<string, (Sprite, string, string)>
        {
            { "A", (endingSprites[0], "Ending A - The Happy Ending", "EndingA") },
            { "B", (endingSprites[1], "Ending B - The Sad Ending", "EndingB") }
        };
        endButton.onClick.AddListener(OnEndButtonClick);
    }


    public void ShowEnding(string endingType)
    {
        if (endingContent.TryGetValue(endingType, out var content))
        {
            endingImage.sprite = content.sprite;
            text.text = content.text;
            // AudioManager.Instance.PlayMusic(content.bgm);
        }
    }

    private void OnEndButtonClick()
    {
        GameManager.Instance.Restart();
    }
}
