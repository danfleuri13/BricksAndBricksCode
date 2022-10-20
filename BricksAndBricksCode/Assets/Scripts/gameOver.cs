using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameOver : MonoBehaviour {

    public Text pontos;
    public Text recorde;

	// Use this for initialization
	void Start () {
        PlacarEnd();
    }

    public void PlacarEnd() {
        pontos.text = PlayerPrefs.GetInt("pontuacao").ToString();
        recorde.text = PlayerPrefs.GetInt("recorde").ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
