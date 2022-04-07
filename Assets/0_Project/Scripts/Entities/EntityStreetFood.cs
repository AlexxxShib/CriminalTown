using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityStreetFood : BaseConnectorTrigger<EntityStreetFood, StreetFoodCrimeConnector>
    {
        private CompStaticCrime _staticCrime;
        
        protected override void Awake()
        {
            base.Awake();

            _staticCrime = GetComponent<CompStaticCrime>();

            _staticCrime.toolType = ToolType.BASEBALL_BAT;
            _staticCrime.timeLock = ToolBox.Get<ConfigMain>().streetFoodTimeLock;
            
            _staticCrime.OnAvailabilityChanged += a => available = a;
        }
    }
}