using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ConfinerFinder : MonoBehaviour
{
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
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        if (confiner == null) return;
        GameObject confinerObject = GameObject.FindWithTag("Confiner");
        if (confinerObject == null) return;
        PolygonCollider2D polygon=confinerObject.GetComponent<PolygonCollider2D>();
        if (polygon != null)
            confiner.m_BoundingShape2D = polygon;
    }
}
