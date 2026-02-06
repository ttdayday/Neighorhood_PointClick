#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector for CloseUpData assets
/// Provides visual sprite preview with clickable hotspot positioning
/// Makes it easy to add and position hotspots by clicking on the sprite
/// </summary>
[CustomEditor(typeof(CloseUpData))]
public class CloseUpDataEditor : Editor
{
    private CloseUpData data;
    private int selectedHotspotIndex = -1;

    private const float PREVIEW_MAX_SIZE = 400f;
    private const float HOTSPOT_HANDLE_SIZE = 8f;

    private Rect lastPreviewRect;

    void OnEnable()
    {
        data = (CloseUpData)target;
    }

    public override void OnInspectorGUI()
    {
        // Update serialized properties
        serializedObject.Update();

        // Draw default inspector fields
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Visual Hotspot Editor", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Click on the sprite preview below to add a new hotspot at that position.", MessageType.Info);

        // Draw sprite preview with hotspots
        if (data.closeUpSprite != null)
        {
            DrawSpritePreview();
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a Close-Up Sprite above to use the visual editor.", MessageType.Warning);
        }

        // Hotspot list controls
        EditorGUILayout.Space(10);
        DrawHotspotList();

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();

        // Repaint continuously to show hover effects
        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
        {
            Repaint();
        }
    }

    /// <summary>
    /// Draws the sprite preview with hotspot overlays
    /// Handles mouse input for adding and positioning hotspots
    /// </summary>
    private void DrawSpritePreview()
    {
        Texture2D texture = data.closeUpSprite.texture;
        if (texture == null)
        {
            EditorGUILayout.HelpBox("Sprite texture is null!", MessageType.Error);
            return;
        }

        // Calculate preview size maintaining aspect ratio
        float aspectRatio = (float)texture.width / texture.height;
        float previewWidth = Mathf.Min(PREVIEW_MAX_SIZE, EditorGUIUtility.currentViewWidth - 40f);
        float previewHeight = previewWidth / aspectRatio;

        // Reserve space for preview
        lastPreviewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);

        // Draw sprite texture
        GUI.DrawTexture(lastPreviewRect, texture, ScaleMode.ScaleToFit);

        // Draw all hotspots
        for (int i = 0; i < data.hotspots.Count; i++)
        {
            DrawHotspotOverlay(lastPreviewRect, data.hotspots[i], i);
        }

        // Handle mouse input
        HandleMouseInput(lastPreviewRect);
    }

    /// <summary>
    /// Draws a single hotspot overlay on the preview
    /// </summary>
    private void DrawHotspotOverlay(Rect previewRect, HotspotData hotspot, int index)
    {
        // Convert normalized position to preview rect position
        float pixelX = hotspot.normalizedPosition.x * previewRect.width;
        float pixelY = (1f - hotspot.normalizedPosition.y) * previewRect.height; // Flip Y for GUI coords

        float pixelWidth = hotspot.normalizedSize.x * previewRect.width;
        float pixelHeight = hotspot.normalizedSize.y * previewRect.height;

        // Calculate hotspot rect in preview space
        Rect hotspotRect = new Rect(
            previewRect.x + pixelX - pixelWidth * 0.5f,
            previewRect.y + pixelY - pixelHeight * 0.5f,
            pixelWidth,
            pixelHeight
        );

        // Draw hotspot rectangle
        Color hotspotColor = (index == selectedHotspotIndex) ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.3f);
        EditorGUI.DrawRect(hotspotRect, hotspotColor);

        // Draw border
        Color borderColor = (index == selectedHotspotIndex) ? Color.green : Color.red;
        Handles.BeginGUI();
        Handles.color = borderColor;
        Handles.DrawPolyLine(
            new Vector3(hotspotRect.xMin, hotspotRect.yMin),
            new Vector3(hotspotRect.xMax, hotspotRect.yMin),
            new Vector3(hotspotRect.xMax, hotspotRect.yMax),
            new Vector3(hotspotRect.xMin, hotspotRect.yMax),
            new Vector3(hotspotRect.xMin, hotspotRect.yMin)
        );
        Handles.EndGUI();

        // Draw hotspot ID label
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 10;
        labelStyle.alignment = TextAnchor.MiddleCenter;

        // Shadow for better readability
        GUI.Label(new Rect(hotspotRect.x + 1, hotspotRect.y + 1, hotspotRect.width, 20), hotspot.hotspotId, labelStyle);
        labelStyle.normal.textColor = borderColor;
        GUI.Label(new Rect(hotspotRect.x, hotspotRect.y, hotspotRect.width, 20), hotspot.hotspotId, labelStyle);

        // Draw center handle
        DrawHandle(previewRect, hotspot.normalizedPosition, borderColor);
    }

    /// <summary>
    /// Draws a small handle at normalized position
    /// </summary>
    private void DrawHandle(Rect previewRect, Vector2 normalizedPos, Color color)
    {
        float pixelX = previewRect.x + normalizedPos.x * previewRect.width;
        float pixelY = previewRect.y + (1f - normalizedPos.y) * previewRect.height;

        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawSolidDisc(new Vector3(pixelX, pixelY, 0), Vector3.forward, HOTSPOT_HANDLE_SIZE * 0.5f);
        Handles.EndGUI();
    }

    /// <summary>
    /// Handles mouse input on the preview rect
    /// </summary>
    private void HandleMouseInput(Rect previewRect)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && previewRect.Contains(e.mousePosition))
        {
            // Convert mouse position to normalized coordinates
            Vector2 normalizedPos = PixelToNormalized(e.mousePosition, previewRect);

            // Check if clicking on existing hotspot
            int clickedIndex = GetHotspotAtPosition(normalizedPos);

            if (clickedIndex >= 0)
            {
                // Select existing hotspot
                selectedHotspotIndex = clickedIndex;
            }
            else
            {
                // Add new hotspot
                AddNewHotspot(normalizedPos);
            }

            e.Use();
            Repaint();
        }
    }

    /// <summary>
    /// Converts pixel position in preview to normalized coordinates (0-1)
    /// </summary>
    private Vector2 PixelToNormalized(Vector2 pixelPos, Rect previewRect)
    {
        float normalizedX = (pixelPos.x - previewRect.x) / previewRect.width;
        float normalizedY = 1f - ((pixelPos.y - previewRect.y) / previewRect.height); // Flip Y

        // Clamp to 0-1 range
        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        return new Vector2(normalizedX, normalizedY);
    }

    /// <summary>
    /// Gets the index of the hotspot at the given normalized position
    /// Returns -1 if no hotspot found
    /// </summary>
    private int GetHotspotAtPosition(Vector2 normalizedPos)
    {
        for (int i = 0; i < data.hotspots.Count; i++)
        {
            HotspotData hotspot = data.hotspots[i];
            Vector2 halfSize = hotspot.normalizedSize * 0.5f;

            if (normalizedPos.x >= hotspot.normalizedPosition.x - halfSize.x &&
                normalizedPos.x <= hotspot.normalizedPosition.x + halfSize.x &&
                normalizedPos.y >= hotspot.normalizedPosition.y - halfSize.y &&
                normalizedPos.y <= hotspot.normalizedPosition.y + halfSize.y)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Adds a new hotspot at the specified normalized position
    /// </summary>
    private void AddNewHotspot(Vector2 normalizedPos)
    {
        Undo.RecordObject(data, "Add Hotspot");

        HotspotData newHotspot = new HotspotData();
        newHotspot.hotspotId = $"Hotspot{data.hotspots.Count + 1}";
        newHotspot.normalizedPosition = normalizedPos;
        newHotspot.normalizedSize = new Vector2(0.1f, 0.1f); // Default size

        data.hotspots.Add(newHotspot);
        selectedHotspotIndex = data.hotspots.Count - 1;

        EditorUtility.SetDirty(data);
        Debug.Log($"Added hotspot '{newHotspot.hotspotId}' at {normalizedPos}");
    }

    /// <summary>
    /// Draws the hotspot list with controls
    /// </summary>
    private void DrawHotspotList()
    {
        EditorGUILayout.LabelField($"Hotspots ({data.hotspots.Count})", EditorStyles.boldLabel);

        if (data.hotspots.Count == 0)
        {
            EditorGUILayout.HelpBox("No hotspots. Click on the sprite preview above to add one.", MessageType.Info);
            return;
        }

        for (int i = 0; i < data.hotspots.Count; i++)
        {
            DrawHotspotListItem(i);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Hotspot (Bottom)"))
        {
            AddNewHotspot(new Vector2(0.5f, 0.5f)); // Center
        }
    }

    /// <summary>
    /// Draws a single hotspot list item
    /// </summary>
    private void DrawHotspotListItem(int index)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        // Select button
        bool isSelected = (index == selectedHotspotIndex);
        if (GUILayout.Button(isSelected ? "●" : "○", GUILayout.Width(25)))
        {
            selectedHotspotIndex = index;
        }

        EditorGUILayout.LabelField($"#{index + 1}: {data.hotspots[index].hotspotId}", EditorStyles.boldLabel);

        // Delete button
        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            DeleteHotspot(index);
            return;
        }

        EditorGUILayout.EndHorizontal();

        // Show selected hotspot details
        if (isSelected)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"Position: ({data.hotspots[index].normalizedPosition.x:F3}, {data.hotspots[index].normalizedPosition.y:F3})");
            EditorGUILayout.LabelField($"Size: ({data.hotspots[index].normalizedSize.x:F3}, {data.hotspots[index].normalizedSize.y:F3})");
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Deletes a hotspot at the specified index
    /// </summary>
    private void DeleteHotspot(int index)
    {
        if (index < 0 || index >= data.hotspots.Count)
            return;

        Undo.RecordObject(data, "Delete Hotspot");
        string hotspotId = data.hotspots[index].hotspotId;
        data.hotspots.RemoveAt(index);

        if (selectedHotspotIndex == index)
            selectedHotspotIndex = -1;
        else if (selectedHotspotIndex > index)
            selectedHotspotIndex--;

        EditorUtility.SetDirty(data);
        Debug.Log($"Deleted hotspot '{hotspotId}'");
    }
}
#endif
