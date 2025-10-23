using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class PauseMenuBoundaryTests
{
    private PauseMenuController controller;

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        Debug.Log("[Boundary] 🔄 Initializing: loading PauseMenuTestScene...");
        SceneManager.LoadScene("PauseMenuTestScene");
        yield return null;
        Debug.Log("[Boundary] ✅ Scene loaded.");
    }

    [SetUp]
    public void Setup()
    {
        controller = Object.FindFirstObjectByType<PauseMenuController>();
        Assert.IsNotNull(controller, "[Boundary] ❌ PauseMenuController not found in scene.");
        Debug.Log("[Boundary] ✅ Controller initialized successfully.");
    }

    [UnityTest]
    public IEnumerator Boundary_LongRunForFiveSecondsWithLagDetection()
    {
        Debug.Log("[Boundary] ▶️ Starting LongRun test...");
        controller.SendMessage("PauseGame");
        Debug.Log("[Boundary] ⏸ Game paused.");
        yield return null;

        float start = Time.realtimeSinceStartup;
        float lastFrameTime = Time.realtimeSinceStartup;
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
                    $"[Boundary] ⚠️ Lag spike {frameDuration:F3}s, active panel: {activePanel}"
                );
                lagCount++;
            }

            Assert.AreEqual(0f, Time.timeScale, "[Boundary] ❌ Game should remain paused.");
            yield return null;
        }

        controller.SendMessage("ResumeGame");
        Debug.Log("[Boundary] ▶️ Game resumed.");
        yield return null;

        Assert.AreEqual(1f, Time.timeScale, "[Boundary] ❌ Game should resume after long run.");
        Debug.Log($"[Boundary] ✅ LongRun test PASSED. Duration: 5s, Lag spikes: {lagCount}.");
    }

    [UnityTest]
    public IEnumerator Boundary_EscapeFlow()
    {
        Debug.Log("[Boundary] ▶️ Starting EscapeFlow test...");
        controller.SendMessage("PauseGame");
        Debug.Log("[Boundary] ⏸ Game paused.");
        yield return null;

        controller.SendMessage("OpenSettings");
        Debug.Log("[Boundary] ⚙️ Settings opened.");
        yield return null;

        controller.SendMessage("BackToPause");
        Debug.Log("[Boundary] ↩️ Returned to Pause menu.");
        yield return null;

        controller.SendMessage("ResumeGame");
        Debug.Log("[Boundary] ▶️ Game resumed.");
        yield return null;

        Assert.AreEqual(1f, Time.timeScale, "[Boundary] ❌ Game should resume after ESC flow.");
        Debug.Log("[Boundary] ✅ EscapeFlow test PASSED.");
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

