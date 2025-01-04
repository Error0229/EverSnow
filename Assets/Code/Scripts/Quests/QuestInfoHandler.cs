using UnityEngine;
using TMPro;
using System.Collections;

public class QuestInfoHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;
    [SerializeField] private float slideInDuration = 0.5f;
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Quest currentQuest;
    private RectTransform rectTransform;
    private Coroutine slideCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        questText.text = currentQuest.Description + currentQuest.GetStatus();
    }

    public void SetQuest(Quest quest)
    {
        currentQuest = quest;
        questText.text = quest.QuestName;

        // Setup initial position and start slide in
        Vector2 endPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(endPosition.x + slideDistance, endPosition.y);

        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideAnimation(endPosition.x));
    }

    public string GetQuestName()
    {
        return currentQuest.QuestName;
    }

    public void FinishQuest()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideAnimation(rectTransform.anchoredPosition.x + slideDistance, true));
    }

    private IEnumerator SlideAnimation(float targetX, bool destroy = false)
    {
        AudioManager.Instance.PlaySFX("CancelEquip");
        float startX = rectTransform.anchoredPosition.x;
        float elapsed = 0f;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideInDuration);
            float newX = Mathf.Lerp(startX, targetX, t);
            rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);
            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(targetX, rectTransform.anchoredPosition.y);

        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
