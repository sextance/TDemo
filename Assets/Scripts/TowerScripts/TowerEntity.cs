using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : MonoBehaviour
{
    public int health;
    public int state; //0 constructing, 1 normal-alive, 
                      //2 converting, 3 just finish converting, 4 normal-solidificate, 
                      //5 shattered by enemy, 6 self destruction (self explosion)

    float constructTime;
    float convertingTime;
    float convertingCoolDonwnTime;
    bool isConstructing;
    bool isConverting;
    bool isConvertingCoolDown;

    // float
    // bool isSolidificated;

    protected void OnEnable()
    {
        state = 0;
        constructTime = 2.0f;
        convertingTime = 2.0f;
        convertingCoolDonwnTime = 5.0f;
        isConstructing = true;
        isConverting = false;
        isConvertingCoolDown = false;
    }

    protected void FixedUpdate()
    {
        UpdateFunctionTime();
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
            for (int i = 0; i < GameManager.gm.enemies.Count; i++)
            {
                GameManager.gm.SearchAndGo(GameManager.gm.enemies[i]);
            }
        } else {
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
                constructTime = 2.0f;
                isConstructing = false;
            }
        }

        if (isConverting) // if converting, in state 2
        {
            convertingCoolDonwnTime -= Time.deltaTime;
            if (convertingCoolDonwnTime <= 0)
            {
                state = 3;
                isConverting = false;
                //call GameMnager
            }
        }

        if (isConvertingCoolDown)
        {
            convertingTime -= Time.deltaTime;
            if (convertingTime <= 0)
            {
                state = 3;
                isConvertingCoolDown = false;
            }
        }
    }

    void GetCommand(int commandHandle)
    {

        switch (commandHandle)
        {
            case 0:
                break;
            case 1:
                ConvertLeft();
                break;
            case 2:
                ConvertRight();
                break;
            case 3:
                Solidification();
                break;
            case 4:
                SelfDestruction();
                break;
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

    public virtual bool ConvertLeft()
    {
        bool allowance;
        if (state != 1)
        {
            Debug.Log("Not allow to convert");
            allowance = false;
        } else if (!isConvertingCoolDown) {
            Debug.Log("Conveting in cooling down time!");
            allowance = false;
        } else if (GameManager.gm.money < 10)
        {
            allowance = false;
            Debug.Log("Not enough Money");
        } else {
            allowance = true;
        }
        return allowance;
    }



    public virtual bool ConvertRight()
    {
        bool allowance;
        if (state != 1)
        {
            Debug.Log("Not allow to convert");
            allowance = false;
        } else if (!isConvertingCoolDown)
        {
            Debug.Log("Conveting in cooling down time!");
            allowance = false;
        } else if (GameManager.gm.money < 10)
        {
            allowance = false;
            Debug.Log("Not enough Money");
        } else {
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
        } else {
            health = this.health * 3;
            state = 4;
            allowance = true;
        }
        /*Reseve for audio*/
        return allowance;
    }

    public virtual bool SelfDestruction()
    {
        bool allowance;
        if ( state == 1 || state == 4)
        {
            health = 0;
            state = 6;
            /*Reseve for audio*/
            allowance = false;
        } else {
            allowance = true;
        }
        return allowance;
    }
}
