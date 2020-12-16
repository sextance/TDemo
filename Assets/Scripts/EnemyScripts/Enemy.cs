using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    EnemyFactory originFactory;

    [SerializeField]
    NavMeshAgent navEnemy = default;

    [SerializeField]
    Transform TargetPoint = default;

    public float Health { get; set; }

    float attack;
    float attackRange;
    float tauntRange;

    private void Update()
    {
        FindAndGo();
        Attack();
    }

    public EnemyType enemyType => EnemyType.common;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin Factory!");
            originFactory = value;
        }
    }

    public void Initialize(float health,float attack,float attackingRange,float tauntRange)
    {
        this.attack = attack;
        this.attackRange = attackingRange;
        this.tauntRange = tauntRange;
        Health = health;
    }

    public bool GameUpdate()
    {
        if(this == null)
        {
            return false;
        }
        return true;
    }

    bool FindAndGo()
    {
        navEnemy.SetDestination(TargetPoint.position);
        return true;
    }

    void Attack()
    {
        if(navEnemy.velocity.x == 0 && navEnemy.velocity.z == 0)
        {
            Debug.Log("Attack Tower");
        }
    }

    void ApplyDamge(float damge)
    {
        Health -= damge;
    }

    public enum EnemyType
    {
        T,DPS,common
    }
}
