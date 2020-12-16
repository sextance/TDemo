using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    public Enemy enemy { get; private set; }

    public Vector3 Position => transform.position;

    private void Awake()
    {
        enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(enemy != null, "Target Point without Enemy root");
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target Point without sphere collider");
    }
}
