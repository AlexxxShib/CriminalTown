using System;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Components.Connectors
{
    public class BaseConnectorTrigger<T, TM> : SignalReceiver where TM : BaseConnector<T> where T : MonoBehaviour
    {
        public MobirayLogger logger;
        
        public bool available = true;
        public bool stayTrigger = true;
        public CompTriggerAgent triggerAgent;
        
        private bool _hasTriggerAgent;
        
        protected T _owner;
        protected TM _connector;

        protected virtual void Awake()
        {
            _owner = GetComponent<T>();

            _hasTriggerAgent = triggerAgent != null;

            if (_hasTriggerAgent)
            {
                triggerAgent.onCallTriggerEnter += OnEnter;
                triggerAgent.onCallTriggerStay += OnStay;
                triggerAgent.onCallTriggerExit += c => OnExit(c);
            }
        }

        protected virtual void OnEnter(Collider other)
        {
            if (!available)
            {
                return;
            }
            
            Connect(other);
        }

        protected virtual void OnStay(Collider other)
        {
            if (!available || !stayTrigger)
            {
                return;
            }
            
            if (_connector == null)
            {
                Connect(other);
                return;
            }

            if (!_connector.IsConnected)
            {
                _connector.OnEnter(_owner);
            }
        }

        protected virtual bool OnExit(Collider other)
        {
            if (!available || _connector == null)
            {
                return false;
            }
            
            var connector = other.GetComponentInParent<TM>();

            if (connector == _connector && _connector.IsConnected)
            {
                if (!_connector.OnExit(_owner))
                {
                    _connector = null;

                    return true;
                }
            }

            return false;
        }
        
        private void Connect(Collider other)
        {
            _connector = other.GetComponentInParent<TM>();

            if (_connector != null)
            {
                _connector.OnEnter(_owner);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger && !_hasTriggerAgent)
            {
                OnEnter(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.isTrigger && !_hasTriggerAgent)
            {
                OnStay(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger && !_hasTriggerAgent)
            {
                OnExit(other);
            }
        }
    }
}