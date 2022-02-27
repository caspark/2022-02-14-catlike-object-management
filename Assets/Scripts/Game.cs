using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : PersistableObject {
    public static Game Instance { get; private set; }

    const int saveVersion = 2;

    [SerializeField] private ShapeFactory shapeFactory;

    [SerializeField] private PersistentStorage storage;

    [SerializeField] private int levelCount;

    public SpawnZone SpawnZoneOfLevel { get; set; }

    public float CreationSpeed { get; set; }
    public float DeletionSpeed { get; set; }

    private List<Shape> shapes;

    private float creationProgress;
    private float deletionProgress;

    private int loadedLevelBuildIndex;

    private void Start() {
        Instance = this;
        shapes = new List<Shape>();

        if (Application.isEditor) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level ")) {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        StartCoroutine(LoadLevel(1));
    }

    private void OnEnable() {
        Instance = this;
    }

    void Update() {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) {
            Debug.Log("Keyboard not found, aborting Game.Update()");
            return;
        }

        if (keyboard.escapeKey.wasPressedThisFrame) {
            Application.Quit();
        }
        else if (keyboard.cKey.wasPressedThisFrame) {
            Debug.Log("Spawn key was pressed");
            CreateShape();
        }
        else if (keyboard.dKey.wasPressedThisFrame) {
            Debug.Log("Delete key was pressed");
            DeleteShape();
        }
        else if (keyboard.nKey.wasPressedThisFrame) {
            Debug.Log("Restart key was pressed");
            BeginNewGame();
        }
        else if (keyboard.sKey.wasPressedThisFrame) {
            Debug.Log("Save key was pressed");
            storage.Save(this, saveVersion);
        }
        else if (keyboard.lKey.wasPressedThisFrame) {
            Debug.Log("Load key was pressed");

            BeginNewGame();
            storage.Load(this);
        }

        int key1Code = 40; // the keycode of the '1' key
        int key0Code = 49; // the keycode of the '0' key
        for (int i = key1Code; i < key0Code; i++) {
            if (keyboard.allKeys[i].wasPressedThisFrame) {
                int digitPressed = i - key1Code + 1;
                if (digitPressed > levelCount) {
                    Debug.Log("Level " + digitPressed + " does not exist so cannot be loaded");
                }
                else {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(digitPressed));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f) {
            creationProgress -= 1f;
            CreateShape();
        }

        deletionProgress += Time.deltaTime * DeletionSpeed;
        while (deletionProgress >= 1f) {
            deletionProgress -= 1f;
            DeleteShape();
        }
    }

    private void DeleteShape() {
        if (shapes.Count > 0) {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(shapes.Count);
        writer.Write(loadedLevelBuildIndex);
        foreach (Shape instance in shapes) {
            writer.Write(instance.ShapeId);
            writer.Write(instance.MaterialId);
            instance.Save(writer);
        }
    }
    public override void Load(GameDataReader reader) {
        int version = reader.Version;
        if (version > saveVersion) {
            throw new InvalidOperationException($"Unsupported future save version: {version}");
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        Debug.Log($"Loading {count} shapes from file with save version {version}");
        for (int i = 0; i < count; i++) {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    private void BeginNewGame() {
        foreach (Shape t in shapes) {
            shapeFactory.Reclaim(t);
        }
        shapes.Clear();
    }

    private void CreateShape() {
        Shape instance = shapeFactory.GetRandom(); ;
        var t = instance.transform;
        t.localPosition = SpawnZoneOfLevel.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1.0f) * Vector3.one;
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance);
    }

    IEnumerator LoadLevel(int levelBuildIndex) {
        Debug.Log("Loading level " + levelBuildIndex);
        enabled = false; // make sure input isn't processed while loading the level (disable update() callback)
        if (loadedLevelBuildIndex > 0) {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }
}
