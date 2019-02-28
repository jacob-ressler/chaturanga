using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {
	
	enum PlayStyle { AGGRESSIVE, DEFENSIVE, NORMAL, SPORADIC }
	public enum DifficultyLevel { HUMAN, EASY, MEDIUM, HARD }

	public PieceColor Color;
	public Piece[] Pieces;

	public DifficultyLevel difficulty;
	PlayStyle style;

	public Board board;


	/// <summary>
	/// Initializes a new instance of the <see cref="Player"/> class.
	/// </summary>
	/// <param name="b">The board.</param>
	/// <param name="pc">The player's piece color.</param>
	/// <param name="isAI">If set to <c>true</c> the player is AI-controlled.</param>
	public void SetPlayer (Board b, PieceColor pc, DifficultyLevel diff = DifficultyLevel.HUMAN) {
		// Load this from a file (at least for AI difficulty level).
		this.board = b;
		this.Color = pc;
		this.difficulty = diff;
		if (this.isAI ()) {
			// Pick a random playstyle for this AI.
			Debug.Log (this.Color + " is AI."); 
			this.style = (PlayStyle)UnityEngine.Random.Range (0, 4);
		}

		if (this.board != null) {
			
			this.Pieces = new Piece[8];
			
			//this.IndexPieceMap = new Dictionary<int, Piece> ();
			
			int i = 0; // used for piece[] indexing.
			foreach (Piece p in board.Pieces) {
				if (p.Color == this.Color) {
					//Debug.Log ("Piece added for player " + this.Color.ToString () + ": "+ p.Type.ToString ());
					// This piece is part of our team.
					Pieces [i++] = p;
				}
			}
		}

		 
	}

	public bool isAI () { 
		return this.difficulty != DifficultyLevel.HUMAN;
	} 

	public bool IsOut () {
		//Debug.Log ("Player :: IsOut: Is Pieces null? " + (Pieces == null)); 
		return (GetPiece ("King").HasBeenKilled ());
	}


	public Piece GetPiece (string type) {
		foreach (Piece p in this.Pieces) {
			if (p.Type.ToString () == type) {
				//Debug.LogError (type);
				return p;
			}
		} 
		return null;
	}


	List<Piece> GetPieces (string type) {
		List<Piece> list = new List<Piece> ();
		foreach (Piece p in Pieces) {
			if (p.Type.ToString () == type ) {
				list.Add (p);
			}
		}
		return list;
	}

	public void MakeMove_AI () {
		Debug.Log ("MakeMove_AI"); 
		foreach (Dice d in board.Dice) {
			

			// Get the pieces that would be valid with this dice.
			Piece[] validPieces = GetPiecesForDice (d);

			if (validPieces.Length == 0) {
				// There are no valid pieces for this dice, so skip it.
				continue;
			}
			foreach (Piece p in validPieces) {
				p.FindValidMoves (p.CurrTile);
			}



			FindMove (validPieces);

			// Check if this move was a game-winner.
			if (board.CheckForVictory () == true) {
				board.TriggerVictoryMode (this.Color);
				return;
			}
		}
		// Check if there are only AI left alive. If so, make the next turn button press mandatory.
		for (int i = 0; i < board.players.Length; i++) {
			if (board.players[i].IsOut () == false && board.players[i].isAI () == false) {
				// There is still a human player in the game, so break out of the loop.
				break;
			}
			if (i == board.players.Length - 1) {
				// There are only AI left, so return here and force the user to press Next Turn.
				return;
			}
		}

		// Auto-call the next turn.
		board.NextTurn ();
	}



	Piece[] GetPiecesForDice (Dice d) {
		List<Piece> list = new List<Piece> ();
		foreach (Piece p in this.Pieces) {
			if (d.Type == p.Type) {
				if (p.HasBeenKilled () || p.Type == PieceType.King) {
					if (list.Contains (GetPiece ("Pawn"))) {
						// The list already has the pawns, so skip to the next piece.
						continue;
					}
					// Add the pawns to the list.
					list.AddRange (GetPieces ("Pawn"));
				} 
				if (p.HasBeenKilled () == false) {
					// Add this piece to the list.
					list.Add (p);
				}
			}
		}
		return list.ToArray ();
	}


	void FindMove (Piece[] pieces) {
		PriorityList<Tile> tlist = new PriorityList<Tile> ();
		PriorityList<Piece> plist = new PriorityList<Piece> ();
		for (int j = 0; j < pieces.Length; j++) {
			Piece p = pieces [j];


			for (int i = 0; i < p.ValidMoves.Count; i++) {
				Tile t = p.ValidMoves [i];
				float priority = CalculatePriority (t, p);
				tlist.Add(t.Clone (), priority);
				plist.Add (p.Clone (), priority);
			}
		}



		if (tlist.Count == 0) {
			// There are no valid moves for this dice, so return.
			return;
		}

		// Gets a random tile in the best 1/3, 2/3, or 3/3 of the list, depending on the AI's difficulty level.
		int index = tlist.IndexOf (tlist.GetRandom (1f / (float)difficulty));
		// Get the dummy tile and piece from the list.
		Tile dummyTile = tlist.GetAt (index);
		Piece dummyPiece = plist.GetAt (index);

		//Debug.Log ("CHOSEN MOVE'S PRIORITY: " + tlist.TilePriorityMap [dummyTile]);


		// Use the dummies to get the actual tile and piece.
		Tile moveTile = board.GetTileAt (dummyTile.X, dummyTile.Y);
		Piece movePiece = null;
		foreach (Piece p in this.Pieces) {
			if (p.Color == dummyPiece.Color && p.Type == dummyPiece.Type && p.CurrTile == dummyPiece.CurrTile) {
				movePiece = p;
				break;
			}
		}

		if (movePiece == null) {
			Debug.LogError ("FindMove -- The movePiece is null. This should never happen."); 
			return;
		}

		Piece target = moveTile.GetPiece ();
		if (target != null) {
			// This move will kill a piece.
			target.Kill ();
		}

		Debug.Log ("Wait Then Make Move..."); 

		movePiece.CurrTile = moveTile;

		// Handle pawn revival internally, since the AI can't really interact with buttons...
		if (movePiece.Type == PieceType.Pawn && movePiece.HasReachedEndOfBoard ()) {
			List<Piece> reviveList = new List<Piece> ();
			if (GetPiece ("Boat").HasBeenKilled ()) {
				reviveList.Add (GetPiece ("Boat"));
			}
			if (GetPiece ("Horse").HasBeenKilled ()) {
				reviveList.Add (GetPiece ("Horse"));
			}
			if (GetPiece ("Elephant").HasBeenKilled ()) {
				reviveList.Add (GetPiece ("Elephant"));
			}

			if (reviveList.Count > 0) {
				// We can revive, so pick one at random.
				Piece p = reviveList [UnityEngine.Random.Range (0, reviveList.Count)];
				p.CurrTile = moveTile;
				movePiece.Kill ();
			}

		}

	}


	float CalculatePriority (Tile t, Piece p) {
		float priority = 0;

		// Is this move a killing move? (+)
		priority += IsKillingMove (t, p);
		// Does this move lead to a chance for a killing move next turn? (+)
		priority += IsFutureKillingMove (t, p);
		// Is the piece currently at risk of being killed? (+)
		priority += IsInDanger (p);
		// Does this move put the piece at risk of being killed before next turn? (-)
		priority -= IsDangerous (t, p);
		// Does this move give the piece any valid moves next turn? (-)
		priority -= DisallowsFutureMoves (t, p);

		return priority;
	}


	float IsKillingMove (Tile t, Piece p) {
		float priority = 0;
		Piece target = t.GetPiece ();
		if (target == null) {
			// This isn't a killing move, so return 0.
			priority = 0;
		}
		else if (board.players[(int)target.Color].IsOut ()) {
			// This piece is effectively dead, since its player is out, so give it very low priority.
			priority = 5;
		}
		else if (target.Type == PieceType.Pawn) {
			priority = 50;
		} else if (target.Type == PieceType.Boat) {
			priority = 70;
		} else if (target.Type == PieceType.Horse) {
			priority = 100;
		} else if (target.Type == PieceType.Elephant) {
			priority = 100;
		} else if (target.Type == PieceType.King) {
			priority = 200;
		}

		// Modify priority based on playstyle and return it.
		switch (style) {
		case PlayStyle.AGGRESSIVE:
			return priority * 2f;
		case PlayStyle.DEFENSIVE:
			return priority * 0.5f;
		case PlayStyle.NORMAL:
			return priority;
		case PlayStyle.SPORADIC:
			return priority * UnityEngine.Random.Range (0.5f, 2f);
		}

		// This should never happen, but if it gets, here just return priority.
		Debug.LogError ("IsKillingMove -- Hit a return that should never be hit. Does this player not have a style?");
		return priority;
	}


	float IsFutureKillingMove (Tile tile, Piece p) {
		float priority = 0;

		List<Tile> futureMoves = p.FindValidMoves (tile);
		foreach (Tile t in futureMoves) {
			// Add to the priority for each future killing move.
			priority += IsKillingMove (t, p) * 0.5f;
		}

		// Since IsKillingMove already does the right style priority-weighting, it probably isn't needed here.
		return priority;
	}


	float IsInDanger (Piece p) {
		// Same logic as IsDangerous, just on the piece's current tile as opposed to its potential tile.
		return IsDangerous (p.CurrTile, p);
	}


	float IsDangerous (Tile tile, Piece piece) {
		// In this, high priority is BAD...
		float priority = 0;
		List<Tile> moveList;

		foreach (Piece p in board.Pieces) {
			if (p.Color == this.Color) {
				// We own this piece, so skip it.
				continue;
			}
			moveList = p.FindValidMoves (p.CurrTile);
			if ( moveList == null || moveList.Count == 0 ) {
				continue;
			}
			if (moveList.Contains (tile)) {
				// This enemy piece could make a move to kill our piece, so increase priority.

				priority += 1;

			}
		}

		// Modify priority based on playstyle and return it.
		switch (style) {
		case PlayStyle.AGGRESSIVE:
			return priority * 0.5f;
		case PlayStyle.DEFENSIVE:
			return priority * 2f;
		case PlayStyle.NORMAL:
			return priority;
		case PlayStyle.SPORADIC:
			return priority * UnityEngine.Random.Range (0.5f, 2);
		}

		// This should never happen, but if it gets, here just return priority.
		Debug.LogError ("IsDangerous -- Hit a return that should never be hit. Does this player not have a style?");
		return priority;
	}

	float DisallowsFutureMoves (Tile t, Piece p) {
		float priority = 0;
		if (p.FindValidMoves (t).Count == 0) {
			// Modify priority based on playstyle and return it.
			priority = 10;
		}

		switch (style) {
		case PlayStyle.AGGRESSIVE:
			return priority * 2f;
		case PlayStyle.DEFENSIVE:
			return priority * 0.5f;
		case PlayStyle.NORMAL:
			return priority;
		case PlayStyle.SPORADIC:
			return priority * UnityEngine.Random.Range (0.5f, 2f);
		}

		// This should never happen, but if it gets, here just return priority.
		Debug.LogError ("DisallowsFutureMoves -- Hit a return that should never be hit. Does this player not have a style?");
		return priority;
	}
}
