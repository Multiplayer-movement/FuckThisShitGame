using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Info : MonoBehaviour
{
    public static Player_Info instance;

    public static float Shoot_Speed;//玩家的射速
    public static float Move_Speed = 4;//人物行走速度
    public static float Player_AllHp = 6;//玩家的总血量上限
    public static float Player_Hp = 6;//玩家的当前血量
    public static float Player_Attack_num = 1;//玩家的攻击力
    public static bool IsCanMove = true;//人物是否可以行走
    public static bool IsCanMove_Up = true;//人物是否可以向上移动
    public static bool IsCanMove_Down = true;//人物上是否可以向下移动
    public static bool IsCanMove_Right = true;//人物是否可以向右移动
    public static bool IsCanMove_Left = true;//人物是否可以向左移动
    public static bool IsAllEnemyOver = false;//怪物是否消灭完(玩家是否可以走出门)
    public static bool IsDeath;//玩家是否死亡

    public ArrayList al_blood = new ArrayList ( );//存储所有的血量gameobject

    public static bool IsWuDi;//玩家是否开启无敌状态
    public float time_wudi = 0;//记录玩家无敌的时间
    public float time_blood = 0;//记录玩家流血到的频率

    public Sprite full_HP;//存储满血的红心图片
    public Sprite half_HP;//存储一半红心的图片
    public Sprite empty_HP;//存储空的红心图片

    //门由两部分组成
    Animator up_door_right_ani;//存储上门右的动画播放
    Animator up_door_left_ani;//存储上门左的动画播放

    Animator down_door_right_ani;//存储下门右的动画播放
    Animator down_door_left_ani;//存储下门左的动画播放

    Animator right_door_right_ani;//存储右门右的动画播放
    Animator right_door_left_ani;//存储右门左的动画播放

    Animator left_door_left_ani;//存储左门右的动画播放
    Animator left_door_right_ani;//存储左门左的动画播放

    bool room_up;
    bool room_down;
    bool room_right;
    bool room_left;

    Vector2 room_initial;//记录房间的初始一坐标（初始坐标为（0,0））

    public Vector2 present_room = new Vector2 (2, 2);//记录当前房间的位置（初始坐标为（2,2））//注意这两个房间的区别

    void Awake ( )
    {
        instance = this;
    }

    void Start()
    {
        Shoot_Speed = 0.3f;//玩家的射速
    }

    void Update ( )
    {
        //Debug.Log (IsWuDi);
        //Debug.Log (Player_Hp);
        //当前房间怪物数量小0则打开门
        if ( Room.instance.enemy_num [ present_room ] <= 0 )
        {
            Open_Door_Ani ( );
            if ( present_room == Room.instance.boss_room )
            {
                //Boss被打败，去下一层的传送门显示
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find ("Next_Floor").gameObject.SetActive (true);
            }
        }

        //判断玩家是否受伤害，显示无敌
        if ( IsWuDi )
        {
            time_wudi += Time.deltaTime;
            if ( time_wudi<1 )//该短时间内玩家无敌
            {
                IsWuDi = true;

                time_blood += Time.deltaTime;
                if ( time_blood > 0.05f )
                {
                    GameObject blood_image = Instantiate (Resources.Load ("Player/Blood_Image") as GameObject);
                    al_blood.Add (blood_image);//存储生成的血图片
                    int k = Random.Range (0, 10);
                    blood_image.GetComponent<Image> ( ).sprite = Room.instance.blood_list [ k ];
                    //Debug.Log (GameObject.Find ("Canvas").transform.Find ("Blood"));
                    blood_image.transform.SetParent (GameObject.Find ("Canvas").transform.Find ("Blood"));
                    blood_image.GetComponent<Image> ( ).SetNativeSize ( );
                    blood_image.GetComponent<RectTransform> ( ).anchoredPosition = GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition;
                    //无敌时间内在地板上留下血迹

                    blood_image.transform.localScale = new Vector3 (1, 1, 1);
                    time_blood = 0;
                }
               
            }
            else
            {
                IsWuDi = false;
                time_wudi = 0;
            }
        }

        //如果玩家死亡，按空格后从新开始
        if ( IsDeath )
        {
            if ( Input.GetKeyDown(KeyCode.Space) )
            {
                Player_Hp = 6;
                SceneManager.LoadScene ("Game_Fight");
            }
        }
    }

    void FixedUpdate ( )
    {
        if ( room_up )
        {
            Map_Up ( );
            foreach ( var item in al_blood )
            {
                Destroy ((GameObject)item);
            }
        }
        else if ( room_down )
        {
            Map_Down ( );
            foreach ( var item in al_blood )
            {
                Destroy (( GameObject )item);
            }
        }
        else if ( room_right )
        {
            Map_Right ( );
            if ( GameObject.Find ("Canvas").transform.Find ("Blood").childCount > 0 )
            {
                Destroy (GameObject.Find ("Canvas").transform.Find ("Blood").GetChild (0).gameObject);
            }
        }
        else if ( room_left )
        {
            Map_Left ( );
            foreach ( var item in al_blood )
            {
                Destroy (( GameObject )item);
            }
        }
    }

    //如果room_up = true的话执行该方法,下同
    void Map_Up ( )
    {
        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.y > ( room_initial.y - 750 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (-Vector2.up * Time.deltaTime * 80);
        }
        else//房间移动到指定位置，停止移动，
        {
            //房间停止移动后开始省商城怪物
            room_up = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x, room_initial.y - 750);//房间归为
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<CircleCollider2D> ( ).enabled = true;//碰撞体显示
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (true);//以撒显示
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (true);//以撒显示

            if ( Room.instance.enemy_num [ present_room ] > 0 )//如果进入的房间内怪物数量大于0，则关闭门（将门的collider 打开,播放关闭门的动画）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Down ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.y < ( room_initial.y + 750 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (Vector2.up * Time.deltaTime * 80);
        }
        else//停止移动，
        {
            room_down = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x, room_initial.y + 750);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (true);//以撒显示
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (true);//以撒显示

            if ( Room.instance.enemy_num [ present_room ] > 0 )//如果进入的房间内怪物数量大于0，则关闭门（将门的collider 打开,播放关闭门的动画）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Right ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.x > ( room_initial.x - 1350 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (-Vector2.right * Time.deltaTime * 80);
        }
        else//停止移动，
        {
            room_right = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x - 1350, room_initial.y);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (true);//以撒显示
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (true);//以撒显示

            if ( Room.instance.enemy_num [ present_room ] > 0 )//如果进入的房间内怪物数量大于0，则关闭门（将门的collider 打开,播放关闭门的动画）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Map_Left ( )
    {

        if ( GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition.x < ( room_initial.x + 1350 ) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").transform.Translate (Vector2.right * Time.deltaTime * 80);
        }
        else//停止移动，
        {
            room_left = false;
            GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (room_initial.x + 1350, room_initial.y);
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<CircleCollider2D> ( ).enabled = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (true);//以撒显示
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (true);//以撒显示

            if ( Room.instance.enemy_num [ present_room ] > 0 )//如果进入的房间内怪物数量大于0，则关闭门（将门的collider 打开,播放关闭门的动画）
            {
                Close_Door_Ani ( );
            }
        }
        Room.instance.Show_Room ( );
        Room.instance.Map_Show ( );
    }

    void Open_Door_Ani ( )
    {
        //如果该房间上门还有房间，则把上门的碰撞体打开
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x, present_room.y + 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x, present_room.y - 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x + 1, present_room.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (false);
        }
        if ( Room.instance.room_id.Contains (new Vector2 (present_room.x - 1, present_room.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (false);
        }

        //上门打开的动画播放
        up_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Animator> ( );
        up_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Animator> ( );
        up_door_right_ani.Play ("door_up_right_open");
        up_door_left_ani.Play ("door_up_left_open");

        //下门打开的动画播放
        down_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Animator> ( );
        down_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Animator> ( );
        down_door_right_ani.Play ("door_up_right_open");
        down_door_left_ani.Play ("door_up_left_open");

        //右门打开的动画播放
        right_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Animator> ( );
        right_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Animator> ( );
        right_door_right_ani.Play ("door_up_right_open");
        right_door_left_ani.Play ("door_up_left_open");

        //左门打开的动画播放
        left_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Animator> ( );
        left_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Animator> ( );
        left_door_right_ani.Play ("door_up_right_open");
        left_door_left_ani.Play ("door_up_left_open");
    }

    void Close_Door_Ani ( )
    {
        //所有墙的碰撞体出现
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (true);
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (true);

        //上门关闭的动画播放
        up_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Animator> ( );
        up_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Animator> ( );
        up_door_right_ani.Play ("door_up_right_close");
        up_door_left_ani.Play ("door_up_left_close");

        //下门关闭的动画播放
        down_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Animator> ( );
        down_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Animator> ( );
        down_door_right_ani.Play ("door_up_right_close");
        down_door_left_ani.Play ("door_up_left_close");

        //右门关闭的动画播放
        right_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Animator> ( );
        right_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Animator> ( );
        right_door_right_ani.Play ("door_up_right_close");
        right_door_left_ani.Play ("door_up_left_close");

        //左门关闭的动画播放
        left_door_right_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Animator> ( );
        left_door_left_ani = GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + present_room.x.ToString ( ) + present_room.y.ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Animator> ( );
        left_door_right_ani.Play ("door_up_right_close");
        left_door_left_ani.Play ("door_up_left_close");

        Create_Enemy ( );
    }

    void Create_Enemy ( )
    {
        //房间停止移动后根据当前房间 的怪物数量生成怪物
        for ( int i = 0; i < Room.instance.enemy_num [ present_room ]; i++ )
        {
            float x = Random.Range (-400, 400);
            float y = Random.Range (-200, 200);

            if ( present_room == Room.instance.boss_room )
            {
                GameObject go = ( GameObject )Instantiate (Resources.Load ("enemy/FlyDuke"));
                go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Enemy");
                go.GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (-1, 160);
            }
            else
            {
                int enemy_type = Random.Range (0, 3);//决定怪物种类的随机整数
                if ( enemy_type == 0 )
                {
                    GameObject go = ( GameObject )Instantiate (Resources.Load ("enemy/little_shit"));
                    go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Enemy");
                    go.GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (x, y);
                }
                else if ( enemy_type == 1 )
                {
                    GameObject go = ( GameObject )Instantiate (Resources.Load ("enemy/little_fly"));
                    go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Enemy");
                    go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                    go.GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (x, y);
                }
                else if ( enemy_type == 2 )
                {
                    GameObject go = ( GameObject )Instantiate (Resources.Load ("enemy/eye_enemy"));
                    go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Enemy");
                    go.transform.GetComponent<RectTransform> ( ).localScale = new Vector3 (1, 1, 1);
                    go.GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (x, y);
                }
            }

            //Debug.Log (x + "," + y);
            //go.transform.localPosition = new Vector2 (0, 0);


        }
    }

    /// <summary>
    /// 玩家收到伤害后短暂无敌
    /// </summary>
    void Player_WuDi()
    {
        
    }

    /// <summary>
    /// 防止角色移动到墙边上抖动
    /// 当玩家触碰到怪物的时候，掉血，后退，暂时无敌
    /// </summary>
    /// <param name="c"></param>
    void OnCollisionStay2D ( Collision2D c )
    {
        if ( c.transform.name == "wall_u_l" || c.transform.name == "wall_u_r" || c.transform.name == "door_up" )
        {
            PlayerControl.instance.IsUpMove = false;
        }

        if ( c.transform.name == "wall_d_l" || c.transform.name == "wall_d_r" || c.transform.name == "door_down" )
        {
            PlayerControl.instance.IsDownMove = false;
        }

        if ( c.transform.name == "wall_r_u" || c.transform.name == "wall_r_d" || c.transform.name == "door_right" )
        {
            PlayerControl.instance.IsRightMove = false;
        }

        if ( c.transform.name == "wall_l_u" || c.transform.name == "wall_l_d" || c.transform.name == "door_left" )
        {
            PlayerControl.instance.IsLeftMove = false;
        }

        //当玩家触碰到怪物的时候，掉血，后退，暂时无敌
        if ( c.transform.tag == "Enemy" && ( !IsWuDi ) )
        {
            if ( (Player_Hp != 0)&& (Player_Hp%2==0) )
            {
                //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + (Player_Hp/2).ToString()).GetComponent<Image> ( ).sprite = half_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
            else if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 != 0 ) )
            {
                //Debug.Log ("HP" + ( (Player_Hp / 2) + 0.5 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( (Player_Hp / 2)+0.5 ).ToString ( )).GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }

            if( Player_Hp ==0)
            {
                IsDeath = true;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP1").GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;

                //玩家已经死亡,黑幕显示
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Game_Over_BG").gameObject.SetActive (true);

                //将所有的怪物消失，以免推动玩家
                GameObject.Find ("Canvas").transform.Find ("All_Enemy").gameObject.SetActive (false);

                //将玩家的身体头消失
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (false);
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (false);

                //显示玩家死亡的图片
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("Isaac_Death").gameObject.SetActive (true);
            }
        }

    }

    void OnTriggerStay2D(Collider2D c)
    {
        //当玩家触碰到怪物的时候，掉血，后退，暂时无敌
        if ( c.transform.tag == "Enemy" && ( !IsWuDi ) )
        {
            if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 == 0 ) )
            {
                //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( Player_Hp / 2 ).ToString ( )).GetComponent<Image> ( ).sprite = half_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }
            else if ( ( Player_Hp != 0 ) && ( Player_Hp % 2 != 0 ) )
            {
                //Debug.Log ("HP" + ( (Player_Hp / 2) + 0.5 ).ToString ( ));
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( ( Player_Hp / 2 ) + 0.5 ).ToString ( )).GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;
                IsWuDi = true;
                Player_Hp--;
            }

            if ( Player_Hp == 0 )
            {
                IsDeath = true;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP1").GetComponent<Image> ( ).sprite = empty_HP;
                time_wudi = 0;

                //玩家已经死亡,黑幕显示
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Game_Over_BG").gameObject.SetActive (true);

                //将所有的怪物消失，以免推动玩家
                GameObject.Find ("Canvas").transform.Find ("All_Enemy").gameObject.SetActive (false);

                //将玩家的身体头消失
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("body").gameObject.SetActive (false);
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("head").gameObject.SetActive (false);

                //显示玩家死亡的图片
                GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").Find ("Isaac_Death").gameObject.SetActive (true);
            }
        }
    }

    //限制角色的移动范围，碰到墙壁后不能发生位移
    void OnCollisionExit2D ( Collision2D c )
    {
        if ( c.transform.name == "wall_u_l" || c.transform.name == "wall_u_r" || c.transform.name == "door_up" )
        {
            PlayerControl.instance.IsUpMove = true;
        }

        if ( c.transform.name == "wall_d_l" || c.transform.name == "wall_d_r" || c.transform.name == "door_down" )
        {
            PlayerControl.instance.IsDownMove = true;
        }

        if ( c.transform.name == "wall_r_u" || c.transform.name == "wall_r_d" || c.transform.name == "door_right" )
        {
            PlayerControl.instance.IsRightMove = true;
        }

        if ( c.transform.name == "wall_l_u" || c.transform.name == "wall_l_d" || c.transform.name == "door_left" )
        {
            PlayerControl.instance.IsLeftMove = true;
        }

    }

    /// <summary>
    /// 玩家接触到房间触发器的换，就会更新玩家所在的房间
    /// </summary>
    /// <param name="c"></param>
    void OnTriggerEnter2D ( Collider2D c )
    {
        if ( c.tag == "Up_Door" || c.tag == "Down_Door" || c.tag == "Right_Door" || c.tag == "Left_Door" )
        {
            room_initial = GameObject.Find ("Canvas").transform.Find ("All_Map").GetComponent<RectTransform> ( ).anchoredPosition;//记录房间的初始位置
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<CircleCollider2D> ( ).enabled = false;//房间移动的时候将以撒的collider暂时关闭
        }

        if ( c.transform.tag == "Up_Door" )
        {
            present_room += new Vector2 (0, 1);
            room_up = true;
            //以撒移动到对应位置
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (10, -200);
        }
        else if ( c.transform.tag == "Down_Door" )
        {
            present_room -= new Vector2 (0, 1);
            room_down = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (10, 240);
        }
        else if ( c.transform.tag == "Right_Door" )
        {
            present_room += new Vector2 (1, 0);
            room_right = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (-500, 20);
        }
        else if ( c.transform.tag == "Left_Door" )
        {
            present_room -= new Vector2 (1, 0);
            room_left = true;
            GameObject.Find ("Canvas").transform.Find ("Player").Find ("Isaac").GetComponent<RectTransform> ( ).anchoredPosition = new Vector2 (500, 20);
        }

        //如果去过得房间中没有该房间，则将该房间添加到去过的房间中
        if ( !Room.instance.room_been.Contains (present_room) )
        {
            Room.instance.room_been.Add (present_room);
        }

        if ( c.transform.name == "Next_Floor" )
        {
            GameObject.Find ("Canvas").transform.Find ("Player").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find ("Isaac_Drop").gameObject.SetActive (true);
        }
    }

    //玩家收到伤害后原地留下血迹
    void Show_Blood()
    {

    }

}
