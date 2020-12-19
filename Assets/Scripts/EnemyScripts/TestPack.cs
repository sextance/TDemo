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

public class TestPack : MonoBehaviour
{
    Button scanButton;
    Text scanTowerText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        scanButton = GameObject.Find("ScanButton").GetComponent<Button>();
        scanTowerText = scanButton.transform.Find("Text").GetComponent<Text>();
        scanTowerText.text = "扫描";

        //active UI event;
        scanButton.onClick.AddListener(Scan);
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
        tch.towerShapeInfo = towershape;
        tch.OptType = OptionType.TOWER_CHANGE;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(tch), null);
    }

    //自爆塔
    public static void DestoryOwnTower(TowerShape towershape)
    {
        TowerChange dot = new TowerChange();
        dot.towerShapeInfo = towershape;
        dot.OptType = OptionType.DESTORY_TOWER;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(dot), null);
    }

    //扫描敌人时发送
    void Scan()
    {
        TowerChange st = new TowerChange();
        st.scanTower = GameManager.gm.towerShapes;
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
                SceneManager.LoadScene("EndScene");
            }
        }
    }

}


public class TowerChange
{
    public OptionType OptType;
    public TowerShape towerShapeInfo;
    public int towernum;
    public List<TowerShape> scanTower;
    public string result;
}

public enum OptionType{ 
    UPDATE_TOWER,
    TOWER_CHANGE,
    DESTORY_TOWER,
    SCAN,
    GAME_OVER
}



