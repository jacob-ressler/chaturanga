using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Each piece can be either a King, Elephant, Horse, Boat, or Pawn.
/// </summary>
public enum PieceType { King, Elephant, Horse, Boat, Pawn }

/// <summary>
/// Each piece can be either Black, Blue, Red, Green.
/// The colors represent the 4 teams in the game.
/// Each team will have 1 king, 1 elephant, 1 horse, 1 boat, and 4 pawns.
/// </summary>
public enum PieceColor { Black, Blue, Red, Green }

public class Piece {

	public PieceColor Color { get; protected set; }
	public PieceType Type { get; protected set; }
	public List<Tile> ValidMoves { get; protected set; }

	Board board;

	Tile currTile;
	public Tile CurrTile {
		get {
			return currTile;
		}
		set {
			currTile = value;
			if (PieceTileChanged != null) {
				PieceTileChanged (this);
			}
		}
	}

	// @Actions
	Action<Piece> PieceTileChanged;
	Action<Piece> EnterPawnRevivalMode;
	// Sound Exclusive...
	Action<Piece> PieceSelected;
	Action<Piece> PieceKilled;

	/// <summary>
	/// Initializes a new instance of the <see cref="Piece"/> class.
	/// </summary>
	/// <param name="b">The board for this piece.</param>
	/// <param name="t">The current tile occupied by this piece.</param>
	/// <param name="pc">The color of this piece.</param>
	/// <param name="pt">The type of this piece.</param>
	public Piece (Board b, Tile t, PieceColor pc, PieceType pt) {
		this.board = b;
		this.currTile = t;
		this.Color = pc;
		this.Type = pt;
		ValidMoves = new List<Tile> ();
		//FindValidMoves ();
	}

	protected Piece ( Piece p ) {
		this.board = p.board;
		this.Color = p.Color; 
		this.currTile = p.currTile;
		this.Type = p.Type;
	}

	public Piece Clone() {
		return new Piece (this);
	}

	/// <summary>
	/// Finds all the possible moves for a piece based on its color, type, and current tile.
	/// A specific tile can be specified for future-checking in the AI move finding.
	/// </summary>
	public List<Tile> FindValidMoves(Tile t) {
		List<Tile> possibleMoves = new List<Tile> ();
		if (this.HasBeenKilled ()) {
			// This piece is dead, so return an empty list.
			//Debug.Log ("Piece::FindValidMoves -- Tried to find moves for a piece that is already dead.");
			return possibleMoves;
		}
		// Clear the list of old entries, since they may not be valid in the new location.
		//ValidMoves.Clear ();
		// Use the piece's type to determine how to find the valid moves.
		if (Type == PieceType.Pawn) {
			// Checks both diagonals (i = -1, 1) and straight ahead (i = 0).
			for (int i = -1; i <= 1; i++) {
				// Use the color to determine direction the pawn can go.
				switch (Color) {
				case PieceColor.Black:
					possibleMoves.Add (board.GetTileAt (t.X + i, t.Y + 1));
					break;
				case PieceColor.Blue:
					possibleMoves.Add (board.GetTileAt (t.X + 1,t.Y + i));
					break;
				case PieceColor.Red:
					possibleMoves.Add (board.GetTileAt (t.X + i, t.Y - 1));
					break;
				case PieceColor.Green:
					possibleMoves.Add (board.GetTileAt (t.X - 1, t.Y + i));
					break;
				}
			}
		} else if (Type == PieceType.Boat) {
			// Easiest to just get each one normally. No reason to overcomplicate things.
			possibleMoves.Add (board.GetTileAt (t.X - 2, t.Y - 2));
			possibleMoves.Add (board.GetTileAt (t.X + 2, t.Y - 2));
			possibleMoves.Add (board.GetTileAt (t.X - 2, t.Y + 2));
			possibleMoves.Add (board.GetTileAt (t.X + 2, t.Y + 2));

		} else if (Type == PieceType.Elephant) {
			// This is a bad-ish elephant algorithm, but it makes the validity check a lot simpler...

			// Check in each cardinal direction until there are no more tiles, an enemy, or an ally
			for (int i = 0; i < 2; i++) {
				// determines if checking negative of current position.
				for (int j = 0; j < 2; j++) {
					// determines if checking horizontal or vertical
					for (int k = 1; k < 8; k++) {
						int off = j == 0 ? -k : k;
						if (i == 0) {
							possibleMoves.Add (board.GetTileAt (t.X + off, t.Y));
						} else {
							possibleMoves.Add(board.GetTileAt (t.X, t.Y + off));
						}
						// **Direction Order: -x, x, -y, y
					}
				}
			}
		} else if (Type == PieceType.Horse) {
			// Check the 8 possible L-moves.
			for (int i = -1; i < 3; i = i + 2) {
				possibleMoves.Add(board.GetTileAt (t.X + i, t.Y + 2));
				possibleMoves.Add(board.GetTileAt (t.X + i, t.Y - 2));
				possibleMoves.Add(board.GetTileAt (t.X - 2, t.Y + i));
				possibleMoves.Add(board.GetTileAt (t.X + 2, t.Y + i));
			}
		} else if (Type == PieceType.King) {
			// Check its 8 neighbors.
			for (int i = 0; i < 2; i++) {
				possibleMoves.Add(board.GetTileAt (t.X + 1, t.Y - i));
				possibleMoves.Add(board.GetTileAt (t.X - 1, t.Y + i));
				possibleMoves.Add(board.GetTileAt (t.X + i, t.Y + 1));
				possibleMoves.Add(board.GetTileAt (t.X - i, t.Y - 1));
			}
		}

		if (t != currTile) {
			// This valid moves lookup was done for AI move-finding, so return a valid move list
			// WITHOUT setting it to ValidMoves.
			return ValidatePossibleMoves (possibleMoves);
		}

		// Otherwise just set ValidMoves and return a null.
		ValidMoves = ValidatePossibleMoves (possibleMoves); // Check all the possible moves for validity.
		return null;
		
	}

	/// <summary>
	/// Check all the possible moves for validity.
	/// </summary>
	/// <param name="moves">The list of all possible moves.</param>
	List<Tile> ValidatePossibleMoves (List<Tile> moves) {
		List<Tile> validTemp = new List<Tile> ();

		if (this.Type == PieceType.Pawn) {
			foreach (Tile t in moves) {
				if (t == null) {
					// There is no tile, so continue with next tile.
					continue;
				}
				if (t.X != this.CurrTile.X && t.Y != this.CurrTile.Y) { // validate diagonals
					if (t.GetPiece() != null && t.GetPiece().Color != this.Color) {
						// This is a valid diagonal move.
						validTemp.Add (t);
					}
				} else if (t.GetPiece() == null) { // validate forward
					// This is a valid normal move
					validTemp.Add (t);
				}
			}
		} else if (this.Type == PieceType.Elephant) {
			// **Direction Order: -x, x, -y, y
			for (int i = 0; i < 4; i++) { // each of the 4 cardinal directions
				for (int j = 0; j < 7; j++) { // the 7 (potential) tiles in "moves" for each direction
					Tile t = moves [7 * i + j];
					if (t != null && t.GetPiece() == null) {
						// This is a valid move.
						validTemp.Add (t);
					} else if (t != null && t.GetPiece() != null && t.GetPiece().Color != this.Color) {
						// This is the last valid move in this direction. Add it and force the next direction.
						validTemp.Add (t);
						j = 7;
					} else {
						// The tile was invalid, so all other tiles in that direction will also be invalid.
						// This will force the next direction to be evaluated.
						j = 7;
					}
				}
			}	
		} else if (this.Type == PieceType.Boat) {
			foreach (Tile t in moves) {
				if (t == null) {
					// The tile is not on the board, so check the next tile.
					continue;
				} 
				if (t.GetPiece () != null && t.GetPiece ().Color == this.Color) {
					// The tile has a friendly piece, so check the next tile.
					continue;
				}
				if (t.GetPiece () != null && t.GetPiece ().Type == PieceType.King) {
					// The boat cannot kill kings, so this move is not valid. Check the next tile.
					continue;
				}
				// The tile is valid.
				validTemp.Add (t); 
			}
			
		} else if (this.Type == PieceType.Horse || this.Type == PieceType.King) {
			// These 2 types all follow the same simple validity logic, so handle them all here.
			foreach (Tile t in moves) {
				if (t == null) {
					// The tile is not on the board, so check the next tile.
					continue;
				} 
				if (t.GetPiece () != null && t.GetPiece ().Color == this.Color) {
					// The tile has a friendly piece, so check the next tile.
					continue;
				}
				// The tile is valid.
				validTemp.Add (t); 
			}
		}
		return validTemp;

	}

	/// <summary>
	/// Show the valid moves for this piece.
	/// </summary>
	public void Select() {
		// Change each valid move tile's type to "Move".
		FindValidMoves (this.currTile);
		foreach (Tile t in ValidMoves) {
			t.Type = TileType.Move;
		}

		if (PieceSelected != null) {
			PieceSelected (this);
		}
	}

	/// <summary>
	/// Hide the valid moves for this piece.
	/// </summary>
	public void Deselect() {
		// Restore all the valid move tiles to their default type
		foreach (Tile t in ValidMoves) {
			t.Type = TileType.Default;
		}
	}

	/// <summary>
	/// Kill this piece by nullifying its current tile.
	/// </summary>
	public void Kill () {
		// "Kill" this piece by making it's CurrTile null.
		CurrTile = null;
		if (PieceKilled != null) {
			PieceKilled (this);
		}
	}

	/// <summary>
	/// Check if this piece is dead by seeing if its current tile is null.
	/// </summary>
	/// <returns><c>true</c> if this instance has been killed; otherwise, <c>false</c>.</returns>
	public bool HasBeenKilled () {
		// Since currTile should only be null if a piece is dead, return if it is null.
		return currTile == null;
	}

	/// <summary>
	/// Determines whether this piece has reached end of board (only applies to pawns).
	/// </summary>
	/// <returns><c>true</c> if this instance has reached end of board; otherwise, <c>false</c>.</returns>
	public bool HasReachedEndOfBoard () {
		if (Type == PieceType.Pawn) {
			switch (Color) {
			case PieceColor.Black:
				return (board.GetTileAt (CurrTile.X, CurrTile.Y + 1) == null);
			case PieceColor.Blue:
				return (board.GetTileAt (CurrTile.X + 1,CurrTile.Y) == null);
			case PieceColor.Red:
				return (board.GetTileAt (CurrTile.X, CurrTile.Y - 1) == null);
			case PieceColor.Green:
				return (board.GetTileAt (CurrTile.X - 1, CurrTile.Y) == null);
			}
		}

		Debug.LogError ("HasReachedEndOfBoard -- Called on a non-pawn. Do you have a mis-call somewhere?"); 
		return false;
	}

	public void TriggerRevival () {
		EnterPawnRevivalMode (this);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 
	///  Registering and unregistering callbacks...
	/// 
	////////////////////////////////////////////////////////////////////////////////////////////////////////


	public void RegisterPieceTileChanged (Action<Piece> callback) {
		PieceTileChanged += callback;
	}
		
	public void UnregisterPieceTileChanged (Action<Piece> callback) {
		PieceTileChanged -= callback;
	}

	public void RegisterEnterPawnRevivalMode (Action<Piece> callback) {
		EnterPawnRevivalMode += callback;
	}

	public void UnregisterEnterPawnRevivalMode (Action<Piece> callback) {
		EnterPawnRevivalMode -= callback;
	}

	public void RegisterPieceSelected (Action<Piece> callback) {
		PieceSelected += callback;
	}

	public void UnregisterPieceSelected (Action<Piece> callback) {
		PieceSelected -= callback;
	}

	public void RegisterPieceKilled (Action<Piece> callback) {
		PieceKilled += callback;
	}

	public void UnregisterPieceKilled (Action<Piece> callback) {
		PieceKilled -= callback;
	}
}
