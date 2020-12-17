using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    EnemyFactory enemyFactory;

    [SerializeField]
    float spawnSpeed = 1f;

    [SerializeField]
    float time = 0f;

    [SerializeField]
    float enemySpawnSpeed1 = 0f;

    [SerializeField]
    float enemySpawnSpeed2 = 0f;

    [SerializeField]
    float timeLimit = 0f;

    EnemyCollections enemies = new EnemyCollections();

    List<Vector3> Position = new List<Vector3>();


    void Start()
    {
        
    }

    void Update()
    {
        TimeToSpawn();
        enemies.GameUpdate();

    }

    void SpawnEnemy(float enemySpawnSpeed)//生成敌人
    {
        spawnSpeed += spawnSpeed * Time.deltaTime;
        while (spawnSpeed >= enemySpawnSpeed)
        {
            spawnSpeed -= enemySpawnSpeed;
            Enemy enemy = enemyFactory.Get();
            enemies.Add(enemy);
        }
    }

    void TimeToSpawn()//随时间限制改变怪物生成速度
    {
        time += Time.deltaTime;
        SpawnEnemy(enemySpawnSpeed1);
        if(time > timeLimit)
        {
            SpawnEnemy(enemySpawnSpeed2);
        }
    }

    void SpawnEnemyAround()//获取边缘坐标，生成敌人
    {
        for (int i = 0; i < 8; i += 7)
        {
            for (int j = 0; j < 12; j += 11)
            {
                Position[i] = this.transform.position;
                Enemy enemy = Instantiate(enemyFactory.enemyPrefab, Position[i], Quaternion.identity);
            }
        }
    }

    void TimeToSpawnAround()
    {
        time += Time.deltaTime;
        if(time > 120f)
        {
            for(int i= 0; i < 5; i++)
            {
                spawnSpeed += spawnSpeed * Time.deltaTime;
                if(spawnSpeed >= 1f)
                {
                    SpawnEnemyAround();
                    spawnSpeed -= 1f;
                }
            }
        }
        else if(time > 300f)
        {
            SpawnEnemyAround();
        }
    }
}
