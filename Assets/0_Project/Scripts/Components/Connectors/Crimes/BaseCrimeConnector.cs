using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class BaseCrimeConnector<T> : BaseConnector<T> where T : MonoBehaviour
    {
        public string crimeTag = "crime";
        
        [Space]
        public PlayableDirector cutscene;
        public int cutsceneOuterTrackIndex = 2;
        public bool moveTargetBeforeCutscene = false;

        private EntityPlayer _player;

        protected CompHealth _victimHealth;
        
        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<EntityPlayer>();

            OnConnected += OnConnectionReady;
            OnDisconnected += OnConnectionIsDown;

            cutscene.played += _ => _player.SetCrime(true);
            
            cutscene.stopped += _ =>
            {
                _player.SetCrime(false);

                if (IsConnected)
                {
                    OnExit(ConnectedObject);
                }
            };
        }
        
        public override bool OnEnter(T connectedObject)
        {
            if (_player.isCaught || _player.isHidden)
            {
                return false;
            }

            if (connectedObject.TryGetComponent<CompHealth>(out var health))
            {
                if (health.Death)
                {
                    return false;
                }
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
            
            PlayCutscene(connectedObject.gameObject, moveTargetBeforeCutscene);
        }

        private void OnConnectionIsDown(T connectedObject)
        {
            logger.LogDebug($"-{crimeTag} {connectedObject.gameObject.name}");

            if (_player.CrimeInProgress)
            {
                cutscene.Stop();
            }
            
            _player.SetCrime(false);
            _player.CleanupMoneyEmitter();
        }

        public void OnHit()
        {
            if (_victimHealth.ApplyDamage())
            {
                _player.AddMoney(_victimHealth.damageReward, Random.Range(2, 3));
            }
        }

        public void OnLastHit()
        {
            OnHit();
            
            _victimHealth.CheckDeath();
        }

        private async void PlayCutscene(GameObject target, bool moveTarget)
        {
            if (moveTarget)
            {
                target.transform.DOLookAt(transform.position, 0.25f);
            }

            await transform.DOLookAt(target.transform.position, 0.25f).AwaitFor();

            if (!IsReady)
            {
                return;
            }

            _victimHealth = target.GetComponent<CompHealth>();
            
            // logger.LogDebug($"play cutscene, target {target.name} {_victimHealth}");

            var playableAsset = (TimelineAsset) cutscene.playableAsset;

            var trackKey = playableAsset.GetOutputTrack(cutsceneOuterTrackIndex);
            
            cutscene.SetGenericBinding(trackKey, target.GetComponentInChildren<Animator>());
            cutscene.Play();

            _player.SetupMoneyEmitter();
        }
    }
}