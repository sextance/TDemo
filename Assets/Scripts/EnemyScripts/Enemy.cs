using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
    public static Enemy enemy;
>>>>>>> parent of ea1aad4... 1123

    EnemyFactory originFactory;

    [SerializeField]
    NavMeshAgent navEnemy = default;

    [SerializeField]
    Transform cube = default;

    public Vector3 point;

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


    private void Update()
    {
        FindAndGo();
        Attack();
    }



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

<<<<<<< HEAD
<<<<<<< Updated upstream
=======
    bool FindAndGo()
    {
        if (point != null)
        {
            navEnemy.SetDestination(point);
            return true;
        }
        return false;
    }
>>>>>>> parent of ea1aad4... 1123

    void Attack()
    {
        if (Vector3.Distance(navEnemy.transform.position, point) <= 0.5f)
        {
            Debug.Log("Attack Tower");
        }
    }

    public void ApplyDamge(float damge)
    {
        if (health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return;
        }
        health -= damge;
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

    public TowerShape Search(List<TowerShape>[] pools)
    {
        TowerShape instance = null;
        float min = 19900000;
        float distance = 1000000;
        for (int i=0; i<=pools.Length; i++)
        {
            List<TowerShape> pool = pools[i];
            for(int j=0; j<=pool.Count; j++)
            {
                distance = Vector3.Distance(pool[j].transform.localPosition, this.transform.localPosition);
                if (distance < min)
                {
                    min = distance;
                    instance = pool[j];
                }

            }
        }
        return instance;
    }
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
