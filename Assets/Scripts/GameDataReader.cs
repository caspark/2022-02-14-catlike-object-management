using System.IO;
using UnityEngine;

public class GameDataReader {
    BinaryReader reader;
    public GameDataReader(BinaryReader reader) {
        this.reader = reader;
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
}
