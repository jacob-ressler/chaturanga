using System;

/// <summary>
/// Each tile can be one of 3 types: Default, Move, or Selected.
/// </summary>
public enum TileType { Default, Move, Selected }

public class Tile {

	// @Actions
	Action<Tile> TileTypeChanged;

	TileType type = TileType.Default;
	public TileType Type {
		get {
			return type;
		}
		set {
			type = value;
			if (TileTypeChanged != null) {
				TileTypeChanged (this);
			}
		}
	}

	public int X { get; protected set; }

	public int Y { get; protected set; }

	Board board;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="board">The board this tile is a part of.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile (Board board, int x, int y) {
		this.board = board;
		this.X = x;
		this.Y = y;
	}


	protected Tile ( Tile t ) {
		this.board = t.board;
		this.type = t.type;
		this.X = t.X;
		this.Y = t.Y;
	}

	public Tile Clone() {
		return new Tile (this);
	}


	/// <summary>
	/// Gets the piece on this tile.
	/// </summary>
	/// <returns>The piece on this tile.</returns>
	public Piece GetPiece() {
		return board.GetPieceAt (this);
	}

	/// <summary>
	/// Append a function to be called when the tile's type changes.
	/// </summary>
	/// <param name="callback">A function to be appended to the TileTypeChanged action.</param>
	public void RegisterTileTypeChanged (Action<Tile> callback) {
		TileTypeChanged += callback;
	}

	/// <summary>
	/// Remove a function from being called when the tile's type changes
	/// </summary>
	/// <param name="callback">A function to be removed from the TileTypeChanged action.</param>
	public void UnregisterTileTypeChanged (Action<Tile> callback) {
		TileTypeChanged -= callback;
	}
}
