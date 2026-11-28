using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    public UIWindow window;
    private void Awake()
    {
        if (window == null)
            window = GetComponentInParent<UIWindow>();
        if (window != null)
            window.OnShown += RefreshSkills;
    }
    private void OnDestroy()
    {
        if(window!=null)
            window.OnShown -= RefreshSkills;
    }
    private void Update()
    {
        if (DialogueManager.IsDialogueActive) return;
        if (Input.GetButtonDown("ToggleSkillTree"))
        {
            if (window != null)
                UIWindowManager.Instance?.Toggle(window);
        }
    }
    private void RefreshSkills()
    {
        SkillManager.Instance?.ApplyRestoredSkills();
        SkillTreeManager skillTree = FindObjectOfType<SkillTreeManager>(true);
        if (skillTree != null)
            skillTree.RefreshSkillUI();
    }
}
