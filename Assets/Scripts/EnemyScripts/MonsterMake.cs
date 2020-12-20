using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMake : MonoBehaviour
{
    public float waitTime;
    int shapeid;
    int singleCount;
    Vector3 position;
    OptionType option;
    //var a = new GameObject();
    //a.AddComponent<MonsterMake>().SetData(12);

    public void Start() {
        StartCoroutine(LoopMakeMonster());
        TimeToMakeMonster(shapeid);
        SingleCountMake(shapeid);
    }

    //private void Update()
    //{
    //    if(shapeid == 4)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    public IEnumerator LoopMakeMonster() {

        while (true) {
            yield return new WaitForSeconds(waitTime);
            MakeMonster(shapeid,position);
        }
    }

    public void SetData(int shapeid,Vector3 position,OptionType option) {
        this.shapeid = shapeid;
        this.position = position;
        this.option = option;
    }

    private void MakeMonster(int shapeid,Vector3 position) {
        switch (shapeid)
        {
            case 0:
                for(int i = 0; i < singleCount; i++)
                {
                    EnemyFactory.enemyF.CreatedpsEnemyInOtherWorld(position);
                }
                break;
            case 1:
                for(int i = 0; i < singleCount; i++)
                {
                    EnemyFactory.enemyF.CreateTEnemyInOtherWorld(position);
                }
                break;
            case 2:
                for(int i = 0; i < singleCount; i++)
                {
                    EnemyFactory.enemyF.CreateCommonEnemyInOtherWorld(position);
                }
                break;
        }
    }

    public float TimeToMakeMonster(int shapeid)
    {
        switch (shapeid)
        {
            case 0:
                waitTime = Data.GlobalData.spawnOtherTime0;
                break;
            case 1:
                waitTime = Data.GlobalData.spawnOtherTime1;
                break;
            case 2:
                waitTime = Data.GlobalData.spawnOtherTime2;
                break;
        }
        return waitTime;
    }

    public int SingleCountMake(int shapeid)
    {
        if(option == OptionType.DESTORY_TOWER)
        {
            singleCount = Data.GlobalData.DestoryToMakeCount;
        }
        else if(option == OptionType.TOWER_CHANGE)
        {
            switch (shapeid)
            {
                case 0:
                    singleCount = Data.GlobalData.enemyCount0;
                    break;
                case 1:
                    singleCount = Data.GlobalData.enemyCount1;
                    break;
                case 2:
                    singleCount = Data.GlobalData.enemyCount2;
                    break;
            }
        }
        return singleCount;
    }
}
