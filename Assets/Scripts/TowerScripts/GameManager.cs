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

    int selectTypeHandler; // 0 - non, 1 - tower...
    TowerShape pickTower;
    HexCell pickRegion;
    Vector3 buildPosition;
    GameObject selectedObject;
    Transform highLightObj;
    Material previousMaterial;
    Material selectedMaterial;

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

    float s1 = 1f;
    float s2 = 1f;
    float s3 = 5f;
    int count;
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
    }

    public void CAT()
    {
        if (selectTypeHandler == 2)
            {
                if(money >= data.buildCost)
                {
                    CreateTowerShape(0, pickRegion, Vector3.zero, 0);
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

    public void CDT()
    {
        if (selectTypeHandler == 2)
            {
                if (money >= data.buildCost)
                {
                    CreateTowerShape(1, pickRegion, Vector3.zero, 0);
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
                CreateTowerShape(2, pickRegion, Vector3.zero, 0);
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
            if (this.money > data.solidificateCost)
            {
                DestroyTowerShape(pickTower);
                selectTypeHandler = 0;
                this.money -= data.deconstructionCost;
            }
            else
                Debug.Log("Not enough money to deconstruct!");
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

    }

    void CreateTowerShape(int towerId, HexCell buildRegion, Vector3 localPostion, int initState, float healthFactor = 1)
    {
        Vector3 buildPosition;
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
        TowerEntity e = instance.gameObject.GetComponent<TowerEntity>();
        e.health = (int)(e.maxHealth * healthFactor);

        if (buildRegion == null)
        {
            buildPosition = localPostion;
            buildPosition.y = 0;
        } else {
            e.cell = buildRegion;
            buildPosition = HexCoordinates.FromCoordinate(buildRegion.coordinates);
            instance.coordinates = buildRegion.coordinates;
            buildRegion.available = false;
        }

        //Move the root of prefabs to ground
        t.localPosition = buildPosition + Vector3.up * instance.transform.localScale.y;
        if (t.localScale.y >= 15.0f)
            t.localScale /= data.factorScale;

        //Create link if production tower
        if(towerId == 2)
            mapManager.hexGrid.CreatePowerLinkToCell(
                instance.gameObject.GetComponent<ProductionTowerEntity>());
        
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
            pickTower.IsSolidificated = false;

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
                this.money -= data.solidificateCost;
        }
        else
            Debug.Log("Tower already solidificreateAttackTowered");
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

        Vector3 position = origin.transform.position;
        DestroyTowerShape(origin);

        if (targetName == "AttackTower")
        {
            CreateTowerShape(0, null, position, 3, healthFactor);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        } else if (targetName == "DefenceTower")
        {
            CreateTowerShape(1, null, position, 3, healthFactor);
            for (int i = 0; i < enemies.Count; i++)
                SearchAndGo(enemies[i]);
        } else if (targetName == "ProductionTower")
        {
            CreateTowerShape(2, null, position, 3, healthFactor);
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
        time += Time.deltaTime;
        SpawnCommonEnemy(enemySpawnSpeed1);
        if (time > timeLimit)
        {
            SpawnCommonEnemy(enemySpawnSpeed2);
        }
    }

    void TimeToSpawnAround()//获取边缘地图坐标，批量生成
    {
        if(TimeManager.timeManager.intAllTime >= TimeManager.timeManager.timelimit1 && TimeManager.timeManager.intAllTime < TimeManager.timeManager.timelimit1+2)
        {
            s1 += Time.deltaTime;
            count = 0;
            while(count < 2 && s1 >= 1f)
            {
                for (int i = 0; i < 1; i++)
                {
                    OnceToCreateAround();
                }
                count++;
                s1 -= 1f;
            }
        }

        if (TimeManager.timeManager.intAllTime >= TimeManager.timeManager.timelimit2 && TimeManager.timeManager.intAllTime < TimeManager.timeManager.timelimit2 + 5)
        {
            s2 += Time.deltaTime;
            count = 0;
            while (count < 5 && s2 >= 1f)
            {
                for (int i = 0; i < 2; i++)
                {
                    OnceToCreateAround();
                }
                count++;
                s2 -= 1f;
            }
        }

        if (TimeManager.timeManager.intAllTime >= TimeManager.timeManager.timelimit3)
        {
            s3 += Time.deltaTime;
            while (s3 >= 5f)
            {
                for (int i = 0; i < 2; i++)
                {
                    OnceToCreateAround();
                }
                s3 -= 5f;
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

}