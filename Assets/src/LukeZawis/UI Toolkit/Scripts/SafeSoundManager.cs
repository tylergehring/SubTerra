using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// Safe wrapper for the teammate’s SoundManager singleton.
/// Tries multiple strategies to set background volume:
/// 1) call SetBackgroundVolume(float) if present,
/// 2) set private field backgroundSource.volume if present,
/// 3) fallback to AudioListener.volume.
/// This file does not require modifying SoundManager.
/// </summary>
public static class SafeSoundManager
{
    private const string CLASS_NAME = "SoundManager";
    private static Type _type;
    private static PropertyInfo _instanceProp;
    private static MethodInfo _setVolumeMethod;
    private static FieldInfo _backgroundSourceField;

    private static void EnsureCache()
    {
        if (_type != null) return;

        // Try exact name first
        _type = Type.GetType(CLASS_NAME);

        // Try common namespaces
        if (_type == null)
        {
            string[] possible = {
                "Audio.SoundManager",
                "Managers.SoundManager",
                "Game.Audio.SoundManager",
                "Game.Managers.SoundManager"
            };
            foreach (var ns in possible)
            {
                _type = Type.GetType(ns);
                if (_type != null) break;
            }
        }

        if (_type == null)
        {
            Debug.LogWarning("SafeSoundManager: SoundManager type not found (will fallback to AudioListener.volume).");
            return;
        }

        // Cache Instance property (static)
        _instanceProp = _type.GetProperty("Instance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        // Cache SetBackgroundVolume method (instance, float) if any
        _setVolumeMethod = _type.GetMethod("SetBackgroundVolume",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, new[] { typeof(float) }, null);

        // Cache private field backgroundSource if present (AudioSource)
        _backgroundSourceField = _type.GetField("backgroundSource",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (_instanceProp == null) Debug.LogWarning($"SafeSoundManager: Instance property not found on {_type.FullName}.");
        if (_setVolumeMethod == null) Debug.Log($"SafeSoundManager: SetBackgroundVolume(float) not found on {_type.FullName} — will try backgroundSource or AudioListener.");
        if (_backgroundSourceField == null) Debug.Log($"SafeSoundManager: backgroundSource field not found on {_type.FullName} — will fallback to AudioListener.volume.");
    }

    /// <summary>
    /// Sets background volume using the best available mechanism.
    /// volume01 is 0..1 (UI slider normalised before calling).
    /// </summary>
    public static void SetBackgroundVolume(float volume01)
    {
        float clamped = Mathf.Clamp01(volume01);
        EnsureCache();

        // If SoundManager type is present, try more precise options
        if (_type != null && _instanceProp != null)
        {
            object instance = null;
            try
            {
                instance = _instanceProp.GetValue(null);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"SafeSoundManager: Failed to get Instance: {ex.GetBaseException().Message}");
            }

            if (instance != null)
            {
                // 1) Try calling SetBackgroundVolume(float)
                if (_setVolumeMethod != null)
                {
                    try
                    {
                        _setVolumeMethod.Invoke(instance, new object[] { clamped });
                        Debug.Log($"SafeSoundManager: Invoked SetBackgroundVolume({clamped}) on {_type.FullName}");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"SafeSoundManager: Invoke SetBackgroundVolume threw: {ex.GetBaseException().Message}");
                    }
                }

                // 2) Try to find private field backgroundSource and set its AudioSource.volume
                if (_backgroundSourceField != null)
                {
                    try
                    {
                        var fieldVal = _backgroundSourceField.GetValue(instance);
                        if (fieldVal is AudioSource src && src != null)
                        {
                            src.volume = clamped;
                            Debug.Log($"SafeSoundManager: Set backgroundSource.volume = {clamped} on {_type.FullName}");
                            return;
                        }
                        else
                        {
                            Debug.LogWarning("SafeSoundManager: backgroundSource field exists but is null or not an AudioSource.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"SafeSoundManager: Setting backgroundSource.volume threw: {ex.GetBaseException().Message}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("SafeSoundManager: SoundManager.Instance is null (will fallback to AudioListener.volume).");
            }
        }

        // 3) Fallback: set AudioListener.volume (global)
        AudioListener.volume = clamped;
        Debug.Log($"SafeSoundManager: Fallback AudioListener.volume = {clamped}");
    }
}
