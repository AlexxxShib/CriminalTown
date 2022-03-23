using System;
using System.Collections.Generic;
using System.Linq;
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
        public MobirayLogger logger;
        
        [Space]
        public Transform entitiesParent;
        
        private List<EntityIsland> _islands;

        private List<Transform> _leftSidePoints = new();
        private List<Transform> _rightSidePoints = new();

        private EntityPlayer _player;
        
        private List<EntityCitizen> _citizens = new();
        private List<EntityCitizen> _polices = new();

        private ConfigMain _config;

        private void Awake()
        {
            ToolBox.Add(this);
            
            _config = ToolBox.Get<ConfigMain>();
            _islands = ToolBox.Get<GameController>().islands;
            
            UpdatePoints();
        }

        private void Start()
        {
            _player = ToolBox.Get<EntityPlayer>();

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
            var deathCitizens = _citizens
                .Where(c => c.Death).ToList();
            
            foreach (var citizen in deathCitizens)
            {
                if (TryDestroyCitizen(citizen))
                {
                    _citizens.Remove(citizen);
                }
            }
            
            var targetCitizensCount = _islands
                .Where(i => i.data.state == IslandState.OPENED)
                .Sum(i => i.citizenCount);
            
            // if (targetCitizensCount >= _citizens.Count(c => !c.Death))
            if (_citizens.Count < targetCitizensCount)
            {
                if (TryAddCitizen(_config.citizenPrefabs.RandomItem(), out var citizen))
                {
                    _citizens.Add(citizen);
                    return;
                }
            }
            
            var targetPoliceCount = targetCitizensCount / _config.policePerCitizen;

            if (_polices.Count < targetPoliceCount)
            {
                if (TryAddCitizen(_config.policePrefabs.RandomItem(), out var police))
                {
                    _polices.Add(police);
                }
            }
        }

        public void ReturnPolice(EntityCitizen citizen, CompHumanControl control)
        {
            var points = new List<Transform>();
            
            points.AddRange(_leftSidePoints);
            points.AddRange(_rightSidePoints);
            
            OnCitizenFinishPath(citizen, control, points);
        }

        private async void OnCitizenFinishPath(EntityCitizen citizen, CompHumanControl control, List<Transform> points)
        {
            logger.LogDebug($"citizen {citizen.gameObject.name} finished path");

            if (TryDestroyCitizen(citizen))
            {
                return;
            }
            
            var destination = FindFarPoint(citizen.transform.position, points);
            
            logger.LogDebug($"citizen {citizen.gameObject.name} continue {destination.position}");

            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            
            control.SetDestination(destination,
                humanControl => OnCitizenFinishPath(citizen, humanControl, points));
        }

        private bool TryAddCitizen(EntityCitizen prefab, out EntityCitizen instance)
        {
            if (!CalculatePoints(out var start, out var end, out var points))
            {
                instance = null;
                return false;
            }
            
            var citizen = instance = Instantiate(prefab, entitiesParent);
            var control = instance.GetComponent<CompHumanControl>();

            control.SetPosition(start.position);
            
            logger.LogDebug(
                $"add citizen {instance.gameObject.name} start: {start.position} finish: {end.position}");
            
            control.SetDestination(end,
                humanControl => OnCitizenFinishPath(citizen, humanControl, points));

            return true;
        }

        private bool TryDestroyCitizen(EntityCitizen citizen)
        {
            var citizenPlayerDistance = (_player.transform.position - citizen.transform.position).magnitude;
            if (citizenPlayerDistance < _config.citizenPlayerDistanceMin)
            {
                return false;
            }

            if (citizen.Panic && _polices.Contains(citizen))
            {
                return false;
            }

            _citizens.Remove(citizen);
            _polices.Remove(citizen);

            Destroy(citizen.gameObject);

            logger.LogDebug($"citizen {citizen.gameObject.name} destroyed");
            return true;

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
            
            foreach (var island in _islands)
            {
                if (island.data.state == IslandState.OPENED)
                {
                    _leftSidePoints.AddRange(island.GetPeoplePoints(0));
                    _rightSidePoints.AddRange(island.GetPeoplePoints(1));
                }
            }
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            UpdatePoints();
        }
    }
}