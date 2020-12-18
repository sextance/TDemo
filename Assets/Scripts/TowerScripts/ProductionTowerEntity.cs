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
        maxHealth = health = 20;
        production = 1;
        coolDownTime = 1.0f;
        isCoolDownTime = true;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (state == 1 || state == 4)
        {
            if (!isCoolDownTime)
            {
                GameManager.gm.money += production;
                Debug.Log("Money: "+ GameManager.gm.money);
                isCoolDownTime = true;
            }
            else
            {
                coolDownTime -= Time.deltaTime;
                if (coolDownTime <= 0f)
                {
                    coolDownTime = 1.0f;
                    isCoolDownTime = false;
                }
            }
        } else if (state == 3) //finish converting
        {
            if (convertDirection == 0) { state = 1; isConvertingCoolDown = true; }
            else if (convertDirection == 1) { ConvertAntiClockwise(); }
            else if (convertDirection == 2) { ConvertClockwise(); }
        }
    }

    public override bool Solidification()
    {
        bool allowance = base.Solidification();
        if (allowance) 
        {
            allowance = true;
            production = production * 3;
        }
        return allowance;
    }

    public void ConvertAntiClockwise()
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>(), "DefenceTower", healthFactor);
    }

    public void ConvertClockwise()
    {
        if (isConvertingFinished)
            GameManager.gm.ConvertTo(this.gameObject.GetComponent<TowerShape>(), "AttackTower", healthFactor);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, powerRange);
    }

}
