using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLauncherController : MonoBehaviour {

	public Player[] players;

	Color red = new Color (1f, .2f, .2f);
	Color yellow = Color.yellow;
	Color green = new Color (.2f, 1f, .2f);

	public GameObject[] sliders;


	// Use this for initialization
	void Start () {
		players = new Player[4];

		for (int i = 0; i < players.Length; i++) {
			// Instantiate the dummy players.
			players [i] = new GameObject(((PieceColor)i).ToString ()).AddComponent<Player> ();
			players [i].gameObject.transform.SetParent (this.transform);
			players [i].SetPlayer (null, (PieceColor)i);

			// Set the slider to green.
			sliders [i].GetComponent<Image> ().color = green;
			// Make sure the sliders all start disabled.
			sliders [i].GetComponent<Scrollbar> ().interactable = false;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame () {
		// Save player prefs.
		SavePlayerSettingsFromLauncher ();
		// Change to game scene.
		SceneManager.LoadScene ("GameScene");
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("GameScene"));
	}

	public void BackToMainMenu () {
		SceneManager.LoadScene ("MainMenu");
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("MainMenu"));
	}

	void SavePlayerSettingsFromLauncher () {
		for (int i = 0; i < 4; i++) {
			// Save team color.
			PlayerPrefs.SetInt ("Player_" + i + "_Color", i);
			// Save difficulty.
			PlayerPrefs.SetInt ("Player_" + i + "_Difficulty", (int)players[i].difficulty);

		}
	}

	// BUTTONS

	public void ToggleAI_Black ( bool toggle) {
		int index = 0;
		if (players [index].difficulty == Player.DifficultyLevel.HUMAN) {
			// Change the difficulty to EASY, MEDIUM, or HARD.
			sliders [index].GetComponent<Scrollbar> ().interactable = true;
			ToggleDifficulty_Black (0);
			return;
		}

		// Otherwise, change the difficulty to human (non-AI).
		players [index].difficulty = Player.DifficultyLevel.HUMAN;
		sliders [index].GetComponent<Scrollbar> ().interactable = false;

	}

	public void ToggleAI_Blue ( bool toggle) {
		int index = 1;
		if (players [index].difficulty == Player.DifficultyLevel.HUMAN) {
			// Change the difficulty to EASY, MEDIUM, or HARD.
			sliders [index].GetComponent<Scrollbar> ().interactable = true;
			ToggleDifficulty_Blue (0);
			return;
		}

		// Otherwise, change the difficulty to human (non-AI).
		players [index].difficulty = Player.DifficultyLevel.HUMAN;
		sliders [index].GetComponent<Scrollbar> ().interactable = false;

	}

	public void ToggleAI_Red ( bool toggle) {
		int index = 2;
		if (players [index].difficulty == Player.DifficultyLevel.HUMAN) {
			// Change the difficulty to EASY, MEDIUM, or HARD.
			sliders [index].GetComponent<Scrollbar> ().interactable = true;
			ToggleDifficulty_Red (0);
			return;
		}

		// Otherwise, change the difficulty to human (non-AI).
		players [index].difficulty = Player.DifficultyLevel.HUMAN;
		sliders [index].GetComponent<Scrollbar> ().interactable = false;

	}

	public void ToggleAI_Green ( bool toggle) {
		int index = 3;
		if (players [index].difficulty == Player.DifficultyLevel.HUMAN) {
			sliders [index].GetComponent<Scrollbar> ().interactable = true;
			// Change the difficulty to EASY, MEDIUM, or HARD.
			ToggleDifficulty_Green (0);
			return;
		}

		// Otherwise, change the difficulty to human (non-AI).
		players [index].difficulty = Player.DifficultyLevel.HUMAN;
		sliders [index].GetComponent<Scrollbar> ().interactable = false;
	}


	public void ToggleDifficulty_Black (float diffIndex) {
		int index = 0;
		Scrollbar s = sliders [index].GetComponent<Scrollbar> ();

		// Change difficulty and visuals
		if (s.value < 0.26) {
			players [index].difficulty = Player.DifficultyLevel.EASY;
			sliders [index].GetComponent<Image> ().color = green;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "EASY";
			t.alignment = TextAnchor.UpperLeft;
		} else if (s.value < 0.75) {
			players [index].difficulty = Player.DifficultyLevel.MEDIUM;
			sliders [index].GetComponent<Image> ().color = yellow;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "MEDIUM";
			t.alignment = TextAnchor.UpperCenter;
		} else { 
			players [index].difficulty = Player.DifficultyLevel.HARD;
			sliders [index].GetComponent<Image> ().color = red;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "HARD";
			t.alignment = TextAnchor.UpperRight;
		}

	}

	public void ToggleDifficulty_Blue (float diffIndex) {
		int index = 1;
		Scrollbar s = sliders [index].GetComponent<Scrollbar> ();

		// Change difficulty and visuals
		if (s.value < 0.26) {
			players [index].difficulty = Player.DifficultyLevel.EASY;
			sliders [index].GetComponent<Image> ().color = green;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "EASY";
			t.alignment = TextAnchor.UpperLeft;
		} else if (s.value < 0.75) {
			players [index].difficulty = Player.DifficultyLevel.MEDIUM;
			sliders [index].GetComponent<Image> ().color = yellow;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "MEDIUM";
			t.alignment = TextAnchor.UpperCenter;
		} else { 
			players [index].difficulty = Player.DifficultyLevel.HARD;
			sliders [index].GetComponent<Image> ().color = red;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "HARD";
			t.alignment = TextAnchor.UpperRight;
		}
	}

	public void ToggleDifficulty_Red (float diffIndex) {
		int index = 2;
		Scrollbar s = sliders [index].GetComponent<Scrollbar> ();

		// Change difficulty and visuals
		if (s.value < 0.26) {
			players [index].difficulty = Player.DifficultyLevel.EASY;
			sliders [index].GetComponent<Image> ().color = green;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "EASY";
			t.alignment = TextAnchor.UpperLeft;
		} else if (s.value < 0.75) {
			players [index].difficulty = Player.DifficultyLevel.MEDIUM;
			sliders [index].GetComponent<Image> ().color = yellow;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "MEDIUM";
			t.alignment = TextAnchor.UpperCenter;
		} else { 
			players [index].difficulty = Player.DifficultyLevel.HARD;
			sliders [index].GetComponent<Image> ().color = red;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "HARD";
			t.alignment = TextAnchor.UpperRight;
		}

	}

	public void ToggleDifficulty_Green (float diffIndex) {
		int index = 3;
		Scrollbar s = sliders [index].GetComponent<Scrollbar> ();

		// Change difficulty and visuals
		if (s.value < 0.26) {
			players [index].difficulty = Player.DifficultyLevel.EASY;
			sliders [index].GetComponent<Image> ().color = green;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "EASY";
			t.alignment = TextAnchor.UpperLeft;
		} else if (s.value < 0.75) {
			players [index].difficulty = Player.DifficultyLevel.MEDIUM;
			sliders [index].GetComponent<Image> ().color = yellow;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "MEDIUM";
			t.alignment = TextAnchor.UpperCenter;
		} else { 
			players [index].difficulty = Player.DifficultyLevel.HARD;
			sliders [index].GetComponent<Image> ().color = red;
			Text t = sliders [index].GetComponentInChildren<Text> ();
			t.text = "HARD";
			t.alignment = TextAnchor.UpperRight;
		}

	}
}
