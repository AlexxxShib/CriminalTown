using System;
using CriminalTown.Data;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.UI
{
    public class UIResourcePanel : SignalReceiver, IReceive<SignalAddMoney>
    {
        public UIResourceItem itemMoney;

        private DataGameState _gameState;

        private Tween _punchTween;

        private void Awake()
        {
            _gameState = ToolBox.Get<DataGameState>();
            
            itemMoney.value.text = $"{_gameState.money:N0}";
        }

        public void HandleSignal(SignalAddMoney signal)
        {
            itemMoney.value.text = $"{_gameState.money:N0}";
            
            _punchTween?.Kill();
            
            transform.localScale = Vector3.one;

            var punchVal = (signal.money > 0 ? 1 : -1) * 0.1f.ToVector();

            transform.DOPunchScale(punchVal, 0.4f, 2)
                .OnComplete(() => transform.localScale = Vector3.one);
        }
    }
}