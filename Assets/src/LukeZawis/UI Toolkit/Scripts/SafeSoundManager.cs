using UnityEngine;

/// <summary>
/// Safe wrapper for the teammate’s SoundManager singleton.
/// Works even if the class is in a different folder/namespace
/// and will never cause a compile error.
/// </>
public static class SafeSoundManager
{
    private const string CLASS_NAME = "SoundManager";

    private static System.Type _type;
    private static System.Reflection.PropertyInfo _instanceProp;
    private static System.Reflection.MethodInfo _setVolumeMethod;

    private static void EnsureCache()
    {
        if (_type != null) return;

        // 1. Try the exact name
        _type = System.Type.GetType(CLASS_NAME);

        // 2. If not found, try common namespaces the team uses
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
                _type = System.Type.GetType(ns);
                if (_type != null) break;
            }
        }

        if (_type == null) return;   // teammate class not in build yet → silent

        // Cache Instance property (static)
        _instanceProp = _type.GetProperty("Instance",
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        // Cache SetBackgroundVolume method (instance, float)
        _setVolumeMethod = _type.GetMethod("SetBackgroundVolume",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic,
            null, new[] { typeof(float) }, null);
    }

    public static void SetBackgroundVolume(float volume01)
    {
        EnsureCache();

        if (_type == null || _instanceProp == null || _setVolumeMethod == null) return;

        var instance = _instanceProp.GetValue(null);
        if (instance == null) return;

        _setVolumeMethod.Invoke(instance, new object[] { Mathf.Clamp01(volume01) });
    }
}