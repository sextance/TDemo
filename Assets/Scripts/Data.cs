using UnityEngine;

public class Data : MonoBehaviour
{
    /*-------------------------------------------------------------------------------------------------------------*/
    //Game
    public int startMoney = 15;                                                    //开局金钱
    public int buildCost = 5;                                                         //建造花费
    public int solidificateCost = 10;                                              //固化花费
    public int deconstructionCost = 5;                                         //自爆花费
    public float cellLength = 17.32051f;                                       //两个相邻六边形中心点距离，可以视为游戏的1 unit

    //All Tower
    public float constructTime = 2.0f;                                          //建造时间
    public float convertingTime = 2.0f;                                        //转化时间
    public float  convertingCoolDonwnTime = 5.0f;                    //转化后冷却时间
    public float factorScale = 1.5f;                                               //固化后塔体积扩大倍数
    public int factorHealth = 2;                                                    //固化后生命值扩大倍数

    //Attack Tower
    public int attackTowerMaxHealth= 10;                                   //固化前最大生命值
    public float attackRange = 1.5f * 17.32051f;                           //固化前攻击/索敌范围
    public int factorAtkRange = 2;                                                //固化后攻击距离扩大倍数
    public int damage = 3;                                                            //固化前单次弹道伤害
    public int factorDamage = 2;                                                  //固化后单次弹道伤害扩大倍数 
    public float attackCoolDownTime = 1.0f;                               //攻击冷却时间
    public float projectileSpeed = 20.0f;                                      //弹道速度 
    public float projectileDelayTime = 0.5f;                                 //弹道延迟消失时间

    //Defence Tower
    public int defenceTowerMaxHealth = 50;                               //固化前最大生命值
    public float tauntRange = 1.5f * 17.32051f;                            //固化前嘲讽范围
    public int factorTauntRange = 2;                                            //固化后索敌范围扩大倍数

    //Production Tower
    public int productionTowerMaxHealth = 20;                         //固化前最大生命值
    public float powerRange = 1.0f * 17.32051f;                          //固化前充能范围
    public int factorPowerRange = 3;                                          //固化后充能范围扩大倍数
    public int production = 1;                                                      //固化前每次生产量
    public int factorProduction = 2;                                             //固化后生产量扩大倍数
    public float productionCoolDownTime = 4.0f;                      //生产间隔时间

    public float enemySpawnSpeed1 = 10f;                              //先期敌人生成速度
    public float enemySpawnSpeed2 = 3f;                              //后期敌人生成速度
    public float timeLimit = 300f;                                     //先后期转变时间点
    public float s1 = 1f;                                            //第一波尸潮时怪物生成间隔
    public float s2 = 1f;                                            //第二波尸潮时怪物生成间隔
    public float s3 = 5f;                                            //第三波尸潮时怪物生成间隔
    public int count1 = 2;                                                //第一波尸潮刷新波数
    public int count2 = 5;                                                //第二波尸潮波数
    public int enemycount1 = 1;                                           //第一波单波敌人生成数量
    public int enemycount2 = 2;                                          //第二波单波敌人生成数量
    public int enmeycount3 = 3;                                          //第三波单波敌人生成数量
    public int timelimit1 = 120;                                           //第一波出现时间
    public int timelimit2 = 300;                                           //第二波出现时间
    public int timelimit3 = 480;                                           //第三波开始时间第三波开始时间


    public int enemyCount0 = 1;                                            //固化攻击塔单次生成敌人数量
    public int enemyCount1 = 1;                                            //固化防御塔单次生成敌人数量
    public int enemyCount2 = 3;                                            //固化生产塔单次生成敌人数量
    public float spawnOtherTime0 = 5f;                                     //固化攻击塔对应敌人生成间隔
    public float spawnOtherTime1 = 5f;                                     //固化防御塔对应敌人生成间隔
    public float spawnOtherTime2 = 5f;                                     //固化生产塔对应敌人生成间隔
    public int DestoryToMakeCount = 5;                                     //自爆塔生成的怪数量

    [SerializeField]
    public Enemy enemyPrefab1 = default;                                   //第一波尸潮的怪类型

    [SerializeField]
    public Enemy enemyPrefab2 = default;                                   //第二波尸潮的怪类型

    [SerializeField]
    public Enemy enemyPrefab3 = default;                                   //第三波尸潮的怪类型
    /*------------------------------------------------EOF---------------------------------------------------------------*/

    static public Data _instance;
    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);
    }

    public static Data GlobalData
    {
        get
        {
            return _instance;
        }
    }
}

