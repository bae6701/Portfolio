using UnityEngine;
using UnityEngine.UI;

public class SkillUIProcess : MonoBehaviour
{
    [SerializeField] private SkillSlotUI[] skillSlots;
    private PlayerController player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        player.OnSkillUsed += OnPlayerSkillUsed;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (i < player.skills.Count)
            {
                skillSlots[i].SetSkill(player.skills[i]);
                int skillIndex = i;
                skillSlots[i].GetComponent<Button>().onClick.AddListener(() => player.UseSkill(skillIndex));
            }
            else
            {
                skillSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnPlayerSkillUsed(int skillIndex, float cooldown)
    {
        if (skillIndex < skillSlots.Length)
        {
            skillSlots[skillIndex].StartVisualCooldown(cooldown);
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnSkillUsed -= OnPlayerSkillUsed;
        }
    }
}
