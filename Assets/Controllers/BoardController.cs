using UnityEngine;
using UnityEngine.SceneManagement;


public class BoardController : MonoBehaviour {

	public static BoardController Instance;

	public Board board { get; protected set; }



	// Use this for initialization
	void OnEnable () {
		// Create a reference to this board controller
		Instance = this;



		// Create an 8x8 board
		board = new Board (8, 8);

	}

	/// <summary>
	/// Starts a new game (@Button).
	/// </summary>
	public void NewGame() {
		if (Instance.board.GameMode != Game_Mode.DEFAULT) {
			// We are not "in-game" in the sense that moves can't be made right now, so just return.
			return;
		}

		SceneManager.LoadScene ("GameScene");
	}


	/// <summary>
	/// Advances to the next turn (@Button).
	/// </summary>
	public void NextTurn() {
		if (Instance.board.GameMode != Game_Mode.DEFAULT) {
			// We are not "in-game" in the sense that moves can't be made right now, so just return.
			return;
		}

		SoundController.Instance.OnButtonClicked ();
		Instance.board.NextTurn ();
	}

	public void MainMenu () {
		if (Instance.board.GameMode != Game_Mode.DEFAULT) {
			// We are not "in-game" in the sense that moves can't be made right now, so just return.
			return;
		}

		SceneManager.LoadScene ("MainMenu");
		SceneManager.SetActiveScene (SceneManager.GetSceneByName ("MainMenu"));
	}
}
