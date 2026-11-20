using System.Collections.Generic;
using UnityEngine;

public class LootPool : MonoBehaviour
{
    [SerializeField] private Loot LootPrefab;
    [SerializeField] private int initialSize = 5;
    private readonly Queue<Loot> pool = new();
    private void Awake()
    {
        for(int i = 0; i < initialSize; i++)
            pool.Enqueue(Create());
    }
    private Loot Create()
    {
        Loot loot = Instantiate(LootPrefab, transform);
        loot.gameObject.SetActive(false);
        loot.Pool = this;
        return loot;    
    }
    public Loot Get(ItemSO item,int quantity,Vector2 position)
    {
        Loot loot=pool.Count>0?pool.Dequeue():Create();
        loot.transform.SetParent(null);
        loot.transform.position = position;
        loot.gameObject.SetActive(true);
        loot.Initialize(item, quantity);
        return loot;
    }
    public void Release(Loot loot)
    {
        loot.CancelInvoke();
        loot.gameObject.SetActive(false);
        loot.transform.SetParent(transform);
        pool.Enqueue(loot);
    }

}
