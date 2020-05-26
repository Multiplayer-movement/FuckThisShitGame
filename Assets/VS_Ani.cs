using UnityEngine;
using System.Collections;

public class VS_Ani : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 动画帧调用，旋转完后将过场动画取消
    /// </summary>
    public void Ani_()
    {
        GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find("BG").gameObject.SetActive (false);
        Room.instance.Ani_Is_Over = true;

        //动画播放完毕后让玩家显示
        GameObject.Find ("Canvas").transform.Find ("Player").gameObject.SetActive (true);
    }
}
