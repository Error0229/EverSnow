using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")] public Transform target;

    public Vector3 offset = new Vector3(0, 2, -5);

    [Header("Camera Movement")] public float smoothTime = 0.2f;

    public float rotationSpeed = 0.4f;
    public float lockOnRotationSpeed = 120f;
    public float autoLockDistance = 5f;

    [Header("Camera Collision")] public float minDistance = 1f;
    public LayerMask collisionLayers;
    public LayerMask enemyLayer;

    [Header("Lock On Settings")]
    public float lockOnFOV = 160f;
    public float lockOnDistance = 20f;
    public UnityEvent evtUnlock;
    public UnityEvent<GameObject> evtLock;
    public float interactDistance = 5f;

    private bool _isLockOn;
    private Transform _lockOnTarget;
    private float currentRotationX;
    private float currentRotationY;

    private Vector3 currentVelocity = Vector3.zero;
    private InputAction look;
    private Vector2 lookDelta;

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        PlayerInputManager.Instance.evtLook.AddListener(Look);
        PlayerInputManager.Instance.evtLockOn.AddListener(LockOn);
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.IsPlayerInGame) return;
        if (_lockOnTarget && Vector3.Distance(_lockOnTarget.position, target.position) > lockOnDistance)
        {
            _lockOnTarget = null;
            _isLockOn = false;
            evtUnlock.Invoke();
            // Lock camera onto the target
        }

        if (!_lockOnTarget)
            HandleCameraRotation();
        // transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref currentVelocity, smoothTime);

        // 計算理想位置
        var desiredPosition = target.position +
                              target.right * offset.x +
                              Vector3.up * offset.y +
                              -transform.forward * Mathf.Abs(offset.z);
        var finalPosition = CheckCameraCollision(desiredPosition);

        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref currentVelocity, smoothTime);
        var dis = Vector3.Distance(transform.position, finalPosition);

        if (Vector3.Distance(transform.position, finalPosition) < autoLockDistance)
            transform.position = finalPosition;
        // transform.position = Vector3.MoveTowards(transform.position, finalPosition, Time.deltaTime * rotationSpeed);


        if (_lockOnTarget)
        {
            var lockOnPosition = _lockOnTarget.position + Vector3.up * offset.y;
            // transform.LookAt(lockOnPosition);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((lockOnPosition - transform.position).normalized), lockOnRotationSpeed * Time.deltaTime);
        }
        else
        {
            // 使鏡頭面向目標
            // transform.LookAt(target.position + Vector3.up * offset.y);
            var targetPosition = target.position + Vector3.up * offset.y;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((targetPosition - transform.position).normalized), rotationSpeed * Time.deltaTime);
        }
    }

    private void Look(Vector2 vec)
    {
        lookDelta = vec;
    }

    private void HandleCameraRotation()
    {
        // 水平旋轉
        currentRotationY += Mathf.Clamp(lookDelta.x * rotationSpeed, -90f, 90f);
        // currentRotationY = Mathf.Clamp(currentRotationX, -90f, 90f);

        // 垂直旋轉（加入限制）
        currentRotationX -= lookDelta.y * rotationSpeed;

        currentRotationX = Mathf.Clamp(currentRotationX, -65f, 65f);

        // 應用旋轉
        var targetRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
    }

    private Vector3 CheckCameraCollision(Vector3 desiredPosition)
    {
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayers))
            // 如果檢測到碰撞，將鏡頭移近目標
            return hit.point + transform.forward * minDistance;
        return desiredPosition;
    }

    // 可選：添加縮放功能
    public void Zoom(float zoomAmount)
    {
        offset.z = Mathf.Clamp(offset.z + zoomAmount, -10f, -1f);
    }

    private void LockOn(bool invoked)
    {
        if (!invoked) return;
        if (_isLockOn)
        {
            _lockOnTarget = null;
            _isLockOn = false;
            evtUnlock.Invoke();
            return;
        }
        var enemiesInRange = new Collider[10];
        var count = Physics.OverlapSphereNonAlloc(target.position, lockOnDistance, enemiesInRange, enemyLayer);
        if (count == 0) return;
        var closestEnemy = enemiesInRange
            .Take(count)
            .Select(c => c.transform)
            .Where(e => Vector3.Angle(target.forward, e.position - target.position) <= lockOnFOV)
            .OrderBy(e => Vector3.Distance(target.position, e.position))
            .FirstOrDefault();
        if (closestEnemy == null) return;
        _lockOnTarget = closestEnemy;
        _isLockOn = true;
        evtLock.Invoke(_lockOnTarget.gameObject);
    }
    public Npc CheckLookAtNpc()
    {
        var lookAtCollider = new Collider[5];
        var pie = GameManager.Instance.PlayerInstance.Entity.Soul;
        var count = Physics.OverlapSphereNonAlloc(pie.transform.position, lockOnDistance / 3, lookAtCollider, LayerMask.GetMask("NPC"));
        foreach (var cldr in lookAtCollider.Take(count))
        {
            var directionToNpc = (cldr.transform.position - pie.transform.position).normalized;
            var angle = Vector3.Angle(pie.transform.forward, directionToNpc);
            // print($"angle: {angle}, lockOnFOV: {lockOnFOV}, directionToNpc: {directionToNpc}");
            if (angle <= lockOnFOV / 2)
            {
                // Optionally, perform a raycast to ensure there's no obstruction
                // if (Physics.Raycast(transform.position, directionToNpc, out var hit, lockOnDistance))
                // {
                //     if (hit.collider.gameObject.layer != LayerMask.NameToLayer("NPC")) continue; // Skip if not an NPC()
                return cldr.transform.parent.GetComponent<Npc>();
                // }
            }
        }
        return null;
    }

    public Item CheckLookAtItem()
    {
        var lookAtCollider = new Collider[5];
        var pie = GameManager.Instance.PlayerInstance.Entity.Soul;
        // the look at sphere should be spawn at the line prolong from the camera to the player
        var lookAtPosition = pie.transform.position + pie.transform.forward * interactDistance;
        var count = Physics.OverlapSphereNonAlloc(lookAtPosition, interactDistance, lookAtCollider, LayerMask.GetMask("Item"));
        foreach (var cldr in lookAtCollider.Take(count).OrderBy(c => Vector3.Distance(c.transform.position, pie.transform.position)))
        {
            var directionToItem = (cldr.transform.position - pie.transform.position).normalized;
            var angle = Vector3.Angle(pie.transform.forward, directionToItem);
            // draw both the direction and the angle
            Debug.DrawRay(pie.transform.position, directionToItem * lockOnDistance, Color.red);
            Debug.DrawRay(pie.transform.position, pie.transform.forward * lockOnDistance, Color.green);
            if (angle <= lockOnFOV)
            {
                // get the item from the collider's parent
                return cldr.transform.parent.GetComponent<Item>();
            }
        }
        return null;
    }
}
