using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
#if UNITY_ANDROID
using System;
#endif

// Internal engine for handling platform-specific haptic/vibration calls.
// Should be accessed primarily via the Haptics class.
internal static class HapticPlatform
{
    // --- iOS Specific Imports ---
#if UNITY_IOS
    [DllImport("__Internal")] private static extern bool _HasVibrator();
    [DllImport("__Internal")] private static extern void _Vibrate();
    [DllImport("__Internal")] private static extern void _impactOccurred(string style);
    [DllImport("__Internal")] private static extern void _notificationOccurred(string style);
    [DllImport("__Internal")] private static extern void _selectionChanged();
#endif

    // --- Android Specific Variables ---
#if UNITY_ANDROID
    private static AndroidJavaClass unityPlayer = null;
    private static AndroidJavaObject currentActivity = null;
    private static AndroidJavaObject vibrator = null;
    private static AndroidJavaObject context = null;
    private static AndroidJavaClass vibrationEffect = null;
    private static bool androidSupportsAmplitude = false;
#endif

    private static bool isInitialized = false;
    private const int AmplitudeDefault = -1; // Keep default amplitude definition here

    // --- Properties ---
    public static bool IsInitialized => isInitialized;
    public static bool SupportsAmplitudeControl =>
#if UNITY_ANDROID
        androidSupportsAmplitude;
#else
        false; // iOS doesn't use numeric amplitude control in this way
#endif

    // Initializes platform-specific components. MUST be called before use.
    public static void Initialize()
    {
        if (isInitialized) return;

#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

                if (AndroidVersion >= 26)
                {
                    try {
                        vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
                        androidSupportsAmplitude = (vibrator != null && vibrator.Call<bool>("hasAmplitudeControl"));
                    } catch (Exception) {
                        vibrationEffect = null;
                        androidSupportsAmplitude = false;
                    }
                } else {
                    androidSupportsAmplitude = false;
                }
                 if (vibrator == null) { // Check if service itself failed
                    Debug.LogError("HapticPlatform: Vibrator service is null after initialization attempt.");
                    isInitialized = true; // Mark as initialized to prevent retries, but it won't work
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"HapticPlatform Init Failed: {e.Message}");
                vibrator = null; // Ensure vibrator is null if init fails
                isInitialized = true; // Mark as initialized to prevent retries
                return;
            }
        }
#endif
        isInitialized = true;
        Debug.Log($"HapticPlatform Initialized. Platform: {Application.platform}. Supports Amplitude (Android): {SupportsAmplitudeControl}");
    }

    // --- Platform Agnostic Methods ---

    // Checks if a vibrator/haptic engine exists.
    public static bool CanVibrate()
    {
        if (!isInitialized) { Debug.LogWarning("HapticPlatform.CanVibrate called before Initialize()."); return false; }

        if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            if (vibrator != null) {
                try { return vibrator.Call<bool>("hasVibrator"); }
                catch (Exception) { return true; } // Assume yes if method missing but object exists
            } else { return false; }
#else
            return false;
#endif
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return _HasVibrator();
#else
            return false;
#endif
        }
        return false;
    }

     // Generic vibration call (use Handheld.Vibrate for simplicity or map to a default)
    public static void SimpleVibrate() {
        if (Application.isMobilePlatform) Handheld.Vibrate();
    }


    // --- iOS Specific Execution ---
#if UNITY_IOS
    public static void ExecuteImpactIOS(ImpactFeedbackStyle style) {
        if (!isInitialized || Application.platform != RuntimePlatform.IPhonePlayer) return;
        #if !UNITY_EDITOR
        _impactOccurred(style.ToString());
        #endif
    }
    public static void ExecuteNotificationIOS(NotificationFeedbackStyle style) {
         if (!isInitialized || Application.platform != RuntimePlatform.IPhonePlayer) return;
        #if !UNITY_EDITOR
        _notificationOccurred(style.ToString());
        #endif
    }
    public static void ExecuteSelectionIOS() {
         if (!isInitialized || Application.platform != RuntimePlatform.IPhonePlayer) return;
        #if !UNITY_EDITOR
        _selectionChanged();
        #endif
    }
#endif

    // --- Android Specific Execution ---
#if UNITY_ANDROID
    public static void ExecuteOneShotAndroid(long milliseconds, int amplitude) {
        if (!isInitialized || Application.platform != RuntimePlatform.Android || vibrator == null) return;

        if (vibrationEffect != null && androidSupportsAmplitude) {
            if (amplitude == 0) amplitude = AmplitudeDefault;
            amplitude = Mathf.Clamp(amplitude, -1, 255);
             if (amplitude == 0) amplitude = 1;

            try {
                AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", effect);
            } catch (Exception e) {
                Debug.LogError($"HapticPlatform Error (createOneShot): {e.Message}. Falling back.");
                ExecuteOneShotAndroid(milliseconds, AmplitudeDefault); // Retry with default
            }
        } else { // Fallback
            try { vibrator.Call("vibrate", milliseconds); }
            catch (Exception e) { Debug.LogError($"HapticPlatform Error (simple): {e.Message}"); }
        }
    }

    public static void ExecuteWaveformAndroid(long[] pattern, int[] amplitudes, int repeat) {
         if (!isInitialized || Application.platform != RuntimePlatform.Android || vibrator == null) return;
         if (pattern == null || pattern.Length < 2) { Debug.LogError("HapticPlatform: Pattern needs >= 2 elements."); return; }

        if (vibrationEffect != null && androidSupportsAmplitude) {
            if (amplitudes != null && amplitudes.Length != pattern.Length) {
                Debug.LogWarning("HapticPlatform: Pattern/Amplitude length mismatch. Using default amplitudes.");
                amplitudes = null;
            }
            try {
                AndroidJavaObject effect;
                if (amplitudes != null) {
                    for (int i = 0; i < amplitudes.Length; i++) { amplitudes[i] = Mathf.Clamp(amplitudes[i], 0, 255); }
                    effect = vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, amplitudes, repeat);
                } else {
                    effect = vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
                }
                vibrator.Call("vibrate", effect);
            } catch (Exception e) {
                 Debug.LogError($"HapticPlatform Error (createWaveform): {e.Message}. Falling back.");
                 ExecuteWaveformAndroid(pattern, repeat); // Fallback to non-amplitude waveform
            }
        } else { // Fallback
            ExecuteWaveformAndroid(pattern, repeat); // Call simpler version
        }
    }

     // Waveform with default amplitude (handles fallback internally too)
    public static void ExecuteWaveformAndroid(long[] pattern, int repeat) {
        if (!isInitialized || Application.platform != RuntimePlatform.Android || vibrator == null) return;
        if (pattern == null || pattern.Length < 2) { Debug.LogError("HapticPlatform: Pattern needs >= 2 elements."); return; }

        if (vibrationEffect != null) { // API 26+
            try {
                AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
                vibrator.Call("vibrate", effect);
            } catch (Exception e) {
                Debug.LogError($"HapticPlatform Error (default amp waveform): {e.Message}. Falling back to legacy.");
                 try { vibrator.Call("vibrate", pattern, repeat); } // Legacy fallback
                 catch (Exception e2) { Debug.LogError($"HapticPlatform Error (legacy pattern): {e2.Message}"); }
            }
        } else { // Pre-API 26
             try { vibrator.Call("vibrate", pattern, repeat); }
             catch (Exception e) { Debug.LogError($"HapticPlatform Error (legacy pattern): {e.Message}"); }
        }
    }

    // Cancels ongoing Android vibration.
    public static void CancelAndroidVibration() {
         if (Application.platform == RuntimePlatform.Android && vibrator != null && isInitialized) {
             try { vibrator.Call("cancel"); }
             catch (Exception e) { Debug.LogError($"HapticPlatform Cancel Error: {e.Message}"); }
        }
    }

     // Gets Android API level.
    public static int AndroidVersion {
        get {
            if (Application.platform != RuntimePlatform.Android) return 0;
            int apiLevel = 0;
            try {
                using (var version = new AndroidJavaClass("android.os.Build$VERSION")) {
                    apiLevel = version.GetStatic<int>("SDK_INT");
                }
            } catch (Exception) { /* Fallback parsing if needed */ }
            return apiLevel;
        }
    }
#endif
}

// --- Enums for iOS types
public enum ImpactFeedbackStyle { Heavy, Medium, Light, Rigid, Soft }
public enum NotificationFeedbackStyle { Error, Success, Warning }