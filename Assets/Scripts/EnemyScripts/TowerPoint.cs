using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    public TowerShape towerShape { get; private set; }

    public Vector3 Position => transform.position;

    private void Awake()
    {
        towerShape = transform.root.GetComponent<TowerShape>();
        Debug.Assert(towerShape != null, "Target Point without Enemy root");
        Debug.Assert(GetComponent<SphereCollider>() != null, "Target Point without sphere collider");
    }
}
