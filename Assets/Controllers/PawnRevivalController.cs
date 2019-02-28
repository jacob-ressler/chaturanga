using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PawnRevivalController : MonoBehaviour {

	Piece pawn; // The pawn that triggered pawn revival.

	GameObject Boat;
	GameObject Horse; 
	GameObject Elephant;

	Dictionary<GameObject, Piece> gameObjectPieceMap;

	// Use this for initialization
	void Start () {

		// This GO will be hidden far away from the camera's view until pawn revival is entered.
		HideGO ();

		// Instantiate the dictionary.
		gameObjectPieceMap = new Dictionary<GameObject, Piece> ();

		// Get a reference to the GO for each button.
		Boat = GameObject.Find ("Boat Button");
		Horse = GameObject.Find ("Horse Button");
		Elephant = GameObject.Find ("Elephant Button");

		foreach (Piece p in BoardController.Instance.board.Pieces) {
			if (p.Type == PieceType.Pawn) {
				p.RegisterEnterPawnRevivalMode (OnEnterPawnRevivalMode);
			}
		}



	}

	public void OnEnterPawnRevivalMode (Piece pawn) {
		Debug.Log ("OnEnterPawnRevivalMode -- Selected Piece:" + pawn.Color.ToString () + "_" + pawn.Type.ToString ());
		// Clear previous entries in the dictionary, since they likely won't apply with this pawn.
		gameObjectPieceMap.Clear ();
		// Stash a reference to the pawn in a class-local variable.
		this.pawn = pawn;
		// Check for which buttons to have active.
		foreach (Piece p in BoardController.Instance.board.Pieces) {
			if (p.Color == pawn.Color) {
				if (p.Type == PieceType.Boat) {
					if (p.HasBeenKilled ()) {
						// The boat is revivable.
						gameObjectPieceMap.Add (Boat, p);
						continue;
					}
					// The boat is NOT revivable.
					MakeNonrevivable (Boat);
				}
				else if (p.Type == PieceType.Horse) {
					if (p.HasBeenKilled ()) {
						// The horse is revivable.
						gameObjectPieceMap.Add (Horse, p);
						continue;
					}
					// The horse is NOT revivable.
					MakeNonrevivable (Horse);
				}
				else if (p.Type == PieceType.Elephant) {
					if (p.HasBeenKilled ()) {
						// The horse is revivable.
						gameObjectPieceMap.Add (Elephant, p);
						continue;
					}
					// The horse is NOT revivable.
					MakeNonrevivable (Elephant);
				}
			}
		}

		// If the dictionary is empty, there are no revive options. Change the game mode back to DEFAULT and return.
		if (gameObjectPieceMap.Count == 0) {
			BoardController.Instance.board.GameMode = Game_Mode.DEFAULT;
			RestoreDefaults ();
			// Refind all valid moves to avoid the pawn's moves being wrong.
			//BoardController.Instance.board.FindAllValidMoves ();
			return;
		}
		// Show the GO.
		ShowGO ();

		// Play the sound for entering pawn revival.
		SoundController.Instance.OnPawnRevivalModeEntered (pawn);
	}

	/// <summary>
	/// Revive the specified piece (linked to the GO).
	/// </summary>
	/// <param name="go">The GO that is paired with the piece to be revived.</param>
	void Revive (GameObject go) {
		Debug.Log("PawnRevivalController -- Reviving piece.");
		Piece p = gameObjectPieceMap [go];
		Debug.Log ("Revive -- Piece: " + p.Color + "_" + p.Type);
		// Revive the piece and kill the pawn.
		Debug.Log ("Pawn's tile: (" + pawn.CurrTile.X + "," + pawn.CurrTile.Y + ")");
		p.CurrTile = pawn.CurrTile;
		Debug.Log ("Piece's tile: (" + p.CurrTile.X + "," +p.CurrTile.Y +")");

		pawn.Kill ();

		// Set the game mode back to default.
		BoardController.Instance.board.GameMode = Game_Mode.DEFAULT;
		//BoardController.Instance.board.FindAllValidMoves ();

		// Restore default visuals for this screen.
		RestoreDefaults ();
	}

	/// <summary>
	/// Restores the defaults for the pawn revival screen (all buttons active, this GO inactive).
	/// </summary>
	void RestoreDefaults () {
		Debug.Log ("PawnRevivalController -- Restoring default values."); 
		this.pawn = null;
		Boat.GetComponent<Button> ().interactable = true;
		Boat.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		Horse.GetComponent<Button> ().interactable = true;
		Horse.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		Elephant.GetComponent<Button> ().interactable = true;
		Elephant.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		HideGO ();
	}

	/// <summary>
	/// Update the visuals to make the GO represent non-revivability.
	/// </summary>
	/// <param name="go">The GO whose corresponding piece is non-revivable.</param>
	void MakeNonrevivable (GameObject go) {
		go.GetComponent<Button> ().interactable = false;
		go.GetComponent<Image> ().color = new Color (1, 1, 1, .5f);
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

	/////////////////////////////////////////////////////////////////////////////////////////
	///  
	/// These functions are called by the buttons and in turn call Revive() on the correct GO.
	/// 
	/////////////////////////////////////////////////////////////////////////////////////////
	public void ReviveBoat () {
		Revive (Boat); 
	}

	public void ReviveHorse () {
		Revive (Horse); 
	}

	public void ReviveElephant () {
		Revive (Elephant); 
	}
}
