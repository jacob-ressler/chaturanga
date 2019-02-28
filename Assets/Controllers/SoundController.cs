using UnityEngine;

public class SoundController : MonoBehaviour {

	public static SoundController Instance;

	#region Private Fields
	const float SHORT_DELAY = 0.1f; //.2f; // 200ms delay
	const float LONG_DELAY = 0.2f; //1f; // 1s delay
	float  delay = 0;

	public bool musicOn;
	public bool sfxOn;


	AudioClip bgMusic;
	float bgMusicTime;

	AudioClip bgAudio;
	float bgAudioTime;
	#endregion

	// Use this for initialization
	void Start () {
		
		Instance = this;

		musicOn = PlayerPrefs.GetInt ("Music_Enabled") == 1 ? true : false;
		sfxOn = PlayerPrefs.GetInt ("SFX_Enabled") == 1 ? true : false;

		if (gameObject.scene.name == "GameScene") {
			BoardController.Instance.board.RegisterPieceSelected (OnPieceSelected);
			BoardController.Instance.board.RegisterPieceTileChanged (OnPieceTileChanged);
			BoardController.Instance.board.RegisterPieceKilled (OnPieceKilled);
			BoardController.Instance.board.RegisterEnterVictoryMode (OnVictoryModeEntered);
		}

		bgMusic = Resources.Load<AudioClip> ("Sounds/Background/Folk_Round");
		bgAudio = Resources.Load<AudioClip> ("Sounds/Background/Conversation");

		if (musicOn == false) {

			return;
		}

		AudioSource.PlayClipAtPoint (bgMusic, Camera.main.transform.position, .2f);
		bgMusicTime = bgMusic.length;

		AudioSource.PlayClipAtPoint (bgAudio, Camera.main.transform.position, .05f);
		bgAudioTime = bgAudio.length;
	}
	
	// Update is called once per frame
	void Update () {
		if (musicOn) {
			
		
			bgMusicTime -= Time.deltaTime;
			bgAudioTime -= Time.deltaTime;
			if (bgMusicTime <= 0) {
				AudioSource.PlayClipAtPoint (bgMusic, Camera.main.transform.position, .2f);
				bgMusicTime = bgMusic.length;
			}

			if (bgAudioTime <= 0) {
				AudioSource.PlayClipAtPoint (bgAudio, Camera.main.transform.position, .05f);
				bgAudioTime = bgAudio.length;
			}
		}

		if (delay > 0) {
			delay -= Time.deltaTime;
		}
	}

	public void OnMusicOnChanged () {
		if (musicOn) {
			AudioSource.PlayClipAtPoint (bgMusic, Camera.main.transform.position, .2f);
			bgMusicTime = bgMusic.length;

			AudioSource.PlayClipAtPoint (bgAudio, Camera.main.transform.position, .05f);
			bgAudioTime = bgAudio.length;
		}
		else {
			AudioSource[] gos = GameObject.FindObjectsOfType<AudioSource> ();
			foreach (AudioSource go in gos) {
				GameObject.Destroy (go.gameObject);
			}
		}
	}


	/// <summary>
	/// Raises the button clicked sound event.
	/// </summary>
	/// <param name="isBackButton">If set to <c>true</c>, the button clicked was a back button.</param>
	public void OnButtonClicked (string soundname = "Default") {
		if (delay > 0 || sfxOn == false) {
			// Not enough time has elapsed since the last sound effect.
			return;
		}

		AudioClip ac;
		string path = "Sounds/Button/Button_";

		// Get the right button sound.
		ac = Resources.Load<AudioClip> (path + soundname);

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		delay = SHORT_DELAY;
	}


	/// <summary>
	/// Raises the piece selected sound event.
	/// </summary>
	/// <param name="piece_data">Piece data.</param>
	void OnPieceSelected (Piece piece_data)  {
		if (delay > 0 || sfxOn == false) {
			// Not enough time has elapsed since the last sound effect.
			return;
		}

		AudioClip ac;
		string path = "Sounds/Selection/";

		ac = Resources.Load<AudioClip> (path + piece_data.Type.ToString ());

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		delay = SHORT_DELAY;
	}


	/// <summary>
	/// Raises the piece tile changed sound event.
	/// </summary>
	/// <param name="piece_data">Piece data.</param>
	void OnPieceTileChanged (Piece piece_data) {
		if (delay > 0 || sfxOn == false) {
			// Not enough time has elapsed since the last sound effect.
			return;
		}

		// Find sound for the given piece.
		AudioClip ac;
		string path = "Sounds/Move/";

		ac = Resources.Load<AudioClip> (path + piece_data.Type.ToString ());

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		delay = SHORT_DELAY;
	}


	void OnPieceKilled (Piece piece_data) {
		if (delay > 0 || sfxOn == false) {
			// Not enough time has elapsed since the last sound effect.
			return;
		}

		// Find sound for the given piece.
		AudioClip ac;
		string path = "Sounds/Killed/";

		ac = Resources.Load<AudioClip> (path + piece_data.Type.ToString ());

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		delay = SHORT_DELAY;
	}


	void OnVictoryModeEntered (PieceColor pc) {
		
		if (sfxOn == false) {
			return;
		}

		// Find sound for the given piece.
		AudioClip ac;
		string path = "Sounds/Victory/";

		ac = Resources.Load<AudioClip> (path + pc.ToString ());

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position, 0.7f);
		delay = SHORT_DELAY;
	}


	public void OnPawnRevivalModeEntered (Piece p) {

		if (delay > 0 || sfxOn == false) {
			// Not enough time has elapsed since the last sound effect.
			return;
		}

		// Find sound for the given piece.
		AudioClip ac;
		string path = "Sounds/PawnRevival/";
		
		ac = Resources.Load<AudioClip> (path + p.Color.ToString ());

		ac = CheckIfClipExists (ac, path);

		// Play the audio at camera position.
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		delay = SHORT_DELAY;
	}








	/// <summary>
	/// Checks if clip exists.
	/// </summary>
	/// <returns>Audio clip.</returns>
	/// <param name="ac">Audio clip that may or may not exist.</param>
	AudioClip CheckIfClipExists (AudioClip ac, string path) {
		if (ac == null) {
			//Debug.LogError ("SoundController -- Tried to play a sound that does not exist. Did you forget to add it to Resources?" +
			//	"\nPATH: " + path);
			// This sound doesn't exist, so let's try to get a more general sound using the path.
			ac = Resources.Load<AudioClip> (path + "Default");
			if (ac == null) {
				//Debug.LogError ("SoundController -- No default sound for path: " + path);
				// Set  the sound to the general default.
				return Resources.Load<AudioClip> ("Sounds/Missing_Default");
			}


		}

		// The sound exists, so return it.
		return ac;
	}

}
