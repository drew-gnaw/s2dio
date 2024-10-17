using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class SlidingState : BaseState
    {
        public SlidingState(PlayerController player, Animator animator) : base(player, animator) { }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            // Check if player is sliding left or right and adjust behavior accordingly
        }

        public override void FixedUpdate()
        {
            player.HandleSlide();
        }
    }
}