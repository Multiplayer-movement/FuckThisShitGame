using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Start_Scene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int i = Random.Range (1, 37);
        GameObject.Find ("Canvas").transform.Find ("BG_2").Find ("Image").GetComponent<Image> ( ).sprite = Resources.Load<Sprite> ("UI/P" + i.ToString ( ));
        GameObject.Find ("Canvas").transform.Find ("BG_2").Find ("Image").GetComponent<Image> ( ).SetNativeSize ( );

        GameObject.Find ("Manager").GetComponent<AudioSource> ( ).Play ( );
    }
	
	// Update is called once per frame
	void Update () {
        if ( transform.name == "Manager" )
        {
            if ( Input.GetKeyDown (KeyCode.Space) )
            {
                GameObject.Find ("Canvas").transform.Find ("BG_2").gameObject.SetActive (true);
            }
        }
	}
}
