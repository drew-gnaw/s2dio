using UnityEngine;
using S2dio.Player;

namespace S2dio.State {
    public class FallState : BaseState
    {
        public FallState(PlayerController player, Animator animator) : base(player, animator) { }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            player.HandleFall();
        }
    }
}