using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 12;
    public int height = 8;
    bool CoordinateOffset;

    public HexCell cellPrefab;
    public HexCell[] cells;

    public Text cellLabelPrefab;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[height * width];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, y, i++);
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    /*public void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Y * width;
        Debug.Log(index.ToString() + "," + cells.Length.ToString());
        HexCell cell = cells[index];
        Debug.Log("touched at " + coordinates.ToString());
    }*/

    void CreateCell(int x, int yInCoordinate, int i)
    {
        Vector3 position;
        position.x = (x + yInCoordinate * 0.5f - yInCoordinate / 2) * (HexMetrics.innerRadius * 2f);
        position.y = -1f;
        position.z = yInCoordinate * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);

        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, yInCoordinate, CoordinateOffset);
        cell.name = cell.coordinates.ToString();
        cell.available = true;
        cell.powered = false;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToString();

    }

    public void CreatePowerLinkToCell(ProductionTowerEntity productionTower)
    {
        HexCell cell = productionTower.cell;
        int x = cell.coordinates.X;
        int y = cell.coordinates.Y;
        int range = productionTower.powerRange;
        for (int i = x - range; i <= x + range; i++)
        {
            for (int j = y - range; j <= y + range; j++)
            {
                // check if the hexagonal boundary conditions is satisfied
                if ((i + j) - (x + y) >= -range && (i + j) - (x + y) <= range)
                    EnpowerCell(i, j, productionTower);
            }
        }
    }

    void EnpowerCell(int x, int y, ProductionTowerEntity productionTower)
    {
        int index = x + y * 12;
        // check if the cells boundary condition is satisfied
        if (index > 0 && index < height * width)
        {
            HexCell cell = cells[index];
            // Avoid repeat addition
            if (cell.powerLinks.Find(o => o.cell == productionTower.cell) == null) 
            {
                cell.powerLinks.Add(productionTower);
            }
        }
    }

    public void BreakPowerLinkToCell(ProductionTowerEntity productionTower)
    {
        HexCell cell = productionTower.cell;
        int x = cell.coordinates.X;
        int y = cell.coordinates.Y;
        int range = productionTower.powerRange;
        for (int i = x - range; i <= x + range; i++)
        {
            for (int j = y - range; j <= y + range; j++)
            {
                // check if the hexagonal boundary conditions is satisfied
                if ((i + j) - (x + y) >= -range && (i + j) - (x + y) <= range)
                    DisempowerCell(i, j, productionTower);
            }
        }
    }

    void DisempowerCell(int x, int y, ProductionTowerEntity productionTower)
    {
        int index = x + y * 12;
        // check if the cells boundary condition is satisfied
        if (index > 0 && index < height * width)
        {
            HexCell cell = cells[index];
            // Avoid reamove null 
            int findIndex = cell.powerLinks.FindIndex(o => o.cell == productionTower.cell);
            if (findIndex != -1)
            {
                int last = cell.powerLinks.Count - 1;
                cell.powerLinks[findIndex] = cell.powerLinks[last];
                cell.powerLinks.RemoveAt(last);
            }
        }
    }

}