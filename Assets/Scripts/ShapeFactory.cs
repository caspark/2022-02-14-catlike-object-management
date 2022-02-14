using UnityEngine;

[CreateAssetMenu(fileName = "ShapeFactory", menuName = "ObjectMgmt/ShapeFactory", order = 0)]
public class ShapeFactory : ScriptableObject {
    [SerializeField]
    private Shape[] prefabs;

    public Shape Get(int shapeId) {
        Shape instance = Instantiate(prefabs[shapeId]);
        instance.ShapeId = shapeId;
        return instance;
    }

    public Shape GetRandom() {
        return Get(Random.Range(0, prefabs.Length));
    }
}
