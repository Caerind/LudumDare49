using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtesterComponent : MonoBehaviour
{
    public float speed = 2.0f;
    public float targetReachedDistance = 0.2f;
    public bool hasProjectile = false;

    private PoliceSquadComponent policeSquad = null;
    private Vector2 targetPosition = Vector2.zero;

    private int bodyType;
    private SpriteRenderer bodySprite;
    private SpriteRenderer headSprite;
    private SpriteRenderer pancarteSprite;
    private int walkCurrentIndex;
    public float walkTimeToChange = 0.5f;
    private float walkAccumulator = 0.0f;

    public float maxTimeWaitReachPos = 1.0f;
    private float waitTimeRemaining = 0.0f;

    public float fleeSpeed = 8.0f;
    public float fleeSpeedDuration = 2.0f;
    private float fleeSpeedRemaining = 0.0f;

    public float throwChance = 0.9999f;
    public float throwMinProt = 4;

    public float pancarteChance = 0.25f;

    private void Awake()
    {
        headSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        bodySprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        pancarteSprite = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.Instance?.AddEntity(gameObject);

        // Body
        int maxTypeIndex = GameManager.Instance.protestersWalkSprites.Count / GameManager.Instance.protestersSpritesPerType;
        bodyType = Random.Range(0, maxTypeIndex);
        bodySprite.sprite = GameManager.Instance.protestersWalkSprites[bodyType * GameManager.Instance.protestersSpritesPerType];
        // TODO : Determine 'weapon'

        // Head
        headSprite.sprite = GameManager.Instance.protestersHeadSprites[Random.Range(0, GameManager.Instance.protestersHeadSprites.Count - 1)];

        // Pancarte
        float r = Random.Range(0.0f, 1.0f);
        if (r <= pancarteChance)
        {
            pancarteSprite.enabled = true;
            pancarteSprite.sprite = GameManager.Instance.protestersPancartesSprites[Random.Range(0, GameManager.Instance.protestersPancartesSprites.Count - 1)];
        }
        else
        {
            pancarteSprite.enabled = false;
        }
    }
    private void OnDestroy()
    {
        GameManager.Instance?.RemoveEntity(gameObject);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        bool reachedPosBefore = Vector2.Distance(targetPosition, transform.position.ToVector2()) < targetReachedDistance;

        if (IsFleeing())
        {
            fleeSpeedRemaining -= Time.deltaTime;
        }

        Vector3 delta = (targetPosition - transform.position.ToVector2()).ToVector3();
        if (delta.sqrMagnitude > 0.01f) // Walking
        {
            // Walk animation
            walkAccumulator += Time.deltaTime;
            if (walkAccumulator > walkTimeToChange)
            {
                walkAccumulator = 0.0f;
                walkCurrentIndex = (walkCurrentIndex + 1) % 2;
                bodySprite.sprite = GameManager.Instance.protestersWalkSprites[bodyType * GameManager.Instance.protestersSpritesPerType + walkCurrentIndex];
            }

            float spd = (IsFleeing()) ? fleeSpeed : speed;

            // Mvt
            transform.position += delta.normalized * spd * Time.deltaTime;
        }
        else
        {
            // Reset walk anim
            if (walkCurrentIndex == 1)
            {
                walkAccumulator = 0.0f;
                walkCurrentIndex = 0;
                bodySprite.sprite = GameManager.Instance.protestersWalkSprites[bodyType * GameManager.Instance.protestersSpritesPerType + walkCurrentIndex];
            }
        }

        if (Vector2.Distance(targetPosition, transform.position.ToVector2()) < targetReachedDistance) // Idling
        {
            if (IsFocusingSquad()) // Fighting
            {
                if (policeSquad.GetProtestersCount() >= throwMinProt)
                {
                    float r = Random.Range(0.0f, 1.0f);
                    if (r > throwChance)
                    {
                        policeSquad.AddFatigue(GameManager.Instance.fatiguePerProjectile);
                        GameManager.Instance.ThrowProjectile(transform.position.ToVector2(), policeSquad.GetRandomPointInProtesterThrowRange(), true);
                    }
                }
            }
            else // Idling
            {
                if (!reachedPosBefore)
                {
                    // Test if we target squad or not
                    GameObject squad = GameManager.Instance.GetClosestPoliceSquad(transform.position);
                    PoliceSquadComponent squadComp = squad.GetComponent<PoliceSquadComponent>();
                    Vector2 squadCenter = squad.transform.position.ToVector2();
                    Vector2 zoneCenter = GameManager.Instance.GetZoneCenter();
                    Vector2 position = transform.position.ToVector2();

                    float factor1 = Vector2.Distance(position, zoneCenter) / GameManager.Instance.GetZoneRadius(); // How far from the center of the zone
                    float factor2 = Vector2.Dot((squadCenter - zoneCenter).normalized, (position - zoneCenter).normalized);
                    if (factor1 * factor2 > 0.9f)
                    {
                        policeSquad = squadComp;
                        squadComp.AddProtester(gameObject);
                    }
                    else
                    {
                        waitTimeRemaining = Random.Range(0.0f, maxTimeWaitReachPos);
                    }
                }
                else
                {
                    waitTimeRemaining -= Time.deltaTime;
                    if (waitTimeRemaining <= 0.0f)
                    {
                        SetTargetPosition(GameManager.Instance.GetRandomPointInZone()); // Continue Idling
                    }
                }
            }
        }
        else
        {
            // Moving, nothing here
        }
    }

    public void SetTargetPosition(Vector2 targetPos)
    {
        targetPosition = targetPos;

        float dx;
        if (!IsFocusingSquad())
        {
            dx = targetPosition.x - transform.position.x; // If moving, look where you move
        }
        else
        {
            dx = policeSquad.transform.position.x - transform.position.x; // Otherwise, look at the cops
        }

        Vector3 s = transform.localScale;
        s.x = Mathf.Sign(dx) * Mathf.Abs(s.x); // Look in direction of target, keep the scale coef

        // Pancarte flipping
        if (pancarteSprite.enabled)
        {
            if (dx < 0.0f)
            {
                pancarteSprite.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f); // Inverse the current scale
            }
            else
            {
                pancarteSprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        transform.localScale = s;
    }

    public bool IsFocusingSquad()
    {
        return policeSquad != null;
    }

    public PoliceSquadComponent GetFocusedSquad()
    {
        return policeSquad;
    }

    public void UnfocusSquad()
    {
        policeSquad = null;
        fleeSpeedRemaining = fleeSpeedDuration;
        SetTargetPosition(GameManager.Instance.GetRandomPointInZone());
    }

    public bool IsOverlapping(Vector2 position)
    {
        return bodySprite.bounds.Contains(position) || headSprite.bounds.Contains(position);
    }

    public bool IsFleeing()
    {
        return fleeSpeedRemaining > 0.0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if (IsFocusingSquad()) // Targeting squad
        {
            Gizmos.DrawLine(policeSquad.transform.position, transform.position);
            Gizmos.DrawSphere(policeSquad.transform.position, 0.1f);
        }
        else // Idling
        {
            Gizmos.DrawLine(targetPosition.ToVector3(), transform.position);
            Gizmos.DrawSphere(targetPosition.ToVector3(), 0.1f);
        }
    }
}
