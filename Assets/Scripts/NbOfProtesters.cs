using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NbOfProtesters : MonoBehaviour
{
    public List<string> organizers = new List<string>();
    public List<string> authorities = new List<string>();

    private TMP_Text organizersText;
    private TMP_Text authoritiesText;

    private float acc = 0.0f;
    private float threshold = 4f;

    public float minThresh = 2f;
    public float maxThresh = 6f;

    // Start is called before the first frame update
    void Start()
    {
        organizersText = transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        authoritiesText = transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        acc += Time.deltaTime;

        if(acc > threshold)
        {
            ChangeText();
            acc = 0;
            threshold = Random.Range(minThresh, maxThresh);
        }
    }

    public void ChangeText()
    {
        if(Random.Range(0f,1f) > 0.5f)
        {
            authoritiesText.text = authorities[Random.Range(0, authorities.Count)];
        }
        else
        {
            organizersText.text = organizers[Random.Range(0, organizers.Count)];
        }
    }
    
}
