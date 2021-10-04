using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusComponent : MonoBehaviour
{
    public GameObject startButton;
    public GameObject gameUI;

    private void Start()
    {
        gameUI.SetActive(false);
        GameManager.Instance.Pause();
    }

    public void OnStartGame()
    {
        startButton.transform.parent.GetComponent<Animator>().SetTrigger("StartGame");

        GameManager.Instance.Play();
        gameUI.SetActive(true);

        Destroy(GameObject.Find("Tuto"));
    }

    public void OnQuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#endif // UNIT_EDITOR

        Application.Quit();
    }

    public void OnTryAgain()
    {
        GameManager.Instance.Reset();
        transform.GetChild(1).GetComponent<Animator>().SetTrigger("GameOverOut");
    }

    public void OnLoose()
    {
        transform.GetChild(1).GetComponent<Animator>().SetTrigger("GameOverIn");
    }
}
