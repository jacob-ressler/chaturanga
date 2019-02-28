using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryController : MonoBehaviour {

	Dictionary<string, Sprite> playerSpriteMap;

	// Use this for initialization
	void Start () {

		// Hide the GO until one of the teams wins.
		HideGO ();

		BoardController.Instance.board.RegisterEnterVictoryMode (OnEnterVictoryMode);

		// Instantiate the dictionary.
		playerSpriteMap = new Dictionary<string, Sprite> ();

		// Load the victory screen sprites.
		Sprite [] sprites = Resources.LoadAll<Sprite> ("Art/VictoryScreens/");

		// Add pairs to the dictionary.
		for (int i = 0; i < 4; i++) {
			PieceColor pc = (PieceColor)i;
			foreach (Sprite s in sprites) {
				if (pc.ToString () == s.name) {
					// Found a match, so add the pair to the dictionary.
					playerSpriteMap.Add (pc.ToString (), s);
					continue;
				}
			}
		}

	}


	/// <summary>
	/// Activate the victory screen for the specified piece color.
	/// </summary>
	/// <param name="pc">The winning piece color.</param>
	void OnEnterVictoryMode (PieceColor pc) {
		// Set the victory screen's image to match the piece color that won.
		GameObject go = GameObject.Find ("Winner");
		go.GetComponent<Image> ().sprite = playerSpriteMap [pc.ToString ()];
		if (pc == PieceColor.Black) {
			gameObject.GetComponent<Image> ().color = new Color (.2f, .2f, .2f, .3f);
		} else if (pc == PieceColor.Blue) {
			gameObject.GetComponent<Image> ().color = new Color (.2f, .2f, 1, .3f);
		} else if (pc == PieceColor.Red) {
			gameObject.GetComponent<Image> ().color = new Color (1, .2f, .2f, .3f);
		} else if (pc == PieceColor.Green) {
			gameObject.GetComponent<Image> ().color = new Color (.2f, 1, .2f, .3f);
		}

		// Show the GO and play victory music.
		ShowGO ();

	}

	void HideGO () {
		//gameObject.GetComponent<Canvas> ().targetDisplay = 2; 
		gameObject.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		gameObject.transform.position = new Vector3 (1000, 1000, 0);
	}

	void ShowGO () {
		gameObject.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceOverlay;
		//gameObject.GetComponent<Canvas> ().targetDisplay = 1; 
	}


	public void GoToMainMenu() {
		// Load the main menu scene and set it active.
		SceneManager.LoadScene ("MainMenu");
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("MainMenu"));
	}


	public void PlayAgain() {
		// Reload the game scene to start a new game.
		SceneManager.LoadScene ("GameScene");
	}
}
