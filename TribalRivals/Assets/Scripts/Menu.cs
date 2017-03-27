using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Button playButton;
    public Button quitButton;
    public GameObject Tutorial;
    public GameObject Tutorial2;

    public 
	// Use this for initialization
	void Start(){
        Tutorial.SetActive(false);
        Tutorial2.SetActive(false);
        playButton.onClick.AddListener(play);
        quitButton.onClick.AddListener(quit);
    }

    void play(){
        Tutorial.SetActive(true);
    }

    void help(){
        
    }

    void quit(){
        Application.Quit();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !Tutorial.activeSelf && !Tutorial2.activeSelf) { Tutorial.SetActive(false); return; }
        if (Input.GetKeyDown(KeyCode.Escape) && Tutorial.activeSelf && !Tutorial2.activeSelf) { Tutorial.SetActive(false); Tutorial2.SetActive(true); return; }
        if (Input.GetKeyDown(KeyCode.Escape) && !Tutorial.activeSelf && Tutorial2.activeSelf) { Tutorial.SetActive(false); Tutorial2.SetActive(false); SceneManager.LoadScene(1); return; }
    }
}
