using UnityEngine;

public class PersistentRoot : MonoBehaviour
{
    public static PersistentRoot Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (WorldStateManager.Instance == null)
            gameObject.AddComponent<WorldStateManager>();
    }
    private void OnDestroy()
    {
        if(Instance==this) Instance = null; 
    }
}

