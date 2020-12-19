using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTowerEntity : TowerEntity
{
    //Data data = Data.GlobalData;
    public float tauntRange;

    bool isActive;

    void OnEnable()
    {
        base.OnEnable();
        tauntRange = data.tauntRange;
        maxHealth = health = data.defenceTowerMaxHealth;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        isActive = cell.powered;
        if(isActive || state == 4)
        {
            if (state != 0 && state != 2)
                TauntEnemy();
        }
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (state == 3)//finish converting
        {
            if (convertDirection == 0) { state = 1; isConvertingCoolDown = true; }
            else if (convertDirection == 1) { ConvertAntiClockwise(); }
            else if (convertDirection == 2) { ConvertClockwise(); }
        }
    }

    void TauntEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, tauntRange, LayerMask.GetMask("Enemy"));
        if (targets.Length > 0)
        {
            Enemy t;
            foreach (Collider collider in targets)
            {
                t = collider.gameObject.GetComponent<Enemy>();
                if (t == null)
                    break;
                else
                {
                    t.ForceAttack(this);
                }
            }
        }
    }

    public override bool Solidification()
    {
        bool allowance = base.Solidification();
        if (allowance)
        {
            allowance = true;
            tauntRange = tauntRange * data.factorTauntRange;
        }
        return allowance;
    }

    public void ConvertAntiClockwise()
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>(), "AttackTower", healthFactor);
    }

    public void ConvertClockwise()
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>(), "ProductionTower", healthFactor);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, tauntRange);
    }
}
