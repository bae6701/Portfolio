using UnityEngine;

public class AnimEventBridge : MonoBehaviour
{
    private Character character;

    void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    public void BasicAttack()
    {
        character?.BasicAttack();
    }
    public void SkillAttack()
    {
        character?.SkillAttack();
    }

    public void SkillEnd()
    {
        character?.EndSkillUsage();
    }
}
