using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : ScriptableObject
{
    [SerializeField]
    public Enemy enemyPrefab = default;

    public Enemy Get()
    {
        Vector3 Position = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        Enemy instance = Instantiate(enemyPrefab,Position,Quaternion.identity);
        instance.OriginFactory = this;
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy == this, "Wrong Factory Reclaimed!");
        Destroy(enemy.gameObject);
    }
}
