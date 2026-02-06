using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager for close-up navigation and hotspot spawning
/// Maintains a navigation stack for proper back button behavior
/// Dynamically spawns and manages hotspot UI elements
/// </summary>
public class HotspotManager : MonoBehaviour
{
    // Singleton pattern (like ThoughtBubbleManager and CloseUpViewController)
    private static HotspotManager instance;
    public static HotspotManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HotspotManager>();
            }
            return instance;
        }
    }

    [Header("UI References")]
    [Tooltip("Prefab for hotspot UI (button with RectTransform)")]
    public GameObject hotspotPrefab;

    [Tooltip("Container for spawned hotspots (child of CloseUpCanvas)")]
    public RectTransform hotspotsContainer;

    [Header("Debug")]
    [Tooltip("Show debug logs for navigation and hotspot spawning")]
    public bool showDebugLogs = true;

    // Navigation stack - tracks history of viewed close-ups
    private Stack<CloseUpData> navigationStack = new Stack<CloseUpData>();

    // Currently displayed close-up
    private CloseUpData currentCloseUp = null;

    // Active hotspot UI instances
    private List<HotspotUI> activeHotspots = new List<HotspotUI>();

    void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        // Validate references
        if (hotspotPrefab == null)
        {
            Debug.LogError("HotspotManager: hotspotPrefab is not assigned!");
        }

        if (hotspotsContainer == null)
        {
            Debug.LogError("HotspotManager: hotspotsContainer is not assigned!");
        }
    }

    /// <summary>
    /// Shows a close-up view and pushes it to the navigation stack
    /// </summary>
    public void ShowCloseUp(CloseUpData data, bool addToStack = true)
    {
        if (data == null)
        {
            Debug.LogWarning("HotspotManager: Attempted to show null CloseUpData!");
            return;
        }

        if (!data.IsValid())
        {
            Debug.LogWarning($"HotspotManager: CloseUpData '{data.name}' has validation errors!");
            // Continue anyway - might still be usable
        }

        if (showDebugLogs)
        {
            Debug.Log($"HotspotManager: Showing close-up '{data.closeUpId}' (addToStack={addToStack})");
        }

        // Push to navigation stack if requested
        if (addToStack)
        {
            navigationStack.Push(data);
        }

        // Store current close-up
        currentCloseUp = data;

        // Hide thought bubble (if visible) before showing close-up
        if (ThoughtBubbleManager.Instance != null)
        {
            ThoughtBubbleManager.Instance.HideThought();
        }

        // Show the sprite via CloseUpViewController
        if (CloseUpViewController.Instance != null)
        {
            CloseUpViewController.Instance.ShowCloseUp(data.closeUpSprite);
        }
        else
        {
            Debug.LogError("HotspotManager: CloseUpViewController.Instance is null!");
        }

        // Spawn hotspots for this close-up
        SpawnHotspots(data);
    }

    /// <summary>
    /// Goes back to the previous close-up in the navigation stack
    /// If stack is empty, hides the close-up view entirely
    /// </summary>
    public void GoBack()
    {
        if (navigationStack.Count == 0)
        {
            if (showDebugLogs)
            {
                Debug.Log("HotspotManager: GoBack called but stack is empty - ignoring");
            }
            return;
        }

        // Pop current close-up from stack
        navigationStack.Pop();

        if (showDebugLogs)
        {
            Debug.Log($"HotspotManager: Going back (stack count now: {navigationStack.Count})");
        }

        // If stack still has items, show the previous close-up
        if (navigationStack.Count > 0)
        {
            CloseUpData previousCloseUp = navigationStack.Peek();
            ShowCloseUp(previousCloseUp, addToStack: false); // Don't push again!
        }
        else
        {
            // Stack is empty - hide close-up view and return to main scene
            if (showDebugLogs)
            {
                Debug.Log("HotspotManager: Stack empty, hiding close-up view");
            }

            currentCloseUp = null;
            ClearActiveHotspots();

            if (CloseUpViewController.Instance != null)
            {
                CloseUpViewController.Instance.HideCloseUp();
            }
        }
    }

    /// <summary>
    /// Checks if currently viewing a close-up (navigation stack not empty)
    /// </summary>
    public bool IsInCloseUp()
    {
        return navigationStack.Count > 0;
    }

    /// <summary>
    /// Gets the current close-up being viewed (or null if not in close-up)
    /// </summary>
    public CloseUpData GetCurrentCloseUp()
    {
        return currentCloseUp;
    }

    /// <summary>
    /// Clears the navigation stack and hides all close-ups
    /// Use with caution - can disrupt navigation flow
    /// </summary>
    public void ClearNavigation()
    {
        if (showDebugLogs)
        {
            Debug.Log("HotspotManager: Clearing navigation stack");
        }

        navigationStack.Clear();
        currentCloseUp = null;
        ClearActiveHotspots();

        if (CloseUpViewController.Instance != null)
        {
            CloseUpViewController.Instance.HideCloseUp();
        }
    }

    /// <summary>
    /// Spawns hotspot UI elements for the given close-up data
    /// Clears any existing hotspots first
    /// </summary>
    private void SpawnHotspots(CloseUpData closeUpData)
    {
        // Clear existing hotspots
        ClearActiveHotspots();

        if (closeUpData.hotspots == null || closeUpData.hotspots.Count == 0)
        {
            if (showDebugLogs)
            {
                Debug.Log($"HotspotManager: No hotspots to spawn for '{closeUpData.closeUpId}'");
            }
            return;
        }

        if (hotspotPrefab == null || hotspotsContainer == null)
        {
            Debug.LogError("HotspotManager: Cannot spawn hotspots - prefab or container not assigned!");
            return;
        }

        // Spawn each hotspot
        foreach (var hotspotData in closeUpData.hotspots)
        {
            if (!hotspotData.IsValid())
            {
                continue; // Skip invalid hotspots
            }

            // Instantiate hotspot UI
            GameObject hotspotObj = Instantiate(hotspotPrefab, hotspotsContainer);
            hotspotObj.name = $"Hotspot_{hotspotData.hotspotId}";

            // Initialize the hotspot component
            HotspotUI hotspotUI = hotspotObj.GetComponent<HotspotUI>();
            if (hotspotUI != null)
            {
                hotspotUI.Initialize(hotspotData, hotspotsContainer);
                activeHotspots.Add(hotspotUI);

                if (showDebugLogs)
                {
                    Debug.Log($"HotspotManager: Spawned hotspot '{hotspotData.hotspotId}' at {hotspotData.normalizedPosition}");
                }
            }
            else
            {
                Debug.LogError($"HotspotManager: Hotspot prefab doesn't have HotspotUI component!");
                Destroy(hotspotObj);
            }
        }
    }

    /// <summary>
    /// Destroys all active hotspot UI elements
    /// </summary>
    private void ClearActiveHotspots()
    {
        foreach (var hotspot in activeHotspots)
        {
            if (hotspot != null)
            {
                Destroy(hotspot.gameObject);
            }
        }

        activeHotspots.Clear();

        if (showDebugLogs && activeHotspots.Count > 0)
        {
            Debug.Log($"HotspotManager: Cleared {activeHotspots.Count} hotspots");
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
