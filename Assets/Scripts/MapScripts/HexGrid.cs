using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	public int width = 12;
	public int height = 8;
	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;
	bool CoordinateOffset;

	public HexCell cellPrefab;
	HexCell[] cells;

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

	public void TouchCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Y * width;
		Debug.Log(index.ToString()+","+cells.Length.ToString());
		HexCell cell = cells[index];
		cell.color = touchedColor;
		Debug.Log("touched at " + coordinates.ToString());
	}

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
		cell.color = defaultColor;

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToString();

	}
}