using System;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityTool : MonoBehaviour
    {
        public ToolType type;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<EntityPlayer>();

            if (player)
            {
                var gameState = ToolBox.Get<DataGameState>();

                if (!gameState.tools.Contains(type))
                {
                    gameState.tools.Add(type);
                    
                    ToolBox.Signals.Send<SignalNewTool>();
                }
                
                Destroy(gameObject);
            }
        }
    }
}