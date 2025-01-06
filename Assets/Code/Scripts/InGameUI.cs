using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : Singleton<InGameUI>
{
    [SerializeField] private TextMeshProUGUI iceCountText;
    [SerializeField] private GameObject iceImage;
    [SerializeField] private Image currentWeapon;
    [SerializeField] private Image healthSnowman;

    [SerializeField] private List<Sprite> healthSprites;

    [SerializeField] private GameObject hintPanel;

    [SerializeField] private TextMeshProUGUI hintText;

    [SerializeField] private RectTransform notificationPanel;

    [SerializeField] private float slideDistance = 1600f; // Increased for full screen width

    [SerializeField] private float slideDuration = 0.3f; // Duration of slide animation
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Button debugButton;

    private Vector2 notificationHiddenPos;
    private Vector2 notificationVisiblePos;
    private Coroutine currentNotificationCoroutine;


    [SerializeField] private TextMeshProUGUI notificationText;
    protected void Start()
    {
        debugButton.onClick.AddListener(() =>
        {
            GameManager.Instance.GoToEnding("A");
        });
    }
    private void Update()
    {
        var iceCount = GameManager.Instance.PlayerInstance.GetIceCount();
        if (iceCount == 0)
        {
            iceImage.SetActive(false);
            iceCountText.text = "";
        }
        else
        {
            iceImage.SetActive(true);
            iceCountText.text = "x" + iceCount.ToString();
        }

        var weapon = GameManager.Instance.PlayerInstance.GetEquippedWeapon();
        if (weapon != null)
        {
            currentWeapon.enabled = true;
            currentWeapon.sprite = weapon.Icon;
        }
        else currentWeapon.enabled = false;

        switch (GameManager.Instance.PlayerInstance.Health)
        {
            case 3:
                healthSnowman.sprite = healthSprites[0];
                break;
            case 2:

                healthSnowman.sprite = healthSprites[1];
                break;
            case 1:
                healthSnowman.sprite = healthSprites[2];
                break;
        }
    }

    public Sprite GetHealthSprite()
    {
        return healthSprites[Math.Clamp(3 - GameManager.Instance.PlayerInstance.Health, 0, 2)];
    }

    public void ShowHint(string text)
    {
        hintPanel.SetActive(true);
        hintText.text = text;
    }

    public void HideHint()
    {
        if (hintPanel)
        {
            hintPanel.SetActive(false);
            hintText.text = "";
        }
    }

    protected override void Init()
    {
        base.Init();
        // Store the original position as the visible position
        notificationVisiblePos = notificationPanel.anchoredPosition;
        // Calculate hidden position (moved right by slideDistance)
        notificationHiddenPos = notificationVisiblePos + Vector2.right * slideDistance;
        // Start hidden
        notificationPanel.anchoredPosition = notificationHiddenPos;
        notificationPanel.gameObject.SetActive(false);
    }

    public void ShowNotification(string text, float lifespan = 2f)
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }

        currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(text, lifespan));
    }

    private IEnumerator ShowNotificationCoroutine(string text, float lifespan)
    {
        notificationPanel.gameObject.SetActive(true);
        notificationText.text = text;

        // Slide in from left
        float elapsed = 0;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float curvedT = slideCurve.Evaluate(t);
            notificationPanel.anchoredPosition = Vector2.Lerp(notificationHiddenPos, notificationVisiblePos, curvedT);
            yield return null;
        }

        // Wait for lifespan
        yield return new WaitForSeconds(lifespan);

        // Slide out to left
        Vector2 exitPosition = notificationVisiblePos + Vector2.right * slideDistance;
        elapsed = 0;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float curvedT = slideCurve.Evaluate(t);
            notificationPanel.anchoredPosition = Vector2.Lerp(notificationVisiblePos, exitPosition, curvedT);
            yield return null;
        }

        notificationPanel.gameObject.SetActive(false);
        currentNotificationCoroutine = null;
    }

    public void HideNotification()
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
            currentNotificationCoroutine = StartCoroutine(HideNotificationImmediate());
        }
    }

    private IEnumerator HideNotificationImmediate()
    {
        float elapsed = 0;
        Vector2 startPos = notificationPanel.anchoredPosition;
        Vector2 exitPosition = notificationVisiblePos + Vector2.right * slideDistance;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float curvedT = slideCurve.Evaluate(t);
            notificationPanel.anchoredPosition = Vector2.Lerp(startPos, exitPosition, curvedT);
            yield return null;
        }

        notificationPanel.gameObject.SetActive(false);
        currentNotificationCoroutine = null;
    }
}
