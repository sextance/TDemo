using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public HexGrid hexGrid;

	void Awake()
    {
    }

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			HandleInput();
		}
	}

	public void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			Vector3 position = hit.point;
			position = hexGrid.transform.InverseTransformPoint(position);
			HexCoordinates coordinates = HexCoordinates.FromPosition(position);
			Vector3 centerPosition = HexCoordinates.FromCooradiante(coordinates);
			hexGrid.TouchCell(coordinates);
			Debug.Log("!");
		}
	}
}
