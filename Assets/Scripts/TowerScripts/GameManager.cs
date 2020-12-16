using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager gm; //Only one game manager instance is allowance
    public MapManager mapManager;

    /*public ServerMsg... serverMsg.....*/
    public TowerShapeFactory towerShapeFactory;

    //KeyCode to call event
    public KeyCode createAttackTower = KeyCode.Z;
    public KeyCode createDefenseTower = KeyCode.X;
    public KeyCode createProductionTower = KeyCode.C;
    public KeyCode destroyTower = KeyCode.D;
    public KeyCode newGameKey = KeyCode.N;

    List<TowerShape> towerShapes;

    int selectTypeHandler; // 0 - non, 1 - tower...
    TowerShape pickTower;
    Vector3 buildPosition;

    /*Reserve for other objects*/
    void Start()
    {
        /* Reserve for Sence logic*/
        towerShapes = new List<TowerShape>();
        pickTower = null;
        selectTypeHandler = 0;
        buildPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createAttackTower))
        {
            if (selectTypeHandler == 2)
                CreateTowerShape(0, buildPosition);
            else
                Debug.Log("No building region selected.");
        }
        else if (Input.GetKeyDown(createDefenseTower))
        {
            if (selectTypeHandler == 2)
            {
                CreateTowerShape(1, buildPosition);
                selectTypeHandler = 0;
            }
                
            else
                Debug.Log("No building region selected.");
        }
        else if (Input.GetKeyDown(createProductionTower))
        {
            if (selectTypeHandler == 2)
                CreateTowerShape(2, buildPosition);
            else
                Debug.Log("No building region selected.");
        }
        else if (Input.GetKeyDown(destroyTower)) 
        {
            if (selectTypeHandler == 1 && pickTower != null)
            {
                DestroyTowerShape(pickTower);
                selectTypeHandler = 0;
            }
            else
                Debug.Log("No tower selected");
        }
        //if ( (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButton(0))
            MobilePick();
    }

    void CreateTowerShape(int towerId, Vector3 position)
    {
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
     
        //Move the root of prefabs to ground
        t.localPosition = position + Vector3.up * instance.transform.localScale.y/2;
        /*Reserve for replicating tower problem*/
        towerShapes.Add(instance);
    }

    void DestroyTowerShape(TowerShape pickTower)
    {
        if (towerShapes.Count > 0)
        {
            //int index = Random.Range(0, towerShapes.Count);
            int index = towerShapes.FindIndex(a => a.SerialId == pickTower.SerialId);
            towerShapeFactory.Reclaim(towerShapes[index]);

            //Switch the index of selected and last one
            int lastIndex = towerShapes.Count - 1;
            towerShapes[index] = towerShapes[lastIndex];
            towerShapes.RemoveAt(lastIndex);
        }
        else
        {
            Debug.LogError("No tower in pools to destroy!");
        }
    }

    void MobilePick()
    {
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Untagged") //any non-interactive objects
            {
                selectTypeHandler = 0; 
            }
            else if (hit.transform.tag == "Tower")
            {
                pickTower = hit.collider.GetComponent<TowerShape>();
                Debug.Log("hit:" + hit.collider.gameObject.name);
                selectTypeHandler = 1;
            }
            else if (hit.transform.tag == "Map")
            {
                HexCell instance = hit.collider.GetComponent<HexCell>();
                buildPosition = HexCoordinates.FromCooradiante(instance.coordinates);
                Debug.Log("hit: map");
                selectTypeHandler = 2;
            }
            /*Reserve for other object*/
        }
    }
}
