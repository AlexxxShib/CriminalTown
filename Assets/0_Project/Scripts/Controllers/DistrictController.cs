using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CriminalTown.Configs;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Controllers
{
    public class DistrictController : SignalReceiver, IReceive<SignalTryDestroyCitizen>
    {
        public MobirayLogger logger;

        [Space]
        public int layerWater = 4;
        public Transform entitiesParent;
        public List<EntityCitizen> citizenPrefabs;

        [Space]
        public List<Transform> points;
        public List<EntityCitizen> citizens;
        public List<EntityCitizen> polices;

        public Action<DistrictController> OnPanic;

        private ConfigMain _config;

        private EntityPlayer _player;
        private Camera _mainCamera;

        private void Start()
        {
            _config = ToolBox.Get<ConfigMain>();
            _player = ToolBox.Get<EntityPlayer>();
            
            _mainCamera = Camera.main;
        }

        public void UpdatePoints()
        {
            points.Clear();
            
            var allPoints = transform.GetChildren();

            foreach (var point in allPoints)
            {
                var ray = new Ray(point.position + Vector3.up * 2, Vector3.down);
                
                if (Physics.Raycast(ray, out var hit, 10,-5, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.gameObject.layer != layerWater)
                    {
                        points.Add(point);
                    }
                }
            }
        }

        public void ReturnPolice(EntityCitizen police)
        {
            police.TryGetComponent<CompHumanControl>(out var control);
            
            OnCitizenFinishPath(police, control);
        }

        public bool TryAddCitizen()
        {
            if (!TryAddCitizen(citizenPrefabs.RandomItem(), out var citizen))
            {
                return false;
            }
            
            citizens.Add(citizen);

            return true;
        }
        
        public bool TryAddPolice(bool activated)
        {
            if (!TryAddCitizen(_config.policePrefabs.RandomItem(), out var citizen))
            {
                return false;
            }
            
            polices.Add(citizen);
            
            this.StartTimer(0.5f, () =>
            {
                if (activated)
                {
                    citizen.SetPanic();
                } 
            });

            return true;
        }
        
        private bool TryAddCitizen(EntityCitizen prefab, out EntityCitizen instance)
        {
            if (!TryGetPoints(out var start, out var finish))
            {
                instance = null;
                return false;
            }
            
            var citizen = instance = Instantiate(prefab, entitiesParent);
            var control = instance.GetComponent<CompHumanControl>();

            control.SetPosition(start.position);
            
            logger.LogDebug(
                $"add citizen {instance.gameObject.name} start: {start.position} finish: {finish.position}");
            
            control.SetDestination(finish,
                humanControl => OnCitizenFinishPath(citizen, humanControl));

            return true;
        }

        private async void OnCitizenFinishPath(EntityCitizen citizen, CompHumanControl control)
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
                humanControl => OnCitizenFinishPath(citizen, humanControl));
        }

        private bool TryGetPoints(out Transform start, out Transform finish)
        {
            start = finish = null;
            
            if (points.Count < 2)
            {
                return false;
            }

            var availablePoints = new List<Transform>();
            
            foreach (var point in points)
            {
                // var distance = (point.position - _player.transform.position).magnitude;

                if (!IsVisible(point.position))
                {
                    availablePoints.Add(point);
                }
            }

            if (availablePoints.Count == 0)
            {
                return false;
            }
            
            start = availablePoints.RandomItem();
            finish = FindFarPoint(start.position, points);

            return true;
        }

        private bool IsVisible(Vector3 point)
        {
            return _mainCamera.IsVisible(new Bounds(point, 0.5f.ToVector()));
        }
        
        private Transform FindFarPoint(Vector3 start, List<Transform> availablePoints)
        {
            Transform farPoint = null;
            var maxDistance = 0f;
            
            foreach (var point in availablePoints)
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

        public void TryDestroyCitizens()
        {
            var deathCitizens = citizens.Where(c => c.Death).ToList();

            foreach (var citizen in deathCitizens)
            {
                TryDestroyCitizen(citizen);
            }
        }
        
        private bool TryDestroyCitizen(EntityCitizen citizen)
        {
            var citizenPos = citizen.transform.position;
            
            // var citizenPlayerDistance = (_player.transform.position - citizenPos).magnitude;
            
            if (IsVisible(citizenPos))
            {
                return false;
            }

            if (citizen.Panic)
            {
                if (polices.Contains(citizen))
                {
                    return false;
                }

                if (_config.activateCitizenPanic && citizen.Snitch && !citizen.Death)
                {
                    OnPanic?.Invoke(this);
                }
            }

            DestroyCitizen(citizen);
            return true;
        }
        
        public void DestroyCitizen(EntityCitizen citizen)
        {
            if (citizens.Remove(citizen))
            {
                logger.LogDebug($"citizen {citizen.gameObject.name} destroyed");
            }

            if (polices.Remove(citizen))
            {
                logger.LogDebug($"police {citizen.gameObject.name} destroyed");
            }
            
            Destroy(citizen.gameObject);
        }

        public void HandleSignal(SignalTryDestroyCitizen signal)
        {
            DestroyCitizen(signal.citizen);
        }
    }
}