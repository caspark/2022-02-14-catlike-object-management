using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

public class Game : PersistableObject {
    [SerializeField] private PersistableObject cubePrefab;

    [SerializeField] private PersistentStorage storage;

    List<PersistableObject> objects;

    string savePath;

    private void Awake() {
        objects = new List<PersistableObject>();
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
            SpawnCube();
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

    public override void Load(GameDataReader reader) {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++) {
            Transform t = Instantiate(cubePrefab).transform;
            t.GetComponent<PersistableObject>().Load(reader);
            objects.Add(t.GetComponent<PersistableObject>());
        }
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(objects.Count);
        foreach (PersistableObject t in objects) {
            t.Save(writer);
        }
    }

    private void BeginNewGame() {
        foreach (PersistableObject t in objects) {
            Destroy(t.gameObject);
        }
        objects.Clear();
    }

    private void SpawnCube() {
        PersistableObject persistableObject = Instantiate(cubePrefab);
        GameObject cube = persistableObject.gameObject;
        var t = cube.transform;
        t.position = Random.insideUnitSphere * 5.0f;
        t.rotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1.0f) * Vector3.one;
        objects.Add(persistableObject);
    }
}
