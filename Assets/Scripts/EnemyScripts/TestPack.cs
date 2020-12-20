using BaseFramework.Network;
using Newtonsoft.Json;
using NinjaMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class TestPack : MonoBehaviour
{
    Button EnemyButton;
    Text scanTowerText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnemyButton = GameObject.Find("EnemyButton").GetComponent<Button>();
        scanTowerText = EnemyButton.transform.Find("EnemyText").GetComponent<Text>();
        scanTowerText.text = "扫描";

        //active UI event;
        EnemyButton.onClick.AddListener(Scan);
    }


    //塔创建或删除时调用，发送塔数量计算势力比
    public static void TowerNum(List<TowerShape> towershapes)
    {
        TowerChange tn = new TowerChange();
        tn.towernum = towershapes.Count;
        tn.OptType = OptionType.UPDATE_TOWER;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(tn), null);
    }

    //塔固化时调用，发送固化塔的类型与位置
    public static void TowerChange(TowerShape towershape)
    {
        TowerChange tch = new TowerChange();
        tch.ti = new TowerInfo(towershape.ShapeId,towershape.transform.localPosition.x, towershape.transform.localPosition.y, towershape.transform.localPosition.z);
        tch.OptType = OptionType.TOWER_CHANGE;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(tch), null);
    }

    //自爆塔
    public static void DestoryOwnTower(TowerShape towershape)
    {
        TowerChange dot = new TowerChange();
        dot.ti = new TowerInfo(towershape.ShapeId, towershape.transform.localPosition.x, towershape.transform.localPosition.y, towershape.transform.localPosition.z);
        dot.OptType = OptionType.DESTORY_TOWER;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(dot), null);
    }

    //扫描请求发送
    void Scan()
    {
        TowerChange st = new TowerChange();
        st.OptType = OptionType.SCAN;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(st), null);
    }

    //游戏结束
    public static void GameOver()
    {
        if (TimeManager.timeManager.allTime >= 10f)
        {
            if (GameManager.gm.towerShapes.Count == 0 /*|| GameManager.gm.CheckPower() == false*/)
            {
                TowerChange gg = new TowerChange();
                gg.result = "false";
                gg.OptType = OptionType.GAME_OVER;
                LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(gg), null);
                //SceneManager.LoadScene("EndScene");
                GameManager.gm.sceneSwitch.ToScene("EndScene");
            }
        }
    }

}


public class TowerChange
{
    public OptionType OptType;
    public int towernum;
    public TowerInfo ti;
    public List<TowerInfo> a = new List<TowerInfo>();
    public string result;
}

public enum OptionType{ 
    UPDATE_TOWER,
    TOWER_CHANGE,
    DESTORY_TOWER,
    SCAN,
    GAME_OVER,
    SCAN_MAKE
}

public class TowerInfo//towershape
{
    public int shapeid;
    public float x, y, z;
    public TowerInfo(int shapeid,float x, float y,float z)
    {
        this.shapeid = shapeid;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}



