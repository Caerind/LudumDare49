using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionMessageComponent : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text = null;
    public Vector2 squadPos;
    public GameObject squad;

    public void OnClick()
    {
        CameraManager.Instance.SetMovingToSquad(squadPos);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (squad != null && squad.GetComponent<PoliceSquadComponent>().GetFatigue() < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
