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

    [SerializeField]
    private Animator animator;
    protected void Awake()
    {
        endingContent = new Dictionary<string, (Sprite, string, string)>
        {
            { "A", (endingSprites[0], "The EverSnow", "EndingA") },
            { "B", (endingSprites[1], "The Spring Thaw", "EndingB") }
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
        switch (endingType)
        {
            case "A":
                animator.Play("EndingA");
                break;
            case "B":
                animator.Play("EndingB");
                break;
        }
    }

    private void OnEndButtonClick()
    {
        GameManager.Instance.Restart();
    }
}
