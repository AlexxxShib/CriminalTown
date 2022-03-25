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
    public class CitizenSystem : SignalReceiver, IReceive<SignalIslandPurchased>, IReceive<SignalPoliceActivated>, IReceive<SignalPlayerCaught>
    {
        public MobirayLogger logger;
        
        [Space]
        public Transform entitiesParent;

        public delegate void OnCatchingProgressDelegat(float progress, bool isHidden, bool isVisible);
        public OnCatchingProgressDelegat OnCatchingProgress;

        private List<EntityIsland> _islands;

        private List<Transform> _leftSidePoints = new();
        private List<Transform> _rightSidePoints = new();

        private EntityPlayer _player;
        
        private List<EntityCitizen> _citizens = new();
        private List<EntityCitizen> _polices = new();

        private ConfigMain _config;

        private float _updateTimer = 0;

        private bool _policeActivated;
        private float _policeCatchingTime;

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

        private void Update()
        {
            if (_policeActivated)
            {
                var playerIsVisible = false;
                
                foreach (var citizen in _polices)
                {
                    if (citizen.TryGetComponent<EntityPolice>(out var police))
                    {
                        playerIsVisible = playerIsVisible || police.SawPlayer;
                    }
                }
                
                _player.hasPoliceVisor = playerIsVisible;

                if (playerIsVisible)
                {
                    _policeCatchingTime = _config.policePassiveTime;
                }
                else
                {
                    _policeCatchingTime -= Time.deltaTime * (_player.isHidden ? _config.hidingTimeBonus : 1);
                }

                var progress = Mathf.Clamp01(_policeCatchingTime / _config.policePassiveTime);
                OnCatchingProgress?.Invoke(progress, _player.isHidden, playerIsVisible);
                
                var policeCatchingTimeout = _policeCatchingTime <= 0;

                if (policeCatchingTimeout)
                {
                    DeactivatePolice();
                }
            }
            
            _updateTimer += Time.deltaTime;
            
            if (_updateTimer >= _config.updateCitizensTime)
            {
                _updateTimer = 0;

                TryAddCitizen();
            }
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

            var targetCitizensCount = 0;
            var targetPoliceCount = 0;
            
            foreach (var island in _islands)
            {
                if (island.data.state == IslandState.OPENED)
                {
                    targetCitizensCount += island.balance.citizensCount;
                    targetPoliceCount += island.balance.policeCount;
                }
            }
            
            // if (targetCitizensCount >= _citizens.Count(c => !c.Death))
            if (_citizens.Count < targetCitizensCount)
            {
                if (TryAddCitizen(_config.citizenPrefabs.RandomItem(), out var citizen))
                {
                    _citizens.Add(citizen);
                    return;
                }
            }
            
            if (_policeActivated)
            {
                targetPoliceCount += _config.policeHelperCount;
            }

            if (_polices.Count < targetPoliceCount)
            {
                if (TryAddCitizen(_config.policePrefabs.RandomItem(), out var police))
                {
                    _polices.Add(police);
                    
                    this.StartTimer(0.5f, () =>
                    {
                        if (_policeActivated)
                        {
                            police.SetPanic();
                        }
                    });
                }
            }
        }

        private void DeactivatePolice()
        {
            var returnPolices = new List<EntityPolice>();
            
            foreach (var citizen in _polices)
            {
                if (citizen.TryGetComponent<EntityPolice>(out var police))
                {
                    police.DisablePanic();
                    
                    returnPolices.Add(police);
                }
            }
            
            returnPolices.ForEach(ReturnPolice);

            _player.hasPoliceVisor = false;

            _policeActivated = false;
            ToolBox.Signals.Send<SignalPoliceDeactivated>();
        }

        public void ReturnPolice(EntityPolice police)
        {
            var points = new List<Transform>();
            
            points.AddRange(_leftSidePoints);
            points.AddRange(_rightSidePoints);

            police.TryGetComponent<EntityCitizen>(out var citizen);
            police.TryGetComponent<CompHumanControl>(out var control);
            
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

            if (citizen.Panic)
            {
                if (_polices.Contains(citizen))
                {
                    return false;
                }

                ToolBox.Signals.Send<SignalPoliceActivated>();
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

        public void HandleSignal(SignalPoliceActivated signal)
        {
            if (_policeActivated)
            {
                return;
            }

            _policeActivated = true;
            _policeCatchingTime = _config.policePassiveTime;
            
            foreach (var police in _polices)
            {
                if (!police.Panic)
                {
                    police.SetPanic();
                }
            }
        }

        public void HandleSignal(SignalPlayerCaught signal)
        {
            DeactivatePolice();
        }
    }
}