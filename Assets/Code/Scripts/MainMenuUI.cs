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
        backButton.onClick.AddListener(ShowMenu);
    }
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
    }
    public void HideMainMenu()
    {
        mainMenuPanel.SetActive(false);
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
