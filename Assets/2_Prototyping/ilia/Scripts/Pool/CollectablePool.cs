using System;
using System.Collections;
using System.Collections.Generic;
using _0_Project.Scripts.Controllers;
using Mobiray.Common;
using Mobiray.Controllers;
using Template;
using Template.Data;
using UnityEngine;
using Random = System.Random;

namespace Prehistoric.Pool
{
    public class CollectablePool : SignalReceiver, IReceive<SignalItemGather>
    {
        public GameObject Prefab;

        public enum ResType
        {
            WOOD,
            MEAT,
            SKINS,
            GOLD
        }

        public ResType ResourceType;
        public int PreloadCapacity = 10;
        public float ShotPower = 3f;
        public float RandomOffset = 1f;
        public float YForce = 5;

        private List<GameObject> _pool;
        private Dictionary<GameObject, bool> _gathered;
        private DataGameState _gameState;

        private void Awake()
        {
            _pool = new List<GameObject>();
            _gathered = new Dictionary<GameObject, bool>();
            _gameState = ToolBox.Get<DataGameState>();
            Preload(PreloadCapacity);
        }

        private GameObject Create(int id)
        {
            var instantiated = Instantiate(Prefab, Vector3.zero, Quaternion.identity, transform);
            instantiated.SetActive(false);
            instantiated.name = instantiated.name + id;
            _pool.Add(instantiated);
            _gathered[instantiated] = false;
            return instantiated;
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Create(i);
            }
        }

        private IEnumerator Animate(GameObject prefab, Vector3 from, Vector3 to, bool isCollectable)
        {
            prefab.SetActive(true);
            //yield return new WaitForSeconds(0.25f);
            prefab.transform.position = from + Vector3.up;
            prefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
            prefab.GetComponent<Rigidbody>().AddForce(
                (to - from) * ShotPower + new Vector3(
                    UnityEngine.Random.Range(-RandomOffset, RandomOffset),
                    YForce + UnityEngine.Random.Range(0, RandomOffset),
                    UnityEngine.Random.Range(-RandomOffset, RandomOffset)
                    ), 
                ForceMode.Impulse
            );
            var collectable = prefab.GetComponent<AutoCollectable>();
            if (collectable)
            {
                collectable.enabled = isCollectable;
            }
            yield break;
        }

        IEnumerator Gather(GameObject prefab, bool transact)
        {
            var animator = prefab.GetComponent<Animator>();
            animator.SetBool("isGathered", true);
            if(transact) {
                switch (prefab.tag)
                {
                    case "wood":
                        _gameState.WoodTransaction(1);
                        break;
                    case "skin":
                        _gameState.SkinsTransaction(1);
                        break;
                    case "food":
                        _gameState.FoodTransaction(1);
                        break;
                }
            }

            yield return new WaitForSeconds(0.2f);
            prefab.transform.position = new Vector3(0, -100, 0);
            animator.SetBool("isGathered", false);
            //yield return new WaitForSeconds(1f);

            prefab.SetActive(false);
            _gathered[prefab] = false;
        }

        public GameObject Play(Vector3 from, Vector3 to, bool isCollectable = true)
        {
            foreach (var prefab in _pool)
            {
                if (!prefab.active)
                {
                    StartCoroutine(Animate(prefab, from, to, isCollectable));
                    return prefab;
                }
            }

            var newPrefab = Create(_pool.Count);
            StartCoroutine(Animate(newPrefab, from, to, isCollectable));
            return newPrefab;
        }

        public void HandleSignal(SignalItemGather signal)
        {
            if (_pool.Contains(signal.Prefab))
            {
                if (!_gathered[signal.Prefab])
                {
                    _gathered[signal.Prefab] = true;
                    StartCoroutine(Gather(signal.Prefab, signal.transact));
                }
            }
        }
    }
}