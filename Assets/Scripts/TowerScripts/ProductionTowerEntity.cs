using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionTowerEntity : TowerEntity
{
    int production;
    float powerRange = 10.0f;

    float coolDownTime;
    bool isCoolDownTime;

    // Update is called once per frame
    void OnEnable()
    {
        base.OnEnable();
        health = 20;
        production = 1;
        coolDownTime = 1.0f;
        isCoolDownTime = true;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isCoolDownTime)
        {
            GameManager.gm.money += production;
            isCoolDownTime = true;
        } else {
            coolDownTime -= Time.deltaTime;
            if (coolDownTime <= 0f)
            {
                coolDownTime = 1.0f;
                isCoolDownTime = false;
            }
        }
    }

    public override bool Solidification()
    {
        bool allowance = base.Solidification();
        if (allowance) 
        {
            if(GameManager.gm.money < 10)
            {
                allowance = false;
                Debug.Log("Not enought money!");
            } else {
                allowance = true;
                production = production * 3;
            }
        }
        return allowance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, powerRange);
    }

}
