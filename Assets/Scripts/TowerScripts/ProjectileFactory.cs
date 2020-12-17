using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ProjectileFactory : ScriptableObject
{
    static public ProjectileFactory pf;
    [SerializeField]
    Projectile[] prefabs;

    [SerializeField]
    bool recycle;
    List<Projectile>[] pools;

    int serialNumber;

    void Awake()
    {
        pf = this;
        recycle = true;
        serialNumber = 0;
    }

    public Projectile Get(int shapeId = 0)
    {
        Projectile instance;

        if (recycle)
        {
            if (pools == null)
                CreatePools();

            //Recycle the last object of pool to enssure consistency of list
            List<Projectile> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            } else {
                instance = Instantiate(prefabs[shapeId]);
                instance.ShapeId = shapeId;
                instance.SerialId = serialNumber++;
            }
        } else {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
            instance.SerialId = serialNumber++;
        }

        //instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public void ReClaim(Projectile projectileToRecycle)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            pools[projectileToRecycle.ShapeId].Add(projectileToRecycle);
            projectileToRecycle.gameObject.SetActive(false);
        } else {
            Destroy(projectileToRecycle.gameObject);
        }
    }

    void CreatePools()
    {
        pools = new List<Projectile>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
            pools[i] = new List<Projectile>();
    }

}
