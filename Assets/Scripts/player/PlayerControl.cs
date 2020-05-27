using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    public bool IsUpMove = true;//防止抖动
    public bool IsDownMove = true;//防止抖动
    public bool IsRightMove = true;//防止抖动
    public bool IsLeftMove = true;//防止抖动
    public Sprite [ ] head_sprite;
    Animator ani_body;//控制敌人身体的动画组件
    Animator ani_head;//控制敌人头的动画组件
    float time = 1;//记录发射间隔
    float h;
    float v;
    enum State
    {
        move_ilde,
        move_up,
        move_down,
        move_right,
        move_left,

        attack_idle,
        attack_up,
        attack_down,
        attack_right,
        attack_left
    }
    State state = new State ( );

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ( )
    {
        ani_body = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").GetComponent<Animator> ( );
        ani_head = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").GetComponent<Animator> ( );
    }

    // Update is called once per frame
    void Update ( )
    {
        Ani_Body_Update ( );
        Ani_Head_Update ( );
        Move ( );
    }

    void FixedUpdate()
    {
        Shoot ( );
    }

    /// <summary>
    /// 敌人动画状态机
    /// </summary>
    void Ani_Body_Update ( )
    {
        switch ( state )
        {
            case State.move_ilde:
                ani_body.SetBool ("IsMove_Up_Down", false);
                ani_body.SetBool ("IsMove_Right", false);
                ani_body.SetBool ("IsMove_Left", false);
                ani_body.SetBool ("IsMove_Idle", true);
                break;
            case State.move_up:
                ani_body.SetBool ("IsMove_Up_Down", true);
                ani_body.SetBool ("IsMove_Right", false);
                ani_body.SetBool ("IsMove_Left", false);
                ani_body.SetBool ("IsMove_Idle", false);
                break;
            case State.move_down:
                ani_body.SetBool ("IsMove_Up_Down", true);
                ani_body.SetBool ("IsMove_Right", false);
                ani_body.SetBool ("IsMove_Left", false);
                ani_body.SetBool ("IsMove_Idle", false);
                break;
            case State.move_right:
                ani_body.SetBool ("IsMove_Up_Down", false);
                ani_body.SetBool ("IsMove_Right", true);
                ani_body.SetBool ("IsMove_Left", false);
                ani_body.SetBool ("IsMove_Idle", false);
                break;
            case State.move_left:
                ani_body.SetBool ("IsMove_Up_Down", false);
                ani_body.SetBool ("IsMove_Right", false);
                ani_body.SetBool ("IsMove_Left", true);
                ani_body.SetBool ("IsMove_Idle", false);
                break;
            default:
                break;
        }
    }

    void Ani_Head_Update ( )
    {
        if ( Input.GetKey (KeyCode.Keypad4) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", true);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.Keypad6) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", true);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.Keypad5) )
        {
            ani_head.SetBool ("IsAttack_Down", true);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.Keypad8) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", true);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.W) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", true);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.S) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", true);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else if ( Input.GetKey (KeyCode.A) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", true);
        }
        else if ( Input.GetKey (KeyCode.D) )
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_Idle", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_down", false);
            ani_head.SetBool ("IsAttack_head_right", true);
            ani_head.SetBool ("IsAttack_head_left", false);
        }
        else
        {
            ani_head.SetBool ("IsAttack_Down", false);
            ani_head.SetBool ("IsAttack_Up", false);
            ani_head.SetBool ("IsAttack_Right", false);
            ani_head.SetBool ("IsAttack_Left", false);
            ani_head.SetBool ("IsAttack_head_up", false);
            ani_head.SetBool ("IsAttack_head_right", false);
            ani_head.SetBool ("IsAttack_head_left", false);
            ani_head.SetBool ("IsAttack_head_down", true);
        }
    }

    void Move ( )
    {
        h = Input.GetAxis ("Horizontal");
        v = Input.GetAxis ("Vertical");

        if ( h > 0 )
        {
            state = State.move_right;
        }
        else if ( h < 0 )
        {
            state = State.move_left;
        }
        else if ( v > 0 )
        {
            state = State.move_up;
        }
        else if ( v < 0 )
        {
            state = State.move_down;
        }
        else
        {
            state = State.move_ilde;
        }

        if ( v > 0 && IsUpMove )
        {
            //Debug.Log ("up");
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position += new Vector3 (0, v * Time.deltaTime * Player_Info.Move_Speed, 0);
        }
        if ( v < 0 && IsDownMove )
        {
            //Debug.Log ("down");
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position += new Vector3 (0, v * Time.deltaTime * Player_Info.Move_Speed, 0);
        }


        if ( h > 0 && IsRightMove )
        {
            //Debug.Log ("right");
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position += new Vector3 (h * Time.deltaTime * Player_Info.Move_Speed, 0, 0);
        }
        if ( h < 0 && IsLeftMove )
        {
            //Debug.Log ("left");
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position += new Vector3 (h * Time.deltaTime * Player_Info.Move_Speed, 0, 0);
        }
    }

    void Shoot ( )
    {
        if ( Input.GetKey (KeyCode.Keypad6) )
        {
            time += Time.deltaTime;

            if ( time>Player_Info.Shoot_Speed )
            {
                GameObject go = (GameObject)Instantiate (Resources.Load ("buttle/buttle"));
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (Vector2.right * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.Keypad4) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (-Vector2.right * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.Keypad8) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.GetComponent<Buttle> ( ).IsVertical = true;
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (Vector2.up * 400);
                time = 0;
            }
        }
        else if ( Input.GetKey (KeyCode.Keypad5) )
        {
            time += Time.deltaTime;

            if ( time > Player_Info.Shoot_Speed )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("buttle/buttle"));
                go.GetComponent<Buttle> ( ).IsVertical = true;
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("Player");
                go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                go.transform.position = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").position;
                go.GetComponent<Rigidbody2D> ( ).AddForce (-Vector2.up * 400);
                time = 0;
            }
        }
        else
        {
            time = 1;
        }

    }
}
