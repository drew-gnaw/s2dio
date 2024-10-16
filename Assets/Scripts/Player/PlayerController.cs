using UnityEngine;
using S2dio.State;
using S2dio.Utils;

namespace S2dio.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float attackCooldownDuration = 0.5f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;

        public LayerMask groundLayer;
        public Transform groundCheck;
        public Animator animator;
        public float groundCheckRadius = 0.2f;
        public WeaponClass currentWeapon;
        public WeaponClass offhandWeapon;

        private Rigidbody2D rb;
        private bool isGrounded = false;
        private StateMachine stateMachine;
        private WalkState walkState;
        private JumpState jumpState;

        // Timers
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer attackTimer;

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

            At(walkState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            Any(walkState, new FuncPredicate(ReturnTowalkState));

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

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            attackTimer = new CountdownTimer(attackCooldownDuration);
        }

        void Update()
        {
            stateMachine.Update();
            HandleGroundCheck();
            HandleInput();
            HandleTimers();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        void HandleInput()
        {
            float moveInput = Input.GetAxis("Horizontal");
            HandleMovement(moveInput);

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
            if (performed && !jumpTimer.IsRunning && !jumpTimer.IsRunning && isGrounded)
            {
                // Start jump action
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                // End jump action (jump button released)
                jumpTimer.Stop();
            }
        }



        public void HandleJump()
        {
            if (!jumpTimer.IsRunning && isGrounded)
            {
                jumpVelocity = 0f;
                return;
            }
            
            if (!jumpTimer.IsRunning) {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            Debug.Log(jumpVelocity);

            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        public void HandleAttack()
        {
            currentWeapon.Attack();
            attackTimer.Start();
        }

        void HandleGroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void HandleTimers()
        {
            jumpTimer.Tick(Time.deltaTime);
            attackTimer.Tick(Time.deltaTime);
        }

        public void HandleMovement(float moveInput)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

}
