using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages close-up views - shows a full-screen detailed image when inspecting objects
/// Similar to ThoughtBubbleManager but for full-screen inspection views
/// </summary>
public class CloseUpViewController : MonoBehaviour
{
    // Singleton pattern (like ThoughtBubbleManager!)
    private static CloseUpViewController instance;
    public static CloseUpViewController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CloseUpViewController>();
            }
            return instance;
        }
    }

    [Header("UI References")]
    [Tooltip("The Image component that displays the close-up sprite")]
    public Image closeUpImage;

    [Tooltip("The CanvasGroup for fade in/out")]
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("Duration of fade animations")]
    public float fadeDuration = 0.3f;

    private bool isVisible = false;
    private Coroutine activeCoroutine;
    private Sprite currentSprite = null;

    void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Make sure we have a CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // IMPORTANT: Hide immediately in Awake to prevent it from showing on startup
        HideImmediate();
    }

    void Start()
    {
        // Double-check it's hidden (in case something enabled it between Awake and Start)
        if (isVisible)
        {
            HideImmediate();
        }
    }

    /// <summary>
    /// Check if close-up view is currently visible
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }

    /// <summary>
    /// Check if a specific close-up is currently showing
    /// </summary>
    public bool IsShowingCloseUp(string spriteName)
    {
        return isVisible && currentSprite != null && currentSprite.name == spriteName;
    }

    /// <summary>
    /// Show a close-up view with the specified detailed sprite
    /// </summary>
    public void ShowCloseUp(Sprite detailedSprite)
    {
        if (detailedSprite == null)
        {
            Debug.LogWarning("CloseUpViewController: Sprite is null!");
            return;
        }

        if (closeUpImage == null)
        {
            Debug.LogError("CloseUpViewController: closeUpImage is not assigned!");
            return;
        }

        Debug.Log($"Showing close-up view with sprite: {detailedSprite.name}");

        // Stop any active animation
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        // Set the sprite and show
        closeUpImage.sprite = detailedSprite;
        closeUpImage.enabled = true;
        currentSprite = detailedSprite;

        activeCoroutine = StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Hide the close-up view and return to normal scene
    /// </summary>
    public void HideCloseUp()
    {
        Debug.Log("Hiding close-up view");

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        isVisible = true;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        HideImmediate();
    }

    private void HideImmediate()
    {
        canvasGroup.alpha = 0f;
        if (closeUpImage != null)
        {
            closeUpImage.enabled = false;
        }
        isVisible = false;
        currentSprite = null;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
