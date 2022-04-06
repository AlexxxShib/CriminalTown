using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class AtmCrimeConnector : BaseConnector<EntityAtm>
    {

        public PlayableDirector cutscene;
        public int cutsceneOuterTrackIndex = 2;

        private EntityPlayer _player;

        private EntityAtm _connectedObject;
        private CompHealth _victimHealth;

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
                    OnExit(_connectedObject);
                }
            };
        }

        public override bool OnEnter(EntityAtm connectedObject)
        {
            if (_player.isCaught || _player.isHidden)
            {
                return false;
            }

            _connectedObject = connectedObject;

            if (connectedObject.TryGetComponent<CompHealth>(out var health))
            {
                if (health.Death)
                {
                    return false;
                }
            }
            
            return base.OnEnter(connectedObject);
        }

        private void OnConnectionReady(EntityAtm atm)
        {
            logger.LogDebug($"+{atm.gameObject.name}");
            
            PlayCutscene(atm.gameObject, false);
        }

        private void OnConnectionIsDown(EntityAtm atm)
        {
            logger.LogDebug($"-{atm.gameObject.name}");

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
            
            await transform.DOLookAt(target.transform.position, 0.25f).IsComplete();

            if (!IsReady)
            {
                return;
            }

            _victimHealth = target.GetComponent<CompHealth>();
            
            var playableAsset = (TimelineAsset) cutscene.playableAsset;

            /*foreach (var binding in playableAsset.outputs)
            {
                logger.LogDebug(binding.streamName);
            }*/
            
            var trackKey = playableAsset.GetOutputTrack(cutsceneOuterTrackIndex);
            
            cutscene.SetGenericBinding(trackKey, target.GetComponentInChildren<Animator>());
            cutscene.Play();

            _player.SetupMoneyEmitter();
        }
        
    }
}