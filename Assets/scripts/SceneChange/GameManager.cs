using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
    public DialogueManager DialogueManager;
    public DialogueHistoryTracker DialogueHistoryTracker;
    public LocatioHistroyTracker LocatioHistroyTracker;
    [Header("持久化对象")]
    public GameObject[] persistentObjects;
    private void Awake()
    {
        if (Instance != null)
        {
            CleanUpAndDestory();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }
    private void MarkPersistentObjects()
    {
        foreach(GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad (obj);
            }
        }
    }
    private void CleanUpAndDestory()
    {
        foreach(GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }
}
