using UnityEngine;
using UnityEngine.UIElements;

public class UIPauseMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseUIDocumentGO;

    private bool isPaused;
    private UIDocument pauseUIDocument;

    private VisualElement pauseMenuPanel;
    private VisualElement settingsPanel;
    private VisualElement controlsPanel;

    private Slider volumeSlider;
    private Toggle godModeToggle;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
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

        var root = pauseUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIPauseMenuController: rootVisualElement is null.");
            return;
        }

        // Query panels
        pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");

        // Query sliders/toggles
        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        // Bind callbacks if elements exist
        var resumeBtn = root.Q<Button>("ResumeSettingsButton");
        if (resumeBtn != null) resumeBtn.clicked += ResumeGame;

        var settingsBtn = root.Q<Button>("SettingsButton");
        if (settingsBtn != null) settingsBtn.clicked += OpenSettings;

        var controlsBtn = root.Q<Button>("ControlsButton");
        if (controlsBtn != null) controlsBtn.clicked += OpenControls;

        var exitBtn = root.Q<Button>("QuitToMainMenuButton");
        if (exitBtn != null) exitBtn.clicked += QuitToMainMenu;

        var backSettingsBtn = root.Q<Button>("BackFromSettingsButton");
        if (backSettingsBtn != null) backSettingsBtn.clicked += BackToPause;

        var backControlsBtn = root.Q<Button>("BackFromControlsButton");
        if (backControlsBtn != null) backControlsBtn.clicked += BackToPause;

        if (volumeSlider != null) volumeSlider.RegisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.RegisterValueChangedCallback(OnGodModeChange);

        // Load settings
        LoadSettings();

        // Keep active — we will show/hide panels manually
        pauseUIDocumentGO.SetActive(true);
    }

    private void OnDisable()
    {
        var root = pauseUIDocument.rootVisualElement;
        if (root == null) return;

        // Unbind to prevent leaks
        var resumeBtn = root.Q<Button>("ResumeSettingsButton");
        if (resumeBtn != null) resumeBtn.clicked -= ResumeGame;

        var settingsBtn = root.Q<Button>("PauseSettingsButton");
        if (settingsBtn != null) settingsBtn.clicked -= OpenSettings;

        var controlsBtn = root.Q<Button>("ControlsButton");
        if (controlsBtn != null) controlsBtn.clicked -= OpenControls;

        var backSettingsBtn = root.Q<Button>("BackFromSettingsButton");
        if (backSettingsBtn != null) backSettingsBtn.clicked -= BackToPause;

        var backControlsBtn = root.Q<Button>("BackFromControlsButton");
        if (backControlsBtn != null) backControlsBtn.clicked -= BackToPause;

        var exitBtn = root.Q<Button>("QuitToMainMenuButton");
        if (exitBtn != null) exitBtn.clicked -= QuitToMainMenu;

        if (volumeSlider != null) volumeSlider.UnregisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.UnregisterValueChangedCallback(OnGodModeChange);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (isPaused)
        {
            if (IsInSubMenu())
                BackToPause();
            else
                ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        pauseMenuPanel.style.display = DisplayStyle.Flex;
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        pauseMenuPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;
    }

    private void QuitToMainMenu()
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menus UI"); // ← change only if your main menu scene has a different name
    }

    private void OpenSettings()
    {
        pauseMenuPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;
    }

    private void OpenControls()
    {
        pauseMenuPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.Flex;
    }

    private void BackToPause()
    {
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;
        pauseMenuPanel.style.display = DisplayStyle.Flex;
    }

    private bool IsInSubMenu()
    {
        return settingsPanel.style.display == DisplayStyle.Flex ||
               controlsPanel.style.display == DisplayStyle.Flex;
    }

    private void OnVolumeChange(ChangeEvent<float> evt)
    {
        float vol = evt.newValue;
        SafeSoundManager.SetBackgroundVolume(vol / 100f);
        PlayerPrefs.SetFloat("Volume", vol);
    }

    private void OnGodModeChange(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("GodMode", evt.newValue ? 1 : 0);
    }

    private void LoadSettings()
    {
        if (volumeSlider != null)
        {
            float vol = PlayerPrefs.GetFloat("Volume", 50f);
            volumeSlider.value = vol;
            SafeSoundManager.SetBackgroundVolume(vol / 100f);
        }

        if (godModeToggle != null)
        {
            bool god = PlayerPrefs.GetInt("GodMode", 0) == 1;
            godModeToggle.value = god;
        }
    }
}