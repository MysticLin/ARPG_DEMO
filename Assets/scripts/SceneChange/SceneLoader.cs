using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
   public static SceneLoader Instance { get; private set; }
    private bool isLoading;
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
    public void LoadScene(string sceneName,string spawnId)
    {
        if (isLoading || string.IsNullOrEmpty(sceneName)) return;
        StartCoroutine(LoadRoutine(sceneName,spawnId));
    }
    private IEnumerator LoadRoutine(string sceneName,string spawnId)
    {
        isLoading = true;
        yield return SceneManager.LoadSceneAsync(sceneName);
        PlacePlayer(spawnId);
        isLoading = false;
    }
    private void PlacePlayer(string spawnId)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        Rigidbody2D rb=player.GetComponent<Rigidbody2D>();
        if(rb!=null)
            rb.velocity = Vector2.zero;
        SpawnPoint spawn=SpawnPoint.Find(spawnId);
        if (spawn == null) return;
        player.transform.position=spawn.transform.position;
        if (rb != null)
            rb.position = spawn.transform.position;
        Physics2D.SyncTransforms();
    }
}
