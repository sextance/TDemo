 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager timeManager;
    private void OnEnable()
    {
        timeManager = this;
    }
    public float allTime;
    public int count1 = 3;  


    public float timeToSpawn = 1f;

    public int intAllTime;
    public int lastTime1;
    public int lastTime2;
    public int lastTime3;

    private void Start()
    {
        allTime = 0f;
        intAllTime = 0;
        lastTime1 = Data.GlobalData.timelimit1;
        lastTime2 = Data.GlobalData.timelimit2;
        lastTime3 = Data.GlobalData.timelimit3;
    }

    private void Awake()
    {
    }

    private void Update()
    {
        allTimeManager();
        TimeToSpawnAroundManager();
    }

    public void allTimeManager()
    {
        allTime += Time.deltaTime;
    }

    public void TimeToSpawnAroundManager()
    {
        timeToSpawn += Time.deltaTime;
        while(timeToSpawn >= 1f)
        {
            intAllTime++;
            if (count1 == 3)
            {
                lastTime1--;
                if (lastTime1 >= 0 && count1 == 3)
                {
                    GameManager.gm.timerText.text = "爆发倒计时: " + TimeManager.timeManager.lastTime1.ToString();
                }
                else if(lastTime1 < 0)
                {
                    count1--;
                    lastTime1 = 0;
                }
            }

            if (count1 == 2)
            {
                lastTime2--;
                if (lastTime2 >= 0 && count1 == 2)
                {
                    GameManager.gm.timerText.text = "爆发倒计时: " + TimeManager.timeManager.lastTime2.ToString();
                }
                else if (lastTime2 < 0)
                {
                    count1--;
                    lastTime2 = 0;
                }
            }

            if (count1 == 1)
            {
                lastTime3--;
                if (lastTime3 >= 0 && count1 == 1)
                {
                    GameManager.gm.timerText.text = "爆发倒计时: " + TimeManager.timeManager.lastTime3.ToString();
                }
                else if (lastTime3 < 0)
                {
                    count1--;
                    lastTime3 = 0;
                }
            }
            else if (count1 == 0)
            {
                lastTime3 = -1;
                GameManager.gm.timerText.text = "爆发倒计时: " + TimeManager.timeManager.lastTime3.ToString();
            }
            timeToSpawn -= 1f;
        }
    }
}
