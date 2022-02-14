using UnityEngine;

public class Shape : PersistableObject {

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

    int shapeId = int.MinValue;

    public void SetMaterial(Material material, int materialId) {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }
}
