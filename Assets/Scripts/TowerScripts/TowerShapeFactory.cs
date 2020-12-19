using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[CreateAssetMenu]
public class TowerShapeFactory : ScriptableObject
{
    public static TowerShapeFactory tsf;

    [SerializeField]
    TowerShape[] prefabs;

    //The order of elements is Attack, Defense, Production
    [SerializeField]
    Material[] materials;

    [SerializeField]
    bool recycle;

    public List<TowerShape>[] pools;

    static int serialNumber;

   // Scene poolScene;

    //Get new tower from factory
    public TowerShape Get(int shapeId = 0, int materialId = 0)
    {
        TowerShape instance;

        if (shapeId != materialId)
            Debug.LogError("Different material and tower id input!");

        if (recycle)
        {
            if (pools == null)
                CreatePools();

            //Recycle the last object of pool to enssure consistency of list
            List<TowerShape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if(lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
                /* ... */
            } else {
                instance = Instantiate(prefabs[shapeId]);
                instance.ShapeId = shapeId;
                instance.SerialId = serialNumber++;
                //SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
            }
        } else {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
            instance.SerialId = serialNumber++;
        }
        
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    //Put destroyed tower back to factory
    public void Reclaim(TowerShape shapeToRecycle)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);

            if( shapeToRecycle.gameObject.GetComponent<TowerEntity>().isSolidicated == true)
            {
                Transform t = shapeToRecycle.transform;
                t.localScale /= Data.GlobalData.factorScale;
            }

            shapeToRecycle.gameObject.SetActive(false);
        } else {
            Destroy(shapeToRecycle.gameObject);
        }
    }

    void CreatePools()
    {
        pools = new List<TowerShape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
            pools[i] = new List<TowerShape>();
    }

    void Awake()
    {
        tsf = this;
        recycle = true;
        serialNumber = 0;
    }
}
