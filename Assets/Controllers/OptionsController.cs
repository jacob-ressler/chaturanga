using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class OptionsController : MonoBehaviour {

	public Button Back;
	public Toggle Sfx;
	public Toggle Music;

	MainMenuController mmc;
	SoundController sc;


	// Use this for initialization
	void Start () {
		mmc = GameObject.FindObjectOfType<MainMenuController> ();
		sc = GameObject.FindObjectOfType<SoundController> ();

		if (PlayerPrefs.HasKey ("SFX_Enabled")) {
			Sfx.isOn = PlayerPrefs.GetInt ("SFX_Enabled") == 1 ? true : false;
		} else {
			Sfx.isOn = true;
			PlayerPrefs.SetInt ("SFX_Enabled", 1); 
		}

		if (PlayerPrefs.HasKey ("Music_Enabled")) {
			Music.isOn = PlayerPrefs.GetInt ("Music_Enabled") == 1 ? true : false;
		} else {
			Music.isOn = true; 
			PlayerPrefs.SetInt ("Music_Enabled", 1);
		}

		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MainMenu () {
		mmc.MainMenuButtons.SetActive (true);
		mmc.OptController.SetActive (false);
	}

	public void SaveOption_SFX () {
		int val = Sfx.isOn ? 1 : 0;
		PlayerPrefs.SetInt ("SFX_Enabled", val);
		sc.sfxOn = Sfx.isOn;
	}

	public void SaveOption_Music () {
		int val = Music.isOn ? 1 : 0;
		PlayerPrefs.SetInt ("Music_Enabled", val);
		sc.musicOn = Music.isOn;
		sc.OnMusicOnChanged ();
	}
}
