using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : MonoBehaviour
{
    public Data data;

    public int health;
    public int maxHealth;
    public int state; //0 constructing, 1 normal-alive, 
                      //2 converting, 3 just finish converting, 4 normal-solidificate, 
                      //5 shattered by enemy, 6 self destruction (self explosion)
    public int convertDirection; // 0 - null, 1 - left, 2 - right
    public int towerType;
    public float healthFactor;

    float constructTime;
    float convertingTime;
    float convertingCoolDonwnTime;
    bool isConstructing;
    public bool isSolidicated;
    public bool isConverting;
    public bool isConvertingFinished;
    public bool isConvertingCoolDown;

    public HexCell cell; // the cell that tower occupied
    public List<TowerEntity> linkTowers; // init if tower is solidificated
    public List<HexCell> linkCells; // init if tower is solidificated
    // float
    // bool isSolidificated;

    protected void OnEnable()
    {
        data = Data.GlobalData;
        state = 0;
        convertDirection = 0;
        constructTime = data.constructTime;
        convertingTime = data.convertingTime;
        convertingCoolDonwnTime = data.convertingCoolDonwnTime;
        isSolidicated = false;
        isConstructing = true;
        isConverting = false;
        isConvertingCoolDown = false;
        isConvertingFinished = false;
        cell = null;
        if (linkTowers.Count > 0) linkTowers.Clear();
        else if (linkTowers == null) linkTowers = new List<TowerEntity>();
        if (linkCells.Count > 0) linkCells.Clear();
        else if (linkCells == null) linkCells = new List<HexCell>();
        healthFactor = 1;
    }

    protected void FixedUpdate()
    {
        UpdateFunctionTime();
        healthFactor = (float)this.health / this.maxHealth;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (health <= 0)
        {
            if (state == 5) // shattered by enemy 
            {
                /*Reserve for animation*/
            }
            if (state == 6) // self destruction by player
            {
                /*Reserve for animation*/
            }
            GameManager.gm.DestroyTowerShape(this.gameObject.GetComponent<TowerShape>());
        }
        else
        {
            if (state == 0) // constructing
            {
                /*Reserve for animation*/
            }
            if (state == 2) // converting
            {
                /*Reserve for animation*/
            }
        }
    }

    void UpdateFunctionTime()
    {
        if (isConstructing) // if constructing, in state 0
        {
            constructTime -= Time.deltaTime;
            if (constructTime <= 0)
            {
                state = 1;
                constructTime = data.constructTime;
                isConstructing = false;
            }
        }

        if (isConverting) // if converting, in state 2
        {
            convertingTime -= Time.deltaTime;
            if (convertingTime <= 0)
            {
                state = 3;
                convertingTime = data.convertingTime;
                isConverting = false;
                isConvertingFinished = true;
            }
        }

        if (isConvertingCoolDown)
        {
            convertingCoolDonwnTime -= Time.deltaTime;
            if (convertingCoolDonwnTime <= 0)
            {
                convertingCoolDonwnTime = data.convertingCoolDonwnTime;
                isConvertingCoolDown = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        /*Reseve for audio*/
        if (health <= 0)
        {
            state = 5;
        }
    }

    public bool ConvertJudge()
    {
        bool allowance;
        if (state != 1)
        {
            Debug.Log("Current State: " + state + ", Not allow to convert");
            allowance = false;
        }
        else if (isConvertingCoolDown)
        {
            Debug.Log("Conveting in cooling down time!");
            allowance = false;
        }
        else
        {
            // condition all satisfy, start converting
            state = 2;
            isConverting = true;
            allowance = true;
        }
        return allowance;
    }

    public virtual bool Solidification()
    {
        bool allowance;
        if (state != 1)
        {
            Debug.Log("Not allow to solidificate!");
            allowance = false;
        }
        else if (GameManager.gm.money < data.solidificateCost)
        {
            allowance = false;
            Debug.Log("Not enough money!");
        } else if( !CheckSurroundings() )
        {
            Debug.Log("Not enough surrounding tower or meet solidification pattern!");
            allowance = false;
        }  else
        {
            /*change state*/
            health = this.health * data.factorHealth;
            state = 4;
            isSolidicated = true;

            /*change shape*/
            Transform t = this.gameObject.transform;
            t.localPosition = (this.linkTowers[0].gameObject.transform.localPosition +
                this.linkTowers[1].gameObject.transform.localPosition + this.gameObject.transform.localPosition) / 3;
            t.localScale *= data.factorScale;

            /*destroy surrounding towers*/
            GameManager.gm.DestroyTowerShape(this.linkTowers[0].gameObject.GetComponent<TowerShape>());
            GameManager.gm.DestroyTowerShape(this.linkTowers[1].gameObject.GetComponent<TowerShape>());

            /*remark cells*/
            this.linkCells[0].available = false;
            this.linkCells[1].available = false;

            /*return flag*/
            allowance = true;
        }
        /*Reseve for audio*/
        return allowance;
    }

    public virtual bool SelfDestruction()
    {
        bool allowance;
        if (state != 1)
        {
            allowance = false;
        } else
        {
            health = 0;
            state = 6;
            /*Reseve for audio*/
            allowance = true;
        }
        return allowance;
    }

    public virtual void SetHealth(int originHealth, int originMaxHealth)
    {
        int t = this.health;
        this.health = t * originHealth / originMaxHealth;
    }

    public bool CheckSurroundings() 
    {
        bool allowance = false;
        Collider[] surroundingTowers = Physics.OverlapSphere(this.transform.localPosition, data.cellLength, LayerMask.GetMask("Tower"));
        List<TowerEntity> tmpList = new List<TowerEntity>();
        foreach (Collider tower in surroundingTowers)
        {
            TowerEntity t = tower.gameObject.GetComponentInParent<TowerEntity>();
            if (t.state == 1 && t != this)
                tmpList.Add(t);
        }
        for (int i =0; i< tmpList.Count; i++)
        {
            for(int j = i+1; j< tmpList.Count; j++)
            {
                if ( Vector3.Distance(tmpList[i].transform.localPosition, tmpList[j].transform.localPosition)<= data.cellLength + 0.3f
                     && tmpList[i].towerType == this.towerType && tmpList[j].towerType == this.towerType
                    )
                {
                    this.linkTowers.Add(tmpList[i]); this.linkCells.Add(tmpList[i].cell);
                    this.linkTowers.Add(tmpList[j]); this.linkCells.Add(tmpList[j].cell);
                }
            }
        }

        if (this.linkTowers.Count >= 2)
            allowance = true;

        return allowance;
    }

}
