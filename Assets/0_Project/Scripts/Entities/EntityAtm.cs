using System;
using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityAtm : BaseConnectorTrigger<EntityAtm, AtmCrimeConnector>, IReceive<SignalNewTool>
    {
        public MobirayLogger logger;

        [Space]
        public GameObject notAvailableSign;
        
        private CompHealth _health;
        
        private CompMeshProgress _progressTimeLock;
        private bool _lockTime;

        private DataGameState _gameState;

        protected override void Awake()
        {
            base.Awake();

            _health = GetComponent<CompHealth>();
            _health.OnDeath += OnDeath;

            _progressTimeLock = GetComponentInChildren<CompMeshProgress>();
            _progressTimeLock.gameObject.SetActive(false);

            _gameState = ToolBox.Get<DataGameState>();
            
            UpdateAvailableState();
        }

        private void UpdateAvailableState()
        {
            available = _gameState.tools.Contains(ToolType.CROWBAR);
            
            notAvailableSign.SetActive(!available && !_lockTime);
        }

        private async void OnDeath()
        {
            logger.LogDebug($"death");

            _lockTime = true;
            _progressTimeLock.gameObject.SetActive(_lockTime);

            var progress = 1f;
            var timeLock = ToolBox.Get<ConfigMain>().atmTimeLock;

            await DOTween
                .To(() => progress, value =>
                {
                    progress = value;
                    _progressTimeLock.SetValue(progress);
                }, 0, timeLock)
                .SetEase(Ease.Linear)
                .IsComplete();

            _lockTime = false;
            _progressTimeLock.gameObject.SetActive(_lockTime);
            
            _health.Repair();
        }
        
        public void HandleSignal(SignalNewTool signal)
        {
            UpdateAvailableState();
        }
    }
}