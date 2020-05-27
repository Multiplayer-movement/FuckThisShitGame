using UnityEngine;
using System.Collections;

public class eye_enemy : MonoBehaviour {

    public static eye_enemy instance;
    public float HP = 4;//怪物血量
    float attack_time;//攻击间隔
    public Animator ani; 
    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        ani = transform.GetComponent<Animator> ( );
	}
	
	// Update is called once per frame
	void Update () {
        //血柱旋转
        transform.Find ("blood").Rotate (Vector3.forward, Time.deltaTime * 18);

        //Debug.Log (Vector3.Distance (transform.position, GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").transform.position));
        transform.Find ("eye").transform.GetComponent<RectTransform> ( ).localPosition =
                (GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition - transform.GetComponent<RectTransform> ( ).anchoredPosition)/50;

        //transform.Find ("eye").transform.LookAt (GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").transform.position);

        attack_time += Time.deltaTime;
        if ( attack_time>2 )
        {
            Attack ( );
            attack_time = 0;
        }
    }

    void Attack()
    {

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
                transform.Find ("blood").gameObject.SetActive (false);
                ani.SetBool ("IsDeath", true);
                Room.instance.enemy_num [ Player_Info.instance.present_room ]--;
            }
        }
    }

    public void eye_show()
    {
        transform.Find ("eye").gameObject.SetActive (true);
    }

    /// <summary>
    /// 动画帧调用，销毁自己
    /// </summary>
    public void DestroySelf()
    {
        Destroy (transform.gameObject);
    }
}
