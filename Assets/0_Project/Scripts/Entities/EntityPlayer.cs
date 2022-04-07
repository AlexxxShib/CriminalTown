using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using CriminalTown.Data;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;
using SignalReceiver = Mobiray.Common.SignalReceiver;

namespace CriminalTown.Entities
{
    public class EntityPlayer : SignalReceiver, 
        IReceive<SignalPoliceStatus>, 
        IReceive<SignalAddMoney>, 
        IReceive<SignalIslandPurchased>,
        IReceive<SignalUICaughtContinue>
    {
        public MobirayLogger logger;
        
        [Space]
        public GameObject view;
        public ParticleSystem moneyEmitter;
        public PlayableDirector timelineStealMoney;

        [Space]
        public bool isCaught;
        public bool isHidden;
        public bool hasPoliceVisor;

        public bool CrimeInProgress => _stealMoneyInProgress;
        
        private CompHumanControl _humanControl;
        private IslandConnector _islandConnector;
        private ShelterConnector _shelterConnector;

        private CompTriggerAgent _moneyTriggerAgent;
        private ParticleSystemForceField _ownForceField;

        private CompHelperArrow _helperArrow;

        private ConfigMain _configMain;
        private DataGameState _gameState;

        private List<ParticleSystem.Particle> _moneyParticles = new();
        private int _emitMoneyCounterInside;
        
        private int _hitCounter;
        private bool _stealMoneyInProgress;

        private bool _policeActivated;

        private void Awake()
        {
            _configMain = ToolBox.Get<ConfigMain>();
            _gameState = ToolBox.Get<DataGameState>();
            
            ToolBox.Add(this);
            
            _humanControl = GetComponent<CompHumanControl>();
            
            _islandConnector = GetComponent<IslandConnector>();
            _islandConnector.OnConnected += OnIslandConnected;
            _islandConnector.OnDisconnected += OnIslandDisconnected;

            _shelterConnector = GetComponent<ShelterConnector>();
            _shelterConnector.OnConnected += OnShelterConnected;
            _shelterConnector.OnDisconnected += OnShelterDisconnected;

            _moneyTriggerAgent = moneyEmitter.GetComponent<CompTriggerAgent>();
            _ownForceField = GetComponentInChildren<ParticleSystemForceField>();

            _moneyTriggerAgent.onCallParticleTrigger += OnMoneyParticlesTrigger;

            _helperArrow = GetComponentInChildren<CompHelperArrow>();

            timelineStealMoney.played += director =>
            {
                SetCrime(true);
            };

            timelineStealMoney.stopped += director =>
            {
                SetCrime(false);
            };
        }

        public void SetCrime(bool active)
        {
            _stealMoneyInProgress = active;
        }

        private void Start()
        {
            FindAvailableIsland();
        }

        private void Update()
        {
            if (isHidden && _humanControl.joystick.HasInput)
            {
                GetComponent<NavMeshAgent>().enabled = true;
            }
        }

        private void FindAvailableIsland()
        {
            if (_policeActivated)
            {
                return;
            }
            
            var availableIslands = new List<EntityIsland>();
            var branches = ToolBox.Get<GameController>().islands;
            
            foreach (var branch in branches)
            {
                foreach (var island in branch)
                {
                    if (island.data.state == IslandState.AVAILABLE && 
                        island.data.currentPrice <= _gameState.money)
                    {
                        availableIslands.Add(island);
                    }
                }
            }

            if (availableIslands.Count == 0)
            {
                _helperArrow.Forget();
                return;
            }
            
            availableIslands.Sort(
                (i1, i2) => i1.data.currentPrice - i2.data.currentPrice);
            
            _helperArrow.SetTarget(availableIslands[0].offer.transform);
        }

        public void SetupMoneyEmitter()
        {
            SetupMoneyEmitter(_ownForceField, _ownForceField.GetComponent<Collider>());
        }

        public void SetupMoneyEmitter(ParticleSystemForceField forceField, Collider collider)
        {
            moneyEmitter.externalForces.RemoveAllInfluences();
            moneyEmitter.externalForces.AddInfluence(forceField);

            moneyEmitter.trigger.AddCollider(collider);
        }
        
        public void CleanupMoneyEmitter()
        {
            moneyEmitter.externalForces.RemoveAllInfluences();
            moneyEmitter.trigger.RemoveCollider(_ownForceField.GetComponent<Collider>());
        }

        public void CleanupMoneyEmitter(Collider collider)
        {
            moneyEmitter.externalForces.RemoveAllInfluences();
            moneyEmitter.trigger.RemoveCollider(collider);
        }

        private async void OnIslandConnected(EntityIsland island)
        {
            if (isCaught)
            {
                return;
            }
            
            _helperArrow.Forget();
            
            logger.LogDebug($"+island {island.gameObject.name}");
            
            SetupMoneyEmitter(island.ForceField, island.ForceFieldCollider);

            var emitDelay = _configMain.emitResourceTime;
            var minEmitDelay = emitDelay / 2;

            _emitMoneyCounterInside = 0;

            while (_islandConnector.IsReady && _gameState.money > 0)
            {
                if (island.data.state == IslandState.OPENED)
                {
                    break;
                }

                moneyEmitter.Emit(1);
                // moneyEmitter.Play();
                
                await Task.Delay(TimeSpan.FromSeconds(
                    Mathf.Clamp(emitDelay, minEmitDelay, _configMain.emitResourceTime)));

                emitDelay *= 0.8f;
            }
        }
        
        private void OnIslandDisconnected(EntityIsland island)
        {
            logger.LogDebug($"-island {island.gameObject.name}");
            
            CleanupMoneyEmitter(island.ForceFieldCollider);
            
            this.NextFrame(FindAvailableIsland);
        }

        public void AddMoney(int reward, int emitCount = 1)
        {
            moneyEmitter.Emit(emitCount);
            _gameState.AddMoney(reward);
        }

        private void OnShelterConnected(EntityShelter shelter)
        {
            if (hasPoliceVisor)
            {
                return;
            }
            
            logger.LogDebug($"+shelter {shelter.gameObject.name}");
            
            shelter.EnterShelter();
            view.SetActive(false);

            GetComponent<NavMeshAgent>().enabled = false;

            isHidden = true;
        }

        private void OnShelterDisconnected(EntityShelter shelter)
        {
            if (!isHidden)
            {
                return;
            }
            
            logger.LogDebug($"-shelter {shelter.gameObject.name}");
            
            shelter.LeaveShelter();
            view.SetActive(true);

            isHidden = false;
        }

        private void OnMoneyParticlesTrigger()
        {
            var particlesCount = moneyEmitter.GetTriggerParticles(
                ParticleSystemTriggerEventType.Enter, _moneyParticles);

            if (particlesCount <= 0)
            {
                return;
            }

            if (_islandConnector.IsConnected)
            {
                var emitMoney = (int) (_configMain.emitMoneyMin * Mathf.Pow(1.5f, _emitMoneyCounterInside));

                var island = _islandConnector.ConnectedObject;

                var maxMoney = Mathf.Min(_gameState.money, island.data.currentPrice, emitMoney,
                    _configMain.emitMoneyMax);
                emitMoney = maxMoney;

                _gameState.AddMoney(-emitMoney);
                island.AddMoney(emitMoney);

                // Debug.Log($"add island money {emitMoney}");

                if (island.data.state == IslandState.OPENED)
                {
                    moneyEmitter.Clear(true);
                }

                _emitMoneyCounterInside++;
                return;
            }
        }

        public void Catch()
        {
            logger.LogDebug("player is caught");

            isCaught = true;

            _humanControl.InputEnabled = false;
            
            ToolBox.Signals.Send<SignalPlayerCaught>();
        }

        public void CaughtContinue()
        {
            isCaught = false;

            _humanControl.InputEnabled = true;
        }

        public void HandleSignal(SignalPoliceStatus signal)
        {
            if (!signal.activated && !signal.caught)
            {
                moneyEmitter.Emit(3);
                
                _gameState.AddMoney(ToolBox.Get<ConfigBalance>().policeReward);
                
                FindAvailableIsland();
            }

            _policeActivated = signal.activated;

            if (_policeActivated)
            {
                _helperArrow.Forget();
            }
        }

        public void HandleSignal(SignalAddMoney signal)
        {
            FindAvailableIsland();
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            FindAvailableIsland();
        }

        public void HandleSignal(SignalUICaughtContinue signal)
        {
            CaughtContinue();
        }
    }
}