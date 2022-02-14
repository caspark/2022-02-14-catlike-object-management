using UnityEngine;

public class Shape : PersistableObject {
    static int colorPropertyId = Shader.PropertyToID("_BaseColor");


    public int ShapeId {
        get {
            return shapeId;
        }
        set {
            if (shapeId == int.MinValue && value != int.MinValue) {
                shapeId = value;
            }
            else {
                Debug.LogError("Cannot change shapeId.");
            }
        }
    }

    public int MaterialId { get; private set; }

    private int shapeId = int.MinValue;

    [SerializeField]
    private Color color;
    MeshRenderer meshRenderer;


    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material, int materialId) {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    public void SetColor(Color color) {
        this.color = color;
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }
}
