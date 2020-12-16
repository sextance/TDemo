using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : TowerShape
{
    public int health;

    public int state; // 0 alive, 1 solidificate, 2 dismantle, 3 shatter
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( health <= 0)
        {
            if (state == 2)
                /*Reserve for animation*/
                GameManager.gm.towerShapeFactory.Reclaim(this);
            if (state == 3)
                /*Reserve for animation*/
                GameManager.gm.towerShapeFactory.Reclaim(this);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        /*Reseve for audio*/
        if(health <= 0)
        {
            state = 2;
        }
    }
}
