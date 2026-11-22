using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
  public static HitStop instance {  get; private set; }
    private Coroutine routine;
    private float endTime;
    private void Awake()
    {
        instance=this;
    }
   public void Play(float duration = 0.05f)
    {
        if (PauseService.Instance != null && PauseService.Instance.IsPaused)
            return;
        endTime = Mathf.Max(endTime, Time.unscaledTime + duration);
        if (routine == null)
            routine = StartCoroutine(Run());
    }
    private IEnumerator Run()
    {
        Time.timeScale = 0f;
        while (Time.unscaledTime < endTime)
            yield return null;
        Time.timeScale=PauseService.Instance!=null&&
            PauseService.Instance.IsPaused?0f:1f;
        routine = null;
    }
}
