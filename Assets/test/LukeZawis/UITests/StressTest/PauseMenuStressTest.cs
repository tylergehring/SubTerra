using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class PauseMenuStressTests
{
    private PauseMenuController controller;

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        Debug.Log("[Stress] 🔄 Initializing: loading PauseMenuTestScene...");
        SceneManager.LoadScene("PauseMenuTestScene");
        yield return null;
        Debug.Log("[Stress] ✅ Scene loaded.");
    }

    [SetUp]
    public void Setup()
    {
        controller = Object.FindFirstObjectByType<PauseMenuController>();
        Assert.IsNotNull(controller, "[Stress] ❌ PauseMenuController not found in scene.");
        Debug.Log("[Stress] ✅ Controller initialized successfully.");
    }

    [UnityTest]
    public IEnumerator Stress_RapidToggleForFiveSecondsWithLagAndPanelLogging()
    {
        //Initilizing test and game state
        Debug.Log("[Stress] ▶️ Starting RapidToggle test...");
        controller.SendMessage("PauseGame");
        Debug.Log("[Stress] ⏸ Game paused.");
        yield return null;

        //Detects Lag
        float start = Time.realtimeSinceStartup;
        float lastFrameTime = Time.realtimeSinceStartup;
        int toggleCount = 0;
        int lagCount = 0;

        while (Time.realtimeSinceStartup - start < 5f)
        {
            float now = Time.realtimeSinceStartup;
            float frameDuration = now - lastFrameTime;
            lastFrameTime = now;

            if (frameDuration > 0.05f)
            {
                string activePanel = GetActivePanelName(controller);
                Debug.LogWarning(
                    $"[Stress] ⚠️ Lag spike at toggle {toggleCount}, frame {frameDuration:F3}s, active panel: {activePanel}"
                );
                lagCount++;
            }

            //Stress action (toggle loop)
            controller.SendMessage("OpenSettings");
            Debug.Log($"[Stress] ⚙️ Toggle {toggleCount}: Opened Settings.");
            yield return null;

            controller.SendMessage("BackToPause");
            Debug.Log($"[Stress] ↩️ Toggle {toggleCount}: Returned to Pause.");
            yield return null;

            toggleCount++;
        }

        //Summary of events
        Assert.AreEqual(0f, Time.timeScale, "[Stress] ❌ Game should remain paused during stress test.");
        controller.SendMessage("ResumeGame");
        Debug.Log("[Stress] ▶️ Game resumed.");
        yield return null;

        Debug.Log($"[Stress] ✅ RapidToggle test PASSED. Duration: 5s, Toggles: {toggleCount}, Lag spikes: {lagCount}.");
    }

    private string GetActivePanelName(PauseMenuController ctrl)
    {
        var root = ctrl.GetComponentInChildren<UIDocument>().rootVisualElement;
        if (root == null) return "Unknown";

        if (root.Q<VisualElement>("SettingsMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Settings";
        if (root.Q<VisualElement>("ControlsMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Controls";
        if (root.Q<VisualElement>("PauseMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Pause";

        return "NoneVisible";
    }
}
