using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CriminalTown.States;
using Mobiray.Common;
using Mobiray.StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace CriminalTown.Controllers
{
    public class GameController : BaseGameController
    {
        
        [Header("UI/UX")]
        public GameObject screenLoading;
        public GameObject screenMain;
        public GameObject screenGameOver;
        public GameObject screenLevelComplete;

        [Space]
        public TextMeshProUGUI textLevel;

        [Header("MAIN SETTINGS")]
        public NavMeshSurface islandSurface;
        public Transform parentIslands;

        [Header("STATES")]
        public StatePreparing statePreparing;
        public StateMainLoop stateMainLoop;
        public StateGameOver stateGameOver;
        public StateLevelComplete stateLevelComplete;

        [Header("REALTIME")]
        private MobirayLogger _logger = new MobirayLogger("GameController");

        private StateMachine<GameController> _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            
            screenLoading.SetActive(true);
            
            _stateMachine = new StateMachine<GameController>();

            statePreparing = Instantiate(statePreparing);
            statePreparing.Initialize(this, _stateMachine);

            stateMainLoop = Instantiate(stateMainLoop);
            stateMainLoop.Initialize(this, _stateMachine);

            stateGameOver = Instantiate(stateGameOver);
            stateGameOver.Initialize(this, _stateMachine);

            stateLevelComplete = Instantiate(stateLevelComplete);
            stateLevelComplete.Initialize(this, _stateMachine);

            _stateMachine.Initialize(statePreparing);
        }
        
        private void Update()
        {
            _stateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            _stateMachine.CurrentState.PhysicsUpdate();
        }
        
        private async void RestartScene()
        {
            screenLoading.SetActive(true);

            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClickRestartScene()
        {
            RestartScene();
        }
    }
}