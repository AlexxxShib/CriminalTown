using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mobiray.Common;
using Mobiray.StateMachine;
using Template.States;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Template.Controllers
{
    public class GameController : BaseGameController
    {
        
        [Header("UI/UX")]
        public GameObject ScreenLoading;
        public GameObject ScreenMain;
        public GameObject ScreenGameOver;
        public GameObject ScreenLevelComplete;

        [Space]
        public TextMeshProUGUI TextLevel;

        [Header("MAIN SETTINGS")]
        public Transform TutorialLevelsParent;
        public Transform LevelsParent;

        [Header("STATES")]
        public StatePreparing StatePreparing;
        public StateMainLoop StateMainLoop;
        public StateGameOver StateGameOver;
        public StateLevelComplete StateLevelComplete;

        [Header("REALTIME")]
        public Transform CurrentLevel;
        
        private MobirayLogger logger = new MobirayLogger("GameController");

        private StateMachine<GameController> stateMachine;

        protected override void Awake()
        {
            base.Awake();
            
            ScreenLoading.SetActive(true);
            
            stateMachine = new StateMachine<GameController>();

            StatePreparing = Instantiate(StatePreparing);
            StatePreparing.Initialize(this, stateMachine);

            StateMainLoop = Instantiate(StateMainLoop);
            StateMainLoop.Initialize(this, stateMachine);

            StateGameOver = Instantiate(StateGameOver);
            StateGameOver.Initialize(this, stateMachine);

            StateLevelComplete = Instantiate(StateLevelComplete);
            StateLevelComplete.Initialize(this, stateMachine);

            stateMachine.Initialize(StatePreparing);
        }
        
        private void Update()
        {
            stateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            stateMachine.CurrentState.PhysicsUpdate();
        }
        
        private async void RestartScene()
        {
            ScreenLoading.SetActive(true);

            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClickRestartScene()
        {
            RestartScene();
        }
    }
}