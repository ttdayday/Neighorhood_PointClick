using UnityEngine;

/// <summary>
/// Defines the type of interaction a hotspot performs when clicked
/// </summary>
public enum HotspotType
{
    NestedCloseUp  // Shows another close-up view (for nested inspection)
}

/// <summary>
/// Serializable data structure defining a clickable hotspot within a close-up view
/// Uses normalized coordinates (0-1) for resolution-independent positioning
/// </summary>
[System.Serializable]
public class HotspotData
{
    [Header("Identity")]
    [Tooltip("Unique identifier for this hotspot")]
    public string hotspotId = "NewHotspot";

    [Header("Position (Normalized 0-1)")]
    [Tooltip("Position of hotspot center (0,0 = bottom-left, 1,1 = top-right)")]
    public Vector2 normalizedPosition = new Vector2(0.5f, 0.5f);

    [Tooltip("Size of hotspot clickable area (normalized)")]
    public Vector2 normalizedSize = new Vector2(0.1f, 0.1f);

    [Header("Interaction")]
    [Tooltip("What happens when this hotspot is clicked")]
    public HotspotType type = HotspotType.NestedCloseUp;

    [Header("Nested Close-Up Settings")]
    [Tooltip("The close-up to show when clicked (for NestedCloseUp type)")]
    public CloseUpData targetCloseUp;

    /// <summary>
    /// Checks if this hotspot has valid configuration
    /// </summary>
    public bool IsValid()
    {
        if (type == HotspotType.NestedCloseUp && targetCloseUp == null)
        {
            Debug.LogWarning($"Hotspot '{hotspotId}' is NestedCloseUp type but has no targetCloseUp assigned!");
            return false;
        }
        return true;
    }
}
