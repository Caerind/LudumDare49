using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceSquadComponent : MonoBehaviour
{
    public float rowWidth = 10.0f;
    public float distancePerRow = 2.0f;
    public int[] policemenCountPerRow = new int[] { 4, 4 }; // Used only at start
    public GameObject policemanPrefab = null;
    public float protesterMinDistance = 1.0f;
    public float protesterMaxDistance = 3.0f;
    public float protesterProjectileExtraDistance = 2.0f;
    public float fatigue = 0.0f;
    public float cooldownLBD = 10.0f;
    private float accumulatorLBD = 0.0f;
    public float rectFactor = 0.5f;

    [HideInInspector]
    public List<GameObject>[] policemen = null;

    [HideInInspector]
    public List<GameObject> protesters = new List<GameObject>();

    private PoliceSquadFatigueComponent fatigueComponent = null;

    private void Start()
    {
        Reset();

        policemen = new List<GameObject>[2];
        policemen[0] = new List<GameObject>();
        policemen[1] = new List<GameObject>();

        GameManager.Instance?.AddEntity(gameObject);

        if (policemanPrefab == null)
        {
            Debug.LogError("No policemanPrefab for PoliceSquad");
            return;
        }

        for (int row = 0; row < 2; ++row)
        {
            for (int j = 0; j < policemenCountPerRow[row]; ++j) // pos in row
            {
                GameObject cop = Instantiate(policemanPrefab);
                cop.name = "Policeman-" + j.ToString();
                policemen[row].Add(cop);
                cop.transform.parent = transform;

                PolicemanComponent polComp = cop.GetComponent<PolicemanComponent>();
                if (polComp != null)
                {
                    polComp.SetHasLBD((row == 1)); // 2nd row has LBD
                }
            }
        }

        SetInitialPositions(); // Initial position
        UpdatePositions(); // TargetPosition & Direction
    }
    private void OnDestroy()
    {
        // TODO : Kill cops ?
        GameManager.Instance?.RemoveEntity(gameObject);
    }

    private void SetInitialPositions()
    {
        for (int row = 0; row < 2; ++row)
        {
            for (int j = 0; j < policemen[row].Count; ++j) // pos in row
            {
                policemen[row][j].transform.position = GetPolicemanPos(j, row);
            }
        }
    }

    private void UpdatePositions()
    {
        for (int row = 0; row < 2; ++row)
        {
            for (int j = 0; j < policemen[row].Count; ++j) // pos in row
            {
                PolicemanComponent polComp = policemen[row][j].GetComponent<PolicemanComponent>();
                if (polComp != null)
                {
                    polComp.UpdateRow(this, j, GetPolicemanPos(j, row));
                    polComp.UpdateDirection();
                }
            }
        }

        for (int i = 0; i < protesters.Count; ++i)
        {
            GameObject go = protesters[i];

            ProtesterComponent protester = go.GetComponent<ProtesterComponent>();
            if (protester != null)
            {
                protester.SetTargetPosition(GetProtesterPosition(i, protester.hasProjectile));
            }
        }
    }

    public Vector2 GetPolicemanPos(int index, int row = 0)
    {
        Vector2 start = (transform.position - transform.right * 0.5f * rowWidth).ToVector2();
        Vector2 end = (transform.position + transform.right * 0.5f * rowWidth).ToVector2();
        float factor = (policemen[row].Count > 1) ? index / (float)(policemen[row].Count - 1) : 0.5f;
        Vector2 pos = Vector2.Lerp(start, end, factor) + row * (transform.up * distancePerRow).ToVector2();
        return pos;
    }

    public Vector2 GetProtesterPosition(int index, bool hasProjectile)
    {
        Vector2 start = (transform.position - transform.right * 0.5f * rowWidth).ToVector2();
        Vector2 end = (transform.position + transform.right * 0.5f * rowWidth).ToVector2();
        float factor = (index+1) / (float)(protesters.Count + 1);
        Vector2 pos = Vector2.Lerp(start, end, factor);
        pos -= (Random.Range(protesterMinDistance, protesterMaxDistance) * transform.up).ToVector2();
        if (hasProjectile)
        {
            pos -= (protesterProjectileExtraDistance * transform.up).ToVector2();
        }
        return pos;
    }

    public void AddPoliceman(GameObject policeman, int row = 0)
    {
        policemen[row].Add(policeman);
        UpdatePositions();
    }

    public void RemovePoliceman(GameObject policeman, int row = 0)
    {
        // Probably can detect row instead of getting, but fuck it for now
        policemen[row].Remove(policeman);
    }

    public void RemoveLastPoliceman(int row = 0)
    {
        int count = policemen[row].Count;
        if (count > 0)
        {
            policemen[row].RemoveAt(count - 1);
        }
    }

    public void AddProtester(GameObject protester)
    {
        protesters.Add(protester);
        UpdatePositions();
    }

    public void RemoveProtester(GameObject protester)
    {
        protesters.Remove(protester);
        UpdatePositions();
    }

    public void Reset()
    {
        protesters.Clear();
        accumulatorLBD = cooldownLBD; // Can shoot LBD right at the beginning
        fatigue = 0.0f;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        accumulatorLBD += Time.deltaTime;

        float previousFatigue = fatigue;

        fatigue += protesters.Count * GameManager.Instance.fatiguePerProtesterPerSecond * Time.deltaTime;
        fatigue -= policemen[0].Count * GameManager.Instance.fatiguePerPolicemanFrontPerSecond * Time.deltaTime;
        fatigue -= policemen[1].Count * GameManager.Instance.fatiguePerPolicemanBackPerSecond * Time.deltaTime;
        fatigue = Mathf.Clamp01(fatigue);

        /*
        if (previousFatigue < 0.25f && fatigue > 0.25f)
        {
        }
        */
        if (previousFatigue < 0.5f && fatigue > 0.5f)
        {
            AlerteManager.Instance.SpawnAttentionMessage("We're having trouble at position " + (transform.GetSiblingIndex() + 1).ToString(), (transform.position - transform.up * (protesterMaxDistance + protesterProjectileExtraDistance)).ToVector2(), gameObject);
        }
        /*
        if (previousFatigue < 0.5f && fatigue > 0.5f)
        {
        }
        */

        if (fatigueComponent != null)
        {
            fatigueComponent.UpdateFatigue(fatigue);
        }
    }

    public void SetFatigueComponent(PoliceSquadFatigueComponent fatigueComp)
    {
        fatigueComponent = fatigueComp;
    }

    public bool CanShootLBD()
    {
        return accumulatorLBD >= cooldownLBD;
    }

    public void ShootLBD()
    {
        if (!CanShootLBD())
            return;

        accumulatorLBD = 0.0f;

        for (int j = 0; j < policemen[1].Count; ++j) // LBDs are 2nd row
        {
            PolicemanComponent pol = policemen[1][j].GetComponent<PolicemanComponent>();
            if (pol != null)
            {
                pol.ShootLBD();
            }
        }

        AudioManager.Play("LBD");

        // TODO : Add Coroutine to make protesters flee after 0.x seconds
        ProtestersFlee();
    }

    public void HandleProtester(GameObject protester)
    {
        if (protester == null)
        {
            return;
        }

        ProtesterComponent prot = protester.GetComponent<ProtesterComponent>();
        if (!protesters.Contains(protester) || prot == null || prot.GetFocusedSquad() != this)
        {
            return;
        }

        // Compute nearest & not busy policeman
        GameObject policeman = null;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < policemen[0].Count; ++i)
        {
            GameObject p = policemen[0][i];
            if (p != null)
            {
                PolicemanComponent pol = p.GetComponent<PolicemanComponent>();
                if (pol != null && !pol.IsFocusingProtester() && pol.IsAtItsPost())
                {
                    float distance = Vector2.Distance(p.transform.position, protester.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        policeman = p;
                    }
                }
            }
        }

        if (policeman != null)
        {
            policeman.GetComponent<PolicemanComponent>().FocusProtester(protester);
        }
    }

    public int GetProtestersCount()
    {
        return protesters.Count;
    }

    private void ProtestersFlee()
    {
        // Avoid infinite follow too far from squad
        for (int i = 0; i < policemen[0].Count; ++i)
        {
            policemen[0][i].GetComponent<PolicemanComponent>().UnfocusProtester();
        }

        // Protester flee
        foreach (GameObject go in protesters)
        {
            if (go == null)
            {
                continue;
            }
            ProtesterComponent prot = go.GetComponent<ProtesterComponent>();
            if (prot != null)
            {
                prot.UnfocusSquad();
            }
        }
        protesters.Clear();
    }

    public void AddFatigue(float fatigueBonus)
    {
        fatigue += fatigueBonus;
        fatigue = Mathf.Clamp01(fatigue);
    }

    public Rect GetRect()
    {
        Vector2 size = new Vector2(rowWidth, rowWidth) * rectFactor;
        return new Rect((transform.position + transform.up * distancePerRow * 0.5f).ToVector2() - size * 0.5f, size);
    }

    public Vector2 GetRandomPointInProtesterThrowRange()
    {
        float r = Random.Range(-0.5f * rowWidth, +0.5f * rowWidth);
        return transform.position + transform.right * r + transform.up * distancePerRow * 0.5f;
    }

    public Vector2 GetLBDShootPoint(int indexOfShooter)
    {
        Vector2 start = (transform.position - transform.right * 0.5f * rowWidth).ToVector2();
        Vector2 end = (transform.position + transform.right * 0.5f * rowWidth).ToVector2();
        float factor = (policemen[1].Count > 1) ? indexOfShooter / (float)(policemen[1].Count - 1) : 0.5f;
        Vector2 pos = Vector2.Lerp(start, end, factor) - (transform.up * (protesterMinDistance + protesterMaxDistance) * 0.5f).ToVector2();
        return pos;
    }

    public float GetCooldownValue()
    {
        return Mathf.Clamp01(accumulatorLBD / cooldownLBD);
    }

    public float GetFatigue()
    {
        return fatigue;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth, transform.position + transform.right * 0.5f * rowWidth);
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth + transform.up * distancePerRow, transform.position + transform.right * 0.5f * rowWidth + transform.up * distancePerRow);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.up * distancePerRow * 0.5f, rectFactor * new Vector3(rowWidth, rowWidth, rowWidth));

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth - transform.up * protesterMinDistance, transform.position + transform.right * 0.5f * rowWidth - transform.up * protesterMinDistance);
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth - transform.up * protesterMaxDistance, transform.position + transform.right * 0.5f * rowWidth - transform.up * protesterMaxDistance);
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth - transform.up * protesterMinDistance, transform.position - transform.right * 0.5f * rowWidth - transform.up * protesterMaxDistance);
        Gizmos.DrawLine(transform.position + transform.right * 0.5f * rowWidth - transform.up * protesterMinDistance, transform.position + transform.right * 0.5f * rowWidth - transform.up * protesterMaxDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth + transform.up * distancePerRow * 0.5f, transform.position + transform.right * 0.5f * rowWidth + transform.up * distancePerRow * 0.5f);
        Gizmos.DrawLine(transform.position - transform.right * 0.5f * rowWidth - transform.up * (protesterMinDistance + protesterMaxDistance) * 0.5f, transform.position + transform.right * 0.5f * rowWidth - transform.up * (protesterMinDistance + protesterMaxDistance) * 0.5f);
    }
}
