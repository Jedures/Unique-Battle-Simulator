using Enums;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[System.Serializable]
public struct SoldierCharacteristic
{
    public float health;
    public float damage;
    public int price;

    public PlayerTag playerTag;
    
    public Animator animator;
    
    public float attackTime;
    public bool isAttack;

    public NavMeshAgent agent;
}


