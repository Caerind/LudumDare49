using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions
{
    public static Rect GetOrthographicRect(this Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2.0f;
        Vector2 size = new Vector2(cameraHeight * screenAspect, cameraHeight);
        return new Rect(camera.transform.position.ToVector2() - size * 0.5f, size);
    }
}
