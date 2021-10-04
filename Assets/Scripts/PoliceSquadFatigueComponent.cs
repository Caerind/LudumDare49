using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoliceSquadFatigueComponent : MonoBehaviour
{
    public RectTransform rect = null;
    public RectTransform gaugeRect = null;

    public Gradient gradient;

    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;

        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        rect.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    public void UpdateFatigue(float fatigue)
    {
        gaugeRect.localScale = new Vector3(1.0f, fatigue, 1.0f);
        if(fatigue > 0.7f)
        {
            transform.GetChild(2).GetComponent<Animator>().SetBool("isHigh", true);
        }
        else
        {
            transform.GetChild(2).GetComponent<Animator>().SetBool("isHigh", false);
        }

        transform.GetChild(0).GetComponent<Image>().color = gradient.Evaluate(fatigue);
    }
}
