using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class WalkState : BaseState {
        public WalkState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            // Initialize walking state (e.g., set animations)
        }

        public override void Update() {
            
        }

        public override void OnExit() {
            // Cleanup when exiting the walking state (e.g., reset animations)
        }

        public override void FixedUpdate()
        {
            float moveInput = Input.GetAxis("Horizontal");
            player.HandleMovement(moveInput);
        }
    }
}
