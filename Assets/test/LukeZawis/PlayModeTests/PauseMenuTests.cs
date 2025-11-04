/*
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UIElements;

public class PauseMenuTests
{
    private PauseMenuController controller;

    // --- Initial Setup ---

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        Debug.Log("[Test] 🔄 Initializing: loading PauseMenuTestScene...");
        yield return SceneManager.LoadSceneAsync("PauseMenuTestScene");
        yield return null;

        Debug.Log("[Test] ✅ Scene loaded.");
    }

    [SetUp]
    public void Setup()
    {
        controller = Object.FindFirstObjectByType<PauseMenuController>();
        Assert.IsNotNull(controller, "[Test] ❌ PauseMenuController not found in scene.");
        Debug.Log("[Test] ✅ Controller initialized successfully.");
    }

    // --- Boundary Test 1: Long Run Stability ---

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

            // Lag detection logic for warning
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

    // --- Boundary Test 2: Navigation Flow ---

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

    // --- Stress Test ---

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
        const int maxLagSpikes = 50;
        const float maxFrameDuration = 0.3f;

        while (true)
        {
            if (controller == null)
            {
                Assert.Fail($"[Stress] ❌ Test failed: PauseMenuController was unexpectedly destroyed at toggle {toggleCount}.");
            }

            float now = Time.realtimeSinceStartup;
            float frameDuration = now - lastFrameTime;
            lastFrameTime = now;

            // Lag spike detection logic for warning
            if (frameDuration > 0.05f)
            {
                string activePanel = GetActivePanelName(controller);
                Debug.LogWarning(
                    $"[Stress] ⚠️ Lag spike {frameDuration:F3}s, active panel: {activePanel}"
                );
                lagCount++;

                if (frameDuration > maxFrameDuration)
                {
                    Assert.Fail($"[Stress] ❌ Test failed: Massive lag spike ({frameDuration:F3}s) at toggle {toggleCount}.");
                }

                if (lagCount > maxLagSpikes)
                {
                    Assert.Fail($"[Stress] ❌ Test failed: Too many lag spikes ({lagCount}) at toggle {toggleCount}.");
                }

                bool isValidPanelActive = activePanel == "Pause" || activePanel == "Settings" || activePanel == "Controls";

                if (activePanel == "Unknown (No UI Root)" || activePanel == "NoneVisible")
                {
                    Debug.LogWarning($"[Stress] ⚠️ Transient UI instability detected at toggle {toggleCount}. Proceeding with TimeScale check...");
                }

                Assert.AreEqual(0f, Time.timeScale,
                                $"[Stress] ❌ Game unpaused! TimeScale modified to {Time.timeScale} at toggle {toggleCount}.");
            }

            try
            {
                PerformToggle(controller, toggleCount);
            }
            catch (System.Exception e)
            {
                Assert.Fail($"[Stress] ❌ Test failed due to exception at toggle {toggleCount}: {e.Message}. StackTrace: {e.StackTrace}");
            }

            Assert.AreEqual(0f, Time.timeScale, "[Stress] ❌ TimeScale was modified to non-zero during stress test (Post-Toggle Check).");

            toggleCount++;

            // UPDATED: Safety timeout is 15 seconds
            if (Time.realtimeSinceStartup - start > 15f)
            {
                Debug.LogWarning("[Stress] ⚠️ Test reached 15s safety timeout without failure.");
                break;
            }

            yield return null;
        }

        Assert.AreEqual(0f, Time.timeScale, "[Stress] ❌ Game should remain paused until resumed.");

        if (controller != null)
        {
            controller.SendMessage("ResumeGame");
            Debug.Log("[Stress] ▶️ Game resumed.");
            yield return null;
            Assert.AreEqual(1f, Time.timeScale, "[Stress] ❌ Game failed to resume after test completion.");
        }

        Debug.Log($"[Stress] ✅ RapidToggleUntilFailure test completed. Toggles: {toggleCount}, Lag spikes: {lagCount}, Duration: {(Time.realtimeSinceStartup - start):F2}s.");
    }

    // --- Helper Methods ---

    private void PerformToggle(PauseMenuController ctrl, int toggleCount)
    {
        if (ctrl == null) return;

        // Menu Logic Stress: Toggles settings 50 times in one frame.
        for (int i = 0; i < 50; i++)
        {
            ctrl.SendMessage("OpenSettings");
            ctrl.SendMessage("BackToPause");
        }

        // UI Rendering Stress: Creates and destroys UI elements every 20 frames (increased frequency).
        if (toggleCount % 20 == 0)
        {
            var root = ctrl.GetComponentInChildren<UIDocument>()?.rootVisualElement;
            if (root == null) return;

            const int stressElementCount = 100;

            var stressContainer = new VisualElement();
            root.Add(stressContainer);

            for (int i = 0; i < stressElementCount; i++)
            {
                var tempElement = new VisualElement();
                tempElement.style.width = 1;
                tempElement.style.height = 1;
                stressContainer.Add(tempElement);
            }

            root.Remove(stressContainer);
        }
    }

    private string GetActivePanelName(PauseMenuController ctrl)
    {
        var root = ctrl.GetComponentInChildren<UIDocument>()?.rootVisualElement;

        if (root == null) return "Unknown (No UI Root)";

        if (root.Q<VisualElement>("SettingsMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Settings";
        if (root.Q<VisualElement>("ControlsMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Controls";
        if (root.Q<VisualElement>("PauseMenuPanel")?.resolvedStyle.display == DisplayStyle.Flex)
            return "Pause";

        return "NoneVisible";
    }
}

*/