using System;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using Mobiray.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CriminalTown.Entities
{
    public class EntityCitizen : MonoBehaviour
    {
        public Transform bodyContainer;
        public Transform headContainer;
        public Transform glassesContainer;

        private ConfigMain _config;

        private CompHumanControl _humanControl;

        private void Awake()
        {
            _config = ToolBox.Get<ConfigMain>();
            
            InitializeView();

            _humanControl = GetComponent<CompHumanControl>();
            _humanControl.MaxSpeed = _config.citizenSpeedWalk;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"enter {other.gameObject.name}", other.gameObject);
            
            var connector = other.GetComponentInParent<CitizenConnector>();

            if (connector != null)
            {
                connector.OnEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var connector = other.GetComponentInParent<CitizenConnector>();

            if (connector != null)
            {
                connector.OnExit(this);
            }
        }

        private void InitializeView()
        {
            var material = _config.citizenMaterials.RandomItem();
            
            var body = SetRandomChild(bodyContainer, material, 1);
            gameObject.name = body.name.Replace("Body_", String.Empty);
            
            SetRandomChild(headContainer, material);

            var hasGlasses = 0.1f.Chance();
            
            glassesContainer.gameObject.SetActive(hasGlasses);

            if (hasGlasses)
            {
                SetRandomChild(glassesContainer, material);
            }
        }

        private static GameObject SetRandomChild(Transform container, Material material, int from = 0)
        {
            var variant = Random.Range(from, container.childCount);
            GameObject variantGo = null;
            
            for (int i = from; i < container.childCount; i++)
            {
                var go = container.GetChild(i).gameObject;
                
                go.SetActive(i == variant);

                if (i == variant)
                {
                    go.GetComponent<Renderer>().material = material;

                    variantGo = go;
                }
            }

            return variantGo;
        }
    }
}