using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIPositioner : MonoBehaviour
{
    private Canvas worldSpaceCanvas;
    private RectTransform canvasRect;
    [SerializeField] private Vector3 offset = Vector3.up;
    [SerializeField]
    private Transform mainCameraTransform;
    private Transform targetTransform;

    void Awake()
    {
        worldSpaceCanvas = GetComponent<Canvas>();
        canvasRect = GetComponent<RectTransform>();

        // Ensure proper setup
        if (worldSpaceCanvas != null)
        {
            worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
        }
        worldSpaceCanvas.enabled = false;
    }
    private void LateUpdate()
    {
        if (targetTransform == null) return;
        transform.position = targetTransform.position + offset;
        transform.LookAt(transform.position + mainCameraTransform.forward);
    }
    public void Unlock()
    {
        worldSpaceCanvas.enabled = false;
        enabled = false;
        GetComponentInChildren<Image>().enabled = false;
    }

    public void LockOn(Transform target)
    {
        if (!canvasRect) return;
        worldSpaceCanvas.enabled = true;
        enabled = true;
        targetTransform = target;
        GetComponentInChildren<Image>().enabled = true;
    }
}
