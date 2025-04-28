using UnityEngine;

// MonoBehaviour wrapper for the static Haptics class.
// Attach to a GameObject to trigger haptics via Unity Events.
// IMPORTANT: Ensure Haptics.Initialize() is called once during app startup.
public class EZHaptics : MonoBehaviour
{
    void Start()
    {
        HapticPlatform.Initialize();
         
        if (!HapticPlatform.IsInitialized)
        {
            Debug.LogWarning("HapticFeedbackTrigger: Haptics system was not initialized before use! Make sure to call Haptics.Initialize() at app startup.");
        }
    }
    // --- Unity Event Callable Methods ---
    public void TriggerNope()
    {
        Haptics.TriggerNope();
    }

    public void TriggerLightImpact()
    {
        Haptics.TriggerLightImpact();
    }

    public void TriggerMediumImpact()
    {
        Haptics.TriggerMediumImpact();
    }

    public void TriggerHeavyImpact()
    {
        Haptics.TriggerHeavyImpact();
    }

    public void TriggerSelectionChange()
    {
        Haptics.TriggerSelectionChange();
    }

    public void TriggerError()
    {
        Haptics.TriggerError();
    }

    public void TriggerSuccess()
    {
        Haptics.TriggerSuccess();
    }

    public void TriggerWarning()
    {
        Haptics.TriggerWarning();
    }

    public void TriggerRigidImpact()
    {
        Haptics.TriggerRigidImpact();
    }

    public void TriggerSoftImpact()
    {
        Haptics.TriggerSoftImpact();
    }

    public void Cancel()
    {
        Haptics.Cancel();
    }
    
}