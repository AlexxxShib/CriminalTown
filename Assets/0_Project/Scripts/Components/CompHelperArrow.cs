using System;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Components
{
    public class CompHelperArrow : MonoBehaviour
    {
        public Transform target;
        public Transform arrow;

        public bool visible = true;

        private bool _hasTarget;

        private void OnEnable()
        {
            SetTarget(target);
        }

        public void SetTarget(Transform target)
        {
            if (target)
            {
                _hasTarget = true;
                this.target = target;
            }
            else
            {
                _hasTarget = false;
            }

            arrow.gameObject.SetActive(_hasTarget && visible);
        }

        public void Forget()
        {
            _hasTarget = false;
            arrow.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_hasTarget)
            {
                return;
            }

            var targetPos = target.position.ChangeY(transform.position.y);
            
            arrow.LookAt(targetPos);
        }
    }
}