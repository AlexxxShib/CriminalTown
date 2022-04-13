using System;
using System.Collections.Generic;
using CriminalTown.Data;
using CriminalTown.Entities;
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

            var destroyTools = new List<Transform>();

            var gameState = ToolBox.Get<DataGameState>();

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

                if (child.TryGetComponent<EntityTool>(out var tool))
                {
                    if (gameState.tools.Contains(tool.type))
                    {
                        destroyTools.Add(child);
                    }
                }
            }
            
            foreach (var tool in destroyTools)
            {
                Destroy(tool.gameObject);
            }
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            UpdateChildren();
        }
    }
}