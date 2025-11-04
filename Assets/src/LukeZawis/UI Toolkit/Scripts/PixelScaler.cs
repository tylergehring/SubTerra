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
        float sx = (float)Screen.width / baseResolution.x;
        float sy = (float)Screen.height / baseResolution.y;
        int scale = Mathf.FloorToInt(Mathf.Min(sx, sy));

        root.style.scale = new Scale(new Vector3(scale, scale, 1f));
        Debug.Log($"[PixelScaler] {scale}x scale applied");
    }
}