using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Data data = Data.GlobalData;
    public Enemy targetEnemy;

    public GameObject hit;
    public GameObject flash;
    public float hitOffset = 0f;

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

    public int damage;
    float speed;
    float delayDeadTime;
    bool reachTarget;
    bool startTimer;
    Collider collider;
    Vector3 lastPosition;

    void OnEnable()
    {
        damage = data.damage;
        targetEnemy = null;
        delayDeadTime = data.projectileDelayTime;
        speed = data.projectileSpeed;
        lastPosition = Vector3.zero;
        startTimer = false;
        reachTarget = false;
        collider = GetComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs == null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
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

    private void Update()
    {
        if (reachTarget)
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.forward);
            Vector3 pos = lastPosition;

            /*
            if (hit != null)
            {
                var hitInstance = Instantiate(hit, pos, rot);
                //if (UseFirePointRotation)
                hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
                //else
               // { hitInstance.transform.LookAt(contact.point + contact.normal); }

                var hitPs = hitInstance.GetComponent<ParticleSystem>();
                if (hitPs == null)
                {
                    Destroy(hitInstance, hitPs.main.duration);
                }
                else
                {
                    var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitInstance, hitPsParts.main.duration);
                }
            }
            */
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
