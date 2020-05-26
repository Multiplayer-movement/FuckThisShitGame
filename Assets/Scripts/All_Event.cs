using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class All_Event : MonoBehaviour {

    //动画帧调用的方法（跳场景）
    public void StartGame ( )
    {
        SceneManager.LoadScene ("Game_Fight");
    }
}
