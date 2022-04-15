using System;
using System.Collections.Generic;
using CriminalTown.Configs;
using CriminalTown.Data;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Controllers
{
    public class CitizenSystem : SignalReceiver, IReceive<SignalIslandPurchased>, 
        IReceive<SignalPoliceStatus>, IReceive<SignalPlayerCaught>
    {
        public MobirayLogger logger;

        [Space]
        public Transform sheltersParent;
        public Transform entitiesParent;

        [Header("Debug")]
        public int maxPolices = 100;
        public int maxCitizens = 1000;

        public delegate void CatchingProgressDelegate(float progress, bool isHidden, bool isVisible);
        public CatchingProgressDelegate OnCatchingProgress;

        public bool PoliceActivated => _policeActivated;

        public float PursuitTime
        {
            get
            {
                var crime = _player.lastCrimeType;
                
                if (_player.lastCrimeType == CrimeType.NONE)
                {
                    logger.LogError("last crime is NONE");

                    crime = CrimeType.CITIZEN;
                }

                return _config.maxPursuitTime / 5 * _config.GetCrime(crime).pursuitLevel;
            }
        }

        private List<List<EntityIsland>> _islands;
        private List<DistrictController> _districts;

        private int _targetCitizenCount;
        private int _targetPoliceCount;

        private EntityPlayer _player;
        
        // private List<EntityCitizen> _citizens = new();
        // private List<EntityCitizen> _polices = new();

        private List<EntityShelter> _shelters = new();

        private ConfigMain _config;
        private ConfigBalance _balance;

        private float _updateTimer = 0;

        private bool _policeActivated;
        private float _policeCatchingTime;

        private void Awake()
        {
            ToolBox.Add(this);
            
            _config = ToolBox.Get<ConfigMain>();
            _balance = ToolBox.Get<ConfigBalance>();
            
            _islands = ToolBox.Get<GameController>().islands;
            
            _districts = new List<DistrictController>(GetComponentsInChildren<DistrictController>());
        }

        private void Start()
        {
            _player = ToolBox.Get<EntityPlayer>();
            
            foreach (var district in _districts)
            {
                district.entitiesParent = entitiesParent;
                district.OnPanic += OnDistrictPanic;
            }
            
            UpdatePoints();

            StartTimer();
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;
            
            if (_updateTimer >= _config.updateCitizensTime)
            {
                _updateTimer = 0;

                TryAddCitizen();
            }

            if (!_policeActivated)
            {
                return;
            }
                
            foreach (var shelter in _shelters)
            {
                shelter.SetAvailable(true);
            }
            
            var playerIsVisible = false;

            foreach (var district in _districts)
            {
                foreach (var citizen in district.polices)
                {
                    if (citizen.TryGetComponent<EntityPolice>(out var police))
                    {
                        playerIsVisible = playerIsVisible || police.SawPlayer;

                        foreach (var shelter in _shelters)
                        {
                            if (police.searchFieldOfView.IsVisible(shelter.transform))
                            {
                                shelter.SetAvailable(false);
                            }
                        }
                    }
                }
            }
                
            _player.hasPoliceVisor = playerIsVisible;

            if (playerIsVisible)
            {
                _policeCatchingTime = PursuitTime;
            }
            else
            {
                _policeCatchingTime -= Time.deltaTime * (_player.isHidden ? _config.hidingTimeBonus : 1);
            }

            var progress = Mathf.Clamp01(_policeCatchingTime / _config.maxPursuitTime);
            OnCatchingProgress?.Invoke(progress, _player.isHidden, playerIsVisible);
                
            var policeCatchingTimeout = _policeCatchingTime <= 0;

            if (policeCatchingTimeout)
            {
                DeactivatePolice(false);
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
            var citizenCount = 0;
            var policeCount = 0;
            
            foreach (var district in _districts)
            {
                district.TryDestroyCitizens();

                citizenCount += district.citizens.Count;
                policeCount += district.polices.Count;
            }

            var targetCitizenCount = Mathf.Clamp(_targetCitizenCount, 0, maxCitizens);

            if (citizenCount < targetCitizenCount)
            {
                _districts.Sort((d1, d2) => d1.citizens.Count - d2.citizens.Count);

                foreach (var district in _districts)
                {
                    // logger.LogDebug($"Try add citizen to {district.gameObject.name} with citizens {district.citizens.Count}");
                    
                    if (district.TryAddCitizen())
                    {
                        break;
                    }
                }
            }
            
            var targetPoliceCount = _targetPoliceCount;
            
            if (_policeActivated)
            {
                targetPoliceCount += _config.policeHelperCount;
            }

            if (!_balance.IsPoliceOpened(_islands))
            {
                targetPoliceCount = 0;
            }

            if (targetPoliceCount > maxPolices)
            {
                targetPoliceCount = maxPolices;
            }

            if (policeCount < targetPoliceCount)
            {
                _districts.Sort((d1, d2) => d1.polices.Count - d2.polices.Count);

                foreach (var district in _districts)
                {
                    if (district.TryAddPolice(_policeActivated))
                    {
                        break;
                    }
                }
            }
        }

        private void DeactivatePolice(bool isCaught)
        {
            var returnPolices = new List<EntityPolice>();

            foreach (var district in _districts)
            {
                foreach (var citizen in district.polices)
                {
                    if (citizen.TryGetComponent<EntityPolice>(out var police))
                    {
                        police.DisablePanic();

                        returnPolices.Add(police);
                    }
                }
            }

            returnPolices.ForEach(ReturnPolice);
            
            foreach (var shelter in _shelters)
            {
                shelter.SetAvailable(false);
            }

            _player.hasPoliceVisor = false;
            _policeActivated = false;

            ToolBox.Signals.Send(isCaught ? 
                SignalPoliceStatus.CaughtState() : 
                SignalPoliceStatus.InactiveState());
        }

        public void ReturnPolice(EntityPolice police)
        {
            police.TryGetComponent<EntityCitizen>(out var citizen);
            
            foreach (var district in _districts)
            {
                if (!district.polices.Contains(citizen))
                {
                    continue;
                }
                
                district.ReturnPolice(citizen);
                return;
            }
        }

        private void UpdatePoints()
        {
            foreach (var district in _districts)
            {
                district.UpdatePoints();
            }
            
            _shelters.Clear();

            var shelters = sheltersParent.GetComponentsInChildren<EntityShelter>();
            _shelters.AddRange(shelters);
            // foreach (var shelter in shelters)
            // {
                // if (shelter.gameObject.is)
            // }

            _targetCitizenCount = 0;
            _targetPoliceCount = 0;
            
            foreach (var branch in _islands)
            {
                foreach (var island in branch)
                {
                    if (island.data.state == IslandState.OPENED)
                    {
                        _shelters.AddRange(island.GetComponentsInChildren<EntityShelter>());

                        _targetCitizenCount += island.balance.citizensCount;
                        _targetPoliceCount += island.balance.policeCount;
                    }
                }
            }
        }

        private void OnDistrictPanic(DistrictController district)
        {
            if (_targetPoliceCount > 0)
            {
                ToolBox.Signals.Send(SignalPoliceStatus.ActiveState());
            }
        }

        public void HandleSignal(SignalIslandPurchased signal)
        {
            UpdatePoints();
        }

        public void HandleSignal(SignalPoliceStatus signal)
        {
            if (!signal.activated || _policeActivated)
            {
                return;
            }

            _policeActivated = true;
            _policeCatchingTime = PursuitTime;

            foreach (var district in _districts)
            {
                foreach (var police in district.polices)
                {
                    if (!police.Panic)
                    {
                        police.SetPanic();
                    }
                }
            }
        }

        public void HandleSignal(SignalPlayerCaught signal)
        {
            DeactivatePolice(true);
        }
    }
}