using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityPlayer : MonoBehaviour
    {
        public MobirayLogger logger;
        public ParticleSystem moneyEmitter;
        
        private CompHumanControl _humanControl;
        private IslandConnector _islandConnector;
        private CitizenConnector _citizenConnector;

        private ConfigMain _configMain;
        private DataGameState _gameState;

        private List<ParticleSystem.Particle> _moneyParticles = new();
        private int _emitMoneyCounter;

        private void Awake()
        {
            ToolBox.Add(this);
            
            _humanControl = GetComponent<CompHumanControl>();
            _islandConnector = GetComponent<IslandConnector>();
            _citizenConnector = GetComponent<CitizenConnector>();
            
            _islandConnector.OnConnected += OnIslandConnected;
            _islandConnector.OnDisconnected += OnIslandDisconnected;

            _citizenConnector.OnConnected += OnCitizenConnected;
            _citizenConnector.OnDisconnected += OnCitizenDisconnected;

            _configMain = ToolBox.Get<ConfigMain>();
            _gameState = ToolBox.Get<DataGameState>();

            moneyEmitter.GetComponent<CompTriggerAgent>().onCallParticleTrigger += OnMoneyParticlesTrigger;
        }

        private async void OnIslandConnected(EntityIsland island)
        {
            logger.LogDebug($"+island {island.gameObject.name}");

            var externalForces = moneyEmitter.externalForces;
            externalForces.RemoveAllInfluences();
            externalForces.AddInfluence(island.ForceField);

            var particlesTrigger = moneyEmitter.trigger;
            particlesTrigger.AddCollider(island.ForceFieldCollider);

            var emitDelay = _configMain.emitResourceTime;
            var minEmitDelay = emitDelay / 2;

            _emitMoneyCounter = 0;

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
            
            var particlesTrigger = moneyEmitter.trigger;
            particlesTrigger.RemoveCollider(island.ForceFieldCollider);
        }

        private void OnCitizenConnected(EntityCitizen citizen)
        {
            logger.LogDebug($"+citizen {citizen.gameObject.name}");
            
            // transform.LookAt(citizen.transform);
            // citizen.transform.LookAt(transform);

            transform.DOLookAt(citizen.transform.position, 0.25f);
            citizen.transform.DOLookAt(transform.position, 0.25f);
        }

        private void OnCitizenDisconnected(EntityCitizen citizen)
        {
            logger.LogDebug($"-citizen {citizen.gameObject.name}");
        }

        private void OnMoneyParticlesTrigger()
        {
            var particlesCount = moneyEmitter.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _moneyParticles);

            if (particlesCount <= 0)
            {
                return;
            }

            var emitMoney = (int) (_configMain.emitMoneyMin * Mathf.Pow(1.5f, _emitMoneyCounter));

            var island = _islandConnector.ConnectedObject;

            var maxMoney = Mathf.Min(_gameState.money, island.data.currentPrice, emitMoney, _configMain.emitMoneyMax);
            emitMoney = maxMoney;
            
            _gameState.AddMoney(-emitMoney);
            island.AddMoney(emitMoney);

            // Debug.Log($"add island money {emitMoney}");

            if (island.data.state == IslandState.OPENED)
            {
                moneyEmitter.Clear(true);
            }

            _emitMoneyCounter++;
        }

        
    }
}