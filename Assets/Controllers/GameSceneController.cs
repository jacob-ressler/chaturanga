using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Diagnostics;

public class GameSceneController : MonoBehaviour {

	public static GameSceneController Instance;

	public GameObject gui;
	public GameObject turnDisplay;
	public GameObject controllers;
	// Use this for initialization
	void OnEnable () {
		Instance = this;
		GameObject.Instantiate (controllers, this.transform);
		GameObject.Instantiate (turnDisplay, this.transform);
		GameObject.Instantiate (gui, this.transform);

	}



	// Update is called once per frame
	void Update () {
		
	}

	public void AI_WaitForMove () {
		// do nothing...
		CancelInvoke ();
	}
}
