using System;
using System.Collections;
using Enums;
using Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerController : MonoBehaviour, ISoldier
    {
        #region vars

        public SoldierCharacteristic soldierCharacteristic;

        private IStat m_SoldierStat { get; set; }

        private Transform m_Enemy = null;

        #region Hashable Vars

        private static readonly int Attack1 = Animator.StringToHash("Attack1");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Dead = Animator.StringToHash("Dead");

        #endregion

        #endregion

        #region Unity Methods

        private void Awake()
        {
            m_SoldierStat = new SoldierStat(soldierCharacteristic.health, soldierCharacteristic.damage);

            soldierCharacteristic.isAttack = false;
        }

        private void Update()
        {
            Death();

            GoInAttack();

            AnimationController();
        }

        #endregion

        #region AI

        public void GoInAttack()
        {
            if (!LevelManager._levelManager.isStart)
            {
                soldierCharacteristic.agent.destination = transform.position;
                return;
            }

            m_Enemy = FindEnemy();
            
            if (m_Enemy != null)
            {
                if (m_Enemy.GetComponent<PlayerController>().m_SoldierStat.GetHp() <= 0)
                {
                    m_Enemy = null;
                    return;
                }

                if (Vector3.Distance(transform.position, m_Enemy.position) > 2.5f)
                {
                    soldierCharacteristic.agent.destination = m_Enemy.position;
                }
                else
                {
                    soldierCharacteristic.agent.destination = transform.position;

                    if (soldierCharacteristic.attackTime >= 0)
                        soldierCharacteristic.attackTime -= Time.deltaTime;
                    else
                    {
                        soldierCharacteristic.isAttack = true;
                        soldierCharacteristic.attackTime = Random.Range(1, 3);
                    }
                }
            }
            else
            {
                soldierCharacteristic.agent.destination = transform.position;
            }
        }

        public Transform FindEnemy()
        {
            Transform target = null;
       
            var closestDistance = Mathf.Infinity;

            var enemies = soldierCharacteristic.playerTag == PlayerTag.Enemy
                ? LevelManager._levelManager.GetAlliesList()
                : LevelManager._levelManager.GetEnemyList();
            
            foreach(var potentialTarget in enemies)
            {
                if (!(Vector3.Distance(transform.position, potentialTarget.transform.position) < closestDistance) ||
                    potentialTarget == null)
                    continue;
                
                closestDistance = Vector3.Distance(transform.position, potentialTarget.transform.position);
                
                target = potentialTarget.transform;
            }	
			
            return target ? target : null;
        }

        #endregion

        #region Player Controllers 

        public void Attack() => DoDamage(m_Enemy.GetComponent<PlayerController>().m_SoldierStat);

        public void Death()
        {
            if (m_SoldierStat.GetHp() > 0)
                return;

            if (soldierCharacteristic.playerTag != PlayerTag.Enemy)
                LevelManager._levelManager.DeleteAllieFromList(this.gameObject);
            else
                LevelManager._levelManager.DeleteEnemyFromList(this.gameObject);

            soldierCharacteristic.animator.SetTrigger(Dead);

            StartCoroutine(Dying());
        }

        public void DoDamage(IStat enemy)
        {
            if (enemy.GetHp() <= 0)
                return;
            
            enemy.Damage(m_SoldierStat);
        }

        private void AnimationController()
        {
            if (m_SoldierStat.GetHp() < 0)
                return;

            if (!(soldierCharacteristic.isAttack))
            {
                soldierCharacteristic.animator.SetTrigger(
                    soldierCharacteristic.agent.destination == transform.position ? Idle : Walk);
            }
            else
            {
                soldierCharacteristic.animator.SetTrigger(Attack1);
                soldierCharacteristic.isAttack = false;
            }
        }

        #endregion

        #region Interfaces

        private IEnumerator Dying()
        {
            yield return new WaitForSeconds(5f);

            Destroy(gameObject);
        }

        #endregion
    }
}