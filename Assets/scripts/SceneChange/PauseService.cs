using System;
using UnityEngine;

public class PauseService : MonoBehaviour
{
   public static PauseService Instance { get; private set; }
    private int pauseCount;
    public bool IsPaused=>pauseCount > 0;
    public event Action<bool> PausedChanged;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnDestroy()
    {
        if(Instance==this)
            Instance = null;
    }
    public void Push()
    {
        pauseCount++;
        Apply();
    }
    public void Pop()
    {
        pauseCount=Mathf.Max(pauseCount-1, 0);
        Apply();
    }
    public void Clear()
    {
        pauseCount = 0;
        Apply();
    }
    private void Apply()
    {
        bool paused=pauseCount > 0;
        Time.timeScale = paused ? 0f : 1f;
        PausedChanged?.Invoke(paused);
    }
}
