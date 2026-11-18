using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTracker : MonoBehaviour
{
    public static DialogueHistoryTracker Instance;
    private readonly HashSet<ActorSO> spokernNPCs = new HashSet<ActorSO>();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void RecordNPC(ActorSO actorSO)
    {
        if(spokernNPCs.Add(actorSO))
        Debug.Log("Just spoke to" + actorSO.actorName);
    }
    public bool HasSpokenWith(ActorSO actorSO)
    {
        return spokernNPCs.Contains(actorSO);
    }
    public List<string> ExportSokenActorIds()
    {
        List<string> result = new();
        foreach(ActorSO actor in spokernNPCs)
            if(actor!=null)result.Add(actor.name);
        return result;
    }
    public void ImportSpokenActorIds(List<string> ids, ItemDatabase database)
    {
        spokernNPCs.Clear();
        if (ids == null || database == null) return;
        foreach(string id in ids)
        {
            ActorSO actor=database.FindActor(id);
            if(actor!=null)spokernNPCs.Add(actor);
        }
    }
}
