using UnityEngine;

/// <summary>
/// Makes a scene object show a close-up view when clicked
/// Replaces FlowerpotZoom.cs with a data-driven approach
/// Inherits from InteractableObject to use existing click detection system
/// </summary>
public class CloseUpInteractable : InteractableObject
{
    [Header("Close-Up Settings")]
    [Tooltip("The close-up data asset to show when this object is clicked")]
    public CloseUpData closeUpData;

    /// <summary>
    /// Called when this object is clicked (via ClickDetector → InteractableObject)
    /// Overrides the base class method to show close-up view
    /// </summary>
    protected override void OnInteractCustom()
    {
        if (closeUpData == null)
        {
            Debug.LogWarning($"CloseUpInteractable on '{gameObject.name}': closeUpData is not assigned!");
            return;
        }

        if (HotspotManager.Instance == null)
        {
            Debug.LogError("CloseUpInteractable: HotspotManager.Instance is null!");
            return;
        }

        // Show the close-up view via HotspotManager (adds to navigation stack)
        HotspotManager.Instance.ShowCloseUp(closeUpData);
    }
}
