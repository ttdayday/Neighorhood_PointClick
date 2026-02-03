using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    // Singleton pattern - only one camera controller exists
    private static CameraZoomController instance;
    public static CameraZoomController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraZoomController>();
            }
            return instance;
        }
    }

    [Header("Zoom Settings")]
    [Tooltip("How long the zoom animation takes")]
    public float zoomDuration = 0.5f;

    private Camera cam;
    private Vector3 originalPosition;
    private float originalSize;
    private bool isZooming = false;

    void Awake()
    {
        // Set up singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        // Get the camera component
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("CameraZoomController: No Camera component found!");
            return;
        }

        // Store the original camera settings
        originalPosition = transform.position;
        originalSize = cam.orthographicSize;

        Debug.Log($"Camera initialized - Original position: {originalPosition}, Original size: {originalSize}");
    }

    /// <summary>
    /// Smoothly zoom the camera to a target position with a specific orthographic size
    /// </summary>
    public void ZoomToTarget(Transform target, float targetSize)
    {
        if (target == null)
        {
            Debug.LogWarning("CameraZoomController: Target is null!");
            return;
        }

        if (isZooming)
        {
            Debug.Log("CameraZoomController: Already zooming, ignoring request");
            return;
        }

        StartCoroutine(ZoomCoroutine(target.position, targetSize));
    }

    /// <summary>
    /// Coroutine that smoothly animates the camera zoom
    /// </summary>
    private IEnumerator ZoomCoroutine(Vector3 targetPosition, float targetSize)
    {
        isZooming = true;

        // Keep the Z position (camera depth)
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, startPosition.z);

        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        Debug.Log($"Starting zoom - From: {startPosition} (size {startSize}) To: {endPosition} (size {targetSize})");

        // Animate over time
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;

            // Smooth interpolation (Lerp = Linear intERPolation)
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null; // Wait one frame
        }

        // Ensure we end exactly at target (avoid floating point errors)
        transform.position = endPosition;
        cam.orthographicSize = targetSize;

        isZooming = false;
        Debug.Log("Zoom complete!");
    }

    /// <summary>
    /// Reset camera to original position and size
    /// </summary>
    public void ResetZoom()
    {
        if (isZooming)
        {
            StopAllCoroutines();
        }

        StartCoroutine(ZoomCoroutine(originalPosition, originalSize));
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
