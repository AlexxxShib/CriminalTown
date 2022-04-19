using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace CriminalTown.Entities
{
    public class EntityShop : BaseConnectorTrigger<EntityShop, ShopCrimeConnector>
    {
        public GameObject salesman;
        public PlayableDirector cutscene;
        public Animator shopView;
        
        private CompStaticCrime _staticCrime;
        
        private static readonly int AnimIdIfDoor = Animator.StringToHash("ifDoor");
        private static readonly int AnimIdIfShop = Animator.StringToHash("ifShop");

        protected override void Awake()
        {
            base.Awake();

            _staticCrime = GetComponent<CompStaticCrime>();

            _staticCrime.toolType = ToolType.HANDGUN;
            _staticCrime.timeLock = ToolBox.Get<ConfigMain>().GetCrime(CrimeType.SHOP).timeLock;
            
            available = ToolBox.Get<DataGameState>()
                .tools.Contains(_staticCrime.toolType);
            
            _staticCrime.OnAvailabilityChanged += a => available = a;

            triggerAgent.onCallTriggerEnter += OnEnterShop;
            triggerAgent.onCallTriggerStay += OnStayShop;
            triggerAgent.onCallTriggerExit += OnExitShop;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            available = ToolBox.Get<DataGameState>()
                .tools.Contains(_staticCrime.toolType);
        }

        private void OnEnterShop(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfShop, true);
            }
        }
        
        private void OnStayShop(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfShop, true);
            }
        }
        
        private void OnExitShop(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfShop, false);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfDoor, true);
            }
        }

        protected override void OnTriggerStay(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfDoor, true);
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (IsPlayer(other))
            {
                shopView.SetBool(AnimIdIfDoor, false);
            }
        }

        private bool IsPlayer(Collider other)
        {
            var player = other.gameObject.GetComponentInParent<EntityPlayer>();

            return player != null;
        }
    }
}