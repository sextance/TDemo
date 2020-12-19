using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int width = 12;
    public int height = 8;
    bool CoordinateOffset;

    public HexCell cellPrefab;
    public HexCell[] cells;
    bool[] brightened;

    public Text cellLabelPrefab;
    Canvas gridCanvas;
    public Material previousMaterial;
    public Material poweredMaterial;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HexCell[height * width];
        brightened = new bool[height * width];
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                brightened[i] = false;
                CreateCell(x, y, i++);
            }
        }
    }

    void Start()
    {

    }

    void LateUpdate()
    {
        for(int i = 0; i < height * width; i++){
            if (cells[i].powerLinks.Count != 0){
                cells[i].GetComponent<MeshRenderer>().material = poweredMaterial;
                brightened[i] = true;
            }
            else if(cells[i].powerLinks.Count == 0 && brightened[i] == true){
                cells[i].GetComponent<MeshRenderer>().material = previousMaterial;
                brightened[i] = false;
            }
        }
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

}
