using UnityEngine;

// Public interface for triggering defined haptic feedback patterns.
// Calls the HapticPlatform to execute platform-specific vibrations.
public static class Haptics
{
    // --- Haptic Design Parameters (Adjust as needed) ---
    // Durations (ms)
    private const long DurationVeryShort = 15;
    private const long DurationShort = 30;
    private const long DurationDefault = 45;
    private const long DurationMedium = 65;
    private const long DurationLong = 100;
    // Amplitudes (1-255 for Android API 26+, -1 for default)
    private const int AmplitudeSoft = 40;
    private const int AmplitudeLight = 85;
    private const int AmplitudeMedium = 155;
    private const int AmplitudeRigid = 210;
    private const int AmplitudeHeavy = 255;
    // Define patterns here where needed
    private static readonly long[] PatternNope = { 0, DurationShort, 100, DurationShort, 100, DurationShort };
    private static readonly int[] AmplitudeNope = { 0, AmplitudeLight, 0, AmplitudeMedium, 0, AmplitudeHeavy };
    
    private static readonly long[] PatternError = { 0, DurationShort, 80, DurationShort, 80, DurationMedium, 80, DurationVeryShort};
    private static readonly int[] AmplitudeError = { 0, AmplitudeMedium, 0, AmplitudeMedium, 0, AmplitudeHeavy, 0, AmplitudeLight };
    
    private static readonly long[] PatternSuccess = { 0, 30, 200, 30 };
    private static readonly int[] AmplitudeSuccess = { 0, AmplitudeSoft, 0, AmplitudeRigid };
    
    private static readonly long[] PatternWarning = { 0, 30, 200, 30 }; //same duration as success
    private static readonly int[] AmplitudeWarning = { 0,AmplitudeRigid, 0, AmplitudeSoft }; //reversed amplitude compared to success

    // --- Public Methods for Triggering Haptics ---

    // Call this once at app start
    public static void Initialize() {
        HapticPlatform.Initialize();
    }

    // Check if haptics are possible
    public static bool CanVibrate() {
        return HapticPlatform.CanVibrate();
    }
    
    public static void TriggerLightImpact() { // iOS Light Impact
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteImpactIOS(ImpactFeedbackStyle.Light);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationShort, AmplitudeLight);
        #endif
    }

    public static void TriggerMediumImpact() { // iOS Medium Impact
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteImpactIOS(ImpactFeedbackStyle.Medium);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationDefault, AmplitudeMedium);
        #endif
    }

    public static void TriggerHeavyImpact() { // iOS Heavy Impact
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteImpactIOS(ImpactFeedbackStyle.Heavy);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationDefault, AmplitudeHeavy);
        #endif
    }
    public static void TriggerRigidImpact() { // iOS Rigid Impact
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteImpactIOS(ImpactFeedbackStyle.Rigid);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationVeryShort, AmplitudeRigid);
        #endif
    }

    public static void TriggerSoftImpact() { // iOS Soft Impact
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteImpactIOS(ImpactFeedbackStyle.Soft);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationDefault, AmplitudeSoft);
        #endif
    }

    public static void TriggerSelectionChange() { // iOS Selection Changes
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteSelectionIOS();
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteOneShotAndroid(DurationVeryShort, AmplitudeSoft);
        #endif
    }

    public static void TriggerError() { // iOS Error Notification
         if (!HapticPlatform.IsInitialized) Initialize();
         if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteNotificationIOS(NotificationFeedbackStyle.Error);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteWaveformAndroid(PatternError, AmplitudeError, -1); // Nope and Error appear to be the same in iOS
        #endif
    }

    public static void TriggerSuccess() { // iOS Success Notification
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteNotificationIOS(NotificationFeedbackStyle.Success);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteWaveformAndroid(PatternSuccess, AmplitudeSuccess, -1); 
        #endif
    }

    public static void TriggerWarning() { // iOS Warning Notification
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteNotificationIOS(NotificationFeedbackStyle.Warning);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteWaveformAndroid(PatternWarning, AmplitudeWarning, -1);
        #endif
    }
    
    public static void TriggerNope() { // iOS Nope Notification
        if (!HapticPlatform.IsInitialized) Initialize();
        if (!Application.isMobilePlatform) return;
        #if UNITY_IOS
        HapticPlatform.ExecuteNotificationIOS(NotificationFeedbackStyle.Error);
        #elif UNITY_ANDROID
        HapticPlatform.ExecuteWaveformAndroid(PatternError, AmplitudeError, -1); // Nope and Error appear to be the same in iOS
        #endif
    }


    // Method to stop any repeating vibrations (Android only)
    public static void Cancel() {
        if (!HapticPlatform.IsInitialized) Initialize();
        #if UNITY_ANDROID
        HapticPlatform.CancelAndroidVibration();
        #endif
    }
}