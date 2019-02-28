using UnityEngine;
using UnityEngine.UI;

public class Page : MonoBehaviour {



	public Sprite[] sprites;

	#region Private Fields
	GameObject text;
	GameObject image;
	int index = 0;
	float elapsedTime = 2f;
	#endregion

	// Use this for initialization
	void OnEnable () {
		text = GetComponentInChildren<Text> ().gameObject;
		image = GetComponentInChildren<Image> ().gameObject;
		image.GetComponent<Image> ().sprite = sprites[index];
	}

	void Update () {
		if (sprites != null && sprites.Length > 1) {
			// "Slideshow" through the sprites every 5(?) seconds.
			elapsedTime -= Time.deltaTime;
			if (elapsedTime <= 0) {
				image.GetComponent<Image> ().sprite = sprites [(++index) % sprites.Length];
				elapsedTime = 2f;
			}
		}
	}


	#region Page Display
	public void Display () {
		this.text.SetActive (true);
		this.image.SetActive (true);
	}

	public void Hide () {
		this.text.SetActive (false);
		this.image.SetActive (false);
	}
	#endregion
}
