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

    private bool _suppressUiChange = false;

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

        if (!pauseUIDocumentGO.activeInHierarchy) pauseUIDocumentGO.SetActive(true);

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

        pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");

        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        if (volumeSlider == null) Debug.LogError("UIPauseMenuController: VolumeSlider not found.");
        else Debug.Log("UIPauseMenuController: VolumeSlider bound.");

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

        // Ensure no duplicate registrations
        if (volumeSlider != null) volumeSlider.UnregisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.UnregisterValueChangedCallback(OnGodModeChange);

        if (volumeSlider != null) volumeSlider.RegisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.RegisterValueChangedCallback(OnGodModeChange);

        // Subscribe to settings manager
        SettingsManager.Instance.OnVolumeChanged += OnSettingsVolumeChanged;
        SettingsManager.Instance.OnGodModeChanged += OnSettingsGodModeChanged;

        // Initialize UI
        SettingsManager.Instance.SendCurrentStateToSubscriber(OnSettingsVolumeChanged, OnSettingsGodModeChanged);

        pauseUIDocumentGO.SetActive(true);
    }

    private void OnDisable()
    {
        var root = pauseUIDocument != null ? pauseUIDocument.rootVisualElement : null;
        if (root == null) return;

        var resumeBtn = root.Q<Button>("ResumeSettingsButton");
        if (resumeBtn != null) resumeBtn.clicked -= ResumeGame;

        var settingsBtn = root.Q<Button>("SettingsButton");
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

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnVolumeChanged -= OnSettingsVolumeChanged;
            SettingsManager.Instance.OnGodModeChanged -= OnSettingsGodModeChanged;
        }
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

        // keep audio running while paused
        AudioListener.pause = true;

        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.Flex;
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.None;
        if (controlsPanel != null) controlsPanel.style.display = DisplayStyle.None;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        AudioListener.pause = false;

        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.None;
        if (controlsPanel != null) controlsPanel.style.display = DisplayStyle.None;
    }

    private void QuitToMainMenu()
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menus UI");
    }

    private void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.Flex;
    }

    private void OpenControls()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (controlsPanel != null) controlsPanel.style.display = DisplayStyle.Flex;
    }

    private void BackToPause()
    {
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.None;
        if (controlsPanel != null) controlsPanel.style.display = DisplayStyle.None;
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.Flex;
    }

    private bool IsInSubMenu()
    {
        return (settingsPanel != null && settingsPanel.style.display == DisplayStyle.Flex) ||
               (controlsPanel != null && controlsPanel.style.display == DisplayStyle.Flex);
    }

    private void OnVolumeChange(ChangeEvent<float> evt)
    {
        if (_suppressUiChange) return;
        float vol = evt.newValue;
        Debug.Log($"UIPauseMenuController: User changed volume to {vol}");
        SettingsManager.Instance.SetVolume(vol);
    }

    private void OnGodModeChange(ChangeEvent<bool> evt)
    {
        if (_suppressUiChange) return;
        SettingsManager.Instance.SetGodMode(evt.newValue);
    }

    private void OnSettingsVolumeChanged(float vol)
    {
        _suppressUiChange = true;
        if (volumeSlider != null) volumeSlider.value = vol;
        _suppressUiChange = false;
    }

    private void OnSettingsGodModeChanged(bool enabled)
    {
        _suppressUiChange = true;
        if (godModeToggle != null) godModeToggle.value = enabled;
        _suppressUiChange = false;
    }
}
