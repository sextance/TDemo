using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEntity : MonoBehaviour
{
    public int health;

    int state; // 0 alive, 1 solidificate, 2 dismantle, 3 shatter
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if( health <= 0)
        {
            if (state == 2) // be dismantled by player
            {
                /*Reserve for animation*/
            }
            if (state == 3) // be shattered by enemy
            {
                /*Reserve for animation*/
            }
            GameManager.gm.towerShapeFactory.Reclaim(this.gameObject.GetComponent<TowerShape>());
           
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        /*Reseve for audio*/
        if (health <= 0)
        {
            state = 2;
        }
    }

    public void DismantleTower(int damage)
    {
        health = 0;
        state = 3;
        /*Reseve for audio*/
    }

}
