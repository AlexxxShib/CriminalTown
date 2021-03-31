using System.Collections.Generic;
using Mobiray.Common;
using Mobiray.Controllers;
using Mobiray.Data;
using Mobiray.Data.Items;
using Mobiray.Movement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mobiray.Entities
{

    public enum FireType
    {
        NONE, SITTING, PRONE, STANDING
    }
    
    public class EntitySoldier : SignalReceiver, IVictim 
    {

        public bool InitOnAwake;
        public bool ChooceRandomTarget;
        public bool ChooceRandomTargetSimple;

        [Header("Data")]
        // public DataMarineState MarineState;

        [Header("Stats")]
        public float Health;
        public float CurrentHealth;
        public float Attack;
        public bool Death;
        public float ReloadSpeedFactor = 1;
        public ItemWeapon CurrentWeapon;

        [Header("Settings")]
        public ItemWeapon DefaultWeapon;
        public SkinnedMeshRenderer MeshRenderer;
        // public Color DamagedColor;
        // public GameObject CommanderSign;
        // public GameObject ShieldEffect;
        // public bool SpecialSkin = false;
        
        public FollowController FollowController { get; private set; }
        public AbsoluteMovementController MovementController => movementController;
        
        [HideInInspector]
        public List<Transform> Followers = new List<Transform>();

        private AbsoluteMovementController movementController;
        
        private Transform weapon;
        private Transform weaponPositionsParent;
        private DataWeaponPositions weaponPositions;
        
        private Animator animator;

        private Rigidbody rigidbody;
        private bool foundRigidbody;

        private Color normalColor;

        private static readonly int AnimationIdle = Animator.StringToHash("Idle");
        private static readonly int AnimationStartRun = Animator.StringToHash("StartRun");
        private static readonly int AnimationStopRun = Animator.StringToHash("StopRun");
        private static readonly int AnimationFireSitting = Animator.StringToHash("FireSitting");
        private static readonly int AnimationFireProne = Animator.StringToHash("FireProne");
        private static readonly int AnimationFireStanding = Animator.StringToHash("FireStanding");

        private static readonly int[] DeathAnimations =
        {
            Animator.StringToHash("Death"),
            Animator.StringToHash("Death1"),
            Animator.StringToHash("Death2"),
            Animator.StringToHash("Death3"),
        };

        //attack
        public List<Transform> nearEnemies = new List<Transform>();
        public Transform currentEnemy;
        
        private float currentReloadTime;
        private float reloadTime;

        private bool init = false;
        public bool immunity = false;
        public FireType fireType = FireType.NONE;
        public FireType recommendedFireType = FireType.NONE;
        
        private MobirayLogger logger = new MobirayLogger("EntitySoldier");
        
        public Material Material
        {
            get => MeshRenderer.material;
            set => MeshRenderer.material = value;
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            foundRigidbody = rigidbody != null;
            
            if (InitOnAwake)
            {
                Init();
            }
        }

        public void Init()
        {
            logger.SetTag(gameObject.name);
            
            // MarineState.SoldierId = -1;
            // MarineState.KillCount = 0;
            
            FollowController = GetComponent<FollowController>();
            
            movementController = GetComponent<AbsoluteMovementController>();
            movementController.enabled = true;
            movementController.OnMoveStateChanged = OnMoveStateChanged;

            animator = GetComponent<Animator>();
            weapon = transform.Find("Weapon");
            weaponPositionsParent = transform.Find("WeaponPositions");

            SetWeapon(DefaultWeapon);

            GenerateMaterial();
            
            Death = false;

            init = true;
            immunity = true;
        }

        public void SetMaterial(Material material)
        {
            Material = material;
            GenerateMaterial();
        }

        public void GenerateMaterial()
        {
            normalColor = Material.GetColor(EmissionColor); 
            Material = new Material(Material);
            
            Material.EnableKeyword("_EMISSION");
            Material.name = "Generated Material";
        }

        public void SetWeapon(ItemWeapon itemWeapon)
        {
            if (weapon.childCount > 0)
            {
                Destroy(weapon.GetChild(0).gameObject);
                Destroy(weaponPositionsParent.GetChild(0).gameObject);
            }
            CurrentWeapon = itemWeapon;

            var weaponPositionsGO = Instantiate(itemWeapon.WeaponPositions, weaponPositionsParent).transform;
            weaponPositionsGO.localPosition = Vector3.zero;
            weaponPositionsGO.localRotation = Quaternion.identity;
            
            weaponPositions = weaponPositionsGO.GetComponent<DataWeaponPositions>();
            
            var weaponGO = Instantiate(itemWeapon.PrefabGO, weapon).transform;
            // weaponGO.gameObject.SetActive(!Hostage);
            
            weaponGO.localPosition = Vector3.zero;
            weaponGO.localRotation = Quaternion.identity;

            // if (itemWeapon.AnimatorController.name != animator.runtimeAnimatorController.name)
            {
                animator.runtimeAnimatorController = itemWeapon.AnimatorController;
                ResetAnimationTriggers();
                SetAnimation(AnimationIdle);
                weapon.SetSame(weaponPositions.Idle);
            }
        }

        Vector3 delta = new Vector3(0, 0.6f, 0);
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Update()
        {
            if (!init) return;
            
            if (currentReloadTime < reloadTime)
            {
                currentReloadTime += Time.deltaTime;
                return;
            }
            
            if (Time.frameCount % 10 != 0) return;
            ClearRigidbody();
            
            //FIND SHOT ENEMY
        }

        public void BulletAttack(float attack, int soldierId)
        {
            if (Death || immunity) return;
            
            var prevHealth = CurrentHealth;
            CurrentHealth -= attack;

            if (CurrentHealth < 0) CurrentHealth = 0;

            /*if (healthBar != null)
            {
                healthBar.SetProgress(prevHealth / Health, CurrentHealth / Health, isCritical);
            }*/

            if (CurrentHealth > 0) return;
            
            Death = true;
            // ClearAttack();
            movementController.enabled = false;
            ResetAnimationTriggers();
            weapon.SetSame(weaponPositions.Death);
            SetAnimation(DeathAnimations[Random.Range(0, DeathAnimations.Length)]);
            
            this.StartTimer(0.2f, ClearRigidbody);
            
            // ToolBox.Get<SoundController>().PlayDeath();
        }

        public bool IsDeath() { return Death; }

        private void ResetAnimationTriggers()
        {
            animator.ResetTrigger(AnimationIdle);
            animator.ResetTrigger(AnimationStartRun);
            animator.ResetTrigger(AnimationStopRun);
            animator.ResetTrigger(AnimationFireSitting);
            animator.ResetTrigger(AnimationFireProne);
            animator.ResetTrigger(AnimationFireStanding);
            foreach (var deathAnimation in DeathAnimations)
            {
                animator.ResetTrigger(deathAnimation);
            }
        }

        private void OnMoveStateChanged(MoveState prevState, MoveState moveState)
        {
            if (!init || Death) return;
            
            ResetAnimationTriggers();
            fireType = FireType.NONE;
            
            // if (Commander) logger.LogDebug("OnMoveStateChanged " + moveState);
            
            switch (moveState)
            {
                case MoveState.MOVE:
                    weapon.SetSame(weaponPositions.Moving);
                    SetAnimation(AnimationStartRun);
                    break;

                case MoveState.STOP:

                    /*if (SilentMode)
                    {
                        weapon.SetSame(weaponPositions.Shooting);
                        SetAnimation(AnimationFireSitting);
                        return;
                    }*/

                    if (nearEnemies.Count > 0)
                    {
                        AttackState();
                    }
                    else
                    {
                        weapon.SetSame(weaponPositions.Idle);
                        SetAnimation(AnimationIdle);
                    }
                    
                    /*this.StartTimer(0.25f, () =>
                    {
                        if (movementController.MoveState == MoveState.STOP && HasShelter())
                        {
                            SetShieldEffect();
                        }
                    });*/
                    break;
            }
        }

        private void SetAnimation(int animationId)
        {
            animator.SetTrigger(animationId);
            // this.StartTimer(0.025f,() => animator.SetTrigger(animationId));
        }
        
        private void AttackState()
        {
            if (!init || Death) return;
            
            ResetAnimationTriggers();

            if (fireType == FireType.NONE)
            {
                fireType = FireType.SITTING;
            }

            switch (fireType)
            {
                case FireType.SITTING:
                    weapon.SetSame(weaponPositions.Shooting);
                    SetAnimation(AnimationFireSitting);
                    break;
                
                case FireType.PRONE:
                    weapon.SetSame(weaponPositions.ShootingProne);
                    SetAnimation(AnimationFireProne);
                    break;
                
                case FireType.STANDING:
                    weapon.SetSame(weaponPositions.ShootingStanding);
                    SetAnimation(AnimationFireStanding);
                    break;
            }

            // ChooseEnemy();
        }

        /*private void ChooseEnemy()
        {
            if (nearEnemies.Count == 0) return;

            if (ChooceRandomTarget)
            {
                if (ChooceRandomTargetSimple)
                {
                    currentEnemy = nearEnemies[Random.Range(0, nearEnemies.Count)];
                }
                else
                {
                    var enemies = EnemiesInDistance(gameSettings.SoldierFindRange * CurrentWeapon.Range);
                    if (enemies.Count == 0) return;

                    currentEnemy = enemies[Random.Range(0, enemies.Count)];
                }
            }
            else
            {
                currentEnemy = FindNearEnemy();
            }
            
            if (currentEnemy != null) LookAtEnemy();
        }

        private void LookAtEnemy()
        {
            transform.DOLookAt(currentEnemy.position.ChangeY(0), 0.3f);
        }

        private Transform FindNearEnemy()
        {
            var currentPos = transform.position;
            var minDistance = float.MaxValue;

            Transform nearEnemy = null;

            foreach (var enemy in nearEnemies)
            {
                var distance = (currentPos - enemy.transform.position).magnitude;
                
                if (distance < minDistance)
                {
                    nearEnemy = enemy;
                    minDistance = distance;
                }
            }

            return nearEnemy;
        }

        private List<Transform> EnemiesInDistance(float distance)
        {
            return nearEnemies
                .Where(e => (e.transform.position - transform.position).magnitude <= distance)
                .ToList();
        }

        public void SetAttack(List<Transform> nearEnemies)
        {
            this.nearEnemies.Clear();

            // if (!init || Death || nearEnemies.Count == 0) return;
            if (!init || Death || nearEnemies.Count == 0)
            {
                ClearAttack();
                return;
            }
            
            this.nearEnemies.AddRange(nearEnemies);

            if (movementController.MoveState == MoveState.STOP)
            {
                AttackState();
                ChooseEnemy();
            }
        }

        public void ClearAttack()
        {
            currentEnemy = null;
            nearEnemies.Clear();
            if (Enemy && transform.position.y > 1f) 
                recommendedFireType = FireType.STANDING;
            else
                recommendedFireType = FireType.NONE;
        }*/

        private void ClearRigidbody()
        {
            if (foundRigidbody)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
        
    }
}