using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private int initialSize = 10;
    private readonly Queue<Arrow> pool = new();
    private void Awake()
    {
        for(int i=0;i<initialSize;i++)
            pool.Enqueue(Create());
    }
    private Arrow Create()
    {
        Arrow arrow = Instantiate(arrowPrefab, transform);
        arrow.gameObject.SetActive(false);
        arrow.Pool = this;
        return arrow;
    }
    public Arrow Get(Vector2 position)
    {
        Arrow arrow = pool.Count > 0 ? pool.Dequeue() : Create();
        arrow.transform.SetParent(null);
        arrow.transform.localScale=Vector3.one;
        arrow.transform.position= position;
        arrow.gameObject.SetActive(true);
        return arrow;
    }
    public void Release(Arrow arrow)
    {
        arrow.CancelInvoke();
        arrow.gameObject.SetActive(false);
        arrow.transform.SetParent(transform);
        pool.Enqueue(arrow);
    }
}
