using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollections 
{
    List<Enemy> enemies = new List<Enemy>();

    public void Add(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void GameUpdate()
    {
        for(int i=0;i<enemies.Count;i++)
        {
            if(!enemies[i].GameUpdate())
            {
                int last = enemies.Count - 1;
                enemies[i] = enemies[last];
                enemies.RemoveAt(last);
                i -= 1;
            }
        }
    }
}
