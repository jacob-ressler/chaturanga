using System;


public class Dice {

	public PieceType Type { get; set;}

	Action<Dice> DiceTypeChanged;

	/// <summary>
	/// Initializes a new instance of the <see cref="Dice"/> class.
	/// </summary>
	/// <param name="b">The board for the dice.</param>
	public Dice () {
		Roll ();
	}

	/// <summary>
	/// Roll the dice.
	/// </summary>
	public void Roll() {
		// Make Type one of 4 pieces at random (excludes Pawn, since it goes with King).
		Type = (PieceType)UnityEngine.Random.Range(0, 4);

		// Update the graphics to match the new type.
		if (DiceTypeChanged != null) {
			DiceTypeChanged (this);
		}
	}
		
	//FIXME: Make sure only one dice gets used and not both (will likely be handled in Board/BoardController).
	/// <summary>
	/// Marks this dice as used. This will happen if it was used to make a move.
	/// </summary>
	public void Use () {
		// Show that the dice has been used by giving it a type of None.
		Type = PieceType.Pawn;

		// Update the graphics to the blank dice sprite.
		if (DiceTypeChanged != null) {
			DiceTypeChanged (this);
		}
	}

	/// <summary>
	/// Append a function to the DiceTypeChanged action.
	/// </summary>
	/// <param name="call">The function to be appended.</param>
	public void RegisterDiceTypeChanged(Action<Dice> call) {
		DiceTypeChanged += call;
	}

	/// <summary>
	/// Remove a function from the DiceTypeChanged action.
	/// </summary>
	/// <param name="call">The function to be removed.</param>
	public void UnregisterDiceTypeChanged(Action<Dice> call) {
		DiceTypeChanged -= call;
	}
}
