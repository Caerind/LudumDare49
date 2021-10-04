using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int protestersSpritesPerType = 2;
    public List<Sprite> protestersWalkSprites;
    public List<Sprite> protestersHeadSprites;
    public List<Sprite> protestersThrowSprites;
    public List<Sprite> protestersPancartesSprites;
    public GameObject protesterPrefab = null;
    private GameObject protestersGroup = null;

    public GameObject projectilePrefab = null;
    public List<Sprite> projectilesProtesters;
    public Sprite projectileLBD;

    public List<GameObject> protesters = new List<GameObject>();
    public List<GameObject> policemen = new List<GameObject>();
    public List<GameObject> squads = new List<GameObject>();
    private ProtesterZoneComponent zone = null;
    private ProtesterSpawnerComponent spawner = null;

    public float fatiguePerProtesterPerSecond = 0.025f;
    public float fatiguePerPolicemanFrontPerSecond = 0.007f;
    public float fatiguePerPolicemanBackPerSecond = 0.007f;
    public float fatiguePerProjectile = 0.05f;

    private float chaos = 0.0f;
    public float chaosHit = 0.005f;
    public float chaosLBD = 0.1f;
    public float chaosDecayPerSecond = 0.001f;
    public int squadAtMaxFatigueCount = 2;

    public GameObject fatigueBar = null;
    public GameObject lbdButton = null;

    private bool soundsStartedYet = false;

    [SerializeField] private MenusComponent menus = null;

    private bool isPlaying = true;

    public void AddEntity(GameObject go)
    {
        ProtesterComponent protester = go.GetComponent<ProtesterComponent>();
        if (protester != null)
        {
            protesters.Add(go);
            return;
        }

        PolicemanComponent policeman = go.GetComponent<PolicemanComponent>();
        if (policeman != null)
        {
            policemen.Add(go);
            return;
        }

        PoliceSquadComponent squad = go.GetComponent<PoliceSquadComponent>();
        if (squad != null)
        {
            squads.Add(go);
            return;
        }
    }

    public void RemoveEntity(GameObject go)
    {
        ProtesterComponent protester = go.GetComponent<ProtesterComponent>();
        if (protester != null)
        {
            protesters.Remove(go);
            return;
        }

        PolicemanComponent policeman = go.GetComponent<PolicemanComponent>();
        if (policeman != null)
        {
            policemen.Remove(go);
            return;
        }

        PoliceSquadComponent squad = go.GetComponent<PoliceSquadComponent>();
        if (squad != null)
        {
            squads.Remove(go);
            return;
        }
    }

    public void SpawnProtester(Vector2 position)
    {
        if (protesterPrefab == null)
        {
            Debug.LogError("No protesterPrefab in GameManager");
            return;
        }

        GameObject protester = Instantiate(protesterPrefab);
        protester.transform.position = position.ToVector3();
        protester.transform.parent = protestersGroup.transform;

        // Give random target pos
        ProtesterComponent protComp = protester.GetComponent<ProtesterComponent>();
        protComp.SetTargetPosition(GameManager.Instance.GetRandomPointInZone());
    }

    public Vector2 GetZoneCenter()
    {
        return (zone != null) ? zone.transform.position.ToVector2() : Vector2.zero;
    }

    public float GetZoneRadius()
    {
        return (zone != null) ? zone.radius : 1.0f;
    }

    public Vector2 GetRandomPointInZone()
    {
        return (zone != null) ? zone.GetRandomPointInZone() : Vector2.zero;
    }

    public GameObject GetClosestPoliceSquad(Vector2 position)
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;
        foreach (GameObject squad in squads)
        {
            float distance = Vector2.Distance(position, squad.transform.position.ToVector2());
            if (distance < minDistance)
            {
                closest = squad;
                minDistance = distance;
            }
        }
        return closest;
    }

    public GameObject GetCurrentPoliceSquad()
    {
        Rect camRect = Camera.main.GetOrthographicRect();
        Vector2 camPos = Camera.main.transform.position.ToVector2();

        GameObject squad = GetClosestPoliceSquad(camPos);
        PoliceSquadComponent squadComp = squad.GetComponent<PoliceSquadComponent>();

        if (squadComp.GetRect().Overlaps(camRect))
        {
            return squad;
        }
        else
        {
            GameObject closest = null;
            float minDistance = float.MaxValue;
            foreach (GameObject s in squads)
            {
                PoliceSquadComponent sComp = s.GetComponent<PoliceSquadComponent>();
                float distance = Vector2.Distance(camPos, s.transform.position.ToVector2());
                if (s != squad && sComp.GetRect().Overlaps(camRect) && distance < minDistance)
                {
                    closest = squad;
                    minDistance = distance;
                }
            }
            return closest;
        }
    }

    public void ThrowProjectile(Vector2 _startPos, Vector2 _targetPos, bool isProtester = true)
    {
        GameObject go = Instantiate(projectilePrefab);
        go.transform.position = _startPos.ToVector3();
        ProjectileComponent proj = go.GetComponent<ProjectileComponent>();
        proj.targetPos = _targetPos;

        if (isProtester)
        {
            go.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            go.GetComponent<SpriteRenderer>().sprite = projectilesProtesters[Random.Range(0, projectilesProtesters.Count - 1)];
        }
        else
        {
            go.transform.localScale *= 0.25f;
            go.GetComponent<SpriteRenderer>().sprite = projectileLBD;
            proj.speed *= 2.0f;
        }
    }

    private void Awake()
    {
        zone = GetComponent<ProtesterZoneComponent>();
        spawner = GetComponent<ProtesterSpawnerComponent>();

        protestersGroup = new GameObject();
        protestersGroup.name = "Protesters";
    }

    private void Start()
    {
        if (zone == null)
        {
            Debug.LogError("No zone given to GameManager");
        }

    }

    private void Update()
    {
        if (!IsPlaying())
        {
            return;
        }

        if (!soundsStartedYet)
        {
            AudioManager.Play("mainTheme");
            AudioManager.Play("crowd");
            soundsStartedYet = true;
        }


        int fatigueAtMax = 0;

        // Reset all squads fatigues comps
        foreach (GameObject s in squads)
        {
            PoliceSquadComponent sq = s.GetComponent<PoliceSquadComponent>();

            if (sq.GetFatigue() > 0.98f)
            {
                fatigueAtMax++;
            }

            sq.SetFatigueComponent(null);
        }

        if (fatigueAtMax < squadAtMaxFatigueCount)
        {
            chaos = Mathf.Clamp01(chaos - chaosDecayPerSecond * Time.deltaTime);
        }

        bool lbdClicked = false;

        // Current squad detection & effects
        GameObject go = GetCurrentPoliceSquad();
        PoliceSquadComponent squad = null;
        if (go == null)
        {
            fatigueBar.SetActive(false);
            lbdButton.SetActive(false);
        }
        else
        {
            squad = go.GetComponent<PoliceSquadComponent>();
            if (squad == null)
            {
                fatigueBar.SetActive(false);
                lbdButton.SetActive(false);
            }
            else
            {
                fatigueBar.SetActive(true);
                lbdButton.SetActive(true);
                squad.SetFatigueComponent(fatigueBar.GetComponent<PoliceSquadFatigueComponent>());

                LBDButtonComponent ldbButtonComp = lbdButton.GetComponent<LBDButtonComponent>();
                lbdClicked = ldbButtonComp.GetClickedAndReset();
                if (lbdClicked && squad != null && squad.CanShootLBD())
                {
                    squad.ShootLBD();
                }

                ldbButtonComp.SetCooldownValue(squad.GetCooldownValue());
            }
        }
        
        bool mouseDown = Input.GetMouseButtonDown(0);
        bool touchDown = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        bool clicked = mouseDown || touchDown;

        if (clicked && !lbdClicked)
        {
            Vector2 screenSpace = Vector2.zero;
            if (mouseDown)
            {
                screenSpace = Input.mousePosition;
            }
            else if (touchDown)
            {
                screenSpace = Input.GetTouch(0).position;
            }

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenSpace).ToVector2();

            bool clickHandled = false;

            if (squad != null)
            {
                int protestersCount = protesters.Count;
                for (int i = 0; i < protestersCount && !clickHandled; ++i)
                {
                    ProtesterComponent protComp = protesters[i].GetComponent<ProtesterComponent>();
                    if (protComp.IsOverlapping(worldPosition) && protComp.IsFocusingSquad() && protComp.GetFocusedSquad() == squad)
                    {
                        squad.HandleProtester(protesters[i]);
                        clickHandled = true;
                    }
                }
            }


            if (!clickHandled)
            {
                CameraManager.Instance.StartCamPress(screenSpace, mouseDown);
            }
        }
    }

    public void AddChaosHit()
    {
        if (chaos + chaosHit >= 1.0f)
        {
            chaos = 1.0f;
            OnLoose();
            return;
        }

        chaos = Mathf.Clamp01(chaos + chaosHit);
        FindObjectOfType<ChaosBarComponent>().transform.GetChild(3).GetComponent<Animator>().SetTrigger("ChaosIncrease");
    }

    public void AddChaosLBD()
    {
        if (chaos + chaosLBD >= 1.0f)
        {
            chaos = 1.0f;
            OnLoose();
            return;
        }

        chaos = Mathf.Clamp01(chaos + chaosLBD);
        FindObjectOfType<ChaosBarComponent>().transform.GetChild(3).GetComponent<Animator>().SetTrigger("ChaosIncrease");
    }

    public float GetChaos() { return chaos; }

    public void OnLoose()
    {
        Pause();
        menus.OnLoose();
    }

    public void Reset()
    {
        Play();
        chaos = 0.0f;
        Transform protestersChilds = GameObject.Find("Protesters").transform;
        for (int i = 0; i < protestersChilds.childCount; i++)
        {
            Destroy(protestersChilds.GetChild(i).gameObject);
        }
        protesters.Clear();
        for (int i = 0; i < squads.Count; i++)
        {
            squads[i].GetComponent<PoliceSquadComponent>().Reset();
        }
        zone.SpawnStartingProtesters();

        Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
    }

    public void Pause() { isPlaying = false; }
    public void Play() { isPlaying = true; }
    public bool IsPlaying()
    {
        return isPlaying;
    }

    private void OnGUI()
    {
        /*
        List<string> texts = new List<string>();
        texts.Add("F1/Select : Change focus");
        texts.Add("ZQSD/WASD/LeftJoystick : Rotation");
        texts.Add("Alt/RightTrigger : Acceleration");
        texts.Add("Ctrl/LeftTrigger : Deceleration");
        texts.Add("A/A : Fire missile");
        texts.Add("Z/B : Fire rocket");
        AutoGUI.Display(10, 10, 300, "Controls", texts);
        */
    }
}
