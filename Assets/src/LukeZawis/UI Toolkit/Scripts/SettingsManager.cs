using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    // Single instance for the run (create in scene or auto-create)
    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("SettingsManager");
                _instance = go.AddComponent<SettingsManager>();
                DontDestroyOnLoad(go);
                _instance.LoadFromPrefs();
            }
            return _instance;
        }
    }

    // Events others subscribe to
    public event Action<float> OnVolumeChanged; // volume in 0..100
    public event Action<bool> OnGodModeChanged;

    // Backing fields
    private float _volume = 50f; // 0..100
    private bool _godMode = false;

    // Public getters
    public float Volume => _volume;
    public bool GodMode => _godMode;

    private const string PREF_VOLUME = "Volume";
    private const string PREF_GODMODE = "GodMode";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadFromPrefs();
    }

    public void LoadFromPrefs()
    {
        _volume = PlayerPrefs.GetFloat(PREF_VOLUME, 50f);
        _godMode = PlayerPrefs.GetInt(PREF_GODMODE, 0) == 1;
        // Ensure audio setter reflects loaded value
        SafeSoundManager.SetBackgroundVolume(_volume / 100f);
    }

    public void SetVolume(float volume00to100)
    {
        float clamped = Mathf.Clamp(volume00to100, 0f, 100f);
        if (Mathf.Approximately(clamped, _volume)) return;
        _volume = clamped;
        PlayerPrefs.SetFloat(PREF_VOLUME, _volume);
        PlayerPrefs.Save();
        // Notify listeners (UI will update), and apply audio
        OnVolumeChanged?.Invoke(_volume);
        SafeSoundManager.SetBackgroundVolume(_volume / 100f);
    }

    public void SetGodMode(bool enabled)
    {
        if (_godMode == enabled) return;
        _godMode = enabled;
        PlayerPrefs.SetInt(PREF_GODMODE, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnGodModeChanged?.Invoke(_godMode);
    }

    // Utility to force-send current values to new subscribers
    public void SendCurrentStateToSubscriber(Action<float> volumeCallback, Action<bool> godCallback)
    {
        volumeCallback?.Invoke(_volume);
        godCallback?.Invoke(_godMode);
    }
}
