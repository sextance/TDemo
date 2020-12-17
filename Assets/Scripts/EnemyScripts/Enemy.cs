using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    EnemyFactory originFactory;

    public NavMeshAgent navMesh = default;

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
}
