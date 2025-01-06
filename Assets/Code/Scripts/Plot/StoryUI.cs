using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
public class StoryUI : Singleton<StoryUI>
{
    // Define a const dict for position and index mapping
    private static readonly Dictionary<string, int> PositionIndexMapping = new Dictionary<string, int>
    {
        ["Left"] = 0,
        ["Right"] = 1,
        ["BottomRight"] = 2,
        ["Top"] = 3
    };

    [Header("UI Elements"), SerializeField]
    private GameObject plotPanel;
    [SerializeField] private GameObject dialogContent;
    [SerializeField] private GameObject dialogBubblePrefab;
    [SerializeField] private List<Image> characterImages = new List<Image>();
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float bubbleMoveDuration = 0.5f;
    [SerializeField] private float bubbleStretchDuration = 0.3f;
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private AnimationCurve CharacterImageShakingCurve;
    [SerializeField] private AnimationCurve CharacterImageJumpCurve;  // Add this field

    public UnityEvent<string> evtOptionClick = new UnityEvent<string>();
    private Dictionary<string, string> CharacterPositionMapping = new Dictionary<string, string>();

    private DialogBubble currentBubble;
    private IList<PlotDialog> dialogs = new List<PlotDialog>();
    private List<UIImageFader> faders = new List<UIImageFader>();

    private Dictionary<string, Coroutine> animationCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, Vector2> characterOriginalPosition = new Dictionary<string, Vector2>();

    public bool IsAnimating
    {
        get => currentBubble != null && currentBubble.IsAnimating;
    }

    private void Start()
    {
        faders = GetComponentsInChildren<UIImageFader>().ToList();
    }
    private void Update()
    {
        var text = $"Story States:\nKissimi: {GameManager.Instance.PlayerInstance.StoryState}\n";
        foreach (var npc in GameManager.Instance.GetNpcs())
        {
            text += $"{npc.RealName}: {npc.StoryState}\n";
        }
        debugText.text = text;
    }
    private bool IsTalkingToPet()
    {
        return CharacterPositionMapping.ContainsKey("Ila") && CharacterPositionMapping["Ila"] != "BottomRight";
    }

    public void ShowCharacter(PlotDialogCharacter character)
    {
        var index = PositionIndexMapping[character.Position ?? "Left"];
        var characterImage = characterImages[index];
        var spritePath = new[] { "Assets", "Art", "Textures", "Character", character.Name, character.Image }.Aggregate((a, b) => $"{a}/{b}") + ".png";
        Debug.Log($"Loading sprite from path: {spritePath}");
        var sprite = defaultSprite;
        if (!File.Exists(spritePath))
        {
            Debug.LogError($"Sprite not found at path: {spritePath}");
        }
        else
        {
            byte[] fileData = File.ReadAllBytes(spritePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }

        characterImage.enabled = true;
        characterImage.sprite = sprite;
        characterImage.preserveAspect = true;
        CharacterPositionMapping[character.Name] = character.Position;
        faders.ForEach(f => f.TriggerFade(true, true));

        // Stop any existing shake animation for this character
        if (animationCoroutines.ContainsKey(character.Name))
        {
            StopCoroutine(animationCoroutines[character.Name]);
            // Reset position
            characterImage.rectTransform.anchoredPosition = characterOriginalPosition[character.Name];
        }

        // Start shaking if animation is "shaking"
        if (character.Animation != String.Empty)
        {
            characterOriginalPosition[character.Name] = characterImage.rectTransform.anchoredPosition;
            switch (character.Animation)
            {
                case "Shaking":
                    animationCoroutines[character.Name] = StartCoroutine(ShakeCharacter(characterImage.rectTransform));
                    break;
                case "Jumping":
                    animationCoroutines[character.Name] = StartCoroutine(JumpCharacter(characterImage.rectTransform));
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator ShakeCharacter(RectTransform imageTransform)
    {
        float elapsed = 0;
        Vector2 originalPosition = imageTransform.anchoredPosition;
        float shakeDuration = 0.125f; // Adjust this to control how long one shake cycle takes
        float shakeAmount = 22f;  // Adjust this to control shake intensity
        var cycleCount = 0;
        var cycleLimit = 5; // Adjust this to control how many shake cycles to perform

        while (cycleCount < cycleLimit) // Continuous shaking
        {
            elapsed += Time.deltaTime;

            if (elapsed > shakeDuration)
            {
                cycleCount++;
                elapsed = 0; // Reset for next cycle
            }

            float curveTime = elapsed / shakeDuration;
            float xOffset = CharacterImageShakingCurve.Evaluate(curveTime) * shakeAmount;

            imageTransform.anchoredPosition = originalPosition + new Vector2(xOffset, 0);

            yield return null;
        }
    }

    private IEnumerator JumpCharacter(RectTransform imageTransform)
    {
        float jumpDuration = 0.33f;
        float jumpHeight = 40f;
        Vector2 originalPosition = imageTransform.anchoredPosition;
        var cycleCount = 0;
        var cycleLimit = 3; // Adjust this to control how many shake cycles to perform

        while (cycleCount < cycleLimit) // Continuous jumping
        {
            float elapsed = 0;

            while (elapsed < jumpDuration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / jumpDuration;

                // Use animation curve to control the jump height
                float yOffset = CharacterImageJumpCurve.Evaluate(normalizedTime) * jumpHeight;
                imageTransform.anchoredPosition = originalPosition + new Vector2(0, yOffset);
                yield return null;
            }

            // Reset position at the end of each jump
            imageTransform.anchoredPosition = originalPosition;


            // Optional: Add a small pause between jumps
            cycleCount++;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ShowDialog(PlotDialog dialogData, bool visibility = true)
    {
        foreach (var character in dialogData.Characters)
        {
            ShowCharacter(character);
        }

        characterImages[PositionIndexMapping["BottomRight"]].sprite = InGameUI.Instance.GetHealthSprite();
        characterImages[PositionIndexMapping["BottomRight"]].enabled = !IsTalkingToPet();

        dialogContent.SetActive(visibility);

        // Move existing bubbles up
        var existingBubbles = dialogContent.transform.Cast<Transform>().ToList();
        foreach (var bubble in existingBubbles)
        {
            StartCoroutine(MoveBubbleUp(bubble.GetComponent<RectTransform>()));
        }

        // Create and animate new bubble
        var dialog = Instantiate(dialogBubblePrefab, dialogContent.transform);
        currentBubble = dialog.GetComponent<DialogBubble>();

        // Get speaker position
        var isLeftSide = false; // Default to right side
        if (CharacterPositionMapping.ContainsKey(dialogData.Speaker))
        {
            isLeftSide = CharacterPositionMapping[dialogData.Speaker] == "Left";
        }
        if (dialogData.SoundEffect != string.Empty)
        {
            AudioManager.Instance.PlayMusic(dialogData.SoundEffect);
        }


        currentBubble.SetUp(dialogData, isLeftSide ? "Left" : "Right");
    }

    private IEnumerator MoveBubbleUp(RectTransform rectTransform)
    {
        float elapsed = 0;
        var startPos = rectTransform.anchoredPosition;
        var targetPos = startPos + new Vector2(0, layoutGroup.spacing);

        while (elapsed < bubbleMoveDuration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / bubbleMoveDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }


    public void StartPlot(Plot plotData)
    {
        plotPanel.SetActive(true);
    }

    public void EndPlot()
    {
        plotPanel.SetActive(false);
        foreach (var characterImage in characterImages)
        {
            characterImage.enabled = false;
        }
        CharacterPositionMapping.Clear();

        foreach (Transform dialogBubble in dialogContent.transform)
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
        currentBubble.OptionApply(option);
        evtOptionClick.Invoke(option);
    }
}
