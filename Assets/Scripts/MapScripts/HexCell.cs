using UnityEngine;
using System.Collections.Generic;

public class HexCell : MonoBehaviour
{
    public List<ProductionTowerEntity> powerLinks;
    public HexCoordinates coordinates;
    public Color color;
    public bool available;
    public bool powered;

    private void Awake()
    {
        powerLinks = new List<ProductionTowerEntity>();
    }

    private void FixedUpdate()
    {
        if (powerLinks.Count == 0)
            powered = false;
        else
            powered = true;
    }
}