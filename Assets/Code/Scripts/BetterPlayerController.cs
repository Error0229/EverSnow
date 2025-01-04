using UnityEngine;
public class BetterPlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    public float walkingSpeed = 10f;
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
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;
    private float verticalVelocity;
    private STATE state = STATE.IDLE;
    private bool triggerEnter;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float footstepInterval = 0.5f; // Adjust this value to control frequency
    private float lastFootstepTime;
    private float fallTimer = 0f;
    [SerializeField]
    private float fallThreshold = 0.3f; // Adjust this value to change how long before entering fall state
    [SerializeField] private float fallAnimationDelay = 0.5f; // Time before fall animation plays
    [SerializeField] private float fallSpeed = 1.0f;
    private float fallAnimationTimer = 0f;
    private AnimatorStateInfo StateInfo
    {
        get => anim.GetCurrentAnimatorStateInfo(0);
    }

    public float floatLimit = 1.5f;
    private bool IsGround
    {
        get => controller.isGrounded;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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

    private void Update()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Small negative value to keep grounded
            fallTimer = 0f; // Reset fall timer when grounded
            fallAnimationTimer = 0f; // Reset animation timer when grounded
        }
        playerVelocity.y += (gravityValue - (controller.isGrounded ? 0f : fallSpeed)) * Time.deltaTime;

        // Update fall timer when not grounded and not in JUMP state
        if (!controller.isGrounded && state != STATE.JUMP)
        {
            fallTimer += Time.fixedDeltaTime;
        }

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
                // Modify the fall check to use the timer
                if (!IsGround && fallTimer >= fallThreshold)
                {
                    GoToState(STATE.FALL);
                }

                // Play footstep sounds while moving
                if (IsGround && movingVec.magnitude > 0.1f)
                {
                    TryPlayFootstep();
                }

                newVelocity = movingVec.magnitude * (isSprinting ? runningSpeed : walkingSpeed);
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
                    playerVelocity.y = Mathf.Sqrt(6f * -2f * gravityValue); // Jump height of 6 units
                }

                if (StateInfo.normalizedTime > 0.9f && StateInfo.IsName("jump")) GoToState(STATE.FALL);
                break;

            case STATE.FALL:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    ResetHorizontalVelocity();
                    triggerEnter = false;
                }

                // Update fall animation timer
                fallAnimationTimer += Time.fixedDeltaTime;

                // Play fall animation only after delay
                if (fallAnimationTimer >= fallAnimationDelay && !StateInfo.IsName("fall"))
                {
                    anim.CrossFadeInFixedTime("fall", 0.1f);
                }

                // Keep the falling movement consistent with horizontal velocity
                break;
            case STATE.ATTACK:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime(attack == Attacks.Normal ? "NormalAttack" : "HeavyAttack", 0.1f);
                    AudioManager.Instance.PlaySFX("PlayerAttack");
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

            case STATE.DEAD:
                // No movement when dead
                return;
            case STATE.KNOCKBACK:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("hit_toBack", 0.1f);
                    triggerEnter = false;
                    knockBackTime = 0;
                }
                else
                {
                    playerVelocity = Vector3.Lerp(playerVelocity, Vector3.zero, Time.deltaTime);
                    ApplyVerticalMovement(playerVelocity);
                    if (knockBackTime >= knockBackMaxTime)
                        GoToState(STATE.IDLE);
                    else
                    {
                        knockBackTime += Time.deltaTime;
                    }
                }
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
        var newVelocity = new Vector3(currentVelocity.x, playerVelocity.y, currentVelocity.z);
        controller.Move(newVelocity * Time.deltaTime);
    }

    private void MoveOn(Vector2 movingInputs, float currentVelocity)
    {

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
        if (weapon.Name == "Axe")
        {
            axeHandler.SetActive(true);

            weapon.Entity.transform.SetParent(axeHandler.transform);
            weapon.Entity.transform.localPosition = Vector3.zero;
            weapon.Entity.transform.localRotation = Quaternion.identity;
            weapon.Entity.SetActive(true);
            knifeHandler.SetActive(false);
        }
        else if (weapon.Name == "Knife")
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
        if (weapon.Name == "Axe")
        {
            axeHandler.SetActive(false);
        }
        else if (weapon.Name == "Knife")
        {
            knifeHandler.SetActive(false);
        }
        weapon.Entity.SetActive(false);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
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
            if (fallAnimationTimer >= fallAnimationDelay)
            {
                GoToState(STATE.ROLL);
            }
            else
            {
                GoToState(STATE.LOCOMOTION);
            }
        }
    }

    private void ResetHorizontalVelocity()
    {
        lastVelocity = lastVelocity * 0.5f;
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
        playerVelocity = Vector3.zero;
        state = STATE.IDLE;
        anim.CrossFadeInFixedTime("idle", 0.1f);
        enabled = false;
    }
    public void Activate()
    {
        enabled = true;
    }

    public void PlayDeathAnimation()
    {
        GoToState(STATE.DEAD);
        anim.CrossFadeInFixedTime("Die", 0.1f);
        enabled = false;
    }

    public void Respawn()
    {
        enabled = true;
        GoToState(STATE.IDLE);
        anim.CrossFadeInFixedTime("idle", 0.1f);
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
        if (lastVelocity < Mathf.Lerp(walkingSpeed, runningSpeed, 0.5f)) return;
        if (_isThrust && controller.isGrounded)
        {
            if (state == STATE.LOCOMOTION || state == STATE.IDLE)
            {
                playerVelocity.y = Mathf.Sqrt(1.5f * -2f * gravityValue); // Jump height of 1.5 units
                GoToState(STATE.JUMP);
            }
        }
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
    private void TryPlayFootstep()
    {
        if (Time.time - lastFootstepTime >= (isSprinting ? footstepInterval * 0.6f : footstepInterval))
        {
            AudioManager.Instance.PlaySFX(footstepSound, transform.position, 0.5f);
            lastFootstepTime = Time.time;
        }
    }

    public string GetInteractionHint()
    {
        var npc = CheckLookAtNpc();
        if (npc != null)
        {
            return "點擊 滑鼠右鍵 進行對話";
        }

        var item = CheckLookAtItem();
        if (item != null)
        {
            return $"點擊 滑鼠右鍵 拾取 {item.RealName}";
        }

        return null;
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
        ATTACK,
        DEAD,
        KNOCKBACK
    }

    #region kickback
    float knockBackTime = 0;
    float knockBackMaxTime = 0.7f;

    public void Knockback(Vector3 transformPosition)
    {
        if (state == STATE.KNOCKBACK)
        {
            return;
        }
        var direction = transform.position - transformPosition;
        playerVelocity = direction * 10;
        AudioManager.Instance.PlaySFX("MonsterAttack", transform.position, 1f);
        GoToState(STATE.KNOCKBACK);
    }
    #endregion
}
