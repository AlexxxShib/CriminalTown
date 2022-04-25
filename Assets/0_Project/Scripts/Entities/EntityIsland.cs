using System;
using System.Collections.Generic;
using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using CriminalTown.Data;
using DG.Tweening;
using GameAnalyticsSDK;
using Mobiray.Common;
using TMPro;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityIsland : MonoBehaviour
    {
        public MobirayLogger logger;
        
        [Space]
        public DataIsland data;
        public IslandConfig balance;
        
        [Space]
        public CompIslandOffer offer;
        public GameObject body;

        [Space]
        public List<EntityIsland> dependIslands;

        public ParticleSystemForceField ForceField { get; private set; }
        public Collider ForceFieldCollider { get; private set; }

        private ConfigMain _configMain;

        private IslandConnector _curConnector;
        private Collider _triggerCollider;

        public void Initialize(DataIsland dataIsland, IslandConfig balanceIsland)
        {
            logger.mainTag = transform.parent.gameObject.name + " " + gameObject.name;
            
            _configMain = ToolBox.Get<ConfigMain>();
            
            data = dataIsland;
            balance = balanceIsland;

            offer = GetComponentInChildren<CompIslandOffer>();
            body = transform.GetChild(1).gameObject;

            ForceField = offer.GetComponentInChildren<ParticleSystemForceField>(true);
            ForceFieldCollider = ForceField.GetComponent<Collider>();
            
            var triggerAgent = offer.GetComponent<CompTriggerAgent>();

            _triggerCollider = triggerAgent.GetComponent<Collider>();

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
                    data.state = IslandState.LOCK;
                    
                    UpdateState();
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
                
                Analytics.SendIslandBought(data);
            }
        }

        public void UpdateState()
        {
            offer.gameObject.SetActive(data.state is IslandState.AVAILABLE or IslandState.LOCK);
            // offer.gameObject.SetActive(data.state == IslandState.AVAILABLE || data.state == IslandState.LOCK);
            
            offer.openLock.SetActive(data.state == IslandState.LOCK);
            
            offer.textPrice.gameObject.SetActive(data.state == IslandState.AVAILABLE);
            _triggerCollider.enabled = data.state == IslandState.AVAILABLE;
            
            body.SetActive(data.state == IslandState.OPENED);

            UpdatePrice();
        }

        public void ClearTools()
        {
            //NOT ACTIAL
            /*var gameState = ToolBox.Get<DataGameState>();
            var tools = body.GetComponentsInChildren<EntityTool>(true);

            foreach (var tool in tools)
            {
                if (gameState.tools.Contains(tool.type))
                {
                    Destroy(tool.gameObject);
                }
            }*/
        }

        public void UpdatePrice()
        {
            if (data.state != IslandState.AVAILABLE)
            {
                return;
            }
            
            offer.textPrice.text = $"{data.currentPrice:N0}$";
        }

        private void OnEnterIsland(Collider other)
        {
            var connector = other.GetComponentInParent<IslandConnector>();
            
            if (connector != null)
            {
                if (connector == _curConnector && connector.IsConnected)
                {
                    return;
                }

                _curConnector = connector;
                
                logger.LogDebug($"trigger enter {_curConnector.gameObject.name}", other.gameObject);

                _curConnector.OnEnter(this);
            }
        }

        private void OnExitIsland(Collider other)
        {
            var connector = other.GetComponentInParent<IslandConnector>();
            
            if (connector != null)
            {
                if (!connector.IsConnected)
                {
                    return;
                }
                
                logger.LogDebug($"trigger exit {connector.gameObject.name}", other.gameObject);
                
                connector.OnExit(this);
            }
        }
    }
}