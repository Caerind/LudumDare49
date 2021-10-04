using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    public Vector2 targetPos;
    public float speed = 10.0f;

    private bool isLBD = false;

    private void Awake()
    {
        isLBD = GetComponent<SpriteRenderer>().sprite == GameManager.Instance.projectileLBD;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.x, targetPos.y, 0.0f), speed * Time.deltaTime);
        if (Vector2.Distance(transform.position.ToVector2(), targetPos) < 0.1f)
        {
            if (!isLBD)
            {
                AudioManager.Play("proj");
            }
            Destroy(gameObject);
        }
    }
}
