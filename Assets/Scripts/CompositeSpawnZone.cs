using UnityEngine;

public class CompositeSpawnZone : SpawnZone {
    [SerializeField] SpawnZone[] spawnZones;

    public override Vector3 SpawnPoint {
        get {
            Debug.Assert(spawnZones.Length > 0, $"No spawn zones associated with {name}");
            return spawnZones[Random.Range(0, spawnZones.Length)].SpawnPoint;
        }
    }
}
