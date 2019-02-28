using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {

	Dictionary<Tile, GameObject> TileGameObjectMap;

	Sprite[] tileSprites;

	// Use this for initialization
	void Start () {

		// Get the tile sprites.
		tileSprites = Resources.LoadAll<Sprite> ("Art/TileSprites_Hi-Res");

		// Instantiate Dictionary linking a tile's data to its GO
		TileGameObjectMap = new Dictionary<Tile, GameObject>();

		// This is an empty GO to hold all the tile GOs - for organization
		GameObject tile_holder = new GameObject ();
		tile_holder.name = "Tiles";
		tile_holder.transform.SetParent (this.transform);

		// Create a GameObject for each tile
		for (int x = 0; x < BoardController.Instance.board.Width; x++) {
			for (int y = 0; y < BoardController.Instance.board.Height; y++) {
				// Get the tile data
				Tile tile_data = BoardController.Instance.board.GetTileAt (x, y);
				// Create a GO for the tile
				GameObject tile_go = new GameObject ();

				// Add tile/GO pair to the dictionary
				TileGameObjectMap.Add (tile_data, tile_go);

				tile_go.name = "Tile (" + x + "," + y + ")";
				tile_go.transform.position = new Vector3 (tile_data.X, tile_data.Y);
				tile_go.transform.SetParent (tile_holder.transform);
				SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer> ();
				tile_sr.sortingLayerName = "Tiles";
				tile_sr.sprite = tileSprites [(int)tile_data.Type];

				// Register all function callbacks
				tile_data.RegisterTileTypeChanged (OnTileTypeChanged);
			}
		}
	}

	/// <summary>
	/// Raises the tile type changed event.
	/// Changes the sprite to reflect the type for a given tile.
	/// Used to show selection and valid moves.
	/// </summary>
	/// <param name="tile_data">The data of the tile whose type was changed.</param>
	void OnTileTypeChanged (Tile tile_data) {

		if (TileGameObjectMap.ContainsKey(tile_data) == false) {
			Debug.LogError ("OnTileTypeChanged -- The tile data is not in the dictionary.");
		}
		// Fetch GO linked with the tile.
		GameObject tile_go = TileGameObjectMap [tile_data];

		// Match the tile's type with the corresponding sprite.
		tile_go.GetComponent<SpriteRenderer> ().sprite = tileSprites[(int)tile_data.Type];
	}
}
