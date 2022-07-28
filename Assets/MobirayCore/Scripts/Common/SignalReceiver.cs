using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Common
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

        protected virtual void OnDisable()
        {
            if (IsReceiver)
            {
                ToolBox.Signals.Remove(this);
            }
        }
    }
}