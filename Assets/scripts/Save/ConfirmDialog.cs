using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    private Action onConfirm;
    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
        Hide();

    }
    public void Show(string message,Action callback)
    {
        onConfirm = callback;
        messageText.text = message;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    private void Confirm()
    {
        Action callback = onConfirm;
        onConfirm = null;
        Hide();
        callback?.Invoke();
    }
    private void Cancel()
    {
        onConfirm = null;
        Hide();
    }

    private void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
