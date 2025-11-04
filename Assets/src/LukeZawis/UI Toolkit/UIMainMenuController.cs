using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;

public class UIMainMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject mainUIDocumentGO;
    [Header("Audio")]
    public AudioMixer audioMixer;

    private UIDocument mainUIDocument;
    private VisualElement mainMenuPanel;
    private VisualElement settingsPanel;
    private Slider volumeSlider;
    private Toggle godModeToggle;

    void Start()
    {
        if (mainUIDocumentGO == null)
        {
            Debug.LogError("UIMainMenuController: mainUIDocumentGO not assigned.");
            return;
        }
        mainUIDocument = mainUIDocumentGO.GetComponent<UIDocument>();
        if (mainUIDocument == null)
        {
            Debug.LogError("UIMainMenuController: UIDocument not found.");
            return;
        }

        var root = mainUIDocument.rootVisualElement;
        mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");
        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        volumeSlider = root.Q<Slider>("VolumeSlider");
        godModeToggle = root.Q<Toggle>("GodModeToggle");

        var playButton = root.Q<Button>("PlayButton");
        if (playButton != null) playButton.clicked += StartGame;
        var settingsButton = root.Q<Button>("MainSettingsButton");
        if (settingsButton != null) settingsButton.clicked += OpenSettings;
        var backButton = root.Q<Button>("BackFromSettingsButton");
        if (backButton != null) backButton.clicked += BackToMain;

        LoadSettings();
        if (volumeSlider != null) volumeSlider.RegisterValueChangedCallback(OnVolumeChange);
        if (godModeToggle != null) godModeToggle.RegisterValueChangedCallback(OnGodModeChange);

        Debug.Log("UIMainMenuController: Initialized.");
    }

    private void StartGame()
    {
        // TODO: Your level gen, e.g., FindObjectOfType<LevelGenerator>().GenerateLevel();
        mainUIDocumentGO.SetActive(false);
        Debug.Log("UIMainMenuController: Starting game.");
    }

    private void OpenSettings()
    {
        mainMenuPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;
        Debug.Log("UIMainMenuController: Settings opened.");
    }

    private void BackToMain()
    {
        settingsPanel.style.display = DisplayStyle.None;
        mainMenuPanel.style.display = DisplayStyle.Flex;
        Debug.Log("UIMainMenuController: Back to main menu.");
    }

    private void OnVolumeChange(ChangeEvent<float> evt)
    {
        float vol = evt.newValue / 100f;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(vol) * 20f);
        PlayerPrefs.SetFloat("Volume", evt.newValue);
        Debug.Log($"UIMainMenuController: Volume {evt.newValue}");
    }

    private void OnGodModeChange(ChangeEvent<bool> evt)
    {
        PlayerPrefs.SetInt("GodMode", evt.newValue ? 1 : 0);
        Debug.Log($"UIMainMenuController: God Mode {(evt.newValue ? "on" : "off")}");
    }

    private void LoadSettings()
    {
        float vol = PlayerPrefs.GetFloat("Volume", 50f);
        volumeSlider.value = vol;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(vol / 100f) * 20f);

        bool god = PlayerPrefs.GetInt("GodMode", 0) == 1;
        godModeToggle.value = god;
    }
}