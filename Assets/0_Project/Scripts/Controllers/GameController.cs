using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        /*private void Update()
        {
            stateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            stateMachine.CurrentState.PhysicsUpdate();
        }*/
        
        private async void RestartScene()
        {
            ScreenLoading.SetActive(true);

            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}