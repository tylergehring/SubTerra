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

    private void Start()
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

        // Keep active — we will show/hide panels manually
        pauseUIDocumentGO.SetActive(true);
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
            PauseGame(); // ← This sets up and shows pause menu
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        var root = pauseUIDocument.rootVisualElement;

        // Re-query every time (in case scene reload)
        pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        controlsPanel = root.Q<VisualElement>("ControlsPanel");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        // Show only pause menu
        pauseMenuPanel.style.display = DisplayStyle.Flex;
        settingsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;

        // Re-bind buttons every time
        var resumeBtn = root.Q<Button>("ResumeButton");
        if (resumeBtn != null) resumeBtn.clicked += ResumeGame;

        var settingsBtn = root.Q<Button>("PauseSettingsButton");
        if (settingsBtn != null) settingsBtn.clicked += OpenSettings;

        var controlsBtn = root.Q<Button>("ControlsButton");
        if (controlsBtn != null) controlsBtn.clicked += OpenControls;

        var quitBtn = root.Q<Button>("QuitToMainButton");
        if (quitBtn != null) quitBtn.clicked += QuitToMain;

        var backSettingsBtn = root.Q<Button>("BackFromSettingsButton");
        if (backSettingsBtn != null) backSettingsBtn.clicked += BackToPause;

        var backControlsBtn = root.Q<Button>("BackFromControlsButton");
        if (backControlsBtn != null) backControlsBtn.clicked += BackToPause;

        LoadSettings();
        volumeSlider?.RegisterValueChangedCallback(OnVolumeChange);
        godModeToggle?.RegisterValueChangedCallback(OnGodModeChange);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Hide all panels
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.None;
        if (controlsPanel != null) controlsPanel.style.display = DisplayStyle.None;
    }

    private void QuitToMain()
    {
        ResumeGame();

        if (SafeTerrainHandler.TryGetComponent(out _))
            Debug.Log("UIPauseMenuController: Referenced team TerrainHandler for reset.");

        var mainGO = GameObject.Find("MainMenuUI");
        if (mainGO != null) mainGO.SetActive(true);
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
        return (settingsPanel != null && settingsPanel.style.display == DisplayStyle.Flex) ||
               (controlsPanel != null && controlsPanel.style.display == DisplayStyle.Flex);
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
        if (volumeSlider == null || godModeToggle == null) return;

        float vol = PlayerPrefs.GetFloat("Volume", 50f);
        volumeSlider.value = vol;
        SafeSoundManager.SetBackgroundVolume(vol / 100f);

        bool god = PlayerPrefs.GetInt("GodMode", 0) == 1;
        godModeToggle.value = god;
    }
}