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

    public float CreationSpeed { get; set; }
    public float DeletionSpeed { get; set; }

    private List<Shape> shapes;

    private float creationProgress;
    private float deletionProgress;


    private void Awake() {
        shapes = new List<Shape>();
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

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f) {
            creationProgress -= 1f;
            SpawnShape();
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

    private void SpawnShape() {
        Shape instance = shapeFactory.GetRandom(); ;
        var t = instance.transform;
        t.position = Random.insideUnitSphere * 5.0f;
        t.rotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1.0f) * Vector3.one;
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance);
    }
}
