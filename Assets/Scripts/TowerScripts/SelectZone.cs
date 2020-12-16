using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectZone : MonoBehaviour
{
	public Vector3 SpawnPoint
	{
		get
		{
			return transform.TransformPoint(
				Random.onUnitSphere); //通过对象的scale来控制区域半径
		}
	}
}
