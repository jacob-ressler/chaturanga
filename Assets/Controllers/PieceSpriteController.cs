using System.Collections.Generic;
using UnityEngine;

public class PieceSpriteController : MonoBehaviour {

	Dictionary<Piece, GameObject> pieceGameObjectMap;

	// Use this for initialization
	void Start () {
		
		// Instantiate Dictionary linking a piece's data to its GO
		pieceGameObjectMap = new Dictionary<Piece, GameObject>();

		Sprite[] pieceSprites = Resources.LoadAll<Sprite> ("Art/PieceSprites_Hi-Res");

		// This is an empty GO to hold all the piece GOs - for organization
		GameObject piece_holder = new GameObject ();
		piece_holder.name = "Pieces";
		piece_holder.transform.SetParent (this.transform);

		// Create a GameObject for each piece

		for (int i = 0; i < BoardController.Instance.board.Pieces.Length; i++) {
			Piece piece_data = BoardController.Instance.board.Pieces [i];
			GameObject piece_go = new GameObject ();

			// Add piece/GO pair to the dictionary
			pieceGameObjectMap.Add (piece_data, piece_go);

			piece_go.name = piece_data.Color + "_" + piece_data.Type;
			piece_go.transform.position = new Vector3 (piece_data.CurrTile.X, piece_data.CurrTile.Y, 0);
			piece_go.transform.SetParent (piece_holder.transform);
			SpriteRenderer piece_sr = piece_go.AddComponent<SpriteRenderer> ();
			piece_sr.sortingLayerName = "Pieces";
			foreach (Sprite s in pieceSprites) {
				if (s.name == piece_go.name) {
					piece_sr.sprite = s;
				}
			}

			// Register each piece with the necessary visual function callback
			piece_data.RegisterPieceTileChanged (OnPieceTileChanged);
		}
	}

	/// <summary>
	/// Raises the piece tile changed event.
	/// Updates the graphical position of a piece after it has moved tiles.
	/// </summary>
	/// <param name="piece_data">The data of the piece that moved tiles.</param>
	public void OnPieceTileChanged (Piece piece_data) {
		if (pieceGameObjectMap.ContainsKey (piece_data) == false) {
			Debug.LogError ("OnPiecePositionChanged -- The piece data is not in the dictionary.");
		}
		// Fetch GO linked with the piece.
		GameObject piece_go = pieceGameObjectMap [piece_data];

		if (piece_data.HasBeenKilled ()) {
			// Deactivate the GO, instead of re-positioning it.
			piece_go.SetActive (false);
			return;
		}

		if (piece_go.activeSelf == false && piece_data.HasBeenKilled () == false) {
			// The piece must have just been revived, so reactivate it's GO.
			piece_go.SetActive (true);
		}
		// Otherwise, match the piece's position with the GO's position.
		piece_go.transform.position = new Vector3 (piece_data.CurrTile.X, piece_data.CurrTile.Y, 0);
		}
		
}
