using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicemanComponent : MonoBehaviour
{
    private bool hasLBD = false;
    private SpriteRenderer spriteRenderer = null;
    private Animator animator = null;
    private PoliceSquadComponent squad = null;
    private int indexInRow = 0;
    private Vector2 posInRow = Vector2.zero;
    private GameObject protesterFocus = null;
    public float mvtSpeed = 3.0f;
    public float hitDistance = 0.5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance?.AddEntity(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.RemoveEntity(gameObject);
    }

    public void UpdateRow(PoliceSquadComponent _squad, int _indexInRow, Vector2 _posInRow)
    {
        squad = _squad;
        indexInRow = _indexInRow;
        posInRow = _posInRow;
    }

    public void SetHasLBD(bool _hasLBD)
    {
        hasLBD = _hasLBD;
        animator.SetBool("hasLBD", hasLBD);
    }

    public void ShootLBD()
    {
        if (hasLBD)
        {
            GameManager.Instance.AddChaosLBD();
            animator.SetTrigger("shootLBD");

            // TODO : 0.1f later
            GameManager.Instance.ThrowProjectile(transform.position.ToVector2(), squad.GetLBDShootPoint(indexInRow), false);
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        Vector2 targetPos = GetTargetPos();

        Vector3 delta = (targetPos - transform.position.ToVector2()).ToVector3();
        if (delta.sqrMagnitude > 0.01f) // Walking
        {
            // Mvt
            transform.position += delta.normalized * mvtSpeed * Time.deltaTime;
        }
        else
        {
            if (IsFocusingProtester())
            {
                GameManager.Instance.AddChaosHit();
                AudioManager.Play("batonHit");
                AudioManager.Play("hurt" + Random.Range(1, 5).ToString());
                animator.SetTrigger("batonHit");
                ProtesterFlee();
            }
        }
    }

    private void ProtesterFlee()
    {
        ProtesterComponent protester = protesterFocus.GetComponent<ProtesterComponent>();
        if (protester != null)
        {
            protester.UnfocusSquad();
        }
        squad.RemoveProtester(protesterFocus);
        UnfocusProtester();
    }

    public void UpdateDirection()
    {
        Vector2 targetPos;
        if (IsFocusingProtester())
        {
            targetPos = protesterFocus.transform.position.ToVector2();
        }
        else
        {
            targetPos = GameManager.Instance.GetZoneCenter();
        }

        float dx = targetPos.x - transform.position.x;
        if (dx >= 0.0f)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private Vector2 GetTargetPos()
    {
        Vector2 targetPos;
        if (IsFocusingProtester())
        {
            Vector3 pPos = protesterFocus.transform.position;
            Vector2 deltaProt = (pPos - transform.position).normalized.ToVector2();
            targetPos = pPos.ToVector2() - deltaProt * hitDistance;
        }
        else
        {
            targetPos = posInRow;
        }
        return targetPos;
    }

    public bool IsAtItsPost()
    {
        return Vector3.Distance(transform.position, posInRow) < 0.1f;
    }

    public bool IsFocusingProtester()
    {
        return protesterFocus != null;
    }

    public void FocusProtester(GameObject protester)
    {
        protesterFocus = protester;
        AudioManager.Play("batonOpen");
    }

    public void UnfocusProtester()
    {
        protesterFocus = null;
    }
}
