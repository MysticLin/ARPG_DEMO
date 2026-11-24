using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldStateManager : MonoBehaviour
{
   public static WorldStateManager Instance { get; private set; }
    private readonly Dictionary<string, SceneWorldState> states = new();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        ApplyCurrentScene();
    }
    public SceneWorldState GetOrCreate(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return null;
        if (!states.TryGetValue(sceneName, out var state))
        {
            state = new SceneWorldState { sceneName = sceneName };
            states[sceneName] = state;
        }
        return state;
    }
    public bool IsLootRemoved(string sceneName,string lootId)
    {
        if(string.IsNullOrEmpty(sceneName)||string.IsNullOrEmpty(lootId))
            return false;
        return states.TryGetValue(sceneName, out var state) && state.removedLootIds.Contains(lootId);
    }
    public void MarkLootRemoved(string sceneName,string lootId)
    {
        if (string.IsNullOrEmpty(lootId)) return;
        var state = GetOrCreate(sceneName);
        if(state!=null&&!state.removedLootIds.Contains(lootId))
            state.removedLootIds.Add(lootId);
    }
    public bool IsEnemyDead(string sceneName,string enemyId)
    {
        if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(enemyId))
            return false;
        return states.TryGetValue(sceneName, out var state) && state.deadEnemyIds.Contains(enemyId);
    }
    public void MarkEnemyDead(string sceneName,string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId)) return;
        var state=GetOrCreate(sceneName);
        if(state!=null&&!state.deadEnemyIds.Contains(enemyId))
            state.deadEnemyIds.Add(enemyId);
    }
    public List<SceneWorldState> Export()
    {
        return new List<SceneWorldState>(states.Values);
    }
    public void Import(List<SceneWorldState> data)
    {
        states.Clear();
        if (data == null) return;
        foreach(var state in data)
        {
            if(state==null||string.IsNullOrEmpty(state.sceneName))
                continue;
            states[state.sceneName]=state;
        }
    }
    public void Clear()
    {
        states.Clear();
    }
    public void ApplyCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        foreach(var loot in FindObjectsOfType<Loot>(true))
            loot.ApplyWorldState(sceneName);
        foreach (var enemy in FindObjectsOfType<Enemy_Health>(true))
            enemy.ApplyWorldState(sceneName);
    }
}
    
[Serializable]
public class SceneWorldState
{
    public string sceneName;
    public List<string> removedLootIds = new();
    public List<string> deadEnemyIds=new();
}
