using System;
using CriminalTown.Components.Connectors;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    
    public class EntityShelter : MonoBehaviour
    {
        public GameObject viewEmpty;
        public GameObject viewFill;

        [Space]
        public GameObject availableMark;

        public bool Hiding { get; private set; }

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

        public void EnterShelter()
        {
            Hiding = true;
            
            UpdateState();

            viewFill.transform.DOPunchScale(0.2f.ToVector(), 0.5f, 2);
        }

        public void LeaveShelter()
        {
            Hiding = false;
            
            UpdateState();
            
            viewEmpty.transform.DOPunchScale(-0.2f.ToVector(), 0.5f, 2);
        }

        public void OnTriggerEnter(Collider other)
        {
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
    }
}