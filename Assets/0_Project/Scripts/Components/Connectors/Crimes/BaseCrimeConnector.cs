using System;
using System.Collections.Generic;
using CriminalTown.Configs;
using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using MobirayCore.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace CriminalTown.Components.Connectors
{

    [Serializable]
    public class CutsceneTrackValue
    {
        public string key;
        public GameObject value;
    }
    
    public abstract class BaseCrimeConnector<T> : BaseConnector<T> where T : MonoBehaviour
    {
        public string crimeTag = "crime";

        [Space]
        // public HashMap<string, GameObject> cutsceneTrackMap;
        public List<CutsceneTrackValue> trackValues;
        protected Dictionary<string, GameObject> cutsceneTrackMap;

        [Space]
        public PlayableDirector cutscene;
        
        protected ConfigMain _config;

        protected EntityPlayer _player;
        protected CompHealth _victimHealth;
        
        protected override void Awake()
        {
            base.Awake();

            _config = ToolBox.Get<ConfigMain>();

            _player = GetComponent<EntityPlayer>();

            OnConnected += OnConnectionReady;
            OnDisconnected += OnConnectionIsDown;

            cutsceneTrackMap = new Dictionary<string, GameObject>();
            foreach (var trackValue in trackValues)
            {
                cutsceneTrackMap.Add(trackValue.key, trackValue.value);
            }

            /*cutscene.played += _ => _player.SetCrime(true);
            
            cutscene.stopped += _ =>
            {
                _player.SetCrime(false);

                if (IsConnected)
                {
                    OnExit(ConnectedObject);
                }
            };*/
        }
        
        public override bool OnEnter(T connectedObject)
        {
            if (_player.isCaught || _player.isHidden)
            {
                return false;
            }

            if (!connectedObject.TryGetComponent(out _victimHealth))
            {
                return false;
            }

            if (_victimHealth.Death)
            {
                return false;
            }
            
            return base.OnEnter(connectedObject);
        }

        private void OnConnectionReady(T connectedObject)
        {
            if (_player.isCaught || _player.isHidden)
            {
                return;
            }
            
            logger.LogDebug($"+{crimeTag} {connectedObject.gameObject.name}");
            
            SetupCutscene(connectedObject);

            cutscene.played += OnCutscenePlayed;
            cutscene.stopped += OnCutsceneStopped;
            
            cutscene.Play();
        }

        private void OnCutscenePlayed(PlayableDirector director)
        {
            _player.SetCrime(true);
        }
        
        private void OnCutsceneStopped(PlayableDirector director)
        {
            _player.SetCrime(false);

            if (IsConnected)
            {
                OnExit(ConnectedObject);
            }
        }

        private void OnConnectionIsDown(T connectedObject)
        {
            logger.LogDebug($"-{crimeTag} {connectedObject.gameObject.name}");

            if (_player.CrimeInProgress)
            {
                cutscene.Stop();
            }
            
            cutscene.played -= OnCutscenePlayed;
            cutscene.stopped -= OnCutsceneStopped;
            
            _player.SetCrime(false);
            _player.CleanupMoneyEmitter();
        }

        public void OnHit()
        {
            if (_victimHealth.ApplyDamage())
            {
                logger.LogDebug($"add money {_victimHealth.damageReward}");
                
                _player.AddMoney(_victimHealth.damageReward, Random.Range(2, 3));
            }
        }

        public void OnLastHit()
        {
            OnHit();
            
            _victimHealth.CheckDeath();
        }

        protected abstract void SetupCutscene(T target);
        
        protected void ChangeReaction(SignalAsset signal, UnityAction listener)
        {
            var receiver = cutscene.gameObject.GetComponent<UnityEngine.Timeline.SignalReceiver>();
            
            var reaction = receiver.GetReaction(signal);

            if (reaction == null)
            {
                return;
            }
            
            reaction.RemoveAllListeners();
            reaction.AddListener(listener);
        }
    }
}