using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class WallJumpState : BaseState {
        public WallJumpState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() {
            // Set jump parameters, play jump animation, etc.
        }

        public override void Update() {
            // Any logic to run while in the jump state
            // You might want to handle some jump-specific behavior here
        }

        public override void FixedUpdate() {
            player.HandleWallJump(); // Handle jumping logic here
        }

        public override void OnExit() {
            // Reset jump parameters or perform any cleanup needed
        }
    }
}
