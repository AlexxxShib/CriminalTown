using System;
using CriminalTown.Components.Connectors;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    
    public class EntityShelter : SignalReceiver, IReceive<SignalPoliceActivated>, IReceive<SignalPoliceDeactivated>
    {
        public GameObject viewEmpty;
        public GameObject viewFill;

        [Space]
        public GameObject availableMark;

        public bool Hiding { get; private set; }
        public bool IsAvailable { get; private set; }

        private bool _showSign;

        private void Awake()
        {
            UpdateState();
            
            availableMark.SetActive(false);
        }

        private void UpdateState()
        {
            viewEmpty.SetActive(!Hiding);
            viewFill.SetActive(Hiding);
        }

        public void SetAvailable(bool available)
        {
            IsAvailable = available;
            
            if (Hiding)
            {
                IsAvailable = false;
            }
            
            availableMark.SetActive(IsAvailable && _showSign);
        }

        public void EnterShelter()
        {
            Hiding = true;
            
            UpdateState();

            viewFill.transform.DOPunchScale(0.2f.ToVector(), 0.5f, 2);
            
            SetAvailable(false);
        }

        public void LeaveShelter()
        {
            Hiding = false;
            
            UpdateState();
            
            viewEmpty.transform.DOPunchScale(-0.2f.ToVector(), 0.5f, 2);
            
            SetAvailable(true);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!IsAvailable)
            {
                return;
            }
            
            var connector = other.GetComponentInParent<ShelterConnector>();

            if (connector != null)
            {
                connector.OnEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var connector = other.GetComponentInParent<ShelterConnector>();

            if (connector != null)
            {
                connector.OnExit(this);
            }
        }

        public void HandleSignal(SignalPoliceActivated signal)
        {
            _showSign = true;
        }

        public void HandleSignal(SignalPoliceDeactivated signal)
        {
            _showSign = false;
        }
    }
}