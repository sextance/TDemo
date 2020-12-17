using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy targetEnemy;

    public int damage = 1;

    int shapeId = int.MinValue; //Default value
    public int ShapeId
    {
        get { return shapeId; }
        set
        {
            if (shapeId != int.MaxValue && value != int.MinValue)
                shapeId = value;
            else
                Debug.LogError("Not allowed to change shapeId.");
        }
    }

    int serialId = int.MinValue; //Default value
    public int SerialId
    {
        get { return serialId; }
        set
        {
            if (serialId != int.MaxValue && value != int.MinValue)
                serialId = value;
            else
                Debug.LogError("Not allowed to change SerialId.");
        }
    }

    float speed;
    float delayDeadTime;
    bool reachTarget;
    bool startTimer;
    Collider collider;

    void OnEnable()
    {
        targetEnemy = null;
        shapeId = 0;
        delayDeadTime = 0.5f;
        speed = 20.0f;
        startTimer = false;
        reachTarget = false;
        collider = GetComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!reachTarget)
            MoveToPosition();
        else if(startTimer == true)
        {
            delayDeadTime -= Time.deltaTime;
            if(delayDeadTime <= 0f)
                ProjectileFactory.pf.ReClaim(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other == targetEnemy.GetComponent<Collider>() )
        {
            reachTarget = true;
            startTimer = true;
            targetEnemy.ApplyDamge(damage);
        }
    }

    private void MoveToPosition()
    {
        this.transform.position = Vector3.MoveTowards(
    transform.position, targetEnemy.transform.position,
    Time.deltaTime * speed);
    }
}
