using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
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

    Enemy enemy;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnDpsEnemy();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnTEnemy();
        }
        TimeToSpawn();
        enemies.GameUpdate();
        
    }

    void SpawnCommonEnemy(float enemySpawnSpeed)//生成敌人
    {
        spawnSpeed += spawnSpeed * Time.deltaTime;
        while (spawnSpeed >= enemySpawnSpeed)
        {
            spawnSpeed -= enemySpawnSpeed;
            Enemy enemy = enemyFactory.GetEnemy();
            enemies.Add(enemy);
            if(TowerShapeFactory.tsf.pools == null)
            {
                return;
            }
            enemy.Search(TowerShapeFactory.tsf.pools);
        }
    }

    void SpawnTEnemy()
    {
        TEnemy tEnemy = enemyFactory.GetTEnemy();
        enemies.Add(tEnemy);
        if (TowerShapeFactory.tsf.pools == null)
        {
            return;
        }
        enemy.Search(TowerShapeFactory.tsf.pools);
    }

    void SpawnDpsEnemy()
    {
        DpsEnemy dpsEnemy = enemyFactory.GetDpsEnemy();
        enemies.Add(dpsEnemy);
        if (TowerShapeFactory.tsf.pools == null)
        {
            return;
        }
        enemy.Search(TowerShapeFactory.tsf.pools);
    }

    void TimeToSpawn()//随时间限制改变怪物生成速度
    {
        time += Time.deltaTime;
        SpawnCommonEnemy(enemySpawnSpeed1);
        if(time > timeLimit)
        {
            SpawnCommonEnemy(enemySpawnSpeed2);
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
