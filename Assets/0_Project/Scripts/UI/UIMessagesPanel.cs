using System;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.UI
{
    public class UIMessagesPanel : SignalReceiver, IReceive<SignalPoliceStatus>
    {
        public GameObject messageWanted;
        public GameObject messageEscaped;

        private Animator _animator;
        private static readonly int AnimIdMessage = Animator.StringToHash("message");

        private bool _lastPoliceState;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void HandleSignal(SignalPoliceStatus signal)
        {
            if (_lastPoliceState == signal.activated || signal.caught)
            {
                return;
            }

            _lastPoliceState = signal.activated;
            
            messageWanted.SetActive(signal.activated);
            messageEscaped.SetActive(!signal.activated);
            
            _animator.SetTrigger(AnimIdMessage);
        }
    }
}