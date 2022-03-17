using System;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using TMPro;
using UnityEngine;

namespace CriminalTown.Entities
{
    public struct SignalIslandPurchased { }
    
    public class EntityIsland : MonoBehaviour
    {
        public DataIsland data;
        
        [Space]
        public GameObject offer;
        public GameObject body;

        public ParticleSystemForceField ForceField { get; private set; }
        public Collider ForceFieldCollider { get; private set; }

        private TextMeshProUGUI _textPrice;
        
        private ConfigMain _configMain;

        private IslandConnector _curConnector;

        public void Initialize(DataIsland dataIsland)
        {
            _configMain = ToolBox.Get<ConfigMain>();
            
            data = dataIsland;

            ForceField = offer.GetComponentInChildren<ParticleSystemForceField>(true);
            ForceFieldCollider = ForceField.GetComponent<Collider>();
            _textPrice = offer.GetComponentInChildren<TextMeshProUGUI>(true);

            var triggerAgent = offer.GetComponent<CompTriggerAgent>();

            triggerAgent.onCallTriggerEnter += OnEnterIsland;
            triggerAgent.onCallTriggerExit += OnExitIsland;
            
            UpdateState();
        }

        public void SetAvailable()
        {
            data.state = IslandState.AVAILABLE;
            data.currentPrice = _configMain.GetIslandPrice(data.index);
            
            UpdateState();
        }

        public void AddMoney(int money)
        {
            data.currentPrice -= money;
            UpdatePrice();

            if (data.currentPrice <= 0)
            {
                data.state = IslandState.OPENED;
                UpdateState();
                
                _curConnector.OnExit(this);
                
                ToolBox.Signals.Send<SignalIslandPurchased>();
            }
        }

        public void UpdateState()
        {
            offer.SetActive(data.state == IslandState.AVAILABLE);
            body.SetActive(data.state == IslandState.OPENED);
            
            UpdatePrice();
        }

        public void UpdatePrice()
        {
            if (data.state != IslandState.AVAILABLE)
            {
                return;
            }
            
            _textPrice.text = $"{data.currentPrice:N0}$";
        }

        private void OnEnterIsland(Collider other)
        {
            Debug.Log(other.gameObject);

            _curConnector = other.GetComponentInParent<IslandConnector>();
            
            if (_curConnector != null)
            {
                _curConnector.OnEnter(this);
            }
        }

        private void OnExitIsland(Collider other)
        {
            var connector = other.GetComponentInParent<IslandConnector>();
            
            if (connector != null)
            {
                connector.OnExit(this);
            }
        }
    }
}