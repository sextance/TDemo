using BaseFramework.Network;
using Newtonsoft.Json;
using NinjaMessage;
using System;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("通信阶段");
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(tn), null);
    }

    //塔固化时调用，发送固化塔的类型与位置
    public static void TowerChange(TowerShape towershape)
    {
        TowerChange tch = new TowerChange();
        if (towershape.ShapeId == 0)
        {
            tch.towerShapeType.ShapeId = 0;
            tch.vec = towershape.transform.localPosition;
        }
        else if (towershape.ShapeId == 1)
        {
            tch.towerShapeType.ShapeId = 1;
            tch.vec = towershape.transform.localPosition;
        }
        else if (towershape.ShapeId == 2)
        {
            tch.towerShapeType.ShapeId = 2;
            tch.vec = towershape.transform.localPosition;
        }
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(tch), null);
    }

    //扫描敌人时发送
    void Scan()
    {
        TowerChange st = new TowerChange();
        st.scanTower = GameManager.gm.towerShapes;
        LoginRequist.ucl.rpcCall("combat.get_tower_num", JsonConvert.SerializeObject(st), null);
    }

    //游戏结束
    public static void GameOver()
    {
        if(TimeManager.timeManager.allTime >= 10f)
        {
            if (GameManager.gm.towerShapes.Count <= 0)
            {
                LoginRequist.ucl.rpcCall("combat.get_tower_num", "false", null);
                SceneManager.LoadScene("EndScene");
            }
        }
    }
}


public class TowerChange
{
    public Vector3 vec;
    public TowerShape towerShapeType;
    public int towernum;
    public List<TowerShape> scanTower;
}





