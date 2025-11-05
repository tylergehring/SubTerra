using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 8-bit perfect integer scaling for UI Toolkit
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class PixelScaler : MonoBehaviour
{
    [SerializeField] private Vector2Int baseResolution = new Vector2Int(320, 240);
    private UIDocument uiDoc;
    private VisualElement root;
    private Vector2Int lastScreen;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) { Debug.LogError("PixelScaler: No UIDocument."); enabled = false; return; }

        root = uiDoc.rootVisualElement;
        if (root == null) { Debug.LogError("PixelScaler: No root."); enabled = false; return; }

        ApplyScale();
        lastScreen = new Vector2Int(Screen.width, Screen.height);
    }

    private void Update()
    {
        Vector2Int cur = new Vector2Int(Screen.width, Screen.height);
        if (cur != lastScreen) { lastScreen = cur; ApplyScale(); }
    }

    private void ApplyScale()
    {
        // Get device resolution
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float screenAspect = screenWidth / screenHeight;

        // Reference resolution from PanelSettings (1920x1080)
        Vector2 referenceResolution = new Vector2(1920, 1080);
        float referenceAspect = referenceResolution.x / referenceResolution.y;

        // Calculate scale based on aspect ratio
        float scale;
        if (screenAspect > referenceAspect)
        {
            // Wider screens (PC, tablet landscape)
            scale = screenHeight / referenceResolution.y;
        }
        else
        {
            // Taller screens (phone portrait)
            scale = screenWidth / referenceResolution.x;
        }

        // Adjust for pixel-perfect rendering
        int pixelScale = Mathf.FloorToInt(scale * (referenceResolution.y / baseResolution.y));
        pixelScale = Mathf.Max(1, pixelScale); // Ensure scale is at least 1

        // Apply scale to PanelSettings
        uiDoc.panelSettings.scale = pixelScale;
        uiDoc.panelSettings.referenceSpritePixelsPerUnit = Mathf.Max(1, Screen.dpi / 96f); // Adjust for DPI

        // Reset root scale to avoid double-scaling
        root.style.scale = new Scale(Vector3.one);

        Debug.Log($"[PixelScaler] {pixelScale}x scale applied, DPI: {Screen.dpi}");
    }
}