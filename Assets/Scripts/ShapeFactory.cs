using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "ShapeFactory", menuName = "ObjectMgmt/ShapeFactory", order = 0)]
public class ShapeFactory : ScriptableObject {
    [SerializeField] private Shape[] prefabs;
    [SerializeField] private Material[] materials;
    [SerializeField] private bool recycle;

    List<Shape>[] pools;

    Scene poolScene;

    private Scene LoadedPoolScene {
        get {
            if (!poolScene.isLoaded) {
                // This should only happen if Enter Play Mode Options has turned off the Domain
                // Reload - if so, then poolScene will be non-null but also unloaded, so we need to
                // create it again.
                poolScene = SceneManager.CreateScene(name);
                Debug.Log("Pool scene was not loaded so has been recreated");
            }
            return poolScene;
        }
    }

    public Shape Get(int shapeId, int materialId = 0) {
        Shape instance;
        if (recycle) {
            if (pools == null) {
                CreatePools();
            }
            List<Shape> pool = pools[shapeId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0) {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else {
                instance = Instantiate(prefabs[shapeId]);
                SceneManager.MoveGameObjectToScene(instance.gameObject, LoadedPoolScene);
                instance.ShapeId = shapeId;
            }
        }
        else {
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom() {
        return Get(
            Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length)
        );
    }

    public void Reclaim(Shape shapeToRecycle) {
        if (recycle) {
            if (pools == null) {
                CreatePools();
            }
            pools[shapeToRecycle.ShapeId].Add(shapeToRecycle);
            shapeToRecycle.gameObject.SetActive(false);
        }
        else {
            Destroy(shapeToRecycle.gameObject);
        }
    }

    void CreatePools() {
        pools = new List<Shape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++) {
            pools[i] = new List<Shape>();
        }

        if (Application.isEditor) {
            // Debug.Log("Pool scene is in editor");
            poolScene = SceneManager.GetSceneByName(name);
            if (poolScene.isLoaded) {
                GameObject[] rootObjects = poolScene.GetRootGameObjects();
                // Debug.Log("Found existing poll scene with " + rootObjects.Length + " root objects present - reclaiming what we can");
                int reclaimedCount = 0;
                for (int i = 0; i < rootObjects.Length; i++) {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf) {
                        pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                Debug.Log($"Reclaimed {reclaimedCount} shapes from pool's existing scene");
                return;
            }
        }

        poolScene = SceneManager.CreateScene(name);
        // Debug.Log("Pool scene has been created");
    }
}
