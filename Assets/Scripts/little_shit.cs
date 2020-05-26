using UnityEngine;
using System.Collections;

public class little_shit : MonoBehaviour
{
    bool IsCanMove;
    Vector3 aimPos;//当前目标点
    public float HP = 3;
    Animator ani;

    void Awake()
    {
        ani = GetComponent<Animator> ( );
    }

    // Use this for initialization
    void Start ( )
    {
        aimPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate ( )
    {
        if ( IsCanMove )
        {
            Move ( );
        }
    }

    void Move ( )
    {
        if ( Vector2.Distance (transform.position, aimPos) > 0.3f )
        {
            transform.position = Vector3.Lerp (transform.position, aimPos, Time.deltaTime * 2f);
        }
        else
        {
            float i = Random.Range (2, 5);
            Vector2 point = Random.insideUnitCircle * i;
            if ( point.x < 0 )//如果怪物朝右的话
            {
                transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
            }
            else//如果怪物朝左的话
            {
                transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (-1, 1, 1);
            }
            aimPos = new Vector3 (transform.position.x - point.x, transform.position.y - point.y, 0);
        }
    }

    /// <summary>
    /// 怪物碰到物体后重新选取目标
    /// </summary>
    /// <param name="c"></param>
    void OnCollisionStay2D ( Collision2D c )
    {
        float i = Random.Range (2, 5);
        Vector2 point = Random.insideUnitCircle * i;
        aimPos = new Vector3 (transform.position.x - point.x, transform.position.y - point.y, 0);
    }

    /// <summary>
    /// 怪物碰撞到物体
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter2D(Collider2D c)
    {
        if ( c.transform.tag == "Buttle" )
        {
            HP -=Player_Info.Player_Attack_num;
            if ( HP <= 0 )
            {
                transform.GetComponent<CircleCollider2D> ( ).enabled = false;
                IsCanMove = false;
                ani.SetBool ("IsDeath", true);
            }
        }
    }

    /// <summary>
    /// 动画帧事件调用
    /// </summary>
    void IsCanMove_Math()
    {
        IsCanMove = true;
        //等到可以移动的时候将小怪的collider变为true，防止被房子的碰撞体撞飞
        transform.GetComponent<CircleCollider2D> ( ).enabled = true;
    }
   
    /// <summary>
    /// 动画帧调用的销毁自己
    /// </summary>
    void Destroy_self()
    {
        Room.instance.enemy_num [ Player_Info.instance.present_room]--;
        Destroy (transform.gameObject);
    }
}
