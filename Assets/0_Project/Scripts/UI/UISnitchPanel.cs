using System;
using System.Collections.Generic;
using CriminalTown.Entities;
using Mobiray.Common;
using MobirayCore.Scripts.Components;

namespace CriminalTown.UI
{
    public class UISnitchPanel : SignalReceiver, IReceive<SignalCitizenPanic>, IReceive<SignalPoliceStatus>
    {
        public int poolSize = 10;
        
        private List<NavigationMark> _marks = new();

        private Dictionary<EntityCitizen, NavigationMark> _citizenMarks = new();

        private bool _policeActivated;

        private void Awake()
        {
            var markPrefab = GetComponentInChildren<NavigationMark>();
            
            markPrefab.gameObject.SetActive(false);
            
            _marks.Add(markPrefab);
            
            while (transform.childCount < poolSize)
            {
                var instance = Instantiate(markPrefab, transform);
                
                instance.gameObject.SetActive(false);
                
                _marks.Add(instance);
            }
        }

        private void AddMark(EntityCitizen citizen)
        {
            if (_marks.Count == 0)
            {
                return;
            }
            
            var mark = _marks[0];
            _marks.RemoveAt(0);
            
            _citizenMarks.Add(citizen, mark);

            mark.SetTarget(citizen.transform);
            mark.gameObject.SetActive(true);
        }

        private void ClearMarks()
        {
            var citizens = new List<EntityCitizen>(_citizenMarks.Keys);
            
            citizens.ForEach(RemoveMark);
        }

        private void RemoveMark(EntityCitizen citizen)
        {
            if (!_citizenMarks.TryGetValue(citizen, out var mark))
            {
                return;
            }
            
            mark.gameObject.SetActive(false);
            _citizenMarks.Remove(citizen);
            
            _marks.Add(mark);
        }

        public void HandleSignal(SignalCitizenPanic signal)
        {
            if (signal.activated && !signal.citizen.staff && !_policeActivated)
            {
                AddMark(signal.citizen);
            }

            if (!signal.activated)
            {
                RemoveMark(signal.citizen);
            }
        }

        public void HandleSignal(SignalPoliceStatus signal)
        {
            _policeActivated = signal.activated;

            if (_policeActivated)
            {
                ClearMarks();
            }
        }
    }
}