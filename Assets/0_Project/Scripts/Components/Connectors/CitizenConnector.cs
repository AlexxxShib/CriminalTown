using CriminalTown.Entities;

namespace CriminalTown.Components.Connectors
{
    public class CitizenConnector : BaseConnector<EntityCitizen>
    {
        public override bool OnEnter(EntityCitizen citizen)
        {
            if (IsConnected)
            {
                return false;
            }
            
            citizen.GetComponent<CompHumanControl>().InputEnabled = false;
            
            base.OnEnter(citizen);

            return true;
        }

        public override bool OnExit(EntityCitizen citizen)
        {
            if (citizen != ConnectedObject)
            {
                return true;
            }
            
            if (!citizen.Death)
            {
                citizen.GetComponent<CompHumanControl>().InputEnabled = true;
            }
            
            return base.OnExit(citizen);
        }
    }
}