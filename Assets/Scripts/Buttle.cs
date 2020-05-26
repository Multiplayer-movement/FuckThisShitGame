using UnityEngine;
using System.Collections;

public class Buttle : MonoBehaviour {

    // Use this for initialization
    public static Buttle instance;
    Vector2 ori_Pos;//子弹的出生位置
    Animator ani;//存储眼泪的爆炸动画
    public  bool IsVertical;//该眼泪是否是垂直眼泪

    void Awake()
    {
        instance = this;
        ani = transform.GetComponent<Animator> ( );
    }

	void Start () {
        //Debug.Log (ori_Pos);
        ori_Pos = transform.GetComponent<RectTransform> ( ).position;//存储子弹初始位置
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log (transform.position);
        //Debug.Log (ori_Pos.y.ToString ( ) + "-" + transform.position.y.ToString ( ) + "=" +  ( ori_Pos.y- transform.position.y ).ToString());
        //Debug.Log (( ori_Pos.y - transform.position.y ) >= 0.1f);
        //Debug.Log (transform.position.x.ToString ( ) + " - " + ori_Pos.x.ToString ( ) + " = " + ( transform.position.x - ori_Pos.x ).ToString ( ));
        //Debug.Log (transform.position + "/" + ori_Pos);
        if ( ( Mathf.Abs(transform.position.x - ori_Pos.x) > 5 )&&!IsVertical )
        {
            transform.GetComponent<Rigidbody2D> ( ).gravityScale = 1f;
        }
        if ( ( ori_Pos.y-transform.position.y  ) >= 0.3f&&!IsVertical)
        {
            transform.GetComponent<Rigidbody2D> ( ).drag = 999;//发生碰撞，停止移动
            transform.GetComponent<CircleCollider2D> ( ).enabled = false;//将碰撞体消失
            ani.Play ("buttle_boom_ani");
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        
        if ( c.transform.tag != "Player" && c.transform.tag != "Buttle" &&c.transform.parent.parent.name !="blood")
        {
            //Debug.Log (c.transform.parent.name);
            //Debug.Log (c.transform.name);
            transform.GetComponent<Rigidbody2D> ( ).drag = 999;//发生碰撞，停止移动
            transform.GetComponent<CircleCollider2D> ( ).enabled = false;//将碰撞体消失
            if ( ani == null )
            {
                Debug.Log ("null!");
            }
            ani.Play ("buttle_boom_ani");
        }
    }

    //爆炸特效播放完后调用的销毁自己的方法
    void destory_self()
    {
        Destroy (transform.gameObject, 1f);
    }


}
