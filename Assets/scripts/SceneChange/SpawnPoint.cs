using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId;
    public string SpawnId => spawnId;
    public static SpawnPoint Find(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        SpawnPoint[] points=FindObjectsOfType<SpawnPoint>(true);
        foreach (SpawnPoint point in points)
        {
            if(point.spawnId==id)return point;
        }
        return  null;
    }
   
}
