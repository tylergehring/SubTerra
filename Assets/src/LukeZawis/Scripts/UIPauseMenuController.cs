using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseUIDocumentGO; // Assign in Inspector

    private bool isPaused = false;
    private UIDocument pauseUIDocument;

    // Panels
    private VisualElement pauseMenuPanel;
    private VisualElement settingsMenuPanel;
    private VisualElement controlsMenuPanel;

    void Start()
    {
        if (pauseUIDocumentGO == null)
        {
            Debug.LogError("PauseMenuController: pauseUIDocumentGO not assigned.");
            return;
        }

        pauseUIDocument = pauseUIDocumentGO.GetComponent<UIDocument>();
        if (pauseUIDocument == null)
        {
            Debug.LogError("PauseMenuController: UIDocument not found.");
            return;
        }

        pauseUIDocumentGO.SetActive(false); // hide at start
        Debug.Log("PauseMenuController: Initialized. Pause UI hidden at start.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("PauseMenuController: ESC pressed.");
            if (isPaused)
            {
                if (IsInSubMenu())
                {
                    Debug.Log("PauseMenuController: ESC while in submenu → BackToPause()");
                    BackToPause();
                }
                else
                {
                    Debug.Log("PauseMenuController: ESC on main pause menu → ResumeGame()");
                    ResumeGame();
                }
            }
            else
            {
                Debug.Log("PauseMenuController: ESC while playing → PauseGame()");
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
        Debug.Log("PauseMenuController: Game paused. UI enabled.");

        var root = pauseUIDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("PauseMenuController: Root VisualElement is null.");
            return;
        }

        // --- Query panels ---
        pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
        settingsMenuPanel = root.Q<VisualElement>("SettingsMenuPanel");
        controlsMenuPanel = root.Q<VisualElement>("ControlsMenuPanel");

        Debug.Log($"PauseMenuPanel found? {pauseMenuPanel != null}");
        Debug.Log($"SettingsMenuPanel found? {settingsMenuPanel != null}");
        Debug.Log($"ControlsMenuPanel found? {controlsMenuPanel != null}");

        // Show main pause, hide submenus
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.Flex;
        if (settingsMenuPanel != null) settingsMenuPanel.style.display = DisplayStyle.None;
        if (controlsMenuPanel != null) controlsMenuPanel.style.display = DisplayStyle.None;

        // --- Register button callbacks ---
        var settingsButton = root.Q<Button>("SettingsButton");
        Debug.Log($"SettingsButton found? {settingsButton != null}");
        if (settingsButton != null)
        {
            settingsButton.clicked += () =>
            {
                Debug.Log("SettingsButton clicked!");
                OpenSettings();
            };
        }

        var controlsButton = root.Q<Button>("ControlsButton");
        Debug.Log($"ControlsButton found? {controlsButton != null}");
        if (controlsButton != null)
        {
            controlsButton.clicked += () =>
            {
                Debug.Log("ControlsButton clicked!");
                OpenControls();
            };
        }

        var backFromSettings = root.Q<Button>("BackFromSettingsButton");
        Debug.Log($"BackFromSettingsButton found? {backFromSettings != null}");
        if (backFromSettings != null)
        {
            backFromSettings.clicked += () =>
            {
                Debug.Log("BackFromSettingsButton clicked!");
                BackToPause();
            };
        }

        var backFromControls = root.Q<Button>("BackFromControlsButton");
        Debug.Log($"BackFromControlsButton found? {backFromControls != null}");
        if (backFromControls != null)
        {
            backFromControls.clicked += () =>
            {
                Debug.Log("BackFromControlsButton clicked!");
                BackToPause();
            };
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseUIDocumentGO.SetActive(false);
        Debug.Log("PauseMenuController: Game resumed. UI disabled.");
    }

    private void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (settingsMenuPanel != null) settingsMenuPanel.style.display = DisplayStyle.Flex;
        Debug.Log("PauseMenuController: Settings submenu opened.");
    }

    private void OpenControls()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.None;
        if (controlsMenuPanel != null) controlsMenuPanel.style.display = DisplayStyle.Flex;
        Debug.Log("PauseMenuController: Controls submenu opened.");
    }

    private void BackToPause()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.style.display = DisplayStyle.None;
        if (controlsMenuPanel != null) controlsMenuPanel.style.display = DisplayStyle.None;
        if (pauseMenuPanel != null) pauseMenuPanel.style.display = DisplayStyle.Flex;
        Debug.Log("PauseMenuController: Returned to main pause menu.");
    }

    private bool IsInSubMenu()
    {
        return (settingsMenuPanel != null && settingsMenuPanel.style.display == DisplayStyle.Flex) ||
               (controlsMenuPanel != null && controlsMenuPanel.style.display == DisplayStyle.Flex);
    }
}
