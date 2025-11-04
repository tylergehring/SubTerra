using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class UIPauseMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseUIDocumentGO;
    [Header("Audio")]
    public AudioMixer audioMixer;

    private bool isPaused = false;
    private UIDocument pauseUIDocument;

    private VisualElement pauseMenuPanel;
    private VisualElement settingsPanel;
    private VisualElement controlsPanel;

    private Slider volumeSlider;
    private Toggle godModeToggle;

    void Start()
    {
        if (pauseUIDocumentGO == null)
        {
            Debug.LogError("UIPauseMenuController: pauseUIDocumentGO not assigned.");
            return;
        }

        pauseUIDocument = pauseUIDocumentGO.GetComponent<UIDocument>();
        if (pauseUIDocument == null)
        {
            Debug.LogError("UIPauseMenuController: UIDocument not found.");
            return;
        }

        pauseUIDocumentGO.SetActive(false);
        Debug.Log("UIPauseMenuController: Initialized. Pause UI hidden.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("UIPauseMenuController: ESC pressed.");
            if (isPaused)
            {
                if (IsInSubMenu())
                {
                    Debug.Log("UIPauseMenuController: ESC in submenu → BackToPause()");
                    BackToPause();
                }
                else
                {
                    Debug.Log("UIPauseMenuController: ESC on pause → ResumeGame()");
                    ResumeGame();
                }
            }
            else
            {
                Debug.Log("UIPauseMenuController: ESC while playing → PauseGame()");
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        pauseUIDocumentGO.SetActive(true);
        Debug.Log("UIPauseMenuController: Paused. UI enabled.");

        var root = pauseUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIPauseMenuController: Root null.");
            return;
        }

        pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        Debug.Log($"PauseMenuPanel: {pauseMenuPanel != null}");
        Debug.Log($"SettingsPanel: {settingsPanel != null}");
        Debug.Log($"ControlsPanel: {controlsPanel != null}");

        pauseMenuPanel.style.display = DisplayStyle.Flex;
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;

        var resumeButton = root.Q<Button>("ResumeButton");
        if (resumeButton != null) resumeButton.clicked += ResumeGame;

        var settingsButton = root.Q<Button>("PauseSettingsButton");
        if (settingsButton != null)
        {
            settingsButton.clicked += () =>
            {
                Debug.Log("Settings clicked!");
                OpenSettings();
            };
        }

        var controlsButton = root.Q<Button>("ControlsButton");
        if (controlsButton != null)
        {
            controlsButton.clicked += () =>
            {
                Debug.Log("Controls clicked!");
                OpenControls();
            };
        }

        var quitButton = root.Q<Button>("QuitToMainButton");
        if (quitButton != null) quitButton.clicked += QuitToMain;

        var backSettings = root.Q<Button>("BackFromSettingsButton");
        if (backSettings != null) backSettings.clicked += BackToPause;

        var backControls = root.Q<Button>("BackFromControlsButton");
        if (backControls != null) backControls.clicked += BackToPause;

        LoadSettings();
        if (volumeSlider != null) volumeSlider.RegisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.RegisterValueChangedCallback(OnGodModeChange);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseUIDocumentGO.SetActive(false);
        Debug.Log("UIPauseMenuController: Resumed. UI disabled.");
    }

    private void QuitToMain()
    {
        ResumeGame();
        // TODO: Reset level, e.g., FindObjectOfType<LevelGenerator>().Reset();
        // Activate main menu GO
        Debug.Log("UIPauseMenuController: Quit to main.");
    }

    private void OpenSettings()
    {
        pauseMenuPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;
        Debug.Log("UIPauseMenuController: Settings opened.");
    }

    private void OpenControls()
    {
        pauseMenuPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.Flex;
        Debug.Log("UIPauseMenuController: Controls opened.");
    }

    private void BackToPause()
    {
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;
        pauseMenuPanel.style.display = DisplayStyle.Flex;
        Debug.Log("UIPauseMenuController: Back to pause.");
    }

    private bool IsInSubMenu()
    {
        return settingsPanel.style.display == DisplayStyle.Flex || controlsPanel.style.display == DisplayStyle.Flex;
    }

    private void OnVolumeChange(ChangeEvent<float> evt)
    {
        float vol = evt.newValue / 100f;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(vol) * 20f);
        PlayerPrefs.SetFloat("Volume", evt.newValue);
        Debug.Log($"UIPauseMenuController: Volume {evt.newValue}");
    }

    private void OnGodModeChange(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("GodMode", evt.newValue ? 1 : 0);
        Debug.Log($"UIPauseMenuController: God Mode {(evt.newValue ? "on" : "off")}");
    }

    private void LoadSettings()
    {
        if (volumeSlider == null || godModeToggle == null) return;

        float vol = PlayerPrefs.GetFloat("Volume", 50f);
        volumeSlider.value = vol;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(vol / 100f) * 20f);

        bool god = PlayerPrefs.GetInt("GodMode", 0) == 1;
        godModeToggle.value = god;
    }
}