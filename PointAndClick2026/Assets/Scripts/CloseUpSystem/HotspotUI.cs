using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime UI component for clickable hotspots in close-up views
/// Spawned dynamically by HotspotManager based on HotspotData
/// Positions itself using normalized coordinates (0-1) for resolution independence
/// </summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class HotspotUI : MonoBehaviour
{
    private Button button;
    private RectTransform rectTransform;
    private HotspotData data;

    [Header("Debug")]
    [Tooltip("Show visual debug border around hotspot")]
    public bool showDebugBorder = false;

    private Image debugImage;

    void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();

        // Hook up button click
        button.onClick.AddListener(OnHotspotClicked);
    }

    /// <summary>
    /// Initializes this hotspot UI from data
    /// Called by HotspotManager after instantiation
    /// </summary>
    public void Initialize(HotspotData hotspotData, RectTransform parentRect)
    {
        if (hotspotData == null)
        {
            Debug.LogError("HotspotUI: Attempted to initialize with null data!");
            return;
        }

        data = hotspotData;

        // Position the hotspot using normalized coordinates
        PositionHotspot(parentRect);

        // Optional: Setup debug visualization
        if (showDebugBorder)
        {
            SetupDebugVisualization();
        }
    }

    /// <summary>
    /// Positions this hotspot based on normalized coordinates (0-1)
    /// Converts normalized position to anchored position within parent rect
    /// </summary>
    private void PositionHotspot(RectTransform parentRect)
    {
        if (rectTransform == null || parentRect == null)
        {
            Debug.LogError("HotspotUI: RectTransform or parent is null!");
            return;
        }

        // Set anchors to bottom-left (0,0) so position calculations are simpler
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot

        // Get parent dimensions
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // Convert normalized position (0-1) to actual pixel position
        float pixelX = data.normalizedPosition.x * parentWidth;
        float pixelY = data.normalizedPosition.y * parentHeight;

        rectTransform.anchoredPosition = new Vector2(pixelX, pixelY);

        // Convert normalized size (0-1) to actual pixel size
        float pixelWidth = data.normalizedSize.x * parentWidth;
        float pixelHeight = data.normalizedSize.y * parentHeight;

        rectTransform.sizeDelta = new Vector2(pixelWidth, pixelHeight);

        Debug.Log($"HotspotUI '{data.hotspotId}': Positioned at ({pixelX}, {pixelY}) with size ({pixelWidth}, {pixelHeight})");
    }

    /// <summary>
    /// Called when the hotspot button is clicked
    /// Handles interaction based on hotspot type
    /// </summary>
    private void OnHotspotClicked()
    {
        if (data == null)
        {
            Debug.LogError("HotspotUI: Data is null on click!");
            return;
        }

        Debug.Log($"HotspotUI: Clicked hotspot '{data.hotspotId}' (type: {data.type})");

        // Handle different interaction types
        switch (data.type)
        {
            case HotspotType.NestedCloseUp:
                HandleNestedCloseUp();
                break;

            default:
                Debug.LogWarning($"HotspotUI: Unhandled hotspot type: {data.type}");
                break;
        }
    }

    /// <summary>
    /// Handles NestedCloseUp type - shows another close-up view
    /// </summary>
    private void HandleNestedCloseUp()
    {
        if (data.targetCloseUp == null)
        {
            Debug.LogWarning($"HotspotUI '{data.hotspotId}': NestedCloseUp type but targetCloseUp is null!");
            return;
        }

        if (HotspotManager.Instance == null)
        {
            Debug.LogError("HotspotUI: HotspotManager.Instance is null!");
            return;
        }

        // Show the target close-up (adds to navigation stack)
        HotspotManager.Instance.ShowCloseUp(data.targetCloseUp);
    }

    /// <summary>
    /// Sets up debug visualization (colored border)
    /// </summary>
    private void SetupDebugVisualization()
    {
        // Add Image component for visual debugging
        debugImage = gameObject.GetComponent<Image>();
        if (debugImage == null)
        {
            debugImage = gameObject.AddComponent<Image>();
        }

        // Semi-transparent colored border
        debugImage.color = new Color(1f, 0f, 0f, 0.3f); // Red with 30% opacity
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnHotspotClicked);
        }
    }
}
