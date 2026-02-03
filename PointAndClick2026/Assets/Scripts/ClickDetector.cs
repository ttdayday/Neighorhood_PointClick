using UnityEngine;
using UnityEngine.Events;

public class ClickDetector : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Layer mask for objects that can be clicked")]
    public LayerMask clickableLayerMask = ~0;

    [Header("Events")]
    [Tooltip("Event fired when a 2D object is clicked")]
    public UnityEvent<GameObject> onObjectClicked;

    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool debugMode = false;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("ClickDetector: No main camera found in scene!");
        }

        if (onObjectClicked == null)
        {
            onObjectClicked = new UnityEvent<GameObject>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectClick();
        }
    }

    private void DetectClick()
    {
        if (mainCamera == null)
            return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayerMask);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (debugMode)
            {
                Debug.Log($"Clicked on: {clickedObject.name} at position {hit.point}");
            }

            onObjectClicked?.Invoke(clickedObject);

            IInteractable interactable = clickedObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                InteractableObject interactableObj = clickedObject.GetComponent<InteractableObject>();
                if (interactableObj != null)
                {
                    Debug.Log($"[DEBUG] Clicked on interactable: {interactableObj.objectName}");
                }
                else
                {
                    Debug.Log($"[DEBUG] Clicked on interactable: {clickedObject.name}");
                }

                interactable.OnInteract();
            }
            else
            {
                IClickable clickable = clickedObject.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClicked();
                }
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.Log("No object clicked");
            }

            if (ThoughtBubbleManager.Instance != null && ThoughtBubbleManager.Instance.IsVisible())
            {
                ThoughtBubbleManager.Instance.HideThought();
            }
        }
    }

    public GameObject GetObjectAtMousePosition()
    {
        if (mainCamera == null)
            return null;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayerMask);

        return hit.collider != null ? hit.collider.gameObject : null;
    }
}

public interface IClickable
{
    void OnClicked();
}
