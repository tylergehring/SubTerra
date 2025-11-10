using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI displayText;
    private float timer;
    private float fps;

    private void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        fps = Mathf.Lerp(fps, 1.0f / Time.deltaTime, Time.deltaTime);

        displayText.text = Mathf.Ceil(fps).ToString();

    }
}