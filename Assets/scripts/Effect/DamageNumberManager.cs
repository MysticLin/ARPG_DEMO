using UnityEngine;

public static class DamageNumberManager
{
    private static GameObject prefab;
    public static void Spawn(int value,Vector3 position,Color color)
    {
        if (prefab == null)
            prefab = Resources.Load<GameObject>("DamageNumber");
        GameObject go = Object.Instantiate(prefab, position, Quaternion.identity);
        go.GetComponent<DamageNumber>().Show(value, color);
    }
}
