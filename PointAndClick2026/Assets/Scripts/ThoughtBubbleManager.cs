using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThoughtBubbleManager : MonoBehaviour
{
    private static ThoughtBubbleManager instance;
    public static ThoughtBubbleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ThoughtBubbleManager>();
            }
            return instance;
        }
    }

    [Header("UI References")]
    [Tooltip("The Image component for the bubble sprite")]
    public Image bubbleImage;

    [Tooltip("The Image component for the icon sprite")]
    public Image iconImage;

    [Tooltip("The CanvasGroup for fading")]
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("Duration the bubble stays visible")]
    public float displayDuration = 2f;

    [Tooltip("Duration of fade in/out animations")]
    public float fadeDuration = 0.3f;

    [Tooltip("Auto-hide after displayDuration? If false, bubble stays visible until manually hidden.")]
    public bool autoHide = false;

    private Coroutine activeCoroutine;
    private bool isVisible = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Preserve aspect ratio for icons
        if (iconImage != null)
        {
            iconImage.preserveAspect = true;
        }

        HideBubble();
    }

    public bool IsVisible()
    {
        return isVisible;
    }

    public void ShowThought(Sprite iconSprite)
    {
        if (iconSprite == null)
        {
            Debug.LogWarning("ThoughtBubbleManager: Icon sprite is null!");
            return;
        }

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(ShowThoughtCoroutine(iconSprite));
    }

    public void HideThought()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(HideThoughtCoroutine());
    }

    private IEnumerator ShowThoughtCoroutine(Sprite iconSprite)
    {
        iconImage.sprite = iconSprite;

        bubbleImage.enabled = true;
        iconImage.enabled = true;

        yield return StartCoroutine(FadeIn());

        isVisible = true;

        if (autoHide)
        {
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeOut());
            HideBubble();
            isVisible = false;
        }
    }

    private IEnumerator HideThoughtCoroutine()
    {
        yield return StartCoroutine(FadeOut());
        HideBubble();
        isVisible = false;
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

        canvasGroup.alpha = 0f;
    }

    private void HideBubble()
    {
        canvasGroup.alpha = 0f;
        bubbleImage.enabled = false;
        iconImage.enabled = false;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
