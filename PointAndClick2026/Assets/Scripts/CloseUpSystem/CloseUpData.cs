using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject defining a close-up view and its interactive hotspots
/// Create new assets via: Assets > Create > Close-Up System > Close-Up Data
/// </summary>
[CreateAssetMenu(fileName = "NewCloseUp", menuName = "Close-Up System/Close-Up Data", order = 1)]
public class CloseUpData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique identifier for this close-up")]
    public string closeUpId = "new_closeup";

    [Header("Visual")]
    [Tooltip("The full-screen sprite image shown for this close-up view")]
    public Sprite closeUpSprite;

    [Header("Interactive Hotspots")]
    [Tooltip("List of clickable hotspots that appear on this close-up")]
    public List<HotspotData> hotspots = new List<HotspotData>();

    [Header("Optional")]
    [TextArea(2, 4)]
    [Tooltip("Description or notes about this close-up (for development only)")]
    public string description;

    /// <summary>
    /// Validates this close-up configuration
    /// </summary>
    public bool IsValid()
    {
        if (closeUpSprite == null)
        {
            Debug.LogError($"CloseUpData '{name}' has no sprite assigned!");
            return false;
        }

        if (string.IsNullOrEmpty(closeUpId))
        {
            Debug.LogWarning($"CloseUpData '{name}' has no closeUpId set!");
        }

        // Validate all hotspots
        foreach (var hotspot in hotspots)
        {
            if (!hotspot.IsValid())
            {
                Debug.LogWarning($"CloseUpData '{name}' has invalid hotspot: {hotspot.hotspotId}");
            }
        }

        return true;
    }

    /// <summary>
    /// Called when asset is created or loaded in editor
    /// </summary>
    private void OnValidate()
    {
        // Auto-generate closeUpId from asset name if empty
        if (string.IsNullOrEmpty(closeUpId))
        {
            closeUpId = name.ToLower().Replace(" ", "_");
        }
    }
}
