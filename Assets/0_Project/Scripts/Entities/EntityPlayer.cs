using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class EntityPlayer : MonoBehaviour
    {
        private CompHumanControl _humanControl;
        
        private IslandConnector _islandConnector;

        private bool _isConnectedIsland;
        private EntityIsland _connectedIsland;

        private ConfigMain _configMain;
        private DataGameState _gameState;
        
        private void Awake()
        {
            _humanControl = GetComponent<CompHumanControl>();
            
            _islandConnector = GetComponent<IslandConnector>();
            
            _islandConnector.OnConnected += OnIslandConnected;
            _islandConnector.OnDisconnected += OnIslandDisconnected;

            _configMain = ToolBox.Get<ConfigMain>();
            _gameState = ToolBox.Get<DataGameState>();
        }

        private void OnIslandConnected(EntityIsland island)
        {
            Debug.Log($"connected island {island.gameObject.name}");
        }
        
        private void OnIslandDisconnected(EntityIsland island)
        {
            Debug.Log($"disconnected island {island.gameObject.name}");
        }
    }
}