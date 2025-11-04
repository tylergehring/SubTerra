using UnityEngine;
using UnityEngine.UIElements;

public class UIMainMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject mainUIDocumentGO;

    private UIDocument mainUIDocument;
    private VisualElement mainMenuPanel;
    private VisualElement settingsPanel;
    private Slider volumeSlider;
    private Toggle godModeToggle;

    private void Start()
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

        root.Q<Button>("PlayButton")?.RegisterCallback<ClickEvent>(e => StartGame());
        root.Q<Button>("MainSettingsButton")?.RegisterCallback<ClickEvent>(e => OpenSettings());
        root.Q<Button>("BackFromSettingsButton")?.RegisterCallback<ClickEvent>(e => BackToMain());

        LoadSettings();
        volumeSlider?.RegisterValueChangedCallback(OnVolumeChange);
        godModeToggle?.RegisterValueChangedCallback(OnGodModeChange);
    }

    private void StartGame()
    {
        if (SafeTerrainHandler.TryGetComponent(out _))
            Debug.Log("UIMainMenuController: Referenced team TerrainHandler for level gen.");

        // Just hide main menu — DO NOT deactivate GO
        mainUIDocumentGO.SetActive(false);

        // OPTIONAL: Show a "Game Started" message or load scene
    }

    // --------------------------------------------------------------------
    // FIXED: Full method bodies – NO => statements
    // --------------------------------------------------------------------
    private void OpenSettings()
    {
        mainMenuPanel.style.display = DisplayStyle.None;
        settingsPanel.style.display = DisplayStyle.Flex;
    }

    private void BackToMain()
    {
        settingsPanel.style.display = DisplayStyle.None;
        mainMenuPanel.style.display = DisplayStyle.Flex;
    }
    // --------------------------------------------------------------------

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
        float vol = PlayerPrefs.GetFloat("Volume", 50f);
        if (volumeSlider != null) volumeSlider.value = vol;
        SafeSoundManager.SetBackgroundVolume(vol / 100f);

        bool god = PlayerPrefs.GetInt("GodMode", 0) == 1;
        if (godModeToggle != null) godModeToggle.value = god;
    }
}