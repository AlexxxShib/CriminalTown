using System;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.UI
{
    public class UIResourcePanel : SignalReceiver, IReceive<SignalAddMoney>
    {
        public UIResourceItem itemMoney;

        private DataGameState _gameState;

        private void Awake()
        {
            _gameState = ToolBox.Get<DataGameState>();
            
            itemMoney.value.text = $"{_gameState.money:N0}";
        }

        public void HandleSignal(SignalAddMoney signal)
        {
            itemMoney.value.text = $"{_gameState.money:N0}";
        }
    }
}