using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShape : PersistableObject
{
    int shapeId = int.MinValue; //Default value
    public int ShapeId
    {
        get { return shapeId; }
        set
        {
            if (shapeId != int.MaxValue && value != int.MinValue)
                shapeId = value;
            else
                Debug.LogError("Not allowed to change shapeId.");
        }
    }

    MeshRenderer meshRenderer;

    public int MaterialId { get; private set; }
    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // ChangeTowerColor if needed
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;
    Color color;
    public void SetColor(Color color)
    {
        this.color = color;
        if (sharedPropertyBlock == null)
            sharedPropertyBlock = new MaterialPropertyBlock();
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }
}
