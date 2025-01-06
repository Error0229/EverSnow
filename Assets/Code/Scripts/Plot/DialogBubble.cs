using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogBubble : MonoBehaviour
{

    private static readonly Dictionary<string, int> DialogIndexMapping = new Dictionary<string, int>
    {
        ["Common"] = 0,
        ["Thinking"] = 1,
        ["Surprised"] = 2,
        ["Option"] = 3
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

    [SerializeField]
    private Animator animator;
    private int currentAnimationHash;

    private PlotDialog pendingDialogData;
    private bool setupComplete;

    public RectTransform BackgroundRect
    {
        get => backgroundImage.rectTransform;
    }

    public string Speaker { get; set; }
    public string Text { get; set; }

    public bool IsAnimating { get; private set; }

    private void Update()
    {
        if (IsAnimating && !setupComplete)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == currentAnimationHash && stateInfo.normalizedTime >= 1.0f)
            {
                IsAnimating = false;
                setupComplete = true;
                CompleteSetup();
            }
        }
    }

    public void SetUp(PlotDialog dialogData, string direction, bool skip = false)
    {
        setupComplete = false;
        IsAnimating = true;
        pendingDialogData = dialogData;

        // Set animation hash
        currentAnimationHash = Animator.StringToHash(direction == "Left" ? "BubblePopLeft" : "BubblePopRight");
        if (skip)
        {
            // Skip animation and complete setup directly
            setupComplete = true;
            IsAnimating = false;

            animator.Play(currentAnimationHash, 0, 1.0f);
            CompleteSetup(skip);
        }
        else
        {
            // Play sound effect if specified
            if (!string.IsNullOrEmpty(dialogData.SoundEffect))
            {
                AudioManager.Instance.PlaySFX(dialogData.SoundEffect);
            }
            // Play normal animation
            animator.Play(currentAnimationHash);
        }
        // Only set up the background image and position first
        if (dialogData.Options.Any())
        {
            // backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Option"]];
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping[dialogData.DialogImage ?? "Option"]];
        }
        else
        {
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping[dialogData.DialogImage ?? "Common"]];
        }
    }

    private void CompleteSetup(bool skip = false)
    {

        if (pendingDialogData.Options.Any())
        {
            optionPanel.SetActive(true);
            Speaker = pendingDialogData.Speaker ?? GameManager.Instance.PlayerInstance.RealName;
            foreach (var opt in pendingDialogData.Options)
            {
                var option = Instantiate(optionPrefab, optionPanel.transform);
                var oph = option.GetComponent<OptionHandler>();
                oph.Init(opt);
            }
        }
        else
        {
            Text = pendingDialogData.Text;
            uiText.text = pendingDialogData.Text;
            nameText.text = pendingDialogData.Speaker;
            Speaker = pendingDialogData.Speaker;
        }

        if (skip) return;
        var typewriter = GetComponentInChildren<TypewriterEffect>();
        if (typewriter)
        {
            typewriter.enabled = true;
            typewriter.StartTyping(pendingDialogData.Text);
        }
    }

    public void OptionApply(string reply)
    {
        optionPanel.SetActive(false);
        backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Common"]];
        uiText.text = reply;
        nameText.text = pendingDialogData.Speaker;
    }
}
