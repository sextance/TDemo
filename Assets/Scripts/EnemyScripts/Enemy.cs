using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    GameManager.VectorAndNum s = new GameManager.VectorAndNum();
    bool startAttack;
    float attackTime;

    EnemyFactory originFactory;
    public Animator anim;

    public NavMeshAgent navMesh = default;

    public float health, attackingRange,
         searchRange;

    public int attack;

    Collider collider;
    public bool isLock;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin Factory!");
            originFactory = value;
        }
    }

    private void Start()
    {
        collider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        isLock = false;
        attackTime = 0;
    }

    private void OnEnable()
    {
        health = 3f;
        startAttack = true;
        collider = GetComponent<Collider>();
    }

    protected void Update()
    {
        if (health <= 0f)
        {
            OriginFactory.Reclaim(this);
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
        if (health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return;
        }
    }

    /*public void Attack(Enemy enemy)
    {
        s = GameManager.gm.Search(GameManager.gm.towerShapes, enemy);
        float distance = Vector3.Distance(enemy.transform.localPosition, s.point);
        if(distance > 5f) { }
        else if(distance <= 5f)
        {
            GameManager.gm.towerShapes[s.num].GetComponent<AttackTowerEntity>().TakeDamage(attack);
            Debug.Log("Attacking");
            Debug.LogError("Can't Attack");
        }
    }*/

    private void OnTriggerStay(Collider other)
    {
        s = GameManager.gm.Search(GameManager.gm.towerShapes, this);
        if(s.num < 0)
        {
            return;
        } else {
            TowerShape t = GameManager.gm.towerShapes[s.num];
            if (t == other.gameObject.GetComponent<TowerShape>())
            {
                Attack();
            }
            else if (GameManager.gm.towerShapes[s.num] == null)
            {
                GameManager.gm.SearchAndGo(this);
                anim.SetInteger("CommonEnemy", 1);
            }
        }
        
    }

    void Attack()
    {
        s = GameManager.gm.Search(GameManager.gm.towerShapes, this);
        GameObject t = GameManager.gm.towerShapes[s.num].gameObject;
        if (startAttack)
        {
            anim.SetInteger("CommonEnemy", 2);
            t.GetComponent<TowerEntity>().TakeDamage(attack);
            startAttack = false;
        }
        else if (!startAttack)
        {
            anim.SetInteger("CommonEnemy", 0);
            attackTime += Time.deltaTime;
            if (attackTime >= 1f)
            {
                attackTime -= 1f;
                startAttack = true;
            }
        }
    }

    public void ForceAttack(DefenceTowerEntity defenceTowerEntity)
    {
        if (defenceTowerEntity == null)
        {
            isLock = false;
            anim.SetInteger("CommonEnemy", 0);
        }

        else if (isLock == false)
        {
            isLock = true;
            navMesh.SetDestination(defenceTowerEntity.transform.localPosition);
            anim.SetInteger("CommonEnemy", 1);
        }

    }
}
public enum EnemyType
{
    T, DPS, common
}
