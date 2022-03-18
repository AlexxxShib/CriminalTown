using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CriminalTown.Configs;
using CriminalTown.Data;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Controllers
{
    public class CitizenSystem : SignalReceiver, IReceive<SignalIslandPurchased>
    {
        public bool enabledLog = true;
        
        [Space]
        public Transform entitiesParent;
        
        private List<EntityIsland> _islands;
        private int _openedIslandsCount;

        private List<Transform> _leftSidePoints = new();
        private List<Transform> _rightSidePoints = new();

        private List<EntityCitizen> _citizens = new();

        private ConfigMain _config;

        private MobirayLogger _logger = new MobirayLogger("CitizenSystem");

        private void Awake()
        {
            _config = ToolBox.Get<ConfigMain>();
            _islands = ToolBox.Get<GameController>().islands;

            _logger.Enabled = enabledLog;
            
            UpdatePoints();
        }

        private void Start()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            try
            {
                TryAddCitizen();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            this.StartTimer(_config.updateCitizensTime, StartTimer);
        }

        private void TryAddCitizen()
        {
            var targetCitizensCount = _config.GetCitizensCount(_openedIslandsCount);
            if (targetCitizensCount <= _citizens.Count)
            {
                return;
            }

            if (CalculatePoints(out var start, out var end, out var points))
            {
                var citizen = Instantiate(_config.citizenPrefabs.RandomItem(), entitiesParent);
                var control = citizen.GetComponent<CompHumanControl>();
                
                _logger.LogDebug($"add citizen {citizen.gameObject.name} start: {start.position} finish: {end.position}");

                control.SetPosition(start.position);
                control.SetDestination(end, 
                    humanControl => OnCitizenFinishPath(citizen, humanControl, points));
                
                _citizens.Add(citizen);
            }
        }

        private async void OnCitizenFinishPath(EntityCitizen citizen, CompHumanControl control, List<Transform> points)
        {
            var player = ToolBox.Get<EntityPlayer>();

            var citizenPlayerDistance = (player.transform.position - citizen.transform.position).magnitude;
            
            _logger.LogDebug($"citizen {citizen.gameObject.name} finished path");

            if (citizenPlayerDistance > _config.citizenPlayerDistanceMin)
            {
                _citizens.Remove(citizen);
                Destroy(citizen.gameObject);

                _logger.LogDebug($"citizen {citizen.gameObject.name} destroyed");
                return;
            }
            
            var destination = FindFarPoint(citizen.transform.position, points);
            
            _logger.LogDebug($"citizen {citizen.gameObject.name} continue {destination.position}");

            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            
            control.SetDestination(destination,
                humanControl => OnCitizenFinishPath(citizen, humanControl, points));
        }

        private bool CalculatePoints(out Transform start, out Transform end, out List<Transform> points)
        {
            start = end = null;
            
            var side = 0.5f.Chance() ? 0 : 1;

            var player = ToolBox.Get<EntityPlayer>();
            points = side == 0 ? _leftSidePoints : _rightSidePoints;

            var availablePoints = new List<Transform>();
            
            foreach (var point in points)
            {
                var distance = (point.position - player.transform.position).magnitude;

                if (distance > _config.citizenPlayerDistanceMin)
                {
                    availablePoints.Add(point);
                }
            }

            if (availablePoints.Count == 0)
            {
                return false;
            }

            start = availablePoints.RandomItem();

            end = FindFarPoint(start.position, points);

            return true;
        }

        private Transform FindFarPoint(Vector3 start, List<Transform> points)
        {
            Transform farPoint = null;
            var maxDistance = 0f;
            
            foreach (var point in points)
            {
                var distance = (point.position - start).magnitude;

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farPoint = point;
                }
            }

            return farPoint;
        }

        private void UpdatePoints()
        {
            _leftSidePoints.Clear();
            _rightSidePoints.Clear();

            _openedIslandsCount = 0;
            
            foreach (var island in _islands)
            {
                if (island.data.state == IslandState.OPENED)
                {
                    _leftSidePoints.AddRange(island.GetPeoplePoints(0));
                    _rightSidePoints.AddRange(island.GetPeoplePoints(1));

                    _openedIslandsCount++;
                }
            }
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            UpdatePoints();
        }
    }
}