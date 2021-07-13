using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobirayCore.Components
{
    public class RigidbodyAgent : MonoBehaviour
    {

        public Action<Collision> OnEventCollisionEnter;
        public Action<Collision> OnEventCollisionExit;

        private List<RigidbodyAgent> children = new List<RigidbodyAgent>();
        private int childrenInCollision = 0;

        private void OnEnable()
        {
            children.Clear();
            childrenInCollision = 0;
            
            if (!TryGetComponent<Rigidbody>(out var __))
            {
                var rigidbodyChildren = GetComponentsInChildren<Rigidbody>();
                
                foreach (var rigidbodyChild in rigidbodyChildren)
                {
                    var child = rigidbodyChild.gameObject.AddComponent<RigidbodyAgent>();

                    child.OnEventCollisionEnter += OnChildEnterCollision;
                    child.OnEventCollisionEnter += OnChildExitCollision;
                    
                    children.Add(child);
                }
                
            }
        }

        private void OnChildEnterCollision(Collision collision)
        {
            if (childrenInCollision == 0)
            {
                OnEventCollisionEnter?.Invoke(collision);
            }

            childrenInCollision++;
        }

        private void OnChildExitCollision(Collision collision)
        {
            childrenInCollision--;
            
            if (childrenInCollision == 0)
            {
                OnEventCollisionExit?.Invoke(collision);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.parent == transform.parent) return;
            
            OnEventCollisionEnter?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.transform.parent == transform.parent) return;
            
            OnEventCollisionExit?.Invoke(other);
        }
    }
}