using System.Collections.Generic;
using UnityEngine;

public class LocatioHistroyTracker : MonoBehaviour
{
    private readonly HashSet<LocationSO> locationVisited = new HashSet<LocationSO>();
    public void RecordLocation(LocationSO locationSO)
    {
       if(locationVisited.Add(locationSO))
        {
            Debug.Log("Just visited to"+locationSO.displayName);
        }
    }
    public bool HasVisited(LocationSO locationSO)
    {
        return locationVisited.Contains(locationSO);
    }
    public List<string> ExportVisitedLocationIds()
    {
        List<string> result = new();
        foreach (LocationSO location in locationVisited)
            if (location != null) result.Add(location.name);
        return result;
    }
    public void ImportVisitedLocationIds(List<string> ids,ItemDatabase database)
    {
        locationVisited.Clear();
        if (ids == null || database == null) return;
        foreach(string id in ids)
        {
            LocationSO location=database.FindLocation(id);
            if(location!=null)locationVisited.Add(location);
        }
    }
}
