using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Object Information")]
    [Tooltip("Name of this interactable object")]
    public string objectName = "Interactable Object";

    [TextArea(2, 4)]
    [Tooltip("Description of this object")]
    public string description = "This is an interactable object.";

    [Header("Interaction Settings")]
    [Tooltip("Can this object be interacted with?")]
    public bool isInteractable = true;

    [Header("Feedback")]
    [Tooltip("Show interaction message in console")]
    public bool showConsoleMessage = true;

    [Header("Thought Bubble")]
    public Sprite thoughtIcon;

    public void OnInteract()
    {
        if (!isInteractable)
        {
            if (showConsoleMessage)
            {
                Debug.Log($"[{objectName}] This object cannot be interacted with right now.");
            }
            return;
        }

        if (showConsoleMessage)
        {
            Debug.Log($"=== Interacted with: {objectName} ===");
            Debug.Log($"Description: {description}");
            Debug.Log($"Position: {transform.position}");
        }

        if (thoughtIcon != null && ThoughtBubbleManager.Instance != null)
        {
            ThoughtBubbleManager.Instance.ShowThought(thoughtIcon);
        }

        OnInteractCustom();
    }

    protected virtual void OnInteractCustom()
    {
    }
}

public interface IInteractable
{
    void OnInteract();
}
