using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine.Playables;

namespace CriminalTown.Entities
{
    public class EntityAtm : BaseConnectorTrigger<EntityAtm, AtmCrimeConnector>
    {

        public PlayableDirector cutscene;

        private CompStaticCrime _staticCrime;
        
        protected override void Awake()
        {
            base.Awake();

            _staticCrime = GetComponent<CompStaticCrime>();

            _staticCrime.toolType = ToolType.CROWBAR;
            _staticCrime.timeLock = ToolBox.Get<ConfigMain>().atmTimeLock;

            available = ToolBox.Get<DataGameState>()
                .tools.Contains(_staticCrime.toolType);
            
            _staticCrime.OnAvailabilityChanged += a => available = a;
        }
    }
}