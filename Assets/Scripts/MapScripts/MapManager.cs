﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{	
	public HexGrid hexGrid;

	void Awake()
    {
	}

	void Update()
	{

	}

	public void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{

			if(true)
            {
				HexCell instance = hit.collider.GetComponent<HexCell>();
				Debug.Log("hit:" + instance.coordinates.ToString());
            }
		}
	}
}