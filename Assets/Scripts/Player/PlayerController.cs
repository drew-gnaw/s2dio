using UnityEngine;
using S2dio.State;
using S2dio.Utils;
using System.Collections;

namespace S2dio.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public LayerMask groundLayer;
        public BoxCollider2D groundCheck;
        public BoxCollider2D leftWallCheck;
        public BoxCollider2D rightWallCheck;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 7f;
        [SerializeField] float jumpDuration = 0.2f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float maxSlidingSpeed = 4f;
        [SerializeField] float wallJumpPower = 2f;


        public float moveSpeed = 5f;
        public float attackCooldownDuration = 0.5f;
        public Animator animator;
        public float groundCheckRadius = 0.2f;
        public WeaponClass currentWeapon;
        public WeaponClass offhandWeapon;

        private Rigidbody2D rb;
        private bool isGrounded = false;
        private bool isSlidingLeft = false;
        private bool isSlidingRight = false;
        private StateMachine stateMachine;
        private WalkState walkState;
        private JumpState jumpState;
        private SlidingState slidingState;
        private WallJumpState wallJumpState;

        // Timers
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer wallJumpTimer;


        private CountdownTimer attackTimer;
        private bool allowHorizontalInput = true;

        float jumpVelocity;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            SetupStateMachine();
            SetupTimers();
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            walkState = new WalkState(this, animator);
            jumpState = new JumpState(this, animator);
            slidingState = new SlidingState(this, animator);
            wallJumpState = new WallJumpState(this, animator);

            At(walkState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            Any(walkState, new FuncPredicate(ReturnTowalkState));
            // Any(slidingState, new FuncPredicate(() => isSlidingLeft || isSlidingRight));
            At(walkState, slidingState, new FuncPredicate(() => isSlidingLeft || isSlidingRight));
            At(jumpState, slidingState, new FuncPredicate(() => isSlidingLeft || isSlidingRight));
            At(slidingState, walkState, new FuncPredicate(() => !isSlidingLeft && !isSlidingRight));
            At(slidingState, wallJumpState, new FuncPredicate(() => wallJumpTimer.IsRunning));
            At(wallJumpState, walkState, new FuncPredicate(() => !wallJumpTimer.IsRunning));

            stateMachine.SetState(walkState);
        }

        bool ReturnTowalkState()
        {
            return isGrounded && !jumpTimer.IsRunning;
        }

        void SetupTimers()
        {
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            wallJumpTimer = new CountdownTimer(jumpDuration);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            wallJumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            attackTimer = new CountdownTimer(attackCooldownDuration);
        }

        void Update()
        {
            stateMachine.Update();
            HandleGroundCheck();
            HandleWallCheck();
            HandleInput();
            HandleTimers();
        }

        void HandleTimers()
        {
            jumpTimer.Tick(Time.deltaTime);
            wallJumpTimer.Tick(Time.deltaTime);
            attackTimer.Tick(Time.deltaTime);
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void HandleInput()
        {
            if (allowHorizontalInput)
            {
                float moveInput = Input.GetAxis("Horizontal");
                HandleMovement(moveInput);
            }

            if (Input.GetButtonDown("Jump"))
            {
                OnJump(true);
            }
            else if (Input.GetButtonUp("Jump"))
            {
                OnJump(false);
            }

            if (Input.GetButtonDown("Fire1") && !attackTimer.IsRunning)
            {
                HandleAttack();
            }
        }

        void OnJump(bool performed)
        {
            if (isSlidingLeft || isSlidingRight)
            {
                if (performed && !jumpTimer.IsRunning)
                {
                    wallJumpTimer.Start();
                }
                else if (!performed && jumpTimer.IsRunning)
                {
                    wallJumpTimer.Stop();
                }
            }
            else
            {
                if (performed && !jumpTimer.IsRunning && isGrounded)
                {
                    jumpTimer.Start();
                }
                else if (!performed && jumpTimer.IsRunning)
                {
                    jumpTimer.Stop();
                }
            }
        }

        public void HandleJump()
        {
            if (!jumpTimer.IsRunning && isGrounded)
            {
                jumpVelocity = 0f;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        public void HandleWallJump()
        {
            if (!wallJumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            float xVelocity = 0;

            if (isSlidingLeft)
            {
                xVelocity = 1;
            }
            else if (isSlidingRight)
            {
                xVelocity = -1;
            }

            if (xVelocity != 0)
            {
                StartCoroutine(DisableHorizontalMovementCoroutine(0.5f));
            }

            rb.velocity = new Vector2(xVelocity * wallJumpPower, jumpVelocity);
        }


        public void HandleAttack()
        {
            currentWeapon.Attack();
            attackTimer.Start();
        }

        public void HandleSlide()
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxSlidingSpeed));
        }

        void HandleGroundCheck()
        {
            isGrounded = groundCheck.IsTouchingLayers(groundLayer);
        }

        void HandleWallCheck()
        {
            bool isLeftKeyPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            bool isRightKeyPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

            isSlidingLeft = leftWallCheck.IsTouchingLayers(groundLayer) && isLeftKeyPressed;
            isSlidingRight = rightWallCheck.IsTouchingLayers(groundLayer) && isRightKeyPressed;

        }

        public void HandleMovement(float moveInput)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        private IEnumerator DisableHorizontalMovementCoroutine(float seconds)
        {
            allowHorizontalInput = false;
            yield return new WaitForSeconds(seconds);
            allowHorizontalInput = true;
        }
    }

}
