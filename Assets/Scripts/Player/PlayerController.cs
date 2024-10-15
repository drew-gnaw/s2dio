using UnityEngine;
using S2dio.State;

namespace S2dio.Player {
    public class PlayerController : MonoBehaviour {
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
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

        void Start() {
            rb = GetComponent<Rigidbody2D>();
            SetupStateMachine();
        }

        void SetupStateMachine() {
            stateMachine = new StateMachine();

            // Initialize states
            walkState = new WalkState(this, animator);
            jumpState = new JumpState(this, animator);
            
            // Add transitions based on conditions
            stateMachine.AddTransition(walkState, jumpState, new FuncPredicate(() => !isGrounded));
            stateMachine.AddTransition(jumpState, walkState, new FuncPredicate(() => isGrounded));

            // Set initial state
            stateMachine.SetState(walkState);
        }

        void Update() {
            stateMachine.Update();
            HandleGroundCheck();
            HandleInput();
        }

        void HandleInput() {
            // Handle horizontal movement
            float moveInput = Input.GetAxis("Horizontal");
            HandleMovement(moveInput);

            // Jump logic
            if (Input.GetButtonDown("Jump") && isGrounded) {
                HandleJump();
            }

            // Attack logic
            if (Input.GetButtonDown("Fire1")) {
                currentWeapon.Attack();
            }
        }

        public void HandleMovement(float moveInput) {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        public void HandleJump() {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        void HandleGroundCheck() {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // Method to access isGrounded for states
        public bool IsGrounded() {
            return isGrounded;
        }
    }
}
