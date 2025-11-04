using UnityEngine;
using UnityEngine.UIElements;

public class PixelScaler : MonoBehaviour
{
    [Header("Pixel Base Resolution")]
    public Vector2Int baseRes = new Vector2Int(320, 240);

    private UIDocument uiDoc;
    private VisualElement root;
    private Vector2Int lastScreenRes;

    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("PixelScaler: No UIDocument found.");
            return;
        }
        root = uiDoc.rootVisualElement;
        ScalePixelPerfect();
    }

    void Update()
    {
        Vector2Int currRes = new Vector2Int(Screen.width, Screen.height);
        if (currRes != lastScreenRes)
        {
            lastScreenRes = currRes;
            ScalePixelPerfect();
        }
    }

    private void ScalePixelPerfect()
    {
        float scaleX = (float)Screen.width / baseRes.x;
        float scaleY = (float)Screen.height / baseRes.y;
        int scale = Mathf.FloorToInt(Mathf.Min(scaleX, scaleY));
        root.style.scale = new Scale(new Vector3(scale, scale, 1));
        Debug.Log($"PixelScaler: Applied scale {scale}x");
    }
}