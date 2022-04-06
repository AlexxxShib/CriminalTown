using System;
using UnityEngine;

namespace CriminalTown.Components
{
    public class CompHealth : MonoBehaviour
    {
        public int health;
        public int damageReward;

        public int curHealth;
        
        public bool Death { get; private set; }

        public Action OnDeath;

        private void OnEnable()
        {
            Repair();
        }

        public void Repair()
        {
            curHealth = health;
            Death = false;
        }

        public bool ApplyDamage()
        {
            var hasHealth = curHealth > 0;

            if (hasHealth)
            {
                curHealth--;
            }

            return hasHealth;
        }

        public void CheckDeath()
        {
            if (curHealth <= 0 && !Death)
            {
                Death = true;
                OnDeath?.Invoke();
            }
        }
    }
}