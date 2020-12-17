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
    Transform cube = default;

    public Vector3 point;

    public float health,
        attack, attackingRange,
        tauntRange, searchRange;



    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin Factory!");
            originFactory = value;
        }
    }


    public bool GameUpdate()
    {
        if (this == null)
        {
            return false;
        }
        return true;
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

   /* public TowerShape Search(List<TowerShape>[] pools)
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
    }*/
}
public enum EnemyType
{
    T, DPS, common
}
