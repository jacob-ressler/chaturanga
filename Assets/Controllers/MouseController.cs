using UnityEngine;

public class MouseController : MonoBehaviour {

	public GameObject tileHighlight;
	Vector3 currentPosition;
	Tile selectedTile;
	Dice currentDice;


	// Update is called once per frame
	void Update () {
		
		if (BoardController.Instance.board.GameMode != Game_Mode.DEFAULT) {
			// We are not "in-game" in the sense that moves can't be made right now, so just return.
			return;
		}

		if (Input.GetKeyUp ("n")) {
			BoardController.Instance.NextTurn ();
			return;
		}

		if (BoardController.Instance.board.players[BoardController.Instance.board.CurrentTurn].isAI ()) {
			// The AI is going right now, so just return.
			return;
		}

		// Get the current coords of the mouse
		if (Input.mousePresent) {
			HandleMouseControls ();

		} else if (Input.touchSupported) {
			HandleTouchControls ();
		}
	}

	void HandleMouseControls () {
		currentPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		// Get the tile at the mouse's position (if there is one)
		Tile tileMO = GetTileAtWorldPosition (currentPosition);

		// Disabling MO highlighting, because it was causing some weird visual things with touch controls
		UpdateMouseOver (tileMO);

		UpdateMouseClick (tileMO);
	}

	void HandleTouchControls () {
		if (Input.touchCount != 1) {
			// Too many/few touches! Don't do anything.
			return;
		}

		if (Input.touches [0].phase == TouchPhase.Began) {
			// This is the beginning of a touch, so change the current position accordingly
			currentPosition = Camera.main.ScreenToWorldPoint (new Vector3 (
				Input.GetTouch (0).position.x, 
				Input.GetTouch (0).position.y,
				0)
			);

			Tile tileTouched = GetTileAtWorldPosition (currentPosition);

			UpdateMouseClick (tileTouched); 
		}
	}

	/// <summary>
	/// Gets the tile at the mouse's world position.
	/// </summary>
	/// <returns>The tile at the mouse's world position.</returns>
	/// <param name="worldPos">World position of the mouse.</param>
	Tile GetTileAtWorldPosition (Vector3 worldPos) {
		int x = Mathf.FloorToInt (worldPos.x);
		int y = Mathf.FloorToInt (worldPos.y);

		return BoardController.Instance.board.GetTileAt (x, y);
	}

	/// <summary>
	/// If the mouse is over a tile, highlight that tile.
	/// Otherwise, hide the highlight.
	/// </summary>
	/// <param name="tileMO">The tile being moused over.</param>
	void UpdateMouseOver (Tile tileMO) {
		if (tileMO != null) {
			tileHighlight.SetActive (true);
			tileHighlight.transform.position = new Vector3 (tileMO.X, tileMO.Y, 0);
		} else {
			tileHighlight.SetActive (false);
		}
	}
		

	/// <summary>
	/// Handles all mouse clicks / touches.
	/// </summary>
	/// <param name="tileMO">The tile that was clicked / touched.</param>
	void UpdateMouseClick (Tile tileMO) {
		if (tileMO == null) {
			// We are not over a tile, so we should return.
			return;
		}

		// If RMB was clicked, handle it (@DEBUGGING).
		if (Input.GetMouseButtonUp (1)) {
			HandleRightClick (tileMO);
			return;
		}

		// If LMB was clicked or touch occurred, determine how it should be handled.
		if (Input.GetMouseButtonUp (0) || Input.touchCount > 0) {

			// Handle move-making.
			if (tileMO.Type == TileType.Move) {
				HandleMove (tileMO);
				return;
			}

			if (tileMO.Type == TileType.Selected) {
				// Deselect this tile.
				HandleDeselection (tileMO);
				return;
			}

			HandleDeselection (selectedTile);

			// Make sure a tile was actually clicked on before determining the action to be taken.
			if (tileMO == null) {
				Debug.LogError ("UpdateMouseClick -- The tile clicked on is null.");
				return;
			}
				
			if (tileMO.Type == TileType.Default) {
				// Select this tile.
				HandleSelection (tileMO);
				return;
			}


		}
	}


	/// <summary>
	/// Handles the deselection of a tile.
	/// </summary>
	/// <param name="tile">The tile to be deselected.</param>
	void HandleDeselection (Tile tile) {
		// Deselect the tile. Do this so long as the tile clicked on was not a "Move" tile.
		if (tile != null) {
			tile.Type = TileType.Default;
			// Deselect the tile's piece as well, to make sure there aren't any leftover "Move" tiles.
			if (tile.GetPiece () != null) {
				Piece pieceMO = tile.GetPiece ();
				pieceMO.Deselect ();
				Debug.Log ("Tile (" + selectedTile.X + "," + selectedTile.Y + ") selected: " + 
					pieceMO.Color.ToString () + "_" + pieceMO.Type.ToString ());
			}
		}
	}


	/// <summary>
	/// Handles any right clicks.
	/// </summary>
	/// <param name="tileMO">The tile clicked on.</param>
	void HandleRightClick (Tile tileMO) {
		// This is for debugging. Right-click to get the tile's coords and it's piece.
		if (tileMO != null) {
			Debug.Log ("Tile: (" + tileMO.X + "," + tileMO.Y + ")\n");
		}
		else {
			Debug.Log ("This is not a valid tile, so no information can be given.");
		}
		if(tileMO.GetPiece () != null) {
			Debug.Log ("Piece: " + tileMO.GetPiece().Color.ToString () + "_" + tileMO.GetPiece().Type.ToString ());
		}
		else {
			Debug.Log ("This tile has no piece.");
		}
	}


	/// <summary>
	/// Handles left clicks on move tiles.
	/// </summary>
	/// <param name="tileMO">The tile clicked on.</param>
	void HandleMove (Tile tileMO) {
		// First, check if the tile clicked on has a piece. This will be used to kill the piece,
		// since 2 pieces can't occupy the same tile.
		Piece pieceMO = tileMO.GetPiece ();

		// The tile is a valid move for the piece on selectedTile, so it should make the move.
		Piece selectedPiece = selectedTile.GetPiece();
		selectedPiece.CurrTile = tileMO;
		//Debug.Log("Piece moved to Tile ("+tileMO.X+","+tileMO.Y+") from Tile (" +
			//selectedTile.X + "," + selectedTile.Y + ").");

		// Deselect everything and restore default tile values.
		selectedPiece.Deselect ();
		selectedTile.Type = TileType.Default;
		tileMO.Type = TileType.Default;

		// Mark the current dice as used, so it can't keep being used.
		// Since it uses the same type enum as pieces, Pawn acts as it's "used" type.
		currentDice.Use ();

		// Check if this was a killing move.
		if (pieceMO != null) {
			// Kill the piece.
			pieceMO.Kill ();
		}

		// Check if both dice have been used.
		if (BoardController.Instance.board.BothDiceHaveBeenUsed ()) {
			// Both dice have been used so automatically go to the next turn.
			// Call the board's function instead of the one here so the sound doesn't play.
			BoardController.Instance.board.NextTurn ();
		}


		if (BoardController.Instance.board.CheckForVictory () == true) {
			// This move has ended the game. Go to the victory screen.
			BoardController.Instance.board.TriggerVictoryMode (selectedPiece.Color);
			return;
		}

		// Check if this move triggered a pawn revival sequence
		if (selectedPiece.Type == PieceType.Pawn && selectedPiece.HasReachedEndOfBoard ()) {
			// Enter pawn revival mode.
			//Debug.Log ("HANDLEMOVE -- Selected Piece:" + selectedPiece.Color.ToString () + "_" + selectedPiece.Type.ToString ());
			BoardController.Instance.board.OnEnterPawnRevivalMode (selectedPiece);
			return;
		}

		// Since a move has been made, the valid moves should be recalculated for all pieces.
		//BoardController.Instance.board.FindAllValidMoves ();


	}


	/// <summary>
	/// Handles right clicks on tiles with selectable pieces.
	/// </summary>
	/// <param name="tileMO">Tile M.</param>
	void HandleSelection (Tile tileMO) {
		Piece pieceMO = tileMO.GetPiece();
		if (pieceMO == null) {
			Debug.Log ("HandleSelection -- The tile clicked on has no piece.");
			return;
		}
		if (pieceMO.Color != (PieceColor)BoardController.Instance.board.CurrentTurn) {
			Debug.Log ("HandleSelection -- The piece's color does not match the current turn.");
			return;
		}

		// If the piece doesn't match either of the dice, return.
		currentDice = GetDiceForPiece (pieceMO);
		if ( currentDice == null) {
			Debug.Log ("HandleSelection -- The piece's type does not match either of the dice.");
			return;
		}

		// Otherwise, the piece and the tile are valid and should both be selected.
		Select (tileMO, pieceMO);
	}
		

	/// <summary>
	/// Select the specified tile and piece.
	/// </summary>
	/// <param name="tileMO">The tile clicked on.</param>
	/// <param name="pieceMO">The piece clicked on.</param>
	void Select (Tile tileMO, Piece pieceMO) {
		selectedTile = tileMO;
		selectedTile.Type = TileType.Selected;
		pieceMO.Select ();
		Debug.Log ("Tile (" + selectedTile.X + "," + selectedTile.Y + ") selected: " + 
			pieceMO.Color.ToString () + "_" + pieceMO.Type.ToString ());
		return;
	}


	/// <summary>
	/// Gets the dice for piece.
	/// </summary>
	/// <returns>The dice for piece.</returns>
	/// <param name="p">The piece.</param>
	Dice GetDiceForPiece (Piece p) {
		foreach (Dice d in BoardController.Instance.board.Dice) {
			if (p.Type == d.Type && p.Type != PieceType.Pawn) {
				// This piece matches one of the dice.
				return d;
			}
			// Check if the piece's type is a pawn.
			if (p.Type == PieceType.Pawn) {
				// Check if the pawn is selectable (what a mess...).
				if (BoardController.Instance.board.GetPieceOfName (p.Color.ToString () + "_" +
					d.Type.ToString ()).HasBeenKilled () ||
					d.Type == PieceType.King) {
					// This piece matches one of the dice.
					return d;
				}
			}
		}
		return null;
	}

}
