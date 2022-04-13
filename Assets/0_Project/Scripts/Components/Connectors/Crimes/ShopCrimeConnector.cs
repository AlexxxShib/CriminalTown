using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class ShopCrimeConnector : BaseCrimeConnector<EntityShop>
    {
        protected override async void SetupCutscene(EntityShop target)
        {
            cutscene = target.cutscene;

            ChangeReaction(_config.signalLastHit, OnLastHit);
            
            var salesman = target.salesman.transform;
            
            salesman.DOLookAt(transform.position, _config.lookAtTime);

            await transform.DOLookAt(salesman.position, _config.lookAtTime).AwaitFor();

            if (!IsReady)
            {
                return;
            }
            
            _player.SetupMoneyEmitter();
            
            var playableAsset = (TimelineAsset) cutscene.playableAsset;
            
            foreach (var outputTrack in playableAsset.GetOutputTracks())
            {
                if (cutsceneTrackMap.TryGetValue(outputTrack.name, out var value))
                {
                    cutscene.SetGenericBinding(outputTrack, value);
                }
            }
        }

        public override bool OnExit(EntityShop connectedObject)
        {
            if (_victimHealth.Death)
            {
                ToolBox.Signals.Send(SignalPoliceStatus.ActiveState());
            }
            
            return base.OnExit(connectedObject);
        }
    }
}