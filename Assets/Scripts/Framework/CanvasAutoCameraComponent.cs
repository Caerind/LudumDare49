using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAutoCameraComponent : MonoBehaviour
{
    [SerializeField] private Camera camToAttach = null;

    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = (camToAttach != null) ? camToAttach : Camera.main;
        }
    }
}
