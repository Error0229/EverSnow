using System.Collections.Generic;
using System.Linq;
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

    public RectTransform BackgroundRect => backgroundImage.rectTransform;

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

    public string Speaker { get; set; }
    public string Text { get; set; }

    private PlotDialog pendingDialogData;
    private bool isAnimating = false;
    private bool setupComplete = false;
    private int currentAnimationHash;

    public bool IsAnimating => isAnimating;

    public void SetUp(PlotDialog dialogData, string direction)
    {
        setupComplete = false;
        isAnimating = true;
        pendingDialogData = dialogData;

        // Set animation hash
        currentAnimationHash = Animator.StringToHash(direction == "Left" ? "BubblePopLeft" : "BubblePopRight");
        animator.Play(currentAnimationHash);

        // Only set up the background image and position first
        if (dialogData.Options.Any())
        {
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping["Option"]];
        }
        else
        {
            backgroundImage.sprite = dialogBubbleSprites[DialogIndexMapping[dialogData.DialogImage ?? "Common"]];
        }
    }

    private void Update()
    {
        if (isAnimating && !setupComplete)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == currentAnimationHash && stateInfo.normalizedTime >= 1.0f)
            {
                isAnimating = false;
                setupComplete = true;
                CompleteSetup();
            }
        }
    }

    private void CompleteSetup()
    {
        Text = pendingDialogData.Text;
        uiText.text = pendingDialogData.Text;
        nameText.text = pendingDialogData.Speaker;

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
            Speaker = pendingDialogData.Speaker;
        }

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
    }
}
