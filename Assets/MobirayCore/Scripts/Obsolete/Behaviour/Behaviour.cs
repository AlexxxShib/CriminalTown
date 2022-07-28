using UnityEngine;

namespace Mobiray.Behaviour
{
    public class Behaviour : MonoBehaviour
    {
        private void Awake() { Setup(); }

        private void OnEnable() { GetComponent<Entity>().Signals.Add(this); }

        private void OnDisable() { GetComponent<Entity>().Signals.Remove(this); }

        protected virtual void Setup() { }
    }
}