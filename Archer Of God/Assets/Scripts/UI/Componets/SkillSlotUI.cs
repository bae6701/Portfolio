using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image skillIconImage;
    [SerializeField] private Image cooldownOverlayImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private CancellationTokenSource _cts;

    public void SetSkill(Skill skill)
    {
        if (skill != null && skill.skillIcon != null)
        {
            skillIconImage.sprite = skill.skillIcon;
            skillIconImage.enabled = true;
        }
        else
        {
            skillIconImage.enabled = false;
        }
        cooldownOverlayImage.enabled = false;
        cooldownText.enabled = false;
    }

    public void StartVisualCooldown(float totalCooldown)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        RunCooldownVisualTask(totalCooldown, _cts.Token).Forget();
    }

    private async UniTaskVoid RunCooldownVisualTask(float totalCooldown, CancellationToken token)
    {
        cooldownOverlayImage.enabled = true;
        cooldownText.enabled = true;

        float remainingCooldown = totalCooldown;

        while (remainingCooldown > 0)
        {
            if (token.IsCancellationRequested) return;

            cooldownOverlayImage.fillAmount = remainingCooldown / totalCooldown;
            cooldownText.text = Mathf.Ceil(remainingCooldown).ToString();

            remainingCooldown -= Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        cooldownOverlayImage.enabled = false;
        cooldownText.enabled = false;
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
