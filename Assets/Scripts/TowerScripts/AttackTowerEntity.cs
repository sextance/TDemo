using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerEntity : TowerEntity
{
    //Data data = Data.GlobalData;
    //ATK range = 3
    public float attackRange;

    public Projectile projectile;

    int damage;

    float coolDownTime;
    bool isActive;
    bool isTauted;
    bool isCoolDownTime;
    bool isEnemyLocked;

    Enemy lockTarget;

    void OnEnable()
    {
        base.OnEnable();
        towerType = 0;
        damage = data.damage;
        maxHealth = health = data.attackTowerMaxHealth;
        attackRange = data.attackRange;
        isTauted = false;
        isCoolDownTime = false;
        isEnemyLocked = false;
        coolDownTime = data.attackCoolDownTime;
        lockTarget = null;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        isActive = cell.powered;
        if (isActive || state == 4) // will function after solidificated even not powered
        {
            if (state == 1 || state == 4)
            {
                if (!isEnemyLocked)
                    AcquireTargetEnemy();
                else
                    CheckLockEnemyState();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (state == 1 || state == 4 ) // only enabled when normal or normal updated
        {
            if (isActive || state == 4) 
            {
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
                    }
                    else
                    {
                        coolDownTime -= Time.deltaTime;
                        if (coolDownTime <= 0f)
                        {
                            coolDownTime = data.attackCoolDownTime;
                            isCoolDownTime = false;
                        }
                    }
                }
            } 
        } else if (state == 3)//finish converting
        {
            if (convertDirection == 0) { state = 1; isConvertingCoolDown = true; }
            else if (convertDirection == 1) { ConvertAntiClockwise(); }
            else if (convertDirection == 2) { ConvertClockwise(); }
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
            if (isTauted == true)
                isTauted = false;
        }
    }

    public override bool Solidification()
    {
        bool allowance = base.Solidification();
        if (allowance)
        {
            damage = damage *  data.factorDamage;
            attackRange = attackRange * data.factorAtkRange;
        }
        return allowance;
    }

    public void BeTaunted(TEnemy enemy)
    {
        if(isTauted == false)
        {
            isTauted = true;
            isEnemyLocked = true;
            lockTarget = enemy;
        }
    }

    public void ConvertAntiClockwise() // left arrow
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>() ,"ProductionTower", healthFactor);
    }

    public void ConvertClockwise() // right arrow
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>(), "DefenceTower", healthFactor);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, attackRange);
    }
}
