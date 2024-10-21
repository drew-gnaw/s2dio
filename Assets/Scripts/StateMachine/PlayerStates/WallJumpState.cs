using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class WallJumpState : BaseState {
        public WallJumpState(PlayerController player, Animator animator) : base(player, animator) { }
        private int wallJumpDirection;

        public override void OnEnter() {
            player.ZeroYVelocity();
            wallJumpDirection = 0;

            if (player.IsSlidingLeft)
            {
                wallJumpDirection = 1;
            }
            else if (player.IsSlidingRight)
            {
                wallJumpDirection = -1;
            }
            
            player.AddHorizontalVelocity(wallJumpDirection * player.wallJumpPower);
        }

        public override void Update() {
            // Any logic to run while in the jump state
            // You might want to handle some jump-specific behavior here
        }

        public override void FixedUpdate() {
            player.HandleWallJump(wallJumpDirection);
        }

        public override void OnExit()
        {
            wallJumpDirection = 0;
        }
    }
}
