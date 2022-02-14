using System.IO;
using UnityEngine;

public class GameDataReader {
    public int Version { get; }
    BinaryReader reader;
    public GameDataReader(BinaryReader reader, int version) {
        this.reader = reader;
        Version = version;
    }

    public float ReadFloat() {
        return reader.ReadSingle();
    }

    public int ReadInt() {
        return reader.ReadInt32();
    }

    public Quaternion ReadQuaternion() {
        Quaternion r = new Quaternion();
        r.x = reader.ReadSingle();
        r.y = reader.ReadSingle();
        r.z = reader.ReadSingle();
        r.w = reader.ReadSingle();
        return r;
    }

    public Vector3 ReadVector3() {
        Vector3 r = new Vector3();
        r.x = reader.ReadSingle();
        r.y = reader.ReadSingle();
        r.z = reader.ReadSingle();
        return r;
    }

    public Color ReadColor() {
        Color r = new Color();
        r.r = reader.ReadSingle();
        r.g = reader.ReadSingle();
        r.b = reader.ReadSingle();
        r.a = reader.ReadSingle();
        return r;
    }
}
