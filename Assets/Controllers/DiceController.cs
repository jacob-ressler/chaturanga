using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {

	Dictionary<Dice, GameObject> diceGameObjectMap;

	Sprite[] diceSprites;

	// Use this for initialization
	void Start () {

		// Instantiate the dictionary that will map each dice to its GO.
		diceGameObjectMap = new Dictionary<Dice, GameObject> ();

		diceSprites = Resources.LoadAll<Sprite> ("Art/DiceSprites_Hi-Res");


		for (int i = 0; i < BoardController.Instance.board.Dice.Length; i++) {
			Dice dice_data = BoardController.Instance.board.Dice[i];

			// Create a GO for each of the dice.
			GameObject dice_go = new GameObject ();
			dice_go.name = "Dice";
			dice_go.transform.SetParent (this.transform);
			foreach (Sprite s in diceSprites) {
				if (s.name == dice_data.Type.ToString ()) {
					dice_go.AddComponent<SpriteRenderer> ().sprite = s;
				}
			}

			// Set the GO's position on the screen based on the value of i
			dice_go.transform.position = new Vector3(-2f, 5 - 3*i);
			dice_go.transform.localScale = Vector3.one * 1.5f;

			// Add the dice data and GO pair to the dictionary.
			diceGameObjectMap.Add (dice_data, dice_go);

			// Add function callback for when the dice's Type changes.
			dice_data.RegisterDiceTypeChanged (OnDiceTypeChanged);
		}
	}
	
	/// <summary>
	/// Raises the dice type changed event.
	/// Updates the graphic for a dice.
	/// </summary>
	/// <param name="dice_data">The data for the dice whose type was changed.</param>
	void OnDiceTypeChanged(Dice dice_data) {
		if (diceGameObjectMap.ContainsKey(dice_data) == false) {
			Debug.LogError ("OnTileTypeChanged -- The tile data is not in the dictionary.");
		}

		GameObject dice_go = diceGameObjectMap [dice_data];
		foreach (Sprite s in diceSprites) {
			if (s.name == dice_data.Type.ToString () ||
				(s.name == "None" && dice_data.Type == PieceType.Pawn)) {
				dice_go.GetComponent<SpriteRenderer> ().sprite = s;
				break;
			}

		}
/*		Debug.Log ("Dice Type: " + dice_data.Type.ToString () +
		"\nDice Sprite: " + dice_go.GetComponent<SpriteRenderer> ().sprite.name);
*/	}
}
