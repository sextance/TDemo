using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEnemy : Enemy
{
    public EnemyType enemyType => EnemyType.T;
    public float tauntRange;

    private void OnEnable()
    {
        tauntRange = 5f;
    }

    private void Update()
    {
        base.Update();
        //tauntTower();
    }

    void tauntTower()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.localPosition, tauntRange, LayerMask.GetMask("Tower"));
        foreach (Collider collider in colliders)
        {
            DefenceTowerEntity t = collider.gameObject.GetComponent<DefenceTowerEntity>();
            if (t == null)
                break;
            else
            {
                // t.嘲讽函数(this);
            }
        }
    }
}
