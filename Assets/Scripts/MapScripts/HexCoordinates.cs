using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
	[SerializeField]
	private int x, y;

	public int X
	{
		get
		{
			return x;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
	}

	public HexCoordinates(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static HexCoordinates FromOffsetCoordinates(int x, int y, bool CoordinateOffset = false)
	{
		if (CoordinateOffset == false)
		{
			return new HexCoordinates(x, y);
		}
		else
		{
			return new HexCoordinates(x - y / 2, y);
		}
	}

	public override string ToString()
	{
		return "(" + X.ToString() + ", " + Y.ToString() + ")";
	}

	public string ToStringOnSeparateLines()
	{
		return X.ToString() + "\n" + Y.ToString();
	}

	public static HexCoordinates FromPosition(Vector3 position)
	{
		float yInCoordinate = position.z * 2f / (HexMetrics.outerRadius * 3f);
		float xInCoordinate = position.x / (HexMetrics.outerRadius * 2f);

		int iX = Mathf.RoundToInt(xInCoordinate);
		int iY = Mathf.RoundToInt(yInCoordinate);

		return new HexCoordinates(iX, iY);
	}

	public static Vector3 FromCoordinate(HexCoordinates coordinates)
	{
		Vector3 position;
		position.x = (coordinates.x + coordinates.y * 0.5f - coordinates.y / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = coordinates.y * (HexMetrics.outerRadius * 1.5f);

		return position;
	}

}