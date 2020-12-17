using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTowerEntity : TowerEntity
{
    public float powerRange = 10.0f;
    public float tauntRange = 10.0f;

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
        TauntEnemy();
    }

    void OnTriggerEnter(Collider other)
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, tauntRange, LayerMask.GetMask("Enemy"));
    }
    void TauntEnemy()
    {

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
