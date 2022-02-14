using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

public class Game : PersistableObject {

    const int saveVersion = 1;

    [SerializeField] private ShapeFactory shapeFactory;

    [SerializeField] private PersistentStorage storage;

    List<Shape> shapes;

    string savePath;

    private void Awake() {
        shapes = new List<Shape>();
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
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
            SpawnShape();
        }
        else if (keyboard.nKey.wasPressedThisFrame) {
            Debug.Log("Restart key was pressed");
            BeginNewGame();
        }
        else if (keyboard.sKey.wasPressedThisFrame) {
            Debug.Log("Save key was pressed");
            storage.Save(this);
        }
        else if (keyboard.lKey.wasPressedThisFrame) {
            Debug.Log("Load key was pressed");

            BeginNewGame();
            storage.Load(this);
        }
    }


    public override void Save(GameDataWriter writer) {
        writer.Write(-saveVersion);
        writer.Write(shapes.Count);
        foreach (Shape instance in shapes) {
            writer.Write(instance.ShapeId);
            writer.Write(instance.MaterialId);
            instance.Save(writer);
        }
    }
    public override void Load(GameDataReader reader) {
        int version = -reader.ReadInt();
        if (version > saveVersion) {
            throw new InvalidOperationException($"Unsupported future save version: {version}");
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        Debug.Log($"Loading {count} shapes from version {version}");
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
            Destroy(t.gameObject);
        }
        shapes.Clear();
    }

    private void SpawnShape() {
        Shape instance = shapeFactory.GetRandom(); ;
        var t = instance.transform;
        t.position = Random.insideUnitSphere * 5.0f;
        t.rotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1.0f) * Vector3.one;
        shapes.Add(instance);
    }
}
