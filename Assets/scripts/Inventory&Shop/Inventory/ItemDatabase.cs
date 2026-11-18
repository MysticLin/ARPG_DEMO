using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
[CreateAssetMenu(fileName ="ItemDatabase",menuName ="Game/Item Database")]
public class ItemDatabase :ScriptableObject
{
    [SerializeField] private ItemSO[] items;
    [SerializeField] private QuestSO[] quests;
    [SerializeField] private ActorSO[] actors;
    [SerializeField] private LocationSO[] locations;
    public ItemSO FindItem(string id)
    {
        if (items == null) return null;
        foreach(ItemSO item in items)
            if(item!=null&&item.name==id)return item;
        return null;
    }
    public QuestSO FindQuest(string id)
    {
        if(quests==null)return null;
        foreach(QuestSO quest in quests)
            if(quest!=null&&quest.name==id)return quest;
        return null;
    }
    public ActorSO FindActor(string id)
    {
        if (actors == null) return null;
        foreach(ActorSO actor in actors)
            if(actor!=null&&actor.name==id)return actor;
        return null;
    }
    public LocationSO FindLocation(string id)
    {
        if(locations == null) return null;
        foreach(LocationSO location in locations)
            if(location!=null&&location.name==id)return location;
        return null;
    }
}
