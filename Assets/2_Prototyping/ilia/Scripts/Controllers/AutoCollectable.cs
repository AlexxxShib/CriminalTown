using System;
using Prehistoric.Controllers;
using UnityEngine;

namespace _0_Project.Scripts.Controllers
{
    public class AutoCollectable : MonoBehaviour
    {
        private Transform _target;
        public float Speed = 0.05f;

        private void OnEnable()
        {
            _target = FindObjectOfType<CrowdController>().Chief.transform;
        }

        private void OnDisable()
        {
            _target = null;
        }

        private void Update()
        {
            if (_target != null)
            {
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, _target.position.x, Speed),
                    Mathf.Lerp(transform.position.y, _target.position.y, Speed), Mathf.Lerp(transform.position.z, _target.position.z, Speed));
            }
        }
    }
}