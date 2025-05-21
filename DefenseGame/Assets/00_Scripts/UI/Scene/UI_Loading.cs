using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Popup
{
    enum Images
    {
        LoadingBar,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        SetProgress(0.0f);
    }

    public void SetProgress(float value)
    {
        value = Mathf.Clamp01(value);

        GetImage((int)Images.LoadingBar).fillAmount = value;
    }
}
