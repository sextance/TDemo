using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
<<<<<<< Updated upstream

    EnemyFactory originFactory;

    public NavMeshAgent navMesh = default;

    public float health,
        attack, attackingRange,
        tauntRange, searchRange;
=======
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
>>>>>>> Stashed changes

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin Factory!");
            originFactory = value;
        }
    }

<<<<<<< Updated upstream

    public bool GameUpdate()
    {
        if (this == null)
=======
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
>>>>>>> Stashed changes
        {
            return false;
        }
        return true;
    }

<<<<<<< Updated upstream


    public void ApplyDamge(float damge)
    {
        
        health -= damge;
        Debug.Assert(damge > 0, "Wrong Damage!");
        if (health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return;
        }
    }

   /* public void SearchTower()
    {
        Vector3 a =  this.transform.localPosition;
        Vector3 b = a;
        b.y += 5f;
        
        Collider[] towerColliders = Physics.OverlapCapsule(a,b,searchRange,LayerMask.GetMask("Tower"));
        if(towerColliders.Length <= 0f)
        {
            return;
        }
        else
        {
            point = towerColliders[0].transform.localPosition;
        }
       
    }*/

    
}
public enum EnemyType
{
    T, DPS, common
=======
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
>>>>>>> Stashed changes
}
