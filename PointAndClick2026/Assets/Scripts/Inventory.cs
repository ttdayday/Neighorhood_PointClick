using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager for player inventory
/// Stores collected items and provides methods to add/remove/check items
/// </summary>
public class Inventory : MonoBehaviour
{
    // Singleton pattern
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventory>();
            }
            return instance;
        }
    }

    [Header("Debug")]
    [Tooltip("Show debug logs for inventory operations")]
    public bool showDebugLogs = true;

    // List of collected item names
    private List<string> collectedItems = new List<string>();

    // Dictionary mapping item names to their sprites (for UI display)
    private Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Optional: Make persistent across scenes
        // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    public void AddItem(string itemName, Sprite itemSprite = null)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("Inventory: Attempted to add item with null/empty name!");
            return;
        }

        if (collectedItems.Contains(itemName))
        {
            if (showDebugLogs)
            {
                Debug.Log($"Inventory: Item '{itemName}' already collected");
            }
            return;
        }

        collectedItems.Add(itemName);

        if (itemSprite != null)
        {
            itemSprites[itemName] = itemSprite;
        }

        if (showDebugLogs)
        {
            Debug.Log($"Inventory: Added '{itemName}' (Total items: {collectedItems.Count})");
        }

        // TODO: Trigger inventory UI update event
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public void RemoveItem(string itemName)
    {
        if (collectedItems.Contains(itemName))
        {
            collectedItems.Remove(itemName);
            itemSprites.Remove(itemName);

            if (showDebugLogs)
            {
                Debug.Log($"Inventory: Removed '{itemName}'");
            }
        }
    }

    /// <summary>
    /// Checks if an item is in the inventory
    /// </summary>
    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    /// <summary>
    /// Gets the sprite for an item (if available)
    /// </summary>
    public Sprite GetItemSprite(string itemName)
    {
        if (itemSprites.ContainsKey(itemName))
        {
            return itemSprites[itemName];
        }
        return null;
    }

    /// <summary>
    /// Gets all collected item names
    /// </summary>
    public List<string> GetAllItems()
    {
        return new List<string>(collectedItems); // Return copy
    }

    /// <summary>
    /// Gets the number of items in inventory
    /// </summary>
    public int GetItemCount()
    {
        return collectedItems.Count;
    }

    /// <summary>
    /// Clears all items from inventory
    /// </summary>
    public void ClearInventory()
    {
        collectedItems.Clear();
        itemSprites.Clear();

        if (showDebugLogs)
        {
            Debug.Log("Inventory: Cleared all items");
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
