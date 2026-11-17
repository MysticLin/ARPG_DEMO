using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    public CanvasGroup statsCanvas;
    private bool skillTreeOpen = false;
    private void Update()
    {
        if (DialogueManager.IsDialogueActive) return;
        if (Input.GetButtonDown("ToggleSkillTree"))
        {
            if (skillTreeOpen)
            {
                PauseService.Instance?.Pop();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
                skillTreeOpen = false;
            }
            else{
                PauseService.Instance?.Push();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true;
                skillTreeOpen = true;
            }
        }
    }
    private void OnDisable()
    {
        if (skillTreeOpen)
        {
            skillTreeOpen = false;
            PauseService.Instance?.Pop();
            if (statsCanvas != null)
            {
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false;
            }
        }
    }
}
