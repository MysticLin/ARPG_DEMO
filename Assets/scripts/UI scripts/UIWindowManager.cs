using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindowManager : MonoBehaviour
{
    public static UIWindowManager Instance {  get; private set; }
    private readonly LinkedList<UIWindow> windows = new();
    private bool paused;
    private int topSortingOrder = 100;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance= this;
    }
    private void OnDisable()
    {
        if (paused)
        {
            PauseService.Instance?.Pop();
            paused = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && windows.Count > 0)
            CloseTop();
    }
    public void Toggle(UIWindow window)
    {
        if(window==null)return;
        if (!window.IsOpen)
        {
            Open(window);
            return;
        }
        if (windows.Last == null || windows.Last.Value != window)
        {
            BringToFront(window);
            return;
        }
        Close(window);
    }
    public void Open(UIWindow window)
    {
        if (window == null) return;
        window.Show();
        windows.Remove(window);
        windows.AddLast(window);
        window.transform.SetAsLastSibling();//渲染到同一父物体的最下面，也就是最先展示
        ApplySorting(window);
        SyncPause();
    }
    public void Close(UIWindow window)
    {
        if(window==null)return;
        window.Hide();
        windows.Remove(window);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);//清空输入焦点
        SyncPause();
    }
    public void CloseTop()
    {
        if (windows.Last == null) return;
        Close(windows.Last.Value);
    }
    public void BringToFront(UIWindow window)
    {
        if (window == null || !window.IsOpen) return;
        windows.Remove(window);
        windows.AddLast(window);
        window.transform.SetAsLastSibling();
        ApplySorting(window);
    }
    private void ApplySorting(UIWindow window)
    {
        if (window == null) return;
        Canvas canvas=window.GetComponent<Canvas>();
        if (canvas == null)
            canvas = window.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        canvas.overrideSorting=true;
        canvas.sortingOrder = ++topSortingOrder;
    }
    private void SyncPause()
    {
        bool needPause = false;
        foreach(var window in windows)
        {
            if (window != null && window.pausesGame)
            {
                needPause = true;
                break;
            }
        }
        if (needPause && !paused)
        {
            PauseService.Instance?.Push();
            paused = true;
        }
        else if (!needPause && paused)
        {
            PauseService.Instance?.Pop();
            paused = false;
        }
    }
}
