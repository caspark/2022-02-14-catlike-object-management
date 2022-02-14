using UnityEngine;

[CreateAssetMenu(fileName = "ShapeFactory", menuName = "ObjectMgmt/ShapeFactory", order = 0)]
public class ShapeFactory : ScriptableObject {
    [SerializeField] private Shape[] prefabs;
    [SerializeField] private Material[] materials;

    public Shape Get(int shapeId, int materialId = 0) {
        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeId = shapeId;
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom() {
        return Get(
            Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length)
        );
    }
}
