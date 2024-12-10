using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace brolive
{
    public enum EnemyStates
    {
        idle, pursue, melee, ranged, shotgun, ultimate, dead
    }

    public class EnemyTest : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] GameObject meleeWeapon;
        [SerializeField] Damageable boss;
        [SerializeField] int lives = 2;

        [SerializeField] Navigator navigator;
        [SerializeField] Animator anim;
        [SerializeField] SpearThrowSpawn spearSpawn;
        [SerializeField] SpearShotgun shotgunSpear;
        [SerializeField] UltScript ultCast;
        Transform _transform;
        [SerializeField] Transform player;
        Rigidbody _rigidbody;

        EnemyStates state = EnemyStates.idle;
        float currentStateElapsed = 0;
        Vector3 currentTargetNodePosition;
        int pathNodeIndex = 0;
        Vector3 targetVelocity;
        bool inMeleeRange = false;
        bool spearThrow = false;
        float currentSpeed = 0;
        float holdSpeed;
        bool spearThrown = false;
        [SerializeField] float spearCooldown = 5f;
        float spearCountdown;
        bool shotgunUsed = false;
        bool shotgunBlast = false;
        [SerializeField] float shotgunCooldown = 5f;
        float shotgunCountdown;
        bool ult = false;
        bool ultUsed = false;
        [SerializeField] float ultCooldown = 15f;
        float ultCountdown;

        // Start is called before the first frame update
        void Start()
        {
            navigator = GetComponent<Navigator>();
            player = FindObjectOfType<PlayerLogic>().transform;
            _rigidbody = GetComponent<Rigidbody>();
            _transform = transform;
            anim = GetComponent<Animator>();
            holdSpeed = speed;
            spearCountdown = spearCooldown;
        }

        // Update is called once per frame
        void Update()
        {
            currentStateElapsed += Time.deltaTime;

            switch (state)
            {
                case EnemyStates.idle:
                    UpdateIdle();
                    break;
                case EnemyStates.melee:
                    UpdateMelee();
                    break;
                case EnemyStates.pursue:
                    UpdatePursue();
                    break;
                case EnemyStates.ranged:
                    UpdateRanged();
                    break;
                case EnemyStates.shotgun:
                    UpdateShotgun();
                    break;
                case EnemyStates.ultimate:
                    UpdateUltimate();
                    break;
                case EnemyStates.dead:
                    UpdateDead();
                    break;
            }
            if (spearThrown)
            {
                if(spearCountdown >= spearCooldown)
                {
                    spearThrown = false;
                }
                else
                {
                    spearCountdown += Time.deltaTime;
                }
            }
            if (shotgunUsed)
            {
                if (shotgunCountdown >= shotgunCooldown)
                {
                    shotgunUsed = false;
                }
                else
                {
                    shotgunCountdown += Time.deltaTime;
                }
            }
            if (ultUsed)
            {
                if (ultCountdown >= ultCooldown)
                {
                    ultUsed = false;
                }
                else
                {
                    ultCountdown += Time.deltaTime;
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity = targetVelocity;
            anim.SetFloat("Speed", currentSpeed);
        }
        
        //idle
        void UpdateIdle()
        {
            //Debug.Log("in idle");
            //anim.SetFloat("Speed", 0);
            if (currentStateElapsed > 2.0f)
            {
                if (inMeleeRange)
                    EnterMelee();
                else
                    AttemptBeginPursue();
            }
            meleeWeapon.SetActive(false);
        }

        //chase
        bool AttemptBeginPursue()
        {
            //Debug.Log("attempting to pursue");
            currentSpeed = speed;
            if (AttemptMakePathToPlayer())
            {
                pathNodeIndex = 0;
                state = EnemyStates.pursue;
                currentStateElapsed = 0;

                return true;
            }

            Debug.Log("failed attempt to pursue");
            return false;
        }

        void UpdatePursue()
        {
            //Debug.Log("in pursue");

            currentTargetNodePosition = navigator.PathNodes[pathNodeIndex];

            //Debug.Log("current target position is " + currentTargetNodePosition + " at index " + pathNodeIndex);

            Vector3 dirToNode = (currentTargetNodePosition - _transform.position);//.normalized;
            dirToNode.y = 0;
            dirToNode.Normalize();

            _transform.forward = dirToNode;

            float distToNode = Vector3.Distance(currentTargetNodePosition, _transform.position);
            meleeWeapon.SetActive(false);

            //Debug.Log("distance to node: " + distToNode);

            if (distToNode < 2f)
            {
                //Debug.Log("close to node");
                pathNodeIndex++;
                if (DistanceToPlayer() < 3f)
                {
                    inMeleeRange = true;
                }
                if (pathNodeIndex >= navigator.PathNodes.Count)
                {
                    pathNodeIndex = 0;
                    AttemptMakePathToPlayer();
                    return;
                }

            }
            else if (lives < 1 && !ultUsed)
            {
                ult = true;
                ultUsed = true;
            }
            else if(DistanceToPlayer()>2f && DistanceToPlayer()<5f && lives<2 && !shotgunUsed)
            {
                shotgunBlast = true;
                shotgunUsed = true;
            }
            else if (DistanceToPlayer() > 5f && !spearThrown)
            {
                spearThrow = true;
                spearThrown = true;
            }
            if (inMeleeRange)
            {
                // do melee attack
                EnterMelee();
                return;
            }
            if (spearThrow)
            {
                EnterThrow();
                return;
            }
            if (shotgunBlast)
            {
                EnterShotgun();
                return;
            }
            if (ult)
            {
                EnterUltimate();
                return;
            }

            targetVelocity = _transform.forward * speed;
            targetVelocity.y = _rigidbody.velocity.y;

            if (currentStateElapsed > 1) // rebuild path every half second
            {
                pathNodeIndex = 1;
                AttemptMakePathToPlayer();
            }
        }

        //melee
        void EnterMelee()
        {
            //Debug.Log("Enter melee");
            // animator.setTrigger("melee");
            var dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            transform.forward = dirToPlayer;
            targetVelocity = Vector3.zero;
            state = EnemyStates.melee;
            currentStateElapsed = 0;
            //anim.SetFloat("Speed", speed);
            inMeleeRange = false;

            StartCoroutine(HandleMelee());
        }

        IEnumerator HandleMelee()
        {
            //timeSinceLastMelee = 0;aawwww
            meleeWeapon.SetActive(true);
            anim.SetTrigger("Spin");
            currentSpeed = 0;
            //meleeWeapon.GetComponent<Animator>().SetTrigger("swing");
            yield return new WaitForSeconds(2f);
            meleeWeapon.SetActive(false);
        }

        void UpdateMelee()
        {
            //Debug.Log("in melee");
            if (currentStateElapsed >= 2.0f)
            {
                state = EnemyStates.idle;
            }
        }

        //ranged
        void EnterThrow()
        {
            var dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            transform.forward = dirToPlayer;
            targetVelocity = Vector3.zero;
            speed = 0;
            currentSpeed = 0;
            state = EnemyStates.ranged;
            currentStateElapsed = 0;
            spearThrow = false;
            StartCoroutine(HandleThrow());
        }
        
        IEnumerator HandleThrow()
        {
            anim.SetTrigger("Throw");
            yield return new WaitForSeconds(1.5f);
            spearSpawn.SpawnSpear();
            speed = holdSpeed;
            currentSpeed = speed;
            spearThrown = true;
            spearCountdown = 0f;
        }
        
        void UpdateRanged()
        {
            if (currentStateElapsed >= 2.0f)
            {
                state = EnemyStates.idle;
            }
        }

        //shotgun
        void EnterShotgun()
        {
            var dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            transform.forward = dirToPlayer;
            targetVelocity = Vector3.zero;
            speed = 0;
            currentSpeed = 0;
            state = EnemyStates.shotgun;
            currentStateElapsed = 0;
            shotgunBlast = false;
            StartCoroutine(HandleShotgun());
        }

        IEnumerator HandleShotgun()
        {
            anim.SetTrigger("Shotgun");
            yield return new WaitForSeconds(1.5f);
            shotgunSpear.ShootSpears();
            speed = holdSpeed;
            currentSpeed = speed;
            shotgunUsed = true;
            shotgunCountdown = 0f;
        }

        void UpdateShotgun()
        {
            if (currentStateElapsed >= 2.0f)
            {
                state = EnemyStates.idle;
            }
        }
        
        //ultimate
        void EnterUltimate()
        {
            var dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            transform.forward = dirToPlayer;
            targetVelocity = Vector3.zero;
            speed = 0;
            currentSpeed = 0;
            state = EnemyStates.ultimate;
            currentStateElapsed = 0;
            ult = false;
            StartCoroutine(HandleUltimate());
        }

        IEnumerator HandleUltimate()
        {
            anim.SetTrigger("Ult");
            yield return new WaitForSeconds(2f);
            ultCast.CastUlt();
            speed = holdSpeed;
            currentSpeed = speed;
            ultUsed = true;
            ultCountdown = 0f;
        }

        void UpdateUltimate()
        {
            if (currentStateElapsed >= 2.0f)
            {
                state = EnemyStates.idle;
            }
        }

        //death
        public void Death()
        {
            if (lives <= 0)
            {
                navigator.enabled = false;
                anim.SetTrigger("Death");
                targetVelocity = Vector3.zero;
                GameManager.instance.GoToNextLevel();
                state = EnemyStates.dead;
            }
            else
            {
                lives--;
                boss.revive();
            }
        }

        void UpdateDead()
        {
            //Debug.Log("in dead");
        }

        //navigate
        bool AttemptMakePathToPlayer()
        {
            return (navigator.CalculatePathToPosition(player.position));
        }

        float DistanceToPlayer()
        {
            return Vector3.Distance(_transform.position, player.position);
        }

        public void SetInMeleeRange(bool inMeleeRange)
        {
            this.inMeleeRange = inMeleeRange;
        }

        
    }
}