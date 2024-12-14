using UnityEngine;

public class BetterPlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("speed");
    public float velocity = 10f;
    public float runningSpeed = 20f;
    private readonly float rotationSpeed = 5f;
    private Animator anim;
    private bool isIdle;
    private bool isSprinting;
    private bool isThrust;
    private bool isWalking;
    private Rigidbody rigid;
    private Vector2 movingVec;
    private Vector2 lookVec;
    private STATE state = STATE.IDLE;
    private bool triggerEnter;
    [SerializeField] private Camera cam;
    private AnimatorStateInfo StateInfo => anim.GetCurrentAnimatorStateInfo(0);

    private bool IsGround => Physics.Raycast(transform.position, Vector3.down, 0.5f);

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = gameObject.transform.GetComponentInChildren<Animator>();
        GoToState(STATE.IDLE);
        KeyboardInputManager.Instance.evtDpadAxis.AddListener(Move);
        KeyboardInputManager.Instance.evtJump.AddListener(Jump);
        KeyboardInputManager.Instance.evtRun.AddListener(Sprint);
        KeyboardInputManager.Instance.evtLook.AddListener(Look);
        
    }

    private void Sprint(bool sprint)
    {
        isSprinting =sprint;
    }


    private void Update()
    {
        var newVelocity = Vector3.zero;
        switch (state)
        {
            case STATE.IDLE:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("idle", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude > 0.1f) GoToState(STATE.LOCOMOTION);
                newVelocity = new Vector3(0f, rigid.linearVelocity.y, 0f);
                break;

            case STATE.LOCOMOTION:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("locomotion", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude <= 0.1f) GoToState(STATE.IDLE);
                if (!IsGround) GoToState(STATE.FALL);
                newVelocity = movingVec * ( isSprinting? runningSpeed : velocity);
                anim.SetFloat(Speed, movingVec.magnitude * 1.0f + isSprinting.CompareTo(false) * 1.0f);
                break;

            case STATE.JUMP:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("jump", 0.1f);
                    triggerEnter = false;
                    rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
                    isThrust = false;
                    newVelocity = rigid.linearVelocity;
                }
                else 
                    newVelocity = rigid.linearVelocity;
                if (StateInfo.normalizedTime > 0.8f && StateInfo.IsName("jump")) GoToState(STATE.FALL);
                break;
            case STATE.FALL:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("fall", 0.1f);
                    triggerEnter = false;
                }
                    newVelocity = rigid.linearVelocity;

                break;
            case STATE.ROLL:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("roll_forward", 0.1f);
                    triggerEnter = false;
                }

                if (StateInfo.normalizedTime > 0.8f && StateInfo.IsName("roll_forward")) GoToState(STATE.IDLE);
                newVelocity = rigid.linearVelocity;
                break;
        }

        MoveOn(newVelocity);
    }

    private void MoveOn(Vector3 currentVelocity)
    {
        if (currentVelocity == Vector3.zero) return;
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0; // 移除垂直分量，保持水平運動
        cameraForward.Normalize();

        Vector3 cameraRight = cam.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // 將輸入向量轉換為世界座標的方向向量
        Vector3 direction = cameraForward * currentVelocity.y + cameraRight * currentVelocity.x;

        // 計算目標旋轉
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 計算並應用運動速度
        var newVelocity = direction;
        print($"direction: {direction}, newVelocity: {newVelocity}");
        newVelocity.y = rigid.linearVelocity.y; // 保持垂直速度
        rigid.linearVelocity = newVelocity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (state == STATE.JUMP)
            GoToState(STATE.IDLE);
        else if (state == STATE.FALL) GoToState(STATE.ROLL);
        //anim.SetBool("isGround", true);
    }

    private void GoToState(STATE newState)
    {
        state = newState;
        triggerEnter = true;
    }

    private void Move(Vector2 vac)
    {
        movingVec = vac;
    }

    private void Look(Vector2 vac)
    {
        lookVec = vac;
    }

    public void Jump(bool _isThrust)
    {
        if (_isThrust && IsGround)
            if (state == STATE.IDLE || state == STATE.LOCOMOTION)
            {
                isThrust = true;
                GoToState(STATE.JUMP);
            }
    }


    private enum STATE
    {
        LOCOMOTION,
        IDLE,
        JUMP,
        FALL,
        ROLL
    }
}