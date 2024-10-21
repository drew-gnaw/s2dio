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

        public bool IsSlidingLeft { get; private set; } = false;
        public bool IsSlidingRight { get; private set; } = false;

        private StateMachine stateMachine;
        private WalkState walkState;
        private JumpState jumpState;
        private SlideState slideState;
        private WallJumpState wallJumpState;
        private FallState fallState;
        
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
            slideState = new SlideState(this, animator);
            wallJumpState = new WallJumpState(this, animator);
            fallState = new FallState(this, animator);

            At(walkState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            Any(walkState, new FuncPredicate(ReturnTowalkState));

            At(jumpState, fallState, new FuncPredicate(() => rb.velocity.y < -0.1f));
            At(wallJumpState, fallState, new FuncPredicate(() => rb.velocity.y < -0.1f));
            At(walkState, fallState, new FuncPredicate(() => rb.velocity.y < -0.1f));
            
            At(fallState, slideState, new FuncPredicate(() => IsSlidingLeft || IsSlidingRight));
            
            At(slideState, walkState, new FuncPredicate(() => !IsSlidingLeft && !IsSlidingRight));
            At(slideState, wallJumpState, new FuncPredicate(() => wallJumpTimer.IsRunning));
            At(jumpState, wallJumpState, new FuncPredicate(() => wallJumpTimer.IsRunning));

            stateMachine.SetState(walkState);
        }

        bool ReturnTowalkState()
        {
            return isGrounded && !jumpTimer.IsRunning && !wallJumpTimer.IsRunning;
        }

        void SetupTimers()
        {
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            wallJumpTimer = new CountdownTimer(jumpDuration);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            wallJumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
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
            if (IsSlidingLeft || IsSlidingRight)
            {
                if (performed && !wallJumpTimer.IsRunning)
                {
                    wallJumpTimer.Start();
                }
                else if (!performed && wallJumpTimer.IsRunning)
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
            if (!jumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        public void HandleWallJump(int xVelocity)
        {
            if (!wallJumpTimer.IsRunning)
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            

            Debug.Log(xVelocity);

            rb.velocity = new Vector2(xVelocity * wallJumpPower, jumpVelocity);
        }

        public void HandleFall()
        {
            jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        public void ZeroYVelocity()
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
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

            IsSlidingLeft = leftWallCheck.IsTouchingLayers(groundLayer) && isLeftKeyPressed;
            IsSlidingRight = rightWallCheck.IsTouchingLayers(groundLayer) && isRightKeyPressed;

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
