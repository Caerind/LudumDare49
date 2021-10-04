using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlerteManager : Singleton<AlerteManager>
{
    public GameObject attentionMessagePrefab;
    public Transform root;
    public float messageHeight;
    private float bonusX = 100.0f;
    private float bonusY = 100.0f;

    private void Start()
    {
        bonusX = Screen.width * 0.25f;
        bonusY = Screen.height * 0.33f;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        for (int i = 0; i < root.childCount; ++i)
        {
            root.GetChild(i).GetComponent<RectTransform>().position = i * messageHeight * Vector3.up + new Vector3(bonusX, bonusY, 0.0f);
            root.GetChild(i).GetComponent<RectTransform>().localScale = new Vector3(0.51f, 0.51f, 0.51f); // Don't ever ask me why
        }
    }

    public void SpawnAttentionMessage(string message, Vector2 squadPos, GameObject squad)
    {
        GameObject ins = Instantiate(attentionMessagePrefab);
        ins.transform.SetParent(root);
        AttentionMessageComponent attention = ins.GetComponent<AttentionMessageComponent>();
        attention.squadPos = squadPos; // Save pos from current squad
        attention.text.text = message;
        attention.squad = squad;
    }
}
