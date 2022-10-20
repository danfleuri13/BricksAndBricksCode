using UnityEngine;
using System.Collections;
using System;


public class GameController : MonoBehaviour
{
    
	public Color[] colors;
    

 
	void Start ()
	{
        OnGameStart();
	}

	void OnGameStart ()
	{
        Camera.main.backgroundColor = colors [UnityEngine.Random.Range (0, colors.Length - 1)];
	}
    
    
    public void Play()
    {
        Application.LoadLevel("BricksAndBricks");
    }
    public void Restart()
    {
        Application.LoadLevel("BricksAndBricks");
        
    }
    public void Sair()
    {
        Application.Quit();
    }
}
