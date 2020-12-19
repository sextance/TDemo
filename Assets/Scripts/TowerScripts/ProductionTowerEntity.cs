using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionTowerEntity : TowerEntity
{
    //Data data = Data.GlobalData;
    List<HexCell> linkCells;
    int production;
    public float powerRange;

    float coolDownTime;
    bool isCoolDownTime;
    bool isLinkCreated;

    // Update is called once per frame
    void OnEnable()
    {
        base.OnEnable();
        towerType = 2;
        powerRange = data.powerRange;
        maxHealth = health = data.productionTowerMaxHealth;
        production = data.production;
        coolDownTime = data.productionCoolDownTime;
        isCoolDownTime = true; // when tower finished building, production shall be in CD time
        isLinkCreated = false;

        if (linkCells == null) linkCells = new List<HexCell>();
    }

    void OnDisable()
    {
        BreakPowerLinkToCells();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (state == 1 || state == 4)
        {
            if( !isLinkCreated )
            {
                CreatePowerLinkToCells();
                isLinkCreated = true;
            }
            
            if (!isCoolDownTime)
            {
                GameManager.gm.money += production;
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
            isLinkCreated = false; // renew links
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

    public void CreatePowerLinkToCells()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, powerRange, LayerMask.GetMask("Map"));
       if (targets.Length > 0)
        {
            foreach(Collider target in targets)
            {
                HexCell cell = target.gameObject.GetComponent<HexCell>();
                // Avoid repeat addition
                if ( cell.powerLinks.Find(o => o.cell == this.cell) == null)
                {
                    cell.powerLinks.Add(this);
                    linkCells.Add(cell);
                }
            }
        }
    }

    public void BreakPowerLinkToCells()
    {
        if ( linkCells.Count > 0)
        {
            foreach (HexCell cell in linkCells)
            {
                if (cell.powerLinks.Find(o => o == this)) 
                {
                    cell.powerLinks.Remove(this);
                } else {
                    Debug.LogWarning("Link lost!");
                }
            }
            linkCells.Clear();
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.1f;
        Gizmos.DrawWireSphere(position, powerRange);
    }

}
