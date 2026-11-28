using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindowDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IPointerDownHandler
{
    public UIWindow window;
    [Header("要拖动的面板")]
    public RectTransform targetPanel;
    private Canvas canvas;
    private void Awake()
    {
       if(window==null)
            window=GetComponentInParent<UIWindow>();
        if (targetPanel == null)
            targetPanel = transform.parent as RectTransform;
        if(targetPanel!=null)
            canvas=targetPanel.GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (targetPanel == null) return;
        float scaleFactor = canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
        targetPanel.anchoredPosition += eventData.delta / scaleFactor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UIWindowManager.Instance?.BringToFront(window);
    }

   
}
