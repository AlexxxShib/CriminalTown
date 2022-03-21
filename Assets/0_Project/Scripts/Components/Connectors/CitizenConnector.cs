using CriminalTown.Entities;

namespace CriminalTown.Components.Connectors
{
    public class CitizenConnector : BaseConnector<EntityCitizen>
    {
        public override void OnEnter(EntityCitizen citizen)
        {
            if (IsConnected)
            {
                return;
            }
            
            base.OnEnter(citizen);

            citizen.GetComponent<CompHumanControl>().InputEnabled = false;
        }

        public override void OnExit(EntityCitizen citizen)
        {
            if (citizen != ConnectedObject)
            {
                return;
            }
            
            base.OnExit(citizen);

            if (!citizen.Death)
            {
                citizen.GetComponent<CompHumanControl>().InputEnabled = true;
            }
        }
    }
}