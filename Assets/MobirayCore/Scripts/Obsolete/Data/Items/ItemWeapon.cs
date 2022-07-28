using UnityEngine;

namespace Mobiray.Data.Items
{
    [CreateAssetMenu(fileName = "ItemWeapon", menuName = "Weapons/Weapon")]
    public class ItemWeapon : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public int AvailableFromLevel;
        public int Priority;

        public bool IsAvailable(int lastOpenedLevel) { return lastOpenedLevel >= AvailableFromLevel; }
        
        public GameObject PrefabGO;
        public DataWeaponPositions WeaponPositions;
        public RuntimeAnimatorController AnimatorController;
        public Sprite ImpactSprite;
        // public ConfigBulletPack BulletPack;
        public float BulletSpeed = 40;

        [Header("Balance")]
        public float Attack;
        public float Speed;
        public float Range;
    }
}