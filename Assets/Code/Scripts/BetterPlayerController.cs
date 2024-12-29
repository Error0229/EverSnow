using UnityEngine;
public class BetterPlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    public float velocity = 10f;
    public float runningSpeed = 20f;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject axeHandler;
    [SerializeField] private GameObject knifeHandler;
    [SerializeField] private GameObject soul;
    public Transform Soul => soul.transform;
    private readonly float rotationSpeed = 5f;
    private Animator anim;
    private Attacks attack = Attacks.Normal;
    private ThirdPersonCamera camSoul;
    private bool isIdle;
    private bool isLockOn;
    private bool isSprinting;
    private bool isWalking;
    private float lastVelocity;
    private GameObject lockOnTarget;
    private Vector2 lookVec;
    private Vector2 movingVec;
    private Rigidbody rigid;
    private STATE state = STATE.IDLE;
    private bool triggerEnter;
    private AnimatorStateInfo StateInfo
    {
        get => anim.GetCurrentAnimatorStateInfo(0);
    }

    private bool IsGround
    {
        get => Physics.Raycast(transform.position, Vector3.down, 0.5f);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = gameObject.transform.GetComponentInChildren<Animator>();
        GoToState(STATE.IDLE);
        PlayerInputManager.Instance.evtMoveAxis.AddListener(Move);
        PlayerInputManager.Instance.evtJump.AddListener(Jump);
        PlayerInputManager.Instance.evtRun.AddListener(Run);
        PlayerInputManager.Instance.evtLook.AddListener(Look);
        PlayerInputManager.Instance.evtDodge.AddListener(Dodge);
        PlayerInputManager.Instance.evtStrongAttack.AddListener(HeavyAttack);
        PlayerInputManager.Instance.evtNormalAttack.AddListener(NormalAttack);
        camSoul = cam.GetComponent<ThirdPersonCamera>();
        camSoul.evtLock.AddListener(LockOn);
        camSoul.evtUnlock.AddListener(UnlockOn);
    }

    private void FixedUpdate()
    {

        var newVelocity = 0.0f;
        switch (state)
        {
            case STATE.IDLE:
                if (triggerEnter)
                {
                    ResetHorizontalVelocity();
                    anim.CrossFadeInFixedTime("idle", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude > 0.1f) GoToState(STATE.LOCOMOTION);
                break;

            case STATE.LOCOMOTION:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime(isLockOn ? "lockon_locomotion" : "locomotion", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude <= 0.1f) GoToState(STATE.IDLE);
                if (!IsGround) GoToState(STATE.FALL);


                newVelocity = movingVec.magnitude * (isSprinting ? runningSpeed : velocity);
                if (isLockOn)
                {
                    anim.SetFloat(DirX, movingVec.x);
                    anim.SetFloat(DirY, movingVec.y);
                }
                else
                    anim.SetFloat(Speed, movingVec.magnitude * 1.0f + (isSprinting ? 1.0f : 0.0f));
                break;

            case STATE.JUMP:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("jump", 0.1f);
                    triggerEnter = false;
                    rigid.AddForce(Vector3.up * 6, ForceMode.Impulse); // Apply vertical jump force
                }

                // Maintain horizontal movement direction

                if (StateInfo.normalizedTime > 0.8f && StateInfo.IsName("jump")) GoToState(STATE.FALL);
                break;

            case STATE.FALL:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("fall", 0.1f);
                    triggerEnter = false;
                }

                // Keep the falling movement consistent with horizontal velocity
                break;
            case STATE.ATTACK:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime(attack == Attacks.Normal ? "NormalAttack" : "HeavyAttack", 0.1f);
                    ResetHorizontalVelocity();
                    triggerEnter = false;
                }

                if (StateInfo.normalizedTime > 0.9f && (StateInfo.IsName("NormalAttack") || StateInfo.IsName("HeavyAttack")))
                {
                    if (movingVec.magnitude <= 0.1f)
                    {
                        GoToState(STATE.IDLE);
                    }
                    else GoToState(STATE.LOCOMOTION);
                }

                // Maintain rolling direction and speed
                break;

            case STATE.ROLL:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("roll_forward", 0.1f);
                    triggerEnter = false;
                }

                if (StateInfo.normalizedTime > 0.9f && StateInfo.IsName("roll_forward"))
                {
                    if (movingVec.magnitude <= 0.1f)
                    {
                        GoToState(STATE.IDLE);
                        ResetHorizontalVelocity();
                    }
                    else GoToState(STATE.LOCOMOTION);
                }

                // Maintain rolling direction and speed
                break;
        }

        newVelocity = Mathf.Lerp(lastVelocity, newVelocity, Time.deltaTime);
        MoveOn(movingVec, newVelocity);
        lastVelocity = newVelocity;
    }

    private bool CanChangeDirection()
    {
        return state == STATE.LOCOMOTION;
    }

    private void ApplyVerticalMovement(Vector3 currentVelocity)
    {
        rigid.linearVelocity = new Vector3(currentVelocity.x, rigid.linearVelocity.y, currentVelocity.z);
    }

    private void MoveOn(Vector2 movingInputs, float currentVelocity)
    {
        if (currentVelocity <= 0.01f) return;

        var cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        var cameraRight = cam.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // Calculate movement direction based on camera
        var direction = cameraForward * movingInputs.y + cameraRight * movingInputs.x;

        // Only rotate if we can change direction
        if (direction != Vector3.zero && CanChangeDirection())
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // For states other than LOCOMOTION, use the current forward direction
        if (!CanChangeDirection())
        {
            direction = transform.forward;
        }

        // Calculate and apply movement
        var newVelocity = direction * currentVelocity;
        ApplyVerticalMovement(newVelocity);
    }

    public void Equip(Weapon weapon)
    {
        if (weapon.Name == "斧頭")
        {
            axeHandler.SetActive(true);

            weapon.Entity.transform.SetParent(axeHandler.transform);
            weapon.Entity.transform.localPosition = Vector3.zero;
            weapon.Entity.transform.localRotation = Quaternion.identity;
            weapon.Entity.SetActive(true);
            knifeHandler.SetActive(false);
        }
        else if (weapon.Name == "小刀")
        {
            weapon.Entity.transform.SetParent(knifeHandler.transform);
            weapon.Entity.transform.localPosition = Vector3.zero;
            weapon.Entity.transform.localRotation = Quaternion.identity;
            weapon.Entity.SetActive(true);
            axeHandler.SetActive(false);
            knifeHandler.SetActive(true);
        }
    }
    public void Remove(Weapon weapon)
    {
        if (weapon.Name == "斧頭")
        {
            axeHandler.SetActive(false);
        }
        else if (weapon.Name == "小刀")
        {
            knifeHandler.SetActive(false);
        }
        weapon.Entity.SetActive(false);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (state == STATE.JUMP)
        {
            if (movingVec.magnitude <= 0.1f)
            {
                GoToState(STATE.IDLE);
                ResetHorizontalVelocity();
            }
            else GoToState(STATE.LOCOMOTION);
        }
        else if (state == STATE.FALL)
        {
            GoToState(STATE.ROLL);
        }
    }

    private void ResetHorizontalVelocity()
    {
        lastVelocity = 0f;
        rigid.linearVelocity = new Vector3(0f, rigid.linearVelocity.y, 0f);
    }

    private void HeavyAttack(bool invoked)
    {
        if (!invoked || state == STATE.ATTACK) return;
        attack = Attacks.Heavy;
        GoToState(STATE.ATTACK);
    }
    private void NormalAttack(bool invoked)
    {
        if (!invoked || state == STATE.ATTACK) return;
        attack = Attacks.Normal;
        GoToState(STATE.ATTACK);
    }



    private void GoToState(STATE newState)
    {
        state = newState;
        triggerEnter = true;
    }

    public void EnterDialog()
    {
        state = STATE.IDLE;
        anim.CrossFadeInFixedTime("idle", 0.1f);
    }

    public void Deactivate()
    {
        rigid.linearVelocity = Vector3.zero;
        state = STATE.IDLE;
        anim.CrossFadeInFixedTime("idle", 0.1f);
        enabled = false;
    }
    public void Activate()
    {
        enabled = true;
    }

    private void Move(Vector2 vac)
    {
        movingVec = vac;
    }

    private void Look(Vector2 vac)
    {
        lookVec = vac;
    }

    private void Run(bool isRun)
    {
        isSprinting = isRun;
    }

    public void Jump(bool _isThrust)
    {
        if (lastVelocity < Mathf.Lerp(velocity, runningSpeed, 0.5f)) return;
        if (_isThrust && IsGround)
            if (state == STATE.IDLE || state == STATE.LOCOMOTION)
                GoToState(STATE.JUMP);
    }

    private void Dodge(bool isDodge)
    {
        if (!isDodge) return;
        if (state != STATE.ROLL)
            GoToState(STATE.ROLL);
    }

    private void LockOn(GameObject target)
    {
        lockOnTarget = target;
        isLockOn = true;
        if (state == STATE.LOCOMOTION)
            anim.CrossFadeInFixedTime("lockon_locomotion", 0.1f);
    }
    private void UnlockOn()
    {
        isLockOn = false;
        if (state == STATE.LOCOMOTION)
            anim.CrossFadeInFixedTime("locomotion", 0.1f);
    }

    public Npc CheckLookAtNpc()
    {
        return camSoul.CheckLookAtNpc();
    }

    public Item CheckLookAtItem()
    {
        return camSoul.CheckLookAtItem();
    }
    private enum Attacks
    {
        Heavy,
        Normal
    }

    private enum STATE
    {
        LOCOMOTION,
        IDLE,
        JUMP,
        FALL,
        ROLL,
        ATTACK
    }
}
