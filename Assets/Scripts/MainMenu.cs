using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    public GameManager gameManager;

    private void Start() 
	{
        
    }

    public void StartGame() {
        gameManager.StartGame();
    }


    public void QuitGame() {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
