using CriminalTown.Entities;
using UnityEngine;

namespace CriminalTown.Components.Connectors
{
    public class CitizenCrimeConnector : BaseCrimeConnector<EntityCitizen>
    {
        public override bool OnEnter(EntityCitizen citizen)
        {
            if (base.OnEnter(citizen))
            {
                citizen.GetComponent<CompHumanControl>().InputEnabled = false;

                return true;
            }

            return false;
        }

        public override bool OnExit(EntityCitizen citizen)
        {
            if (!base.OnExit(citizen))
            {
                if (_victimHealth == null)
                {
                    Debug.LogError($"HEALTH NULL {citizen}");
                }
                
                if (!_victimHealth.Death)
                {
                    citizen.GetComponent<CompHumanControl>().InputEnabled = true;
                    citizen.SetPanic();
                }
            }

            return IsConnected;
        }
    }
}