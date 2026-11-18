using System.Runtime.CompilerServices;
using UnityEngine;
[CreateAssetMenu(fileName ="ItemDatabase",menuName ="Game/Item Database")]
public class ItemDatabase :ScriptableObject
{
    [SerializeField] private ItemSO[] items;
    public ItemSO Find(string id)
    {
        if(string.IsNullOrEmpty(id)||items==null)
            return null;
        foreach(ItemSO item in items)
        {
            if(item!=null&&item.name==id)
                return item;
        }
        return null;
    }
}
