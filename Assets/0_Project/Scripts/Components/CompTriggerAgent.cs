using System;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class CompTriggerAgent : MonoBehaviour
    {
        public Action<Collider> onCallTriggerEnter;
        public Action<Collider> onCallTriggerStay;
        public Action<Collider> onCallTriggerExit;
        public Action onCallParticleTrigger;

        private void OnTriggerEnter(Collider other)
        {
            onCallTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onCallTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onCallTriggerExit?.Invoke(other);
        }

        private void OnParticleTrigger()
        {
            onCallParticleTrigger?.Invoke();
        }
        
    }
}