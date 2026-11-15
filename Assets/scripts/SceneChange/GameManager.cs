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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
