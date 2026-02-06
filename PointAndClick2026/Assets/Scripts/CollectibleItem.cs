using UnityEngine;

/// <summary>
/// Makes an object collectible - when clicked, it goes into the inventory
/// Inherits from InteractableObject to use existing click detection
/// </summary>
public class CollectibleItem : InteractableObject
{
    [Header("Item Settings")]
    [Tooltip("Name of this item (for inventory)")]
    public string itemName = "Item";

    [Tooltip("Item sprite/icon for inventory display")]
    public Sprite itemIcon;

    [Header("Collection Behavior")]
    [Tooltip("Hide the object after collecting (recommended)")]
    public bool hideAfterCollection = true;

    [Tooltip("Destroy the object after collecting (vs just hiding)")]
    public bool destroyAfterCollection = false;

    [Tooltip("Play animation/sound before collecting (future feature)")]
    public bool playCollectionEffect = false;

    private bool isCollected = false;

    /// <summary>
    /// Called when this item is clicked
    /// Adds to inventory and hides/destroys the object
    /// </summary>
    protected override void OnInteractCustom()
    {
        if (isCollected)
        {
            Debug.Log($"CollectibleItem: '{itemName}' already collected");
            return;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("CollectibleItem: Inventory.Instance is null!");
            return;
        }

        // Add to inventory
        Inventory.Instance.AddItem(itemName, itemIcon);

        // Mark as collected
        isCollected = true;

        // Play collection effect (if enabled)
        if (playCollectionEffect)
        {
            // TODO: Add particle effect, sound, animation
        }

        // Hide or destroy the object
        if (destroyAfterCollection)
        {
            Destroy(gameObject);
        }
        else if (hideAfterCollection)
        {
            // Hide sprite and disable collider
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Disable this component
            this.enabled = false;
        }

        Debug.Log($"CollectibleItem: Collected '{itemName}'");
    }
}
