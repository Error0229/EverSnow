using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera: MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);

    [Header("Camera Movement")]
    public float smoothTime = 0.3f;
    public float rotationSpeed = 5f;

    [Header("Camera Collision")]
    public float minDistance = 1f;
    public LayerMask collisionLayers;

    private Vector3 currentVelocity = Vector3.zero;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private InputAction look;
    private Vector2 lookDelta;

    void Awake()
    {
        KeyboardInputManager.Instance.evtLook.AddListener(Look);
    }

    void Look(Vector2 vec)
    {
        lookDelta = vec;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
    
        // 計算理想位置
        Vector3 desiredPosition = target.position + 
            target.right * offset.x + 
            Vector3.up * offset.y + 
            -transform.forward * Mathf.Abs(offset.z);
    
        // 碰撞檢測與相機位置更新
        Vector3 finalPosition = CheckCameraCollision(desiredPosition);
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref currentVelocity, smoothTime);
    
        // 使鏡頭面向目標
        transform.LookAt(target.position + Vector3.up * offset.y);
    }

    void HandleCameraRotation()
    {
        // 水平旋轉
        currentRotationY += lookDelta.x * rotationSpeed;

        // 垂直旋轉（加入限制）
        currentRotationX -= lookDelta.y * rotationSpeed;
        currentRotationX = Mathf.Clamp(currentRotationX, -40f, 40f);

        // 應用旋轉
        transform.localRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
    }

    Vector3 CheckCameraCollision(Vector3 desiredPosition)
    {
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayers))
        {
            // 如果檢測到碰撞，將鏡頭移近目標
            return hit.point + transform.forward * minDistance;
        }
        return desiredPosition;
    }

    // 可選：添加縮放功能
    public void Zoom(float zoomAmount)
    {
        offset.z = Mathf.Clamp(offset.z + zoomAmount, -10f, -1f);
    }
}