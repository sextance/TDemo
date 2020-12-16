using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*public ServerMsg... serverMsg.....*/
    public TowerShapeFactory towerShapeFactory;


    //KeyCode to call event
    public KeyCode createAttackTower = KeyCode.Z;
    public KeyCode createDefenseTower = KeyCode.X;
    public KeyCode createProductionTower = KeyCode.C;
    public KeyCode destroyTower = KeyCode.D;
    public KeyCode newGameKey = KeyCode.N;

    List<TowerShape> towerShapes;
    void Start()
    {
        /* Reserve for Sence logic*/

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createAttackTower) ) //需要交互 isZoneSelected
            CreateTowerShape(0);
        if (Input.GetKeyDown(createDefenseTower)) //需要交互 isZoneSelected
            CreateTowerShape(1);
        if (Input.GetKeyDown(createProductionTower)) //需要交互 isZoneSelected
            CreateTowerShape(2);
        if (Input.GetKeyDown(destroyTower)) //需要交互 isTowerSelected
            DestroyTowerShape();
    }

    void CreateTowerShape(int towerId)
    {
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
        /*Reserve*/
        t.localPosition = Random.insideUnitSphere * 5f;

        towerShapes.Add(instance);
    }

    void DestroyTowerShape()
    {
        if(towerShapes.Count > 0)
        {
            int index = Random.Range(0, towerShapes.Count);
            towerShapeFactory.Reclaim(towerShapes[index]);

            //Switch the index of selected and last one
            int lastIndex = towerShapes.Count - 1;
            towerShapes[index] = towerShapes[lastIndex];
            towerShapes.RemoveAt(lastIndex);
        } else {
            Debug.LogError("No tower in pools to destroy!");
        }
    }

}
