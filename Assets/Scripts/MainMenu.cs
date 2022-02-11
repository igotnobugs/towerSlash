using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    private void Start() 
	{
        
    }

    public void StartGame() {
        GameManager.Instance.StartGame();
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
