using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 0.5f;
    public Vector2 newPlayerPosition;
    public string spwanId;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player=collision.transform;
            fadeAnim.Play("FadeToWhite");
            StartCoroutine(DelayFade());
        }
    }
    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(sceneToLoad, spwanId);
            yield break;
        }
        if(player!=null)
            player.position = newPlayerPosition;
        SceneManager.LoadScene(sceneToLoad);
    }
}
