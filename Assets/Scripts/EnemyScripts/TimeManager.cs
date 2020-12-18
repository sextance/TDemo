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

    public int timelimit1;
    public int timelimit2;
    public int timelimit3;

    public float timeToSpawn;

    public int intAllTime=0;

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
        timeToSpawn += Time.deltaTime;
        while(timeToSpawn >= 1f)
        {
            intAllTime++;
            timeToSpawn -= 1f;
        }
    }
}
