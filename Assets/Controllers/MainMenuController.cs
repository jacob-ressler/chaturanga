using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
	#region Public Fields
	public GameObject H2PController;
	public GameObject MusicLicense;
	public GameObject MainMenuButtons;
	public GameObject OptController;
	#endregion

	void Start  () {
		MusicLicense.SetActive (false);
	}


	#region Button Functions
	public void Play() {
		SceneManager.LoadScene ("GameLauncher");
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("GameLauncher"));
	}

	public void HowToPlay() {
		MainMenuButtons.SetActive (false);
		H2PController.SetActive (true); 
	}

	public void Options () {
		MainMenuButtons.SetActive (false);
		OptController.SetActive (true);
	}

	public void ShowMusicLicense () {
		// Shows or hides the music license for the in-game music.
		MusicLicense.SetActive ( !MusicLicense.activeSelf );
	}
	#endregion
}
