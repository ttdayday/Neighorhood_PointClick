using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Makes a UI button clickable within a close-up view to show another close-up (tag detail)
/// Only visible when the parent close-up (flowerpot) is showing
/// </summary>
[RequireComponent(typeof(Button))]
public class CloseUpTag : MonoBehaviour
{
    [Header("Tag Detail")]
    [Tooltip("The close-up sprite to show when this tag is clicked (the tag detail with text)")]
    public Sprite tagDetailSprite;

    [Header("Visibility Settings")]
    [Tooltip("Name of the parent close-up that this tag belongs to (e.g., 'FlowerPotZoomedIn')")]
    public string parentCloseUpName = "FlowerPotZoomedIn";

    private Button button;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        button = GetComponent<Button>();

        // Add CanvasGroup for fade in/out
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Start invisible
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        // Hook up the button click
        button.onClick.AddListener(OnTagClicked);
    }

    void Update()
    {
        // Show/hide based on whether the parent close-up is visible
        if (CloseUpViewController.Instance != null)
        {
            bool shouldBeVisible = CloseUpViewController.Instance.IsShowingCloseUp(parentCloseUpName);

            if (shouldBeVisible && canvasGroup.alpha < 1f)
            {
                Show();
            }
            else if (!shouldBeVisible && canvasGroup.alpha > 0f)
            {
                Hide();
            }
        }
    }

    private void OnTagClicked()
    {
        Debug.Log("Tag clicked! Showing tag detail...");

        if (tagDetailSprite == null)
        {
            Debug.LogWarning("CloseUpTag: tagDetailSprite is not assigned!");
            return;
        }

        if (CloseUpViewController.Instance != null)
        {
            CloseUpViewController.Instance.ShowCloseUp(tagDetailSprite);
        }
    }

    private void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnTagClicked);
        }
    }
}
