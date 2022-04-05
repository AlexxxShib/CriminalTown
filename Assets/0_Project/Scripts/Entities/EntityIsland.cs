using System;
using System.Collections.Generic;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using DG.Tweening;
using Mobiray.Common;
using TMPro;
using UnityEngine;

namespace CriminalTown.Entities
{
    public struct SignalIslandPurchased { }
    
    public class EntityIsland : MonoBehaviour
    {
        public DataIsland data;
        public IslandConfig balance;
        
        [Space]
        public GameObject offer;
        public GameObject body;

        [Space]
        public List<EntityIsland> dependIslands;

        // [Space]
        // public Transform peoplePoints;

        public ParticleSystemForceField ForceField { get; private set; }
        public Collider ForceFieldCollider { get; private set; }

        private TextMeshProUGUI _textPrice;
        
        private ConfigMain _configMain;

        private IslandConnector _curConnector;

        public void Initialize(DataIsland dataIsland, IslandConfig balanceIsland)
        {
            _configMain = ToolBox.Get<ConfigMain>();
            
            data = dataIsland;
            balance = balanceIsland;

            offer = transform.GetChild(0).gameObject;
            body = transform.GetChild(1).gameObject;

            ForceField = offer.GetComponentInChildren<ParticleSystemForceField>(true);
            ForceFieldCollider = ForceField.GetComponent<Collider>();
            
            _textPrice = offer.GetComponentInChildren<TextMeshProUGUI>(true);

            var triggerAgent = offer.GetComponent<CompTriggerAgent>();

            triggerAgent.onCallTriggerEnter += OnEnterIsland;
            triggerAgent.onCallTriggerExit += OnExitIsland;
            
            UpdateState();
            ClearTools();
        }

        public void SetAvailable()
        {
            foreach (var island in dependIslands)
            {
                if (island.data.state != IslandState.OPENED)
                {
                    return;
                }
            }
            
            data.state = IslandState.AVAILABLE;
            data.currentPrice = balance.price;
            
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
                ClearTools();
                
                ToolBox.Signals.Send<SignalIslandPurchased>();

                body.transform.DOPunchPosition(new Vector3(0, 0.5f, 0), 0.5f, 2);

                if (_curConnector)
                {
                    _curConnector.OnExit(this);
                }
            }
        }

        public void UpdateState()
        {
            offer.SetActive(data.state == IslandState.AVAILABLE);
            body.SetActive(data.state == IslandState.OPENED);
            
            UpdatePrice();
        }

        public void ClearTools()
        {
            var gameState = ToolBox.Get<DataGameState>();
            var tools = body.GetComponentsInChildren<EntityTool>(true);

            foreach (var tool in tools)
            {
                if (gameState.tools.Contains(tool.type))
                {
                    Destroy(tool.gameObject);
                }
            }
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