using System;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Components.Connectors
{
    public class BaseConnectorTrigger<T, TM> : SignalReceiver where TM : BaseConnector<T> where T : MonoBehaviour
    {
        public bool available = true;
        public CompTriggerAgent triggerAgent;
        
        private bool _hasTriggerAgent;
        
        private T _owner;
        private TM _curConnector;

        protected virtual void Awake()
        {
            _owner = GetComponent<T>();

            _hasTriggerAgent = triggerAgent != null;

            if (_hasTriggerAgent)
            {
                triggerAgent.onCallTriggerEnter += OnEnter;
                triggerAgent.onCallTriggerStay += OnStay;
                triggerAgent.onCallTriggerExit += OnExit;
            }
        }

        private void OnEnter(Collider other)
        {
            if (!available)
            {
                return;
            }
            
            Connect(other);
        }

        private void OnStay(Collider other)
        {
            if (!available)
            {
                return;
            }
            
            if (_curConnector == null)
            {
                Connect(other);
                return;
            }

            if (!_curConnector.IsConnected)
            {
                _curConnector.OnEnter(_owner);
            }
        }

        private void OnExit(Collider other)
        {
            if (!available)
            {
                return;
            }
            
            var connector = other.GetComponentInParent<TM>();

            if (connector != _curConnector)
            {
                return;
            }
            
            if (!_curConnector || !_curConnector.IsConnected)
            {
                return;
            }

            _curConnector.OnExit(_owner);
        }
        
        private void Connect(Collider other)
        {
            _curConnector = other.GetComponentInParent<TM>();

            if (_curConnector != null)
            {
                _curConnector.OnEnter(_owner);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_hasTriggerAgent)
            {
                OnEnter(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_hasTriggerAgent)
            {
                OnStay(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_hasTriggerAgent)
            {
                OnExit(other);
            }
        }
    }
}