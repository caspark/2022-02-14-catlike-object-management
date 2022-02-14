using System;
using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour {
    string savePath;

    private void Awake() {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObject o, int version) {
        Debug.Log("Saving to: " + savePath);
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create))) {
            writer.Write(-version);
            o.Save(new GameDataWriter(writer));
        }
    }
    public void Load(PersistableObject o) {
        if (!File.Exists(savePath)) {
            throw new InvalidOperationException($"No save file found at {savePath} so cannot load game");
        }
        Debug.Log("Loading from: " + savePath);

        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open))) {
            o.Load(new GameDataReader(reader, -reader.ReadInt32()));
        }
    }
}
