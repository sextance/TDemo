using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTowerEntity : TowerEntity
{
    public float powerRange = 35.0f;
    public float tauntRange = 35.0f;

    void OnEnable()
    {
        base.OnEnable();
        health = 50;
        powerRange = 10.0f;
    }


    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if(state != 0 && state !=2)
            TauntEnemy();
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
            if (GameManager.gm.money < 10)
            {
                allowance = false;
                Debug.Log("Not enought money!");
            } else {
                allowance = true;
                tauntRange = tauntRange * 2;
                powerRange = powerRange * 3;
            }
        }
        return allowance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, tauntRange);
    }
}
