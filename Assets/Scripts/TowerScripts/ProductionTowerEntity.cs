using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionTowerEntity : TowerEntity
{
    Data data = Data.GlobalData;
    int production;
    public int powerRange;

    float coolDownTime;
    bool isCoolDownTime;

    // Update is called once per frame
    void OnEnable()
    {
        base.OnEnable();
        powerRange = data.powerRange;
        maxHealth = health = data.productionTowerMaxHealth;
        production = data.production;
        coolDownTime = data.productionCoolDownTime;
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
            } else 
            {
                coolDownTime -= Time.deltaTime;
                if (coolDownTime <= 0f)
                {
                    coolDownTime = data.productionCoolDownTime;
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
            production = production * data.factorProduction;
            powerRange = powerRange * data.factorPowerRange;
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
