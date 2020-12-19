using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm; //Only one game manager instance is allowance

    /*public ServerMsg... serverMsg.....*/
    /*Object Factory*/
    public Data data;
    public MapManager mapManager;
    public TowerShapeFactory towerShapeFactory;
    public ProjectileFactory projectileFactory;
    public EnemyFactory enemyFactory = default;

    //KeyCode to call event
    public KeyCode createAttackTower = KeyCode.Z;
    public KeyCode createDefenseTower = KeyCode.X;
    public KeyCode createProductionTower = KeyCode.C;
    public KeyCode solidificateTower = KeyCode.S;
    public KeyCode destroyTower = KeyCode.D;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode antiClockwiseConvert = KeyCode.LeftArrow;
    public KeyCode clockwiseConvert = KeyCode.RightArrow;

    public List<TowerShape> towerShapes;
    public List<Enemy> enemies;
    public Text costText;
    public GameObject mapButton;
    public GameObject towerButton1;
    public GameObject towerButton2;
    public GameObject camera;
    bool inEnemyScene;
    float enemySceneTimer;
    float spawnSpeed = 1f;
    float datas1 = Data.GlobalData.s1;
    float datas2 = Data.GlobalData.s2;
    float datas3 = Data.GlobalData.s3;

    int selectTypeHandler; // 0 - non, 1 - tower...
    TowerShape pickTower;
    HexCell pickRegion;
    Vector3 buildPosition;
    GameObject selectedObject;
    Transform highLightObj;
    Material previousMaterial;
    Material selectedMaterial;

    
    //private Animator anim;

    /*Game Datas*/
    public int money;
    /*Reserve for other objects*/
    void Start()
    {
        /* Reserve for Sence logic*/
        gm = this;
        data = Data.GlobalData;
        towerShapes = new List<TowerShape>();
        enemies = new List<Enemy>();
        pickTower = null;
        pickRegion = null;
        selectTypeHandler = 0;
        buildPosition = Vector3.zero;

        highLightObj = GameObject.CreatePrimitive(PrimitiveType.Sphere).GetComponent<Transform>();
        Destroy(highLightObj.GetComponent<Collider>());
        highLightObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/MapMaterials/Silhouette");
        highLightObj.gameObject.SetActive(false);
        selectedMaterial = Resources.Load<Material>("Materials/MapMaterials/Glow");

        mapButton.SetActive(false);
        towerButton1.SetActive(false);
        towerButton2.SetActive(false);
        
        /*Game Data*/
        money = data.startMoney; //start money for player
        enemySceneTimer = 0f;
    }

    public void CAT()
    {
        if (selectTypeHandler == 2)
            {
                if(money >= data.buildCost)
                {
                    CreateTowerShape(0, pickRegion, 0);
                    selectTypeHandler = 0;
                    this.money -= data.buildCost;
                }  else {
                    Debug.Log("Not enough money.");
                }
            }
            else
                Debug.Log("No building region selected.");
    }

    public void CDT()
    {
        if (selectTypeHandler == 2)
            {
                if (money >= data.buildCost)
                {
                    CreateTowerShape(1, pickRegion, 0);
                    selectTypeHandler = 0;
                    this.money -= data.buildCost;
                    for (int i = 0; i < enemies.Count; i++)
                        SearchAndGo(enemies[i]);
                }  else {
                    Debug.Log("Not enough money.");
                }
            } 
            else
                Debug.Log("No building region selected.");
    }

    public void CPT()
    {
        if (selectTypeHandler == 2)
        {
            if (money >= data.buildCost)
            {
                CreateTowerShape(2, pickRegion, 0);
                selectTypeHandler = 0;
                this.money -= data.buildCost;
            } else 
                Debug.Log("Not enough money to build!");
        }
        else
            Debug.Log("No building region selected.");
    }

    public void SAT()
    {
        if (selectTypeHandler == 1 && pickTower != null)
        {
            SolidificateTowerShape(pickTower);
            //selectTypeHandler = 0;
        }
        else
            Debug.Log("No tower selected.");
    }

    public void DT(){
        if (selectTypeHandler == 1 && pickTower != null)
        {
            TowerEntity t = pickTower.gameObject.GetComponent<TowerEntity>();
            if( t.SelfDestruction() == true)
            {
                if (this.money > data.deconstructionCost)
                {
                    DestroyTowerShape(pickTower);
                    selectTypeHandler = 0;
                    this.money -= data.deconstructionCost;
                } else
                    Debug.Log("Not enough money to deconstruct!");
            } else
                Debug.Log("Not allowed to deconstruct!");
        }
        else
            Debug.Log("No tower selected.");
    }

    public void ACC(){
        if (selectTypeHandler == 1 && pickTower != null)
        {
            TowerEntity pickTowerEntity = pickTower.gameObject.GetComponent<TowerEntity>();
            Convert(pickTowerEntity, 1);
            //bool allwance = Convert(pickTowerEntity, 1);
            //if (allwance)
            //    selectTypeHandler = 0;
        } else
            Debug.Log("No tower selected.");
    }

    public void CC(){
        if (selectTypeHandler == 1 && pickTower != null)
        {
            TowerEntity pickTowerEntity = pickTower.gameObject.GetComponent<TowerEntity>();
            Convert(pickTowerEntity, 2);
            //bool allwance = Convert(pickTowerEntity, 2);
            //if (allwance)
            //    selectTypeHandler = 0;
        } else
            Debug.Log("No tower selected.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createAttackTower))
        {
            CAT();
        }
        else if (Input.GetKeyDown(createDefenseTower))
        {
            CDT();
        }
        else if (Input.GetKeyDown(createProductionTower))
        {
            CPT();
        }
        else if (Input.GetKeyDown(solidificateTower))
        {
            SAT();
        }
        else if (Input.GetKeyDown(destroyTower))
        {
            DT();
        }
        else if (Input.GetKeyDown(antiClockwiseConvert))
        {
            ACC();
        }
        else if (Input.GetKeyDown(clockwiseConvert))
        {
            CC();
        }

        //if ( (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButton(0))
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                MobilePick();

        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnDpsEnemy();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnTEnemy();
        }
        TimeToSpawnAround();
        TimeToSpawn();
        GameUpdate();
        costText.text = "COST: " + money.ToString();
        if(inEnemyScene){
            camera.transform.position = new Vector3(1300,150,10);
            enemySceneTimer += Time.deltaTime;
            if(enemySceneTimer >= 2f){
                camera.transform.position = new Vector3(100,150,10);
                inEnemyScene = false;
                enemySceneTimer = 0f;
            }
        }
    }

    void CreateTowerShape(int towerId, HexCell buildRegion, int initState, float healthFactor = 1)
    {
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
        TowerEntity e = instance.gameObject.GetComponent<TowerEntity>();
        e.health = (int)(e.maxHealth * healthFactor);

        if (buildRegion == null)
            Debug.LogError("No cell found?");
        else {
            e.cell = buildRegion;
            buildPosition = HexCoordinates.FromCoordinate(buildRegion.coordinates);
            instance.coordinates = buildRegion.coordinates;
            buildRegion.available = false;
        }

        //Care if move the root of prefabs to ground
        t.localPosition = buildPosition;
        if (t.localScale.y >= 15.0f)
            t.localScale /= data.factorScale;

        //Create link if production tower
        //if(towerId == 2)
        //      mapManager.hexGrid.CreatePowerLinkToCell(
        //       instance.gameObject.GetComponent<ProductionTowerEntity>());
        
        towerShapes.Add(instance);

        instance.GetComponent<TowerEntity>().state = initState;

        for (int i = 0; i < enemies.Count; i++)
            SearchAndGo(enemies[i]);
    }

    public void DestroyTowerShape(TowerShape pickTower)
    {
        if (towerShapes.Count > 0)
        {
            //int index = Random.Range(0, towerShapes.Count);
            int index = towerShapes.FindIndex(a => a.SerialId == pickTower.SerialId);
            //
            foreach(Enemy enemy in enemies)
            {
                if (pickTower.GetComponent<DefenceTowerEntity>() != null)
                {
                    enemy.isLock = false;
                }
            }
            //
            towerShapeFactory.Reclaim(towerShapes[index]);
            mapManager.hexGrid.cells[pickTower.coordinates.X + pickTower.coordinates.Y * 12].available = true;
            //Switch the index of selected and last one
            int lastIndex = towerShapes.Count - 1;
            towerShapes[index] = towerShapes[lastIndex];
            towerShapes.RemoveAt(lastIndex);

            //Disable selected outline
            //highLightObj.gameObject.SetActive(false);
            //selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
            //selectedObject = null;
            highLightObj.gameObject.SetActive(false);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        } else
            Debug.Log("No tower in pools to destroy!");
    }

    void SolidificateTowerShape(TowerShape pickTower)
    {
        if (pickTower != null && pickTower.IsSolidificated == false)
        {
            bool allowance = false;

            if(pickTower.gameObject.GetComponent<AttackTowerEntity>() != null)
            {
                AttackTowerEntity tmp = pickTower.gameObject.GetComponent<AttackTowerEntity>();
                allowance = tmp.Solidification();
            }  else if (pickTower.gameObject.GetComponent<DefenceTowerEntity>() != null)
            {
                DefenceTowerEntity tmp = pickTower.gameObject.GetComponent<DefenceTowerEntity>();
                allowance = tmp.Solidification();
            } else if (pickTower.gameObject.GetComponent<ProductionTowerEntity>() != null)
            {
                ProductionTowerEntity tmp = pickTower.gameObject.GetComponent<ProductionTowerEntity>();
                allowance = tmp.Solidification();
            }
            if (allowance)
            {
                pickTower.IsSolidificated = false;
                this.money -= data.solidificateCost;
            }
        }
        else
            Debug.Log("Tower already solidificreate AttackTowered");
    }

    void MobilePick()
    {
        RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mapButton.SetActive(false);
        towerButton1.SetActive(false);
        towerButton2.SetActive(false);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Untagged") //any non-interactive objects
            {
                selectTypeHandler = 0;
            }
            else if (hit.transform.tag == "Tower")
            {
                pickTower = hit.transform.parent.GetComponent<TowerShape>();
                Debug.Log("hit:" + hit.collider.gameObject.name);
                selectTypeHandler = 1;
                //Selected effect
                if (!selectedObject || selectedObject != hit.transform)
                {
                    if (selectedObject)
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
                towerButton1.SetActive(true);
                towerButton2.SetActive(false);
            }
            else if (hit.transform.tag == "Map")
            {
                pickRegion = hit.collider.GetComponent<HexCell>();
                Debug.Log("hit:" + hit.collider.gameObject.name);
                if (pickRegion.available == true)
                {
                    //buildPosition = HexCoordinates.FromCoordinate(instance.coordinates);
                    Debug.Log("hit: map");
                    selectTypeHandler = 2;
                    if (!selectedObject || selectedObject.name != hit.transform.name)
                    {
                        if (selectedObject)
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
                    mapButton.SetActive(true);
                }
                else
                    Debug.Log("Region already accupied by a tower!");
            }
            /*Reserve for other object*/
        }
        else
        {
            Debug.Log("hit: nothing");
            if (selectedObject)
            {
                highLightObj.gameObject.SetActive(false);
                selectedObject.GetComponent<MeshRenderer>().material = previousMaterial;
                selectedObject = null;
            }
        }
    }

    //leftdirection = 1, right direction = 2;
    bool Convert(TowerEntity pickTowerEntity, int direction)
    {
        //Get allowance
        bool allowance = pickTowerEntity.ConvertJudge(); 
        if (allowance)
        {
            if (pickTowerEntity.gameObject.GetComponent<AttackTowerEntity>() != null)
            {
                AttackTowerEntity t = pickTowerEntity.gameObject.GetComponent<AttackTowerEntity>();
                t.convertDirection = direction;
            } else if(pickTowerEntity.gameObject.GetComponent<DefenceTowerEntity>() != null)
            {
                DefenceTowerEntity t = pickTowerEntity.gameObject.GetComponent<DefenceTowerEntity>();
                t.convertDirection = direction;
            }
            else if (pickTowerEntity.gameObject.GetComponent<ProductionTowerEntity>() != null)
            {
                ProductionTowerEntity t = pickTowerEntity.gameObject.GetComponent<ProductionTowerEntity>();
                t.convertDirection = direction;
            }
        }
        return allowance;
    }

    public void ConvertTo(TowerShape origin, string targetName, float healthFactor)
    {
        if (origin == null)
            Debug.LogError("No Tower found to convert!");

        HexCell cell = origin.gameObject.GetComponent<TowerEntity>().cell;
        if (cell == null) Debug.LogError("?");
        DestroyTowerShape(origin);

        if (targetName == "AttackTower")
        {
            CreateTowerShape(0, cell, 3, healthFactor);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        } else if (targetName == "DefenceTower")
        {
            CreateTowerShape(1, cell, 3, healthFactor);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        } else if (targetName == "ProductionTower")
        {
            CreateTowerShape(2, cell, 3, healthFactor);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        }
    }

    void SpawnCommonEnemy(float enemySpawnSpeed)//生成敌人
    {
        spawnSpeed += spawnSpeed * Time.deltaTime;
        while (spawnSpeed >= enemySpawnSpeed)
        {
            spawnSpeed -= enemySpawnSpeed;
            Enemy enemy = enemyFactory.GetEnemy();
            enemies.Add(enemy);
            SearchAndGo(enemy);
        }
    }

    void SpawnTEnemy()
    {
        TEnemy tEnemy = enemyFactory.GetTEnemy();
        enemies.Add(tEnemy);
        SearchAndGo(tEnemy);
    }

    void SpawnDpsEnemy()
    {
        DpsEnemy dpsEnemy = enemyFactory.GetDpsEnemy();
        enemies.Add(dpsEnemy);
        SearchAndGo(dpsEnemy);
    }


    public void SearchAndGo(Enemy enemy)
    {
        if (enemy.isLock)
        {
            return;
        }
        else
        {
            VectorAndNum se = new VectorAndNum();
            se = Search(towerShapes, enemy);
            enemy.navMesh.SetDestination(se.point);
            enemy.anim.SetInteger("CommonEnemy", 1);
            if (se.num < 0)
            {
                return;
            }
        }
    }

    public struct VectorAndNum
    {
        public Vector3 point;
        public int num;
        public int hasPool;
    }

    void TimeToSpawn()//随时间限制改变怪物生成速度
    {
        SpawnCommonEnemy(data.enemySpawnSpeed1);
        if (TimeManager.timeManager.allTime > data.timeLimit)
        {
            SpawnCommonEnemy(data.enemySpawnSpeed2);
        }
    }

    void TimeToSpawnAround()//获取边缘地图坐标，批量生成
    {
        if(TimeManager.timeManager.intAllTime >= data.timelimit1 && TimeManager.timeManager.intAllTime < data.timelimit1 +data.count1 * data.s1)
        {
            
            data.s1 += Time.deltaTime;
            int count1;
            count1 = data.count1;
            while(count1 > 0 && data.s1 >= datas1)
            {
                for (int i = 0; i < data.enemycount1; i++)
                {
                    OnceToCreateAround();
                }
                count1--;
                data.s1 -= datas1;
            }
        }

        if (TimeManager.timeManager.intAllTime >= data.timelimit2 && TimeManager.timeManager.intAllTime < data.timelimit2 + data.count2 * data.s2)
        {
            
            data.s2 += Time.deltaTime;
            int count2;
            count2 = data.count2;
            while (count2 > 0 && data.s2 >= datas2)
            {
                for (int i = 0; i < data.enemycount2; i++)
                {
                    OnceToCreateAround();
                }
                count2--;
                data.s2 -= datas2;
            }
        }

        if (TimeManager.timeManager.intAllTime >= data.timelimit3)
        {
            data.s3 += Time.deltaTime;
            while (data.s3 >= datas3)
            {
                for (int i = 0; i < data.enmeycount3; i++)
                {
                    OnceToCreateAround();
                }
                data.s3 -= datas3;
            }
        }
    }

    void OnceToCreateAround()
    {
        HexCoordinates[] edge;
        edge = new HexCoordinates[36];
        int count = 0;
        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == 0 || i == 11)
                {
                    edge[count++] = new HexCoordinates(i, j);
                }
                else if (j == 0 || j == 7)
                {
                    edge[count++] = new HexCoordinates(i, j);
                }
            }
        }

        HexCoordinates spawnCoordinates;
        for (int i = 0; i < 36; i++)
        {
            spawnCoordinates = edge[i];
            Vector3 spawnPosition = HexCoordinates.FromCoordinate(spawnCoordinates);
            Enemy enemy = enemyFactory.GetAroundEnemy(spawnPosition);
            enemies.Add(enemy);
            SearchAndGo(enemy);
        }
    }

    public VectorAndNum Search(List<TowerShape> pool, Enemy enemy)
    {
        VectorAndNum r = new VectorAndNum();
        if (pool.Count == 0)
        {
            r.hasPool = 2;
            r.num = -1;
            return r;
        }
        float min = 199000;
        float distance = 1000;
        r.num = 0;
        Vector3 v3 = new Vector3();
        for (int i = 0; i < pool.Count; i++)
        {
            distance = Vector3.Distance(pool[i].transform.localPosition, enemy.transform.localPosition);
            if (distance < min)
            {
                min = distance;
                v3 = pool[i].transform.localPosition;
                r.num = i;
            }
        }
        r.point = v3;
        return r;
    }

    public void Add(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void GameUpdate()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].GameUpdate())
            {
                int last = enemies.Count - 1;
                enemies[i] = enemies[last];
                enemies.RemoveAt(last);
                i -= 1;
            }
        }
    }

    public void SeeEnemy()
    {   
        inEnemyScene = true;
    }


}