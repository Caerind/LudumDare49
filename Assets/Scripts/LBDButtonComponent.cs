using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LBDButtonComponent : MonoBehaviour
{
    private bool clicked = false;
    [SerializeField] private Image filler = null;

    public bool GetClickedAndReset()
    {
        bool wasClicked = clicked;
        clicked = false;
        return wasClicked;
    }

    public void OnClick()
    {
        clicked = true;
    }

    public void SetCooldownValue(float a)
    {
        filler.fillAmount = Mathf.Clamp01(1.0f - a);
    }
}
