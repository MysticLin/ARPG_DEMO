using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuBar;
    private bool isMenuActive;
    private bool menuPaused;
    [SerializeField] private CanvasGroup statsMenu;
    [SerializeField] private CanvasGroup skillsMenu;
    [SerializeField] private CanvasGroup questsMenu;

    [SerializeField] private Image menuToggleImage;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;


    public void ToggleMenu(CanvasGroup target)
    {
        if (DialogueManager.IsDialogueActive) return;
        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu,false);
        SetMenuState(questsMenu, false);
        SetMenuState(target, true);
        SyncPasueState();
    }
    public void ToggleMainMenu()
    {
        if (DialogueManager.IsDialogueActive) return;
        isMenuActive = !isMenuActive;
        SetMenuState(menuBar, isMenuActive);
        menuToggleImage.sprite =isMenuActive ? closeSprite : openSprite;
        SetMenuState(statsMenu, false);
        SetMenuState(skillsMenu, false);
        SetMenuState(questsMenu, false);
        EventSystem.current.SetSelectedGameObject(null);
        SyncPasueState();

    }
    public void SetMenuState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
    private bool IsAnyMenuOpen()
    {
        return isMenuActive || IsCanvasOpen(statsMenu) || IsCanvasOpen(skillsMenu)
            || IsCanvasOpen(questsMenu);
    }
    private bool IsCanvasOpen(CanvasGroup group)
    {
        return group != null && group.alpha > 0 && group.blocksRaycasts;
    }
    private void SyncPasueState()
    {
        bool shouldPasue=IsAnyMenuOpen();
        if (shouldPasue&&!menuPaused)
        {
            PauseService.Instance?.Push();
            menuPaused = true;
        }
        else if (!shouldPasue && menuPaused)
        {
            PauseService.Instance?.Pop();
            menuPaused = false;
        }
    }
    private void OnDisable()
    {
        if (menuPaused)
        {
            PauseService.Instance?.Pop();
            menuPaused = false;
        }
    }
}
