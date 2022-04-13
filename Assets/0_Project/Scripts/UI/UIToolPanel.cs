using System;
using System.Collections.Generic;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.UI
{
    public class UIToolPanel : SignalReceiver, IReceive<SignalNewTool>
    {
        [Serializable]
        public struct Pair<T, TD>
        {
            public T key;
            public TD value;
        }
        
        public List<Pair<ToolType, GameObject>> toolIcons;

        private void Awake()
        {
            UpdateTools();
        }

        private void UpdateTools()
        {
            var gameState = ToolBox.Get<DataGameState>();
            
            foreach (var toolIcon in toolIcons)
            {
                toolIcon.value.SetActive(gameState.tools.Contains(toolIcon.key));
            }
        }

        public void HandleSignal(SignalNewTool signal)
        {
            UpdateTools();
        }
    }
}