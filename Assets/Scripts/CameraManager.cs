using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Vector2 targetPos;
    private bool isMovingToSquad = false;
    public float movingToSquadSpeed = 45.0f;

    public float panSpeed = 20.0f;
    public float panBorderThickness = 100.0f;

    public Rect camMaxBounds;

    [SerializeField] private float dragSpeed = 2.0f;
    private Vector2 camPressPosScreenSpace;
    private bool camPressed = false;
    private bool isMouse = false;

    private void LateUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        if (camPressed && isMouse && !Input.GetMouseButton(0))
        {
            camPressed = false;
        }
        if (camPressed && !isMouse && Input.touchCount == 0)
        {
            camPressed = false;
        }

        if (isMovingToSquad)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.x, targetPos.y, -10.0f), movingToSquadSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position.ToVector2(), targetPos) < 0.1f)
            {
                isMovingToSquad = false;
            }
        }
        else if (camPressed)
        {
            Vector2 screenSpace;
            if (isMouse)
            {
                screenSpace = Input.mousePosition;
            }
            else
            {
                screenSpace = Input.GetTouch(0).position;
            }

            Vector3 pos = Camera.main.ScreenToViewportPoint(camPressPosScreenSpace - screenSpace);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0.0f);

            transform.Translate(move, Space.World);
        }
        else // Keyboard Pan
        {
            Vector3 pos = transform.position;
            if (Input.GetKey("w") || Input.GetKey("z") || Input.GetKey(KeyCode.UpArrow))
            {
                pos.y += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow))
            {
                pos.y -= panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.GetKey("q") || Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
            {
                pos.x -= panSpeed * Time.deltaTime;
            }
            transform.position = pos;
        }

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2.0f;
        Vector2 size = new Vector2(cameraHeight * screenAspect, cameraHeight);

        Vector3 retargetPos = transform.position;
        retargetPos.x = Mathf.Clamp(retargetPos.x, camMaxBounds.x + size.x * 0.5f, camMaxBounds.x + camMaxBounds.width - size.x * 0.5f);
        retargetPos.y = Mathf.Clamp(retargetPos.y, camMaxBounds.y + size.y * 0.5f, camMaxBounds.y + camMaxBounds.height - size.y * 0.5f);
        retargetPos.z = -10.0f;
        if (retargetPos != transform.position)
        {
            isMovingToSquad = false;
            transform.position = retargetPos;
        }
    }

    public void SetMovingToSquad(Vector2 squadPos)
    {
        isMovingToSquad = true;
        targetPos = squadPos;
    }

    public void StartCamPress(Vector2 _camPressPosScreenSpace, bool _isMouse)
    {
        camPressPosScreenSpace = _camPressPosScreenSpace;
        camPressed = true;
        isMouse = _isMouse;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(camMaxBounds.x, camMaxBounds.y, 0.0f), new Vector3(camMaxBounds.x + camMaxBounds.width, camMaxBounds.y, 0.0f));
        Gizmos.DrawLine(new Vector3(camMaxBounds.x + camMaxBounds.width, camMaxBounds.y, 0.0f), new Vector3(camMaxBounds.x + camMaxBounds.width, camMaxBounds.y + camMaxBounds.height, 0.0f));
        Gizmos.DrawLine(new Vector3(camMaxBounds.x + camMaxBounds.width, camMaxBounds.y + camMaxBounds.height, 0.0f), new Vector3(camMaxBounds.x, camMaxBounds.y + camMaxBounds.height, 0.0f));
        Gizmos.DrawLine(new Vector3(camMaxBounds.x, camMaxBounds.y + camMaxBounds.height, 0.0f), new Vector3(camMaxBounds.x, camMaxBounds.y, 0.0f));
    }
}
