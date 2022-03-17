using System;
using System.Collections.Generic;
using _0_Project.Scripts.Controllers.UI;
using Mobiray.Common;
using NaughtyAttributes;
using Prehistoric.Controllers;
using Template;
using Template.Configs;
using Template.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _0_Project.Scripts.Controllers
{
    public class GatheringController : MonoBehaviour
    {
        private ConfigMissions _configMissions;
        private DataGameState _gameState;
        private ProgressBarController _progressBar;
        private Mission _mission;

        private void Awake()
        {
            _configMissions = ToolBox.Get<ConfigMissions>();
            _gameState = ToolBox.Get<DataGameState>();
            _mission = _configMissions.GetMission(_gameState.CurrentMission);
            _progressBar = FindObjectOfType<ProgressBarController>();
            
            //_progressBar.LoadMissionTask(_mission);
            var levelController = FindObjectOfType<LevelChooser>();
            //levelController.SetLevel(_configMissions.GetMission(_gameState.CurrentMission).IndexOnScene);
        }

        private void Start()
        {
            
           /* var allAnimals = new List<AnimalController>(FindObjectsOfType<AnimalController>());
            _animals = new List<AnimalController>();
            foreach (var animal in allAnimals)
            {
                if (animal.gameObject.active)
                {
                    _animals.Add(animal);
                }
            }*/


            
        }

        [Button("End mission")]
        public void EndMission()
        {
            _gameState.CurrentMission++;
            _gameState.SkinsTransaction(1);
            SceneManager.LoadScene("CampScene");
        }
        
        private void Update()
        {
           /* if (_gameover) return;
            bool allDead = true;
            foreach (var animal in _animals)
            {
                if (!animal.Dead || !allDead) allDead = false;
            }

            if (allDead)
            {
                _gameover = true;
                // add finish screen
                _gameState.CurrentMission++;
                _gameState.SkinsTransaction(1);
                SceneManager.LoadScene("CampScene");
            }*/
        }
    }
}