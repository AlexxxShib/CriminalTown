using System;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using TMPro;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityIsland : MonoBehaviour
    {
        public DataIsland data;
        
        [Space]
        public GameObject offer;
        public GameObject body;

        private TextMeshProUGUI _textPrice;

        private ConfigMain _configMain;

        public void Initialize(DataIsland dataIsland)
        {
            _configMain = ToolBox.Get<ConfigMain>();
            
            data = dataIsland;

            _textPrice = offer.GetComponentInChildren<TextMeshProUGUI>(true);

            var triggerAgent = offer.GetComponentInChildren<CompTriggerAgent>(true);

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

            var connector = other.GetComponentInParent<IslandConnector>();
            
            if (connector != null)
            {
                connector.OnEnter(this);
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