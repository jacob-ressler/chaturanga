using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour {


	#region Public Fields
	public Button Previous;
	public Button Next;
	#endregion

	#region Private Fields
	MainMenuController mmc;
	int currPage;
	List<Page> pages;
	#endregion

	// Use this for initialization
	void Start () {
		mmc = GameObject.FindObjectOfType<MainMenuController> ();
		pages = GetComponentsInChildren<Page> ().ToList ();

		currPage = 0;
		pages [currPage].Display ();
		for (int i = 1; i < pages.Count; i++) {
			pages [i].Hide ();
		}
		Previous.interactable = false;
		gameObject.SetActive (false);
	}


	#region Button Functions

	public void NextPage () {
		if (currPage == 0) {
			// Last page.
			Previous.interactable = true;
		}

		pages [currPage].Hide ();
		pages [++currPage].Display ();

		if (currPage == pages.Count - 1) {
			// Last page.
			Next.interactable = false;
		}
	}

	public void PreviousPage () {
		if (currPage == pages.Count - 1) {
			// Last page.
			Next.interactable = true;
		}
		pages [currPage].Hide ();
		pages [--currPage].Display ();

		if (currPage == 0) {
			// Last page.
			Previous.interactable = false;
		}
	}

	public void MainMenu () {
		mmc.MainMenuButtons.SetActive (true);
		mmc.H2PController.SetActive (false);
	}

	#endregion
}
