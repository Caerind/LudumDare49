using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBarComponent : MonoBehaviour
{
    public RectTransform rect;

    private void Update()
    {
        rect.localScale = new Vector3(GameManager.Instance.GetChaos(), 1.0f, 1.0f);
    }
}
