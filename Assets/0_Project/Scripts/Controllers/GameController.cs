using System.Runtime.CompilerServices;
using Mobiray.Common;
using UnityEngine;

namespace Template.Controllers
{
    public class GameController : BaseGameController
    {
        
        [Header("UI/UX")]
        public GameObject ScreenLoading;
        public GameObject ScreenMain;
        
        private MobirayLogger logger = new MobirayLogger("GameController");

        protected override void Awake()
        {
            base.Awake();
            
            ScreenLoading.SetActive(true);
            
            /*stateMachine = new StateMachine<GameController>();

            StatePreparing = Instantiate(StatePreparing);
            StatePreparing.Initialize(this, stateMachine);

            StatePushing = Instantiate(StatePushing);
            StatePushing.Initialize(this, stateMachine);

            stateMachine.Initialize(StatePreparing);*/
        }
    }
}