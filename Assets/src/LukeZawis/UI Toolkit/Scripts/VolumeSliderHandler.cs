using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderHandler : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponent<Slider>();

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        SafeSoundManager.SetBackgroundVolume(value);
    }
}
