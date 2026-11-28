using System;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour,IPointerDownHandler
{
    public CanvasGroup canvasGroup;
    public bool pausesGame = true;
    public bool IsOpen {  get; private set; }
    public event Action OnShown;
    public event Action OnHidden;
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }
    public void Show()
    {
        IsOpen = true;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts =true;
        canvasGroup.interactable = true;
        OnShown?.Invoke();
    }
    public void Hide()
    {
        bool wasOpen=IsOpen;
        IsOpen = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        if(wasOpen)
        OnHidden?.Invoke();
    }
    public void OnPointerDown(PointerEventData evenData)
    {
        UIWindowManager.Instance?.BringToFront(this);
    }
}
