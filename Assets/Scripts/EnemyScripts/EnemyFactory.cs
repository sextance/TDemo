using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : ScriptableObject
{
    [SerializeField]
    public Enemy enemyPrefab = default;
<<<<<<< Updated upstream

    [SerializeField]
    public TEnemy tEnemyPrefab = default;

    [SerializeField]
    public DpsEnemy dpsEnemyPrefab = default;

    public Enemy GetEnemy()
=======
    

    public Enemy Get()
>>>>>>> Stashed changes
    {
        Vector3 Position = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        Enemy instance = Instantiate(enemyPrefab,Position,Quaternion.identity);
        instance.OriginFactory = this;
        return instance;
    }

<<<<<<< Updated upstream
    public TEnemy GetTEnemy()
    {
        Vector3 Position = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        TEnemy instance = Instantiate(tEnemyPrefab, Position, Quaternion.identity);
        instance.OriginFactory = this;
        return instance;
    }

    public DpsEnemy GetDpsEnemy()
    {
        Vector3 Position = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        DpsEnemy instance = Instantiate(dpsEnemyPrefab, Position, Quaternion.identity);
        instance.OriginFactory = this;
        return instance;
    }

=======
>>>>>>> Stashed changes
    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy == this, "Wrong Factory Reclaimed!");
        Destroy(enemy.gameObject);
    }
<<<<<<< Updated upstream

    public void Reclaim(TEnemy tEnemy)
    {
        Debug.Assert(tEnemy == this, "Wrong Factory Reclaimed!");
        Destroy(tEnemy.gameObject);
    }

    public void Reclaim(DpsEnemy dpsEnemy)
    {
        Debug.Assert(dpsEnemy == this, "Wrong Factory Reclaimed!");
        Destroy(dpsEnemy.gameObject);
    }
=======
>>>>>>> Stashed changes
}
