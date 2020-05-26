using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Chest_Audio : MonoBehaviour {

    public static Chest_Audio instance;

    public MovieTexture movie;

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        movie.loop = false;
        movie.Play ( );
        RawImage r = transform.GetComponent<RawImage> ( );
        r.texture = movie;
	}
	
	// Update is called once per frame
	void Update () {
        if ( !movie.isPlaying ||(Input.anyKeyDown&&Input.GetKeyDown(KeyCode.Space)))
        {
            GameObject.Find ("Canvas").transform.Find ("Chest_Audio").gameObject.SetActive (false);
        }
	}
}
