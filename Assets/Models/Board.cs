using UnityEngine;
using System;


public enum Game_Mode { DEFAULT, PAWN_REVIVAL, VICTORY }

public class Board {

	public Game_Mode GameMode { get; set; }

	Tile[,] tiles;
	public Piece[] Pieces { get; protected set; }
	public Dice[] Dice { get; protected set;}

	public int Width { get; protected set; }
	public int Height { get; protected set; }

	// Value of 0, 1, 2, or 3 -- correlates to the PieceColor whose turn it is right now.
	public int CurrentTurn { get; protected set;}

	// @Actions
	Action<int> CurrentTurnChanged;
	Action<PieceColor> EnterVictoryMode;
	// For SFX...
	Action<Piece> PieceSelected;
	Action<Piece> PieceTileChanged;
	Action<Piece> PieceKilled;

	/// <summary>
	/// Initializes a new instance of the <see cref="Board"/> class.
	/// </summary>
	/// <param name="width">The width of the board in tiles.</param>
	/// <param name="height">the height of the board in tiles.</param>
	public Board (int width, int height) {
		// Create a game board of the specified width and height
		this.Width = width;
		this.Height = height;

		// Create tiles
		tiles = new Tile[width,height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x,y] = new Tile (this, x, y);
			}
		}
		//Debug.Log ("Tiles creation complete.");

		// Create pieces
		Pieces = new Piece[32];
		// This is the most hardcode-minimal way I could think of to create the pieces.
		// There is no real order to the pieces in the array, but that shouldn't be a problem.
		int index = 0;
		for (int j = 0; j < 4; j++) {
			// Non-pawns
			Pieces[index++] = new Piece (this, GetTileAt (j, 0), PieceColor.Black, (PieceType)(3-j) );
			Pieces[index++] = new Piece (this, GetTileAt (0, 7-j), PieceColor.Blue, (PieceType)(3-j) );
			Pieces[index++] = new Piece (this, GetTileAt (7-j, 7), PieceColor.Red, (PieceType)(3-j) );
			Pieces[index++] = new Piece (this, GetTileAt (7, j), PieceColor.Green, (PieceType)(3-j) );
			// Pawns
			Pieces[index++] = new Piece (this, GetTileAt (j, 1), PieceColor.Black, PieceType.Pawn );
			Pieces[index++] = new Piece (this, GetTileAt (1, 7-j), PieceColor.Blue, PieceType.Pawn );
			Pieces[index++] = new Piece (this, GetTileAt (7-j, 6), PieceColor.Red, PieceType.Pawn );
			Pieces[index++] = new Piece (this, GetTileAt (6, j), PieceColor.Green, PieceType.Pawn );

		}

		// It would be more efficient to register these callbacks to the pieces inside the loop, but it would also be super ugly...
		foreach (Piece p in Pieces) {
			p.RegisterPieceSelected (OnPieceSelected);
			p.RegisterPieceTileChanged (OnPieceTileChanged);
			p.RegisterPieceKilled (OnPieceKilled);
		}


		// Create players
		LoadPlayerSettingsFromLauncher ();

		// Create dice
		Dice = new Dice[2];
		for (int i = 0; i < Dice.Length; i++) {
			Dice [i] = new Dice ();
		}

		// Roll the dice.
		RollAllDice ();




		// Set the current turn to 0 (Black).
		CurrentTurn = 0;

		if (CurrentTurnChanged != null) {
			CurrentTurnChanged (CurrentTurn);
			return;
		}
		// Also find valid moves NOW, since all the pieces have been instantiated at this point.
		//FindAllValidMoves ();
	}

	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The tile at (x, y).<see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt (int x, int y) {
		if (x < 0 || x > 7 || y < 0 || y > 7) {
			//Debug.Log ("Tile " + x + ", " + y + " is out of range.");
			return null;
		}

		return tiles [x, y];
	}

	/// <summary>
	/// Gets the piece on a tile.
	/// </summary>
	/// <returns>The piece on the tile (or null). <see cref="Piece"/>.</returns>
	/// <param name="t">The tile to get the piece at.</param>
	public Piece GetPieceAt (Tile t) {
		if (t == null) {
			// The tile does not exist, so return null.
			return null;
		}
		int x = t.X;
		int y = t.Y;
		if (x < 0 || x > 7 || y < 0 || y > 7) {
			//Debug.Log ("Piece " + x + ", " + y + " is out of range.");
			return null;
		}
		foreach (Piece p in Pieces) {
			if (p.HasBeenKilled ()) {
				// This piece is dead and should be skipped.
				continue;
			}
			if (p.CurrTile.X == x && p.CurrTile.Y == y) {
				return p;
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the piece of a certain name.
	/// </summary>
	/// <returns>The piece that matches the specified name.</returns>
	/// <param name="name">The name of the desired piece (e.g. "Red_Boat").</param>
	public Piece GetPieceOfName (string name) {
		string s;
		foreach (Piece p in Pieces) {
			if (p == null) {
				// This piece is null, so skip it.
				Debug.LogError ("GetPieceOfName -- A piece is null. Did you lose a reference somewhere?"); 
				continue; 
			}
			// Set the string to this piece's color and type.
			s = p.Color.ToString () + "_" + p.Type.ToString ();

			if (s == name) {
				// This is the matching piece, so return it.
				return p;
			}
		}
		Debug.LogError ("GetPieceOfName -- No piece found with the name: '" + name + "'");
		return null;
	}

	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //

	public bool CheckForVictory () {
		if (GetNextTurn(CurrentTurn) == CurrentTurn) {
			// The current turn is the only valid turn left, which means there is a winner!
			return true;
		}
		return false;
	}

	public void TriggerVictoryMode (PieceColor pc) {
		// Set game mode to VICTORY and activate the victory screen.
		GameMode = Game_Mode.VICTORY;
		if (EnterVictoryMode == null) {
			Debug.LogError("EnterVictoryMode is null. :(");
		}
		EnterVictoryMode (pc);
	}

	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
	// // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //



	/// <summary>
	/// Finds all valid moves for all living pieces.
	/// </summary>
	/*public void FindAllValidMoves () {
		foreach (Piece p in players[CurrentTurn].Pieces) {
			if (p.HasBeenKilled ()) {
				// The piece is dead, so don't bother finding its valid moves.
				continue;
			}
			// This piece may potentially be moved soon, so find it's valid moves.
			p.FindValidMoves (p.CurrTile);
		}
		if (players[CurrentTurn].isAI() == true) {
			players [CurrentTurn].MakeMove_AI ();
		}
	}*/

	/// <summary>
	/// Roll all the dice.
	/// </summary>
	public void RollAllDice () {
		foreach (Dice d in Dice) {
			d.Roll ();
		}
	}

	/// <summary>
	/// Advance to the next turn.
	/// </summary>
	public void NextTurn () {
		// Roll the dice.
		RollAllDice ();

		// Set the current turn to the next team that isn't out (AKA their king is not dead).
		CurrentTurn = GetNextTurn (CurrentTurn);

		// Find the valid moves for the current turn's pieces.
		//FindAllValidMoves ();
		if (players [CurrentTurn].isAI ()) {
			// This is AI, so have it make its move(s).
			players [CurrentTurn].MakeMove_AI ();
		}

		// Make sure the turn graphic get updated.
		if (CurrentTurnChanged != null) {
			CurrentTurnChanged (CurrentTurn);
			return;
		}
		Debug.LogError ("Next Turn -- CurrentTurnChanged is null.");
	}

	// Figure out what the next turn will be.
	int GetNextTurn (int turn) {
		do {
			turn++;
			turn %= 4;
		} while (players[turn].IsOut());

		return turn;
	}

	/// <summary>
	/// Toggles the pawn revival screen on.
	/// </summary>
	/// <param name="p">The pawn that triggered pawn revival.</param>
	public void OnEnterPawnRevivalMode (Piece p) {
		GameMode = Game_Mode.PAWN_REVIVAL;
		Debug.Log ("BOARD -- Selected Piece:" + p.Color.ToString () + "_" + p.Type.ToString ());
		p.TriggerRevival ();
	}

	public bool BothDiceHaveBeenUsed () {
		if (Dice[0].Type == PieceType.Pawn && Dice[1].Type == PieceType.Pawn) {
			// Both dice have been used, so return true.
			return true;
		}
		// At least one of teh dice has not been used, so return false.
		return false;
	}



	void OnPieceSelected (Piece piece_data) {
		if (PieceSelected != null) {
			PieceSelected (piece_data);
		}
	}


	void OnPieceTileChanged (Piece piece_data) {
		if (PieceTileChanged != null) {
			PieceTileChanged (piece_data);
		}
	}

	void OnPieceKilled (Piece piece_data) {
		if (PieceKilled != null) {
			PieceKilled (piece_data);
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////
	/// 
	///  Registering and unregistering callbacks...
	/// 
	//////////////////////////////////////////////////////////////////////////////////////////////



	public void RegisterCurrentTurnChanged (Action<int> call) {
		CurrentTurnChanged += call;
	}

	public void UnregisterCurrentTurnChanged (Action<int> call) {
		CurrentTurnChanged -= call;
	}

	public void RegisterEnterVictoryMode (Action<PieceColor> call) {
		EnterVictoryMode += call;
	}

	public void UnregisterEnterVictoryMode (Action<PieceColor> call) {
		EnterVictoryMode -= call;
	}

	public void RegisterPieceSelected (Action<Piece> call) {
		PieceSelected += call;
	}

	public void UnregisterPieceSelected (Action<Piece> call) {
		PieceSelected -= call;
	}

	public void RegisterPieceTileChanged (Action<Piece> call) {
		PieceTileChanged += call;
	}

	public void UnregisterPieceTileChanged (Action<Piece> call) {
		PieceTileChanged -= call;
	}

	public void RegisterPieceKilled (Action<Piece> call) {
		PieceKilled += call;
	}

	public void UnregisterPieceKilled (Action<Piece> call) {
		PieceKilled -= call;
	}



	/////////////////////////////////////////////////////////////////////////////////////////
	/// 
	/// 							LOADING PLAYER SETTINGS
	/// 
	/////////////////////////////////////////////////////////////////////////////////////////
	public Player[] players;

	void LoadPlayerSettingsFromLauncher () {
		// Create 4 "blank" players.
		players = new Player [4];

		for (int i = 0; i < players.Length; i++) {
			GameObject go = new GameObject (((PieceColor)i).ToString ());
			go.transform.SetParent (BoardController.Instance.transform);
			players [i] = go.AddComponent<Player> ();
			//Debug.Log (players [i].difficulty.ToString ()); 
			
			int color = PlayerPrefs.GetInt ("Player_"+i+"_Color");
			int difficulty = PlayerPrefs.GetInt ("Player_" + i + "_Difficulty");
			players [i].SetPlayer (this, (PieceColor)color, (Player.DifficultyLevel)difficulty);
			Debug.Log (this == players[i].board); 
			Debug.Log (players [i].Pieces == null); 
		}
	}

}
