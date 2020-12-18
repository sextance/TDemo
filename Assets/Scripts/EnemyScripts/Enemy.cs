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

    public float health;
    public float searchRange;
    public float enemyselfRange;

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

    private void FixedUpdate()
    {
        OnTriggerStay(collider);
        OnTriggerExit(collider);
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

    private void OnTriggerStay(Collider other)
    {
        s = GameManager.gm.Search(GameManager.gm.towerShapes, this);
        if(s.num < 0)
        {
            return;
        } else {
            TowerShape t = GameManager.gm.towerShapes[s.num];
            if (t == other.gameObject.GetComponentInParent<TowerShape>())
            {
                transform.LookAt(t.transform);
                Attack();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(GameManager.gm.towerShapes.Count > 0)
        {
            GameManager.gm.SearchAndGo(this);
        }
        
    }

    void Attack()
    {
        s = GameManager.gm.Search(GameManager.gm.towerShapes, this);
        if(s.num < 0)
        {
            return;
        }
        else
        {
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
    }

    public void ForceAttack(DefenceTowerEntity defenceTowerEntity)
    {
        if(GameManager.gm.towerShapes.Count < 0)
        {
            isLock = false;
            anim.SetInteger("CommonEnemy", 0);
        }
        else if (defenceTowerEntity == null)
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
