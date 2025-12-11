using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  // <-- Added for scene loading

public class UIMainMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject mainUIDocumentGO;

    private UIDocument mainUIDocument;
    private VisualElement mainMenuPanel;
    private VisualElement settingsPanel;
    private Slider volumeSlider;
    private Toggle godModeToggle;

    // suppress programmatic UI updates from re-triggering handlers
    private bool _suppressUiChange = false;

    private void Start()
    {
        if (mainUIDocumentGO == null)
        {
            Debug.LogError("UIMainMenuController: mainUIDocumentGO not assigned.");
            return;
        }

        if (!mainUIDocumentGO.activeInHierarchy) mainUIDocumentGO.SetActive(true);

        mainUIDocument = mainUIDocumentGO.GetComponent<UIDocument>();
        if (mainUIDocument == null)
        {
            Debug.LogError("UIMainMenuController: UIDocument not found.");
            return;
        }

        var root = mainUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIMainMenuController: rootVisualElement is null.");
            return;
        }

        mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        if (volumeSlider == null) Debug.LogError("UIMainMenuController: VolumeSlider not found.");
        else Debug.Log("UIMainMenuController: VolumeSlider bound.");

        root.Q<Button>("PlayButton")?.RegisterCallback<ClickEvent>(e => StartGame());
        root.Q<Button>("MainSettingsButton")?.RegisterCallback<ClickEvent>(e => OpenSettings());
        root.Q<Button>("BackFromSettingsButton")?.RegisterCallback<ClickEvent>(e => BackToMain());

        // Ensure no duplicate registrations
        if (volumeSlider != null) volumeSlider.UnregisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.UnregisterValueChangedCallback(OnGodModeChange);

        if (volumeSlider != null) volumeSlider.RegisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.RegisterValueChangedCallback(OnGodModeChange);

        // Subscribe to centralized settings manager
        SettingsManager.Instance.OnVolumeChanged += OnSettingsVolumeChanged;
        SettingsManager.Instance.OnGodModeChanged += OnSettingsGodModeChanged;

        // Initialize UI from settings manager
        SettingsManager.Instance.SendCurrentStateToSubscriber(OnSettingsVolumeChanged, OnSettingsGodModeChanged);
    }

    private void StartGame()
    {
        if (SafeTerrainHandler.TryGetComponent(out _))
            Debug.Log("UIMainMenuController: Referenced team TerrainHandler for level gen.");

        mainUIDocumentGO.SetActive(false);

        // Load the gameplay scene (replace "GameScene" with your actual scene name)
        SceneManager.LoadScene("Final Backup", LoadSceneMode.Single);

    }
    

    private void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.None;
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.Flex;
    }

    private void BackToMain()
    {
        if (settingsPanel != null) settingsPanel.style.display = DisplayStyle.None;
        if (mainMenuPanel != null) mainMenuPanel.style.display = DisplayStyle.Flex;
    }

    // User changed slider -> push to SettingsManager
    private void OnVolumeChange(ChangeEvent<float> evt)
    {
        if (_suppressUiChange) return;
        float vol = evt.newValue;
        Debug.Log($"UIMainMenuController: User changed volume to {vol}");
        SettingsManager.Instance.SetVolume(vol);
    }

    // Programmatic update from SettingsManager -> update UI without re-triggering
    private void OnSettingsVolumeChanged(float vol)
    {
        _suppressUiChange = true;
        if (volumeSlider != null) volumeSlider.value = vol;
        _suppressUiChange = false;
    }

    private void OnGodModeChange(ChangeEvent<bool> evt)
    {
        if (_suppressUiChange) return;
        SettingsManager.Instance.SetGodMode(evt.newValue);
    }

    private void OnSettingsGodModeChanged(bool enabled)
    {
        _suppressUiChange = true;
        if (godModeToggle != null) godModeToggle.value = enabled;
        _suppressUiChange = false;
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnVolumeChanged -= OnSettingsVolumeChanged;
            SettingsManager.Instance.OnGodModeChanged -= OnSettingsGodModeChanged;
        }

        if (volumeSlider != null) volumeSlider.UnregisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.UnregisterValueChangedCallback(OnGodModeChange);
    }
}
