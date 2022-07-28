using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Behaviour
{
    public class Entity : MonoBehaviour
    {
        public int PoolId;

        public bool ReceiveSignals;

        private ToolSignals signals = new ToolSignals();
        public ToolSignals Signals => signals;

        private void OnEnable()
        {
            if (ReceiveSignals)
            {
                ToolBox.Signals.Add(this);
            }
        }

        private void OnDisable()
        {
            if (ReceiveSignals)
            {
                ToolBox.Signals.Remove(this);
            }
        }

      /*  public void ReturnToPool()
        {
            var pool = ToolBox.Get<EntitiesPool>();
            if (pool == null)
            {
                Debug.LogError("EntitiesPool not found");
                return;
            }

            pool.Return(this);
        }*/
    }
}