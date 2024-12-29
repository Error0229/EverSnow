using System;
using TMPro;
using UnityEngine;
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private STATE state;

    private int currentCharIndex;

    private ITypewriterStrategy currentStrategy;
    private float elapsedTime;
    private string fullText;
    private PlayerInputManager inputManager;
    private Action onTypeComplete;
    private TextMeshProUGUI textComponent;
    private bool triggerEnter;

    public bool IsTyping
    {
        get => state == STATE.TYPING;
    }

    public bool IsFinished
    {
        get => state == STATE.FINISHED;
    }

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        inputManager = FindFirstObjectByType<PlayerInputManager>();
        inputManager.evtDialogClick.AddListener(OnDialogClicked);
        SetStrategy(new NormalTypewriter());
        GoToState(STATE.PAUSED);
    }

    private void Update()
    {
        switch (state)
        {
            case STATE.PAUSED:
                break;

            case STATE.TYPING:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    StoryManager.Instance.IsTyping = true;
                }
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= currentStrategy.GetTypeSpeed())
                {
                    elapsedTime = 0;
                    currentCharIndex += 1;

                    if (currentCharIndex >= fullText.Length)
                        GoToState(STATE.FINISHED);
                    else
                        textComponent.text = currentStrategy.ProcessText(fullText, currentCharIndex, elapsedTime);
                }
                else
                {
                    textComponent.text = currentStrategy.PrepareText(fullText, currentCharIndex, elapsedTime);
                }

                break;

            case STATE.FINISHED:
                if (triggerEnter)
                {
                    textComponent.text = currentStrategy.ProcessText(fullText, fullText.Length, 0);
                    triggerEnter = false;
                    StoryManager.Instance.IsTyping = false;
                    onTypeComplete?.Invoke();
                }

                break;
        }
    }

    private void OnDialogClicked(bool pressed)
    {
        if (enabled && pressed && state == STATE.TYPING)
        {
            print("got terminated");
            CompleteTyping();
        }
    }

    public void SetStrategy(ITypewriterStrategy strategy)
    {
        currentStrategy = strategy;
    }

    public void StartTyping(string text, Action onComplete = null)
    {
        fullText = text;
        textComponent.text = "";
        currentCharIndex = 0;
        elapsedTime = 0;
        onTypeComplete = onComplete;
        GoToState(text.Length == 0 ? STATE.PAUSED : STATE.TYPING);
    }

    public void CompleteTyping()
    {
        currentCharIndex = fullText.Length - 1;
        GoToState(STATE.FINISHED);
        onTypeComplete?.Invoke();
    }

    private void GoToState(STATE targetState)
    {
        state = targetState;
        triggerEnter = true;
    }

    private enum STATE
    {
        PAUSED,
        TYPING,
        FINISHED
    }
}
