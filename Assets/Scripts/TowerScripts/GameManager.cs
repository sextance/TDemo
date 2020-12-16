﻿using System.Collections;
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
    public KeyCode solidificateTower = KeyCode.S;
    public KeyCode destroyTower = KeyCode.D;
    public KeyCode newGameKey = KeyCode.N;

    List<TowerShape> towerShapes;

    int selectTypeHandler; // 0 - non, 1 - tower...
    TowerShape pickTower;
    HexCell pickRegion;
    Vector3 buildPosition;
    GameObject selectedObject;
    Transform highLightObj;
    Material previousMaterial;
    Material selectedMaterial;

    /*Reserve for other objects*/
    void Start()
    {
        /* Reserve for Sence logic*/
        towerShapes = new List<TowerShape>();
        pickTower = null;
        pickRegion = null;
        selectTypeHandler = 0;
        buildPosition = Vector3.zero;

        highLightObj = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<Transform>();
        Destroy(highLightObj.GetComponent<Collider>());
        highLightObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/MapMaterials/Silhouette");
        highLightObj.gameObject.SetActive(false);
        selectedMaterial = Resources.Load<Material>("Materials/MapMaterials/Glow");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createAttackTower))
        {
            if (selectTypeHandler == 2) 
            {
                CreateTowerShape(0, pickRegion);
                selectTypeHandler = 0;
            }
            else
                Debug.Log("No building region selected.");
        }
        else if (Input.GetKeyDown(createDefenseTower))
        {
            if (selectTypeHandler == 2)
            {
                CreateTowerShape(1, pickRegion);
                selectTypeHandler = 0;
            } 
            else
                Debug.Log("No building region selected.");
        } 
        else if (Input.GetKeyDown(createProductionTower))
        {
            if (selectTypeHandler == 2)
            {
                CreateTowerShape(2, pickRegion);
                selectTypeHandler = 0;
            }
            else
                Debug.Log("No building region selected.");
        } 
        else if (Input.GetKeyDown(solidificateTower))
        {
            if (selectTypeHandler == 1 && pickTower != null)
            {
                SolidificateTowerShape(pickTower);
                //selectTypeHandler = 0;
            }
            else
                Debug.Log("No tower selected.");
        }
        else if (Input.GetKeyDown(destroyTower)) 
        {
            if (selectTypeHandler == 1 && pickTower != null)
            {
                DestroyTowerShape(pickTower);
                selectTypeHandler = 0;
            }
            else
                Debug.Log("No tower selected.");
        }
        //if ( (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButton(0))
            MobilePick();
    }

    void CreateTowerShape(int towerId, HexCell buildRegion)
    {
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
        Vector3 buildPosition = HexCoordinates.FromCooradiante(buildRegion.coordinates);

        //Move the root of prefabs to ground
        instance.coordinates = buildRegion.coordinates;
        t.localPosition = buildPosition + Vector3.up * instance.transform.localScale.y/2;
        if (t.localScale.x >= 10.0f)
            t.localScale /= 1.5f;

        buildRegion.available = false;
        towerShapes.Add(instance);
    }

    void DestroyTowerShape(TowerShape pickTower)
    {   
        if (towerShapes.Count > 0)
        {
            //int index = Random.Range(0, towerShapes.Count);
            int index = towerShapes.FindIndex(a => a.SerialId == pickTower.SerialId);
            towerShapeFactory.Reclaim(towerShapes[index]);

            mapManager.hexGrid.cells[pickTower.coordinates.X + pickTower.coordinates.Y * 12].available = true;
            //Switch the index of selected and last one
            int lastIndex = towerShapes.Count - 1;
            towerShapes[index] = towerShapes[lastIndex];
            towerShapes.RemoveAt(lastIndex);
            //Disable selected outline
            highLightObj.gameObject.SetActive(false);
            selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
            selectedObject = null;
        } else {
            Debug.LogError("No tower in pools to destroy!");
        }
    }

    void SolidificateTowerShape(TowerShape pickTower)
    {
        if (pickTower != null && pickTower.IsSolidificated == false)
        {
            pickTower.IsSolidificated = false;
            Transform t = pickTower.transform;
            t.localScale *= 1.5f;
        } else
            Debug.Log("Tower already solidificated");
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
            } else if (hit.transform.tag == "Tower")
            {
                pickTower = hit.collider.GetComponent<TowerShape>();
                Debug.Log("hit:" + hit.collider.gameObject.name);
                selectTypeHandler = 1;
                //Selected effect
                if(!selectedObject || selectedObject != hit.transform)
                {
                    if(selectedObject)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
                    }
                    selectedObject = hit.transform.gameObject;
                    previousMaterial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                    highLightObj.position = hit.transform.position;
                    highLightObj.rotation = hit.transform.rotation;
                    highLightObj.localScale = hit.transform.localScale;
                    highLightObj.GetComponent<MeshFilter>().mesh = hit.collider.GetComponent<MeshFilter>().mesh;
                    highLightObj.gameObject.SetActive(true);
                }
            } else if (hit.transform.tag == "Map")
            {
                pickRegion = hit.collider.GetComponent<HexCell>();
                if (pickRegion.available == true)
                {
                    //buildPosition = HexCoordinates.FromCooradiante(instance.coordinates);
                    Debug.Log("hit: map");
                    selectTypeHandler = 2;
                    if(!selectedObject || selectedObject.name != hit.transform.name)
                {
                    if(selectedObject)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
                    }
                    selectedObject = hit.transform.gameObject;
                    previousMaterial = selectedObject.GetComponent<MeshRenderer>().material;
                    selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial;
                    highLightObj.position = hit.transform.position;
                    highLightObj.rotation = hit.transform.rotation;
                    highLightObj.localScale = hit.transform.localScale;
                    highLightObj.GetComponent<MeshFilter>().mesh = hit.collider.GetComponent<MeshFilter>().mesh;
                    highLightObj.gameObject.SetActive(true);
                }
                }
                else
                    Debug.Log("Region already accupied by a tower!");
            }
            /*Reserve for other object*/
        }
        else
        {
            if(selectedObject)
            {   
                highLightObj.gameObject.SetActive(false);
                selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
                selectedObject = null;
            }
        }
    }
}
