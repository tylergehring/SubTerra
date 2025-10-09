using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseUIDocumentGO; // Assign the PauseUIDocument GameObject here

    private bool isPaused = false;
    private UIDocument pauseUIDocument;
    private VisualElement pauseMenuPanel;

    void Start()
    {
        Debug.Log("PauseMenuController: Starting initialization.");

        if (pauseUIDocumentGO == null)
        {
            Debug.LogError("PauseMenuController: pauseUIDocumentGO not assigned. Assign the PauseUIDocument GameObject in Inspector.");
            return;
        }

        pauseUIDocument = pauseUIDocumentGO.GetComponent<UIDocument>();
        if (pauseUIDocument == null)
        {
            Debug.LogError("PauseMenuController: UIDocument component not found on assigned GameObject.");
            return;
        }

        // Disable at start
        pauseUIDocumentGO.SetActive(false);
        Debug.Log("PauseMenuController: PauseUIDocument disabled at start.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("PauseMenuController: Escape key pressed. Toggling pause.");
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
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

        if (pauseUIDocumentGO != null)
        {
            pauseUIDocumentGO.SetActive(true); // Enable GameObject to load root
            Debug.Log("PauseMenuController: PauseUIDocumentGO enabled.");

            // Now query panel after enabling
            var root = pauseUIDocument.rootVisualElement;
            if (root != null)
            {
                pauseMenuPanel = root.Q<VisualElement>("PauseMenuPanel");
                if (pauseMenuPanel != null)
                {
                    pauseMenuPanel.visible = true;
                    Debug.Log("PauseMenuController: Game paused and menu shown.");
                }
                else
                {
                    Debug.LogError("PauseMenuController: PauseMenuPanel not found after enabling.");
                }
            }
            else
            {
                Debug.LogError("PauseMenuController: Root VisualElement still null after enabling.");
            }
        }
        else
        {
            Debug.LogError("PauseMenuController: Cannot show menu - pauseUIDocumentGO is null.");
        }
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseUIDocumentGO != null)
        {
            pauseUIDocumentGO.SetActive(false); // Disable GameObject to hide
            Debug.Log("PauseMenuController: Game resumed and PauseUIDocumentGO disabled.");
        }
        else
        {
            Debug.LogError("PauseMenuController: Cannot hide menu - pauseUIDocumentGO is null.");
        }
    }
}