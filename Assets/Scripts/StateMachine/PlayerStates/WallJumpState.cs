using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class WallJumpState : BaseState {
        public WallJumpState(PlayerController player, Animator animator) : base(player, animator) { }
        private int xVelocity;

        public override void OnEnter() {
            player.ZeroYVelocity();
            xVelocity = 0;

            if (player.IsSlidingLeft)
            {
                xVelocity = 1;
            }
            else if (player.IsSlidingRight)
            {
                xVelocity = -1;
            }
        }

        public override void Update() {
            // Any logic to run while in the jump state
            // You might want to handle some jump-specific behavior here
        }

        public override void FixedUpdate() {
            player.HandleWallJump(xVelocity);
        }

        public override void OnExit()
        {
            xVelocity = 0;
        }
    }
}
