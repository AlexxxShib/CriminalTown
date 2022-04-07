using CriminalTown.Configs;
using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class StreetFoodCrimeConnector : BaseCrimeConnector<EntityStreetFood>
    {
        protected override async void PlayCutscene(EntityStreetFood target, bool moveTarget)
        {
            cutscene = target.cutscene;

            var receiver = target.cutscene.gameObject.GetComponent<UnityEngine.Timeline.SignalReceiver>();
            var signalReaction = receiver.GetReaction(receiver.GetSignalAssetAtIndex(0));
            
            signalReaction.RemoveAllListeners();
            signalReaction.AddListener(OnLastHit);
            
            var lookAtDuration = ToolBox.Get<ConfigMain>().lookAtTime;
            var salesman = target.salesman.transform;
            
            salesman.DOLookAt(transform.position, lookAtDuration);

            await transform.DOLookAt(salesman.position, lookAtDuration).AwaitFor();

            if (!IsReady)
            {
                return;
            }
            
            _player.SetupMoneyEmitter();
            
            var playerAnimator = _player.GetComponentInChildren<Animator>();
            
            var playableAsset = (TimelineAsset) cutscene.playableAsset;

            var trackKey = playableAsset.GetOutputTrack(cutsceneOuterTrackIndex);
            
            cutscene.SetGenericBinding(trackKey, playerAnimator);
            cutscene.Play();
        }
    }
}