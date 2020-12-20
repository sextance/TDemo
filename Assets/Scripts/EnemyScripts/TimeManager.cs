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
    public int counts;


    public float timeToSpawn = 1f;

    public int intAllTime = 0;
    public int lastTime = 180;

    private void Start()
    {
        allTime = 0f;
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
        timeToSpawn -= Time.deltaTime;
        while(timeToSpawn >= 1f)
        {
            intAllTime++;
            lastTime--;
            timeToSpawn -= 1f;
        }
        if(lastTime < 0)
        {
            lastTime += 180;
        }
    }
}
