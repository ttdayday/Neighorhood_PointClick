using UnityEngine;

/// <summary>
/// Makes an object show a detailed close-up view when clicked.
/// Inherits from InteractableObject to reuse the existing click detection system!
/// </summary>
public class FlowerpotZoom : InteractableObject
{
    [Header("Close-Up Settings")]
    [Tooltip("The detailed sprite to show when inspecting this object")]
    public Sprite detailedSprite;

    /// <summary>
    /// Called when this object is clicked (from InteractableObject base class)
    /// </summary>
    protected override void OnInteractCustom()
    {
        // Check if we have a detailed sprite assigned
        if (detailedSprite == null)
        {
            Debug.LogWarning($"FlowerpotZoom: No detailed sprite assigned for {objectName}!");
            return;
        }

        // Check if CloseUpViewController exists
        if (CloseUpViewController.Instance == null)
        {
            Debug.LogWarning("FlowerpotZoom: CloseUpViewController not found in scene!");
            return;
        }

        // Show the close-up view with our detailed sprite
        Debug.Log($"Showing close-up view of {objectName}");
        CloseUpViewController.Instance.ShowCloseUp(detailedSprite);
    }
}
