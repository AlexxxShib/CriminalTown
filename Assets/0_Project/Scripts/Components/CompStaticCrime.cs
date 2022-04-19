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
        public CompMeshProgress progressTimeLock;
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
                    
                    // Debug.Log($"[{gameObject.name}] available: {_available}");
                }
            }
        }

        public Action<bool> OnAvailabilityChanged;


        private CompHealth _health;
        
        private bool _lockTime;

        private DataGameState _gameState;

        protected void Awake()
        {
            _health = GetComponent<CompHealth>();
            _health.OnDeath += OnDeath;

            progressTimeLock.gameObject.SetActive(false);

            _gameState = ToolBox.Get<DataGameState>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
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
            
            progressTimeLock.gameObject.SetActive(_lockTime);

            var progress = 1f;
            // var timeLock = ToolBox.Get<ConfigMain>().atmTimeLock;

            await DOTween
                .To(() => progress, value =>
                {
                    progress = value;
                    progressTimeLock.SetValue(progress);
                }, 0, timeLock)
                .SetEase(Ease.Linear)
                .AwaitFor();

            _lockTime = false;
            progressTimeLock.gameObject.SetActive(_lockTime);
            
            _health.Repair();
        }
        
        public void HandleSignal(SignalNewTool signal)
        {
            UpdateAvailableState();
        }
    }
}