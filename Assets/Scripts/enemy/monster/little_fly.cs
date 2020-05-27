using UnityEngine;
using System.Collections;

public class little_fly : MonoBehaviour {

    public static little_fly instance;
    public float HP = 4;//怪物的血量
    Animator ani;
    Transform player_Pos;
    bool IsCanMove = true;
    float time = 0;//记录怪物几秒后可以行动
    void Awake()
    {
        ani = transform.GetComponent<Animator> ( );
    }
	// Use this for initialization
	void Start () {
        player_Pos = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac");
	}
	
	// Update is called once per frame
	void Update () {
        if ( time>1 )
        {
            Move ( );
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    void Move()
    {
        if ( IsCanMove )
        {
            float dis = Vector2.Distance (transform.position, player_Pos.position);
            //Lerp寻路
            transform.position = Vector2.Lerp (transform.position, player_Pos.position, Time.deltaTime * ( 1 / dis ) * 2);
        }
    }

    /// <summary>
    /// 怪物碰撞到物体
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter2D ( Collider2D c )
    {
        if ( c.transform.tag == "Buttle" )
        {
            HP -= Player_Info.Player_Attack_num;
            if ( HP <= 0 )
            {
                transform.GetComponent<BoxCollider2D> ( ).enabled = false;
                ani.SetBool ("IsDeath", true);
            }
        }
    }

    /// <summary>
    /// 动画帧事件调用，怪物死亡后不能移动
    /// </summary>
    void NotMove()
    {
        IsCanMove = false;
        transform.GetComponent<BoxCollider2D> ( ).enabled = false;
    }

    /// <summary>
    /// 动画帧事件调用，怪物死亡后销毁自己
    /// </summary>
    void DestroySelf()
    {
        Room.instance.enemy_num [ Player_Info.instance.present_room ]--;
        Destroy (transform.gameObject);
    }

    /// <summary>
    /// 动画帧事件调用，显示怪物自己的collider
    /// </summary>
    void Show_collider()
    {
        transform.GetComponent<BoxCollider2D> ( ).enabled = true;
    }
}
