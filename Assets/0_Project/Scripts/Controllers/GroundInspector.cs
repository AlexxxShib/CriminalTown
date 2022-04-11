using System;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Controllers
{
    public class GroundInspector : SignalReceiver, IReceive<SignalIslandPurchased>
    {
        
        public int layerWater = 4;
        
        private void Awake()
        {
            UpdateChildren(true);
        }

        private void UpdateChildren(bool init = false)
        {
            var children = transform.GetChildren();

            foreach (var child in children)
            {
                if (init)
                {
                    child.gameObject.SetActive(false);
                }
                
                var ray = new Ray(child.position + Vector3.up * 2, Vector3.down);
                
                if (Physics.Raycast(ray, out var hit, 10,-5, QueryTriggerInteraction.Ignore))
                {
                    child.gameObject.SetActive(hit.transform.gameObject.layer != layerWater);
                }
            }
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            UpdateChildren();
        }
    }
}