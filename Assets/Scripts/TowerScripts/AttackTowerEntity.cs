using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerEntity : TowerEntity
{
    //ATK range = 3
    public float attackRange = 10.0f;

    public Projectile projectile;

    float coolDownTime;
    bool isCoolDownTime;
    bool isEnemyLocked;

    Enemy lockTarget;

    // Start is called before the first frame update
    void OnEnable()
    {
        health = 10;
        isCoolDownTime = false;
        isEnemyLocked = false;
        coolDownTime = 1.0f;
        lockTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemyLocked)
        {
            if (!isCoolDownTime)
            {
                //Projectile instance = ProjectileFactory.pf.Get();
                Projectile instance = Instantiate(projectile);
                instance.targetEnemy = lockTarget;
                Transform t = instance.transform;
                t.localPosition = this.transform.localPosition + Vector3.up * 10.0f;
                isCoolDownTime = true;
            } else {
                coolDownTime -= Time.deltaTime;
                if(coolDownTime <= 0f)
                {
                    coolDownTime = 1.0f;
                    isCoolDownTime = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isEnemyLocked)
            AcquireTargetEnemy();
        else
            CheckLockEnemyState();
    }

    // Seek Enemy
    void AcquireTargetEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, attackRange, LayerMask.GetMask("Enemy"));
        if(targets.Length > 0)
        {
            lockTarget = targets[0].GetComponentInParent<Enemy>();
            if (lockTarget == null)
                Debug.LogError("Error locking!");
            isEnemyLocked = true;
        }
    }

    void CheckLockEnemyState()
    {
        //Enemy move out of range or died (set as inactive)
        if (Vector3.Distance(lockTarget.transform.localPosition, this.transform.localPosition) >= attackRange 
            || lockTarget.gameObject.activeSelf == false )
        {
            isEnemyLocked = false;
            lockTarget = null;
        } 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, attackRange);
    }
}
