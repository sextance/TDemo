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
    Vector3 lastPosition;

    void OnEnable()
    {
        targetEnemy = null;
        shapeId = 0;
        delayDeadTime = 0.5f;
        speed = 20.0f;
        lastPosition = Vector3.zero;
        startTimer = false;
        reachTarget = false;
        collider = GetComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (!reachTarget)
        {
            MoveToPosition();
            if (Vector3.Distance(this.transform.localPosition, lastPosition) <= 0.01f)
            {
                reachTarget = true;
                startTimer = true;
            }
        } else if(startTimer == true)
        {
            delayDeadTime -= Time.deltaTime;
            if(delayDeadTime <= 0f)
                ProjectileFactory.pf.ReClaim(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (targetEnemy != null)
        {
            if (other == targetEnemy.GetComponent<Collider>())
            {
                reachTarget = true;
                startTimer = true;
                targetEnemy.ApplyDamge(damage);
            }
        }
    }

    private void MoveToPosition()
    {
        if (targetEnemy != null)
        {
            lastPosition = targetEnemy.transform.position;
            this.transform.position = Vector3.MoveTowards(transform.position, lastPosition, Time.deltaTime * speed);
        } else {
            this.transform.position = Vector3.MoveTowards(transform.position, lastPosition, Time.deltaTime * speed);
        }
    }
}
