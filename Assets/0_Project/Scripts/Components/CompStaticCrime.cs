using System;
using CriminalTown.Configs;
using CriminalTown.Data;
using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Components
{
    
    [RequireComponent(typeof(CompHealth))]
    public class CompStaticCrime : SignalReceiver, IReceive<SignalNewTool>
    {
        public GameObject notAvailableSign;

        [Header("Realtime")]
        public ToolType toolType;
        public float timeLock;
        
        private bool _available;
        public bool Available
        {
            get => _available;
            set
            {
                if (value != _available)
                {
                    _available = value;
                    
                    OnAvailabilityChanged?.Invoke(_available);
                }
            }
        }

        public Action<bool> OnAvailabilityChanged;


        private CompHealth _health;
        
        private CompMeshProgress _progressTimeLock;
        private bool _lockTime;

        private DataGameState _gameState;

        protected void Awake()
        {
            _health = GetComponent<CompHealth>();
            _health.OnDeath += OnDeath;

            _progressTimeLock = GetComponentInChildren<CompMeshProgress>();
            _progressTimeLock.gameObject.SetActive(false);

            _gameState = ToolBox.Get<DataGameState>();
            
            UpdateAvailableState();
        }

        private void UpdateAvailableState()
        {
            Available = _gameState.tools.Contains(toolType);
            
            notAvailableSign.SetActive(!Available && !_lockTime);
        }

        private async void OnDeath()
        {
            _lockTime = true;
            _progressTimeLock.gameObject.SetActive(_lockTime);

            var progress = 1f;
            // var timeLock = ToolBox.Get<ConfigMain>().atmTimeLock;

            await DOTween
                .To(() => progress, value =>
                {
                    progress = value;
                    _progressTimeLock.SetValue(progress);
                }, 0, timeLock)
                .SetEase(Ease.Linear)
                .AwaitFor();

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