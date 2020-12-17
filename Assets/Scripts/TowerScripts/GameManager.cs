using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager gm; //Only one game manager instance is allowance

    /*public ServerMsg... serverMsg.....*/
    /*Object Factory*/
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

    List<TowerShape> towerShapes;
    List<Enemy> enemies;

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

    //private Animator anim;

    /*Game Datas*/
    public int money;

    /*Reserve for other objects*/
    void Start()
    {
        /* Reserve for Sence logic*/
        gm = this;
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

        /*Game Data*/
        money = 15; //start money for player
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
                for (int i = 0; i < enemies.Count; i++)
                {
                    SearchAndGo(enemies[i]);
                }
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
                for (int i = 0; i < enemies.Count; i++)
                {
                    SearchAndGo(enemies[i]);
                }
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
                for (int i = 0; i < enemies.Count; i++)
                {
                    SearchAndGo(enemies[i]);
                }
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
                for(int i = 0; i < enemies.Count; i++)
                {
                    SearchAndGo(enemies[i]);
                }
                
            }
            else
                Debug.Log("No tower selected.");
        }
        //if ( (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButton(0))
            MobilePick();

        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnDpsEnemy();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnTEnemy();
        }
        TimeToSpawn();
        GameUpdate();
    }

    void CreateTowerShape(int towerId, HexCell buildRegion)
    {
        TowerShape instance = towerShapeFactory.Get(towerId, towerId);
        Transform t = instance.transform;
        Vector3 buildPosition = HexCoordinates.FromCoordinate(buildRegion.coordinates);

        //Move the root of prefabs to ground
        instance.coordinates = buildRegion.coordinates;
        t.localPosition = buildPosition + Vector3.up * instance.transform.localScale.y / 2;
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
        }
        else
        {
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
        }
        else
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
            }
            else if (hit.transform.tag == "Tower")
            {
                pickTower = hit.collider.GetComponent<TowerShape>();
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
            }
            else if (hit.transform.tag == "Map")
            {
                pickRegion = hit.collider.GetComponent<HexCell>();
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

    void SpawnCommonEnemy(float enemySpawnSpeed)//生成敌人
    {
        spawnSpeed += spawnSpeed * Time.deltaTime;
        while (spawnSpeed >= enemySpawnSpeed)
        {
            spawnSpeed -= enemySpawnSpeed;
            Enemy enemy = enemyFactory.GetEnemy();
            enemies.Add(enemy);
            SearchAndGo(enemy);
            Attack(enemy);


        }
    }

    void SpawnTEnemy()
    {
        TEnemy tEnemy = enemyFactory.GetTEnemy();
        enemies.Add(tEnemy);
        SearchAndGo(tEnemy);
        Attack(tEnemy);
    }

    void SpawnDpsEnemy()
    {
        DpsEnemy dpsEnemy = enemyFactory.GetDpsEnemy();
        enemies.Add(dpsEnemy);
        SearchAndGo(dpsEnemy);
        Attack(dpsEnemy);
    }

    void SearchAndGo(Enemy enemy)
    {
        Vector3 point;
        point = Search(towerShapes, enemy);
        enemy.navMesh.SetDestination(point);
    }

    void Attack(Enemy enemy)
    {
        Vector3 point;
        point = Search(towerShapes,enemy);
        if (Vector3.Distance(enemy.transform.localPosition, point) < 0.5f)
        {
            enemy.gameObject.GetComponent<Animator>().SetBool("walk", false);
            float attack = 1f;
            attack += attack * Time.deltaTime;
            while (attack >= 1f)
            {
                attack -= 1f;
                //TakeDamage(enemy.attack);
            }
        }
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
        if(time > 120f)
        {
            OnceToCreateAround();
        }
        else if(time > 300f)
        {
            OnceToCreateAround();
        }
        else if(time > 480f)
        {
            OnceToCreateAround();
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
            Enemy enemy = enemyFactory.GetEnemy();
            enemies.Add(enemy);
            SearchAndGo(enemy);
            Attack(enemy);
        }
    }

    public Vector3 Search(List<TowerShape> pool, Enemy enemy)
    {
        Vector3 point;
        float min = 199000;
        float distance = 1000;
        Vector3 v3 = new Vector3();
        for (int i = 0; i < pool.Count; i++)
        {
            distance = Vector3.Distance(pool[i].transform.localPosition, enemy.transform.localPosition);
            if (distance < min)
            {
                min = distance;
                v3 = pool[i].transform.localPosition;
            }
        }
        point = v3;
        return point;
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
