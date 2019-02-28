using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;

/// <summary>
/// Custom priority list with some special functions that will be used by game AI.
/// This is in no way optimized for speed, but it will get the job done. 
/// NOTE: HIGHER PRIORITY MEANS BETTER RANKING.
/// </summary>
public class PriorityList <T> {

	List<T> list;

	// Use these to keep track of the connections between pieces and tiles.
	public Dictionary<T, float> TilePriorityMap;
	//maybe unnecessary
	/*public Dictionary<T, int> PiecePriorityMap;
	public Dictionary<T, T> TilePieceMap;*/

	public int Count {
		get {
			return list.Count;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="PriorityList"/> class.
	/// </summary>
	/// <param name="t">T.</param>
	public PriorityList () {
		list = new List<T> ();
		TilePriorityMap = new Dictionary<T, float> ();
		//may be unnecessary...
		/*PiecePriorityMap = new Dictionary<T, int> ();
		TilePieceMap = new Dictionary<T, T> ();*/
	}

	/// <summary>
	/// Add the specified T to the list based on its priority.
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="priority">Priority.</param>
	public void Add (T t, float priority) {
		//
		TilePriorityMap.Add (t, priority);
		/*TilePieceMap.Add (t, p);
		PiecePriorityMap.Add (p, priority); */
		AddByPriority (t, priority);
	}

	/// <summary>
	/// Remove the specified T from the list.
	/// </summary>
	/// <param name="t">T.</param>
	public void Remove (T t) {
		list.Remove (t);
		TilePriorityMap.Remove (t);
	}

	/// <summary>
	/// Checks if T is in the list.
	/// </summary>
	/// <param name="t">T.</param>
	public bool Contains (T t) {
		return list.Contains (t);
	}

	/// <summary>
	/// Clear the list.
	/// </summary>
	public void Clear () {
		list.Clear ();
		TilePriorityMap.Clear ();
	}

	/// <summary>
	/// Converts the list to an array.
	/// </summary>
	/// <returns>The array.</returns>
	public T[] ToArray () {
		return list.ToArray ();
	}

	public T GetAt (int index) {
		return list [index];
	}

	public int IndexOf (T t) {
		return list.IndexOf (t);

	}

	/// <summary>
	/// Gets a random element in the specified percentage of the list.
	/// </summary>
	/// <returns>An element of the list.</returns>
	/// <param name="percentage">Percentage as a float between 0 and 1.</param>
	public T GetRandom (float percentage = 1) {
		if (list.Count == 1) {
			return list [0];
		}
		List<T> sublist = list.GetRange (0, Mathf.Clamp((int)(percentage * list.Count), 1, list.Count));
		int rand = Random.Range (0, sublist.Count);
		return sublist [rand];
	}

	/// <summary>
	/// Inserts the newly added T into the list at based on its priority.
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="priority">Priority.</param>
	void AddByPriority (T t, float priority) {
		// Loop through each element of the list to find where the new element should go.
		for (int i = 0; i < list.Count; i++) {
			if (TilePriorityMap [list[i]] <= priority) {
				// This is where the new element should go, so insert it.
				list.Insert (i, t);
				return;
			}
		}
		// The new element has the lowest priority, so add it to the end.
		list.Add (t);
	}


}
