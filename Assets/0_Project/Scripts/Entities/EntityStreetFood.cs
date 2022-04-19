using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace CriminalTown.Entities
{
    public class EntityStreetFood : BaseConnectorTrigger<EntityStreetFood, StreetFoodCrimeConnector>
    {
        public GameObject salesman;
        public PlayableDirector cutscene;
        
        private CompStaticCrime _staticCrime;
        
        protected override void Awake()
        {
            base.Awake();

            _staticCrime = GetComponent<CompStaticCrime>();

            _staticCrime.toolType = ToolType.BASEBALL_BAT;
            _staticCrime.timeLock = ToolBox.Get<ConfigMain>().GetCrime(CrimeType.STREET_FOOD).timeLock;
            
            available = ToolBox.Get<DataGameState>()
                .tools.Contains(_staticCrime.toolType);
            
            _staticCrime.OnAvailabilityChanged += a => available = a;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            available = ToolBox.Get<DataGameState>()
                .tools.Contains(_staticCrime.toolType);
        }
    }
}