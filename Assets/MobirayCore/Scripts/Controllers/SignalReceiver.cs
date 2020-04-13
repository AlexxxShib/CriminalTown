using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Controllers
{
    public class SignalReceiver : MonoBehaviour
    {
        public bool IsReceiver = true;

        protected virtual void OnEnable()
        {
            if (IsReceiver)
            {
                ToolBox.Signals.Add(this);
            }
        }

        private void OnDisable()
        {
            if (IsReceiver)
            {
                ToolBox.Signals.Remove(this);
            }
        }
    }
}