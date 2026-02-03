using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visibility of the zoom out button
/// Shows button only when camera is zoomed in
/// </summary>
public class ZoomOutButton : MonoBehaviour
{
    private Button button;
    private CanvasGroup canvasGroup;

    void Start()
    {
        button = GetComponent<Button>();

        // Add CanvasGroup for fading
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Connect button click to zoom out
        if (button != null)
        {
            button.onClick.AddListener(OnZoomOutClicked);
        }

        // Start hidden
        Hide();
    }

    void Update()
    {
        // Show button only when close-up view is visible
        if (CloseUpViewController.Instance != null)
        {
            bool isInCloseUp = CloseUpViewController.Instance.IsVisible();

            if (isInCloseUp && canvasGroup.alpha < 1f)
            {
                Show();
            }
            else if (!isInCloseUp && canvasGroup.alpha > 0f)
            {
                Hide();
            }
        }
    }

    private void OnZoomOutClicked()
    {
        if (CloseUpViewController.Instance != null)
        {
            CloseUpViewController.Instance.HideCloseUp();
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
}
