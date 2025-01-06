using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Threading;

public class MainMenuUI : Singleton<MainMenuUI>
{
    [SerializeField] private GameObject menuButtonPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;


    protected override void Init()
    {
        base.Init();
        startButton.onClick.AddListener(OnStartButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }
    public void ShowMainMenu()
    {
        menuButtonPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(ShowMenu);
    }
    public void HideMainMenu()
    {
        mainMenuPanel.SetActive(false);
    }
    public void ShowAudioSetting()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(true);
        menuButtonPanel.SetActive(false);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(HideAudioSetting);
    }
    private void HideAudioSetting()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }


    private void OnStartButtonClick()
    {
        menuButtonPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void OnSettingsButtonClick()
    {
        menuButtonPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void ShowMenu()
    {
        menuButtonPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
