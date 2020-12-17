using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerEntity : TowerEntity
{
    //ATK range = 3
    public float attackRange = 10.0f;

    public Projectile projectile;

    int damage;
    float coolDownTime;
    bool isCoolDownTime;
    bool isEnemyLocked;

    Enemy lockTarget;

    void OnEnable()
    {
        base.OnEnable();
        damage = 1;
        health = 10;
        isCoolDownTime = false;
        isEnemyLocked = false;
        coolDownTime = 1.0f;
        lockTarget = null;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isEnemyLocked)
            AcquireTargetEnemy();
        else
            CheckLockEnemyState();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (isEnemyLocked)
        {
            if (!isCoolDownTime)
            {
                Projectile instance = ProjectileFactory.pf.Get();
                instance.damage = this.damage;
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

    // Seek Enemy
    void AcquireTargetEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, attackRange, LayerMask.GetMask("Enemy"));
        if(targets.Length > 0)
        {
            float minDistance = float.MaxValue;
            float distance = 0;
            foreach(Collider collider in targets)
            {
                distance = Vector3.Distance(this.transform.localPosition, collider.gameObject.transform.localPosition);
                if ( distance <= minDistance)
                {
                    minDistance = distance;
                    lockTarget = collider.GetComponentInParent<Enemy>();
                }
            }

            //lockTarget = targets[0].GetComponentInParent<Enemy>();
            if (lockTarget == null)
                Debug.LogError("Error locking!");
            isEnemyLocked = true;
        }
    }

    void CheckLockEnemyState()
    {
        //Enemy move out of range or died (set as inactive)
        if (lockTarget == null || 
            Vector3.Distance(lockTarget.transform.localPosition, this.transform.localPosition) >= attackRange )
        {
            isEnemyLocked = false;
            lockTarget = null;
        }
    }

    public override bool Solidification()
    {
        bool allowance = base.Solidification();
        if (allowance)
        {
            if (GameManager.gm.money < 10)
            {
                allowance = false;
                Debug.Log("Not enought money!");
            } else {
                damage = damage * 3;
                attackRange = attackRange * 4;
            }
        }
        return allowance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, attackRange);
    }
}
