using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Isaac_drop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //动画帧调用，玩家掉到最下面时调用
    public void Drop_Ani()
    {
        SceneManager.LoadScene ("Game_Fight");
    }
}
