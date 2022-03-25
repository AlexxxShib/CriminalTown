using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
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
    public class EntityPlayer : SignalReceiver, IReceive<SignalPoliceStatus>
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
        private CitizenConnector _citizenConnector;
        private ShelterConnector _shelterConnector;

        private CompTriggerAgent _moneyTriggerAgent;
        private ParticleSystemForceField _ownForceField;

        private ConfigMain _configMain;
        private DataGameState _gameState;

        private List<ParticleSystem.Particle> _moneyParticles = new();
        private int _emitMoneyCounterInside;
        private int _emitMoneyCounterOutside;
        
        private int _hitCounter;
        private bool _stealMoneyInProgress;

        private CrimeType _lastCrimeType;
        private int _lastCrimeReward;

        private void Awake()
        {
            _configMain = ToolBox.Get<ConfigMain>();
            _gameState = ToolBox.Get<DataGameState>();
            
            ToolBox.Add(this);
            
            _humanControl = GetComponent<CompHumanControl>();
            
            _islandConnector = GetComponent<IslandConnector>();
            _islandConnector.OnConnected += OnIslandConnected;
            _islandConnector.OnDisconnected += OnIslandDisconnected;

            _citizenConnector = GetComponent<CitizenConnector>();
            _citizenConnector.OnConnected += OnCitizenConnected;
            _citizenConnector.OnDisconnected += OnCitizenDisconnected;
            
            _shelterConnector = GetComponent<ShelterConnector>();
            _shelterConnector.OnConnected += OnShelterConnected;
            _shelterConnector.OnDisconnected += OnShelterDisconnected;

            _moneyTriggerAgent = moneyEmitter.GetComponent<CompTriggerAgent>();
            _ownForceField = GetComponentInChildren<ParticleSystemForceField>();

            _moneyTriggerAgent.onCallParticleTrigger += OnMoneyParticlesTrigger;

            timelineStealMoney.played += director =>
            {
                _stealMoneyInProgress = true;
            };

            timelineStealMoney.stopped += director =>
            {
                _stealMoneyInProgress = false;
            };
        }

        private void Update()
        {
            if (isHidden && _humanControl.joystick.HasInput)
            {
                GetComponent<NavMeshAgent>().enabled = true;
            }
        }

        private void SetupMoneyEmitter(ParticleSystemForceField forceField, Collider collider)
        {
            moneyEmitter.externalForces.RemoveAllInfluences();
            moneyEmitter.externalForces.AddInfluence(forceField);

            moneyEmitter.trigger.AddCollider(collider);
        }

        private void CleanupMoneyEmitter(Collider collider)
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
            
            logger.LogDebug($"+island {island.gameObject.name}");
            
            SetupMoneyEmitter(island.ForceField, island.ForceFieldCollider);

            var emitDelay = _configMain.emitResourceTime;
            var minEmitDelay = emitDelay / 2;

            _emitMoneyCounterInside = 0;
            _emitMoneyCounterOutside = 0;

            while (_islandConnector.IsReady && _gameState.money > 0)
            {
                if (island.data.state == IslandState.OPENED)
                {
                    break;
                }

                _emitMoneyCounterOutside++;
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
        }

        private async void OnCitizenConnected(EntityCitizen citizen)
        {
            logger.LogDebug($"+citizen {citizen.gameObject.name}");
            
            if (isCaught || isHidden)
            {
                return;
            }
            
            transform.DOLookAt(citizen.transform.position, 0.25f);
            await citizen.transform.DOLookAt(transform.position, 0.25f).IsComplete();

            if (!_citizenConnector.IsReady)
            {
                return;
            }

            _emitMoneyCounterInside = 0;
            _emitMoneyCounterOutside = 0;
            _hitCounter = 0;
            
            var playableAsset = (TimelineAsset) timelineStealMoney.playableAsset;
            
            /*foreach (var binding in playableAsset.outputs)
            {
                logger.LogDebug(binding.streamName);
            }*/
            
            var trackKey = playableAsset.GetOutputTrack(2);
            timelineStealMoney.SetGenericBinding(trackKey, citizen.GetComponentInChildren<Animator>());
            timelineStealMoney.Play();

            _lastCrimeType = CrimeType.STEAL_CITIZEN;
            _lastCrimeReward = citizen.reward;
            
            SetupMoneyEmitter(_ownForceField, _ownForceField.GetComponent<Collider>());
        }

        private void OnCitizenDisconnected(EntityCitizen citizen)
        {
            logger.LogDebug($"-citizen {citizen.gameObject.name}");

            if (_stealMoneyInProgress)
            {
                timelineStealMoney.Stop();
                citizen.SetPanic();
            }

            _stealMoneyInProgress = false;
            
            CleanupMoneyEmitter(_ownForceField.GetComponent<Collider>());
        }

        public void OnHitCitizen()
        {
            _hitCounter++;

            var lastHit = _hitCounter == _configMain.citizenHealth;
            var citizen = _citizenConnector.ConnectedObject;
            
            // logger.LogDebug($"hiy citizen {citizen.gameObject.name}");
            
            if (citizen.ApplyHit(lastHit))
            {
                _emitMoneyCounterOutside++;
                moneyEmitter.Emit(Random.Range(1, 3));
                
                _gameState.AddMoney(citizen.reward);
            }
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

            /*_emitMoneyCounterInside += particlesCount;

            for (int i = 0; i < particlesCount; i++)
            {
                _gameState.AddMoney(_lastCrimeReward);
            }

            if (_emitMoneyCounterInside == _emitMoneyCounterOutside)
            {
                CleanupMoneyEmitter(_ownForceField.GetComponent<Collider>());
            }*/
        }

        public void Catch()
        {
            logger.LogDebug("player is caught");

            isCaught = true;

            _humanControl.InputEnabled = false;
            
            ToolBox.Signals.Send<SignalPlayerCaught>();
        }

        public void HandleSignal(SignalPoliceStatus signal)
        {
            if (!signal.activated)
            {
                moneyEmitter.Emit(3);
                
                _gameState.AddMoney(ToolBox.Get<ConfigBalance>().policeReward);
            }
        }
    }
}