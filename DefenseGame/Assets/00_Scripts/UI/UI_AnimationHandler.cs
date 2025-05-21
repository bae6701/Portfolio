using UnityEngine;

public class UI_AnimationHandler : MonoBehaviour
{
    public void DeActiveObject() => gameObject.SetActive(false);
    public void ActiveObject() => gameObject.SetActive(true);

    public void AnimationReturn() => GetComponent<Animator>().speed = 1.0f;
}
