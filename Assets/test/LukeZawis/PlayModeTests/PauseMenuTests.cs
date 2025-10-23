using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class PauseMenuTests
{
    private PauseMenuController controller;

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        Debug.Log("[Test] 🔄 Initializing: loading PauseMenuTestScene...");
        SceneManager.LoadScene("PauseMenuTestScene");
        yield return new WaitForSecondsRealtime(0.1f); // Ensure scene loads fully
        Debug.Log("[Test] ✅ Scene loaded.");
    }

    [SetUp]
    public void Setup()
    {
        controller = Object.FindFirstObjectByType<PauseMenuController>();
        Assert.IsNotNull(controller, "[Test] ❌ PauseMenuController not found in scene.");
        Debug.Log("[Test] ✅ Controller initialized successfully.");
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

    [UnityTest]
    public IEnumerator Stress_RapidToggleUntilFailure()
    {
        Debug.Log("[Stress] ▶️ Starting RapidToggleUntilFailure test...");
        controller.SendMessage("PauseGame");
        Debug.Log("[Stress] ⏸ Game paused.");
        yield return null;

        float start = Time.realtimeSinceStartup;
        float lastFrameTime = Time.realtimeSinceStartup;
        int toggleCount = 0;
        int lagCount = 0;
        const int maxLagSpikes = 100; // Failure condition: too many lag spikes
        const float maxFrameDuration = 0.5f; // Failure condition: single massive lag spike

        while (true)
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

                if (frameDuration > maxFrameDuration)
                {
                    Assert.Fail($"[Stress] ❌ Test failed: Massive lag spike ({frameDuration:F3}s) detected at toggle {toggleCount}.");
                }

                if (lagCount > maxLagSpikes)
                {
                    Assert.Fail($"[Stress] ❌ Test failed: Too many lag spikes ({lagCount}) detected at toggle {toggleCount}.");
                }
            }

            try
            {
                PerformToggle(controller, toggleCount);
            }
            catch (System.Exception e)
            {
                Assert.Fail($"[Stress] ❌ Test failed due to exception at toggle {toggleCount}: {e.Message}");
            }

            toggleCount++;

            if (Time.realtimeSinceStartup - start > 60f)
            {
                Debug.LogWarning("[Stress] ⚠️ Test reached 60s safety timeout without failure.");
                break;
            }

            yield return null;
        }

        Assert.AreEqual(0f, Time.timeScale, "[Stress] ❌ Game should remain paused during stress test.");
        controller.SendMessage("ResumeGame");
        Debug.Log("[Stress] ▶️ Game resumed.");
        yield return null;

        Debug.Log($"[Stress] ✅ RapidToggleUntilFailure test completed. Toggles: {toggleCount}, Lag spikes: {lagCount}, Duration: {(Time.realtimeSinceStartup - start):F2}s.");
    }

    private void PerformToggle(PauseMenuController ctrl, int toggleCount)
    {
        // Perform multiple toggles to amplify stress
        for (int i = 0; i < 10; i++)
        {
            ctrl.SendMessage("OpenSettings");
            if (toggleCount % 10 == 0) Debug.Log($"[Stress] ⚙️ Toggle {toggleCount}.{i}: Opened Settings.");
            ctrl.SendMessage("BackToPause");
            if (toggleCount % 10 == 0) Debug.Log($"[Stress] ↩️ Toggle {toggleCount}.{i}: Returned to Pause.");
        }
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