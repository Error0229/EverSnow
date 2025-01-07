using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
    [SerializeField] private AnimationCurve CharacterImageShakingCurve;
    [SerializeField] private AnimationCurve CharacterImageJumpCurve; // Add this field

    public UnityEvent<string> evtOptionClick = new UnityEvent<string>();

    private readonly Dictionary<string, Coroutine> animationCoroutines = new Dictionary<string, Coroutine>();
    private readonly Dictionary<string, Vector2> characterOriginalPosition = new Dictionary<string, Vector2>();
    private readonly Dictionary<string, string> CharacterPositionMapping = new Dictionary<string, string>();

    private DialogBubble currentBubble;
    private IList<PlotDialog> dialogs = new List<PlotDialog>();
    private List<UIImageFader> faders = new List<UIImageFader>();

    public bool IsAnimating
    {
        get => currentBubble != null && currentBubble.IsAnimating;
    }

    private void Start()
    {
        faders = GetComponentsInChildren<UIImageFader>().ToList();
    }
    private bool IsTalkingToPet()
    {
        return CharacterPositionMapping.ContainsKey("Ila") && CharacterPositionMapping["Ila"] != "BottomRight";
    }

    public void ShowCharacter(PlotDialogCharacter character)
    {
        var index = PositionIndexMapping[character.Position ?? "Left"];
        var characterImage = characterImages[index];
        var spritePath = $"Textures/Character/{character.Name}/{character.Image}";
        Debug.Log($"Loading sprite from Resources path: {spritePath}");

        var sprite = Resources.Load<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogError($"Sprite not found in Resources: {spritePath}");
            sprite = defaultSprite;
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
        if (character.Animation != string.Empty)
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
            }
        }
    }

    private IEnumerator ShakeCharacter(RectTransform imageTransform)
    {
        float elapsed = 0;
        var originalPosition = imageTransform.anchoredPosition;
        var shakeDuration = 0.125f; // Adjust this to control how long one shake cycle takes
        var shakeAmount = 22f; // Adjust this to control shake intensity
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

            var curveTime = elapsed / shakeDuration;
            var xOffset = CharacterImageShakingCurve.Evaluate(curveTime) * shakeAmount;

            imageTransform.anchoredPosition = originalPosition + new Vector2(xOffset, 0);

            yield return null;
        }
    }

    private IEnumerator JumpCharacter(RectTransform imageTransform)
    {
        var jumpDuration = 0.33f;
        var jumpHeight = 40f;
        var originalPosition = imageTransform.anchoredPosition;
        var cycleCount = 0;
        var cycleLimit = 3; // Adjust this to control how many shake cycles to perform

        while (cycleCount < cycleLimit) // Continuous jumping
        {
            float elapsed = 0;

            while (elapsed < jumpDuration)
            {
                elapsed += Time.deltaTime;
                var normalizedTime = elapsed / jumpDuration;

                // Use animation curve to control the jump height
                var yOffset = CharacterImageJumpCurve.Evaluate(normalizedTime) * jumpHeight;
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

    public void ShowDialog(PlotDialog dialogData, bool skip = false)
    {
        foreach (var character in dialogData.Characters)
        {
            ShowCharacter(character);
        }

        characterImages[PositionIndexMapping["BottomRight"]].sprite = InGameUI.Instance.GetHealthSprite();
        characterImages[PositionIndexMapping["BottomRight"]].enabled = !IsTalkingToPet();

        dialogContent.SetActive(true);

        // Move existing bubbles up
        var existingBubbles = dialogContent.transform.Cast<Transform>().ToList();
        foreach (var bubble in existingBubbles)
        {
            StartCoroutine(MoveBubbleUp(bubble.GetComponent<RectTransform>()));
        }

        // Create and animate new bubble
        var dialog = Instantiate(dialogBubblePrefab, dialogContent.transform);
        currentBubble = dialog.GetComponent<DialogBubble>();
        // refresh the ui
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogContent.GetComponent<RectTransform>());

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

        currentBubble.SetUp(dialogData, isLeftSide ? "Left" : "Right", skip);

    }
    public void Refresh()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogContent.GetComponent<RectTransform>());
    }

    private IEnumerator MoveBubbleUp(RectTransform rectTransform)
    {
        float elapsed = 0;
        var startPos = rectTransform.anchoredPosition;
        var targetPos = startPos + new Vector2(0, layoutGroup.spacing);

        while (elapsed < bubbleMoveDuration)
        {
            if (!rectTransform) break;
            elapsed += Time.deltaTime;
            var t = elapsed / bubbleMoveDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        if (rectTransform) rectTransform.anchoredPosition = targetPos;
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
