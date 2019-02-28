using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour {

	Text text;
	PieceColor currTurn;

	// Use this for initialization
	void Start () {
		// Get the current turn color and set it to the text.
		currTurn = (PieceColor)BoardController.Instance.board.CurrentTurn;
		text = gameObject.GetComponent<Text> ();
		if (text == null) {
			Debug.Log ("TurnController -- The text component is null.");
		}
		text.text = currTurn.ToString ().ToUpper ();
		//Debug.Log ("Current Turn: " + text.text);
		// Set the color of the text.
		switch (currTurn) {
		case PieceColor.Black:
			text.color = Color.black;
			break;
		case PieceColor.Blue:
			text.color = Color.blue;
			break;
		case PieceColor.Red:
			text.color = Color.red;
			break;
		case PieceColor.Green:
			text.color = Color.green;
			break;
		}

		BoardController.Instance.board.RegisterCurrentTurnChanged (OnCurrentTurnChanged);
	}

	/// <summary>
	/// Raises the current turn changed event.
	/// Update the text graphic to show the proper team color as the current turn.
	/// </summary>
	/// <param name="currentTurn">The integer value associated with the current turn.</param>
	void OnCurrentTurnChanged (int currentTurn) {
		// Set text.
		currTurn = (PieceColor)currentTurn;
		text.text = currTurn.ToString ().ToUpper ();
		// Set text color.
		switch (currTurn) {
		case PieceColor.Black:
			text.color = Color.black;
			break;
		case PieceColor.Blue:
			text.color = Color.blue;
			break;
		case PieceColor.Red:
			text.color = Color.red;
			break;
		case PieceColor.Green:
			text.color = Color.green;
			break;
		}
	}

}
