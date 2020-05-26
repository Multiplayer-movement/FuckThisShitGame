using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public static Room instance;

    int room_leve = 1;//当前层数等级（初始在第一层）
    List<Vector2> all_chest_room = new List<Vector2> ( );//所有宝箱房，Boss房的候选房间
    public Vector2 chest_room;//宝箱房坐标
    public Vector2 boss_room;//Boss房坐标
    public Dictionary<Vector2, int> enemy_num = new Dictionary<Vector2, int> ( );//存储对应房间内的怪物数量

    public List<Vector2> room_id = new List<Vector2> ( );//存储每个房间的编号
    public List<Vector2> room_close = new List<Vector2> ( );//存储和玩家所在房间相邻房间（方便高亮处理）
    public List<Vector2> room_been = new List<Vector2> ( );//存储玩家经过的房间(换成稍微亮一点的暗图片)
    public List<Vector2> room_never_been = new List<Vector2> ( );//存储玩家没有去过的房间（换成暗图片）
    public List<Vector2> room_allShow = new List<Vector2> ( );//存储所有需要显示的房间

    public int show_map_num = 1;//用作显示或者关闭地图的判断(奇数为打开，偶数为关闭)

    public bool Ani_Is_Over;//过场动画是否播放过

    public Sprite chest_door_image;//存储宝箱房门的图片(resource.load动态加载没这个方便)
    public Sprite boss_door_image;//存储boss房门的图片
    public Sprite boss_door_in_image;//存储boss门里面的图片
    public Sprite boss_door_right;//存储boss左门图片
    public Sprite boss_door_left;//存储boss右门图片
    public Sprite room_never_been_image;//存储没有去过的房间的图片
    public Sprite room_been_image;//存储去过的房间的图片
    public Sprite room_present_image;//存储当前所在的房间的图片

    public List<Sprite> blood_list = new List<Sprite> ( );

    public AudioClip fight;//存储战斗时候的音乐片段
    public AudioClip save;//存储安全房间内的背景音乐
    public bool IsGoChest;//存储玩家是否已经进过宝箱房
    void Awake ( )
    {
        instance = this;
        room_allShow.Add (new Vector2 (2, 2));
    }

    // Use this for initialization
    void Start ( )
    {
        IsGoChest = false;//每次进入新的层会刷新道具房
        room_been.Add (new Vector2 (2, 2));//玩家一开始就在2,2房间，所以刚开始把2,2房间添加进玩家去过的房间
        CopyRoom ( );
        Create_Room ( );
    }

    // Update is called once per frame
    void Update ( )
    {
        //按下tab键后显示地图
        if ( Input.GetKeyDown (KeyCode.Tab) && ( show_map_num % 2 == 1 ) )//如果为奇数，则按tab关闭地图
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").gameObject.SetActive (false);
            show_map_num++;
        }
        else if ( Input.GetKeyDown (KeyCode.Tab) && ( show_map_num % 2 == 0 ) )//如果为偶数，则按下tab打开地图
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").gameObject.SetActive (true);
            show_map_num++;
        }

        //进入Boss房间后
        if ( Player_Info.instance.present_room == boss_room )
        {

            if ( !Ani_Is_Over )//动画播放过后不再显示
            {
                //播放Boss房间的动画
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Ani_Player").Find("BG").gameObject.SetActive (true);
            }
            
            //boss血条显示
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Boss_HP").gameObject.SetActive (true);
        }

        //Debug.Log (Room.instance.enemy_num [ Player_Info.instance.present_room ]);

        if ( enemy_num[ Player_Info.instance.present_room ] == 0 )
        {
            GameObject.Find ("Manager").GetComponent<AudioSource> ( ).clip = save;
            if ( !GameObject.Find ("Manager").GetComponent<AudioSource> ( ).isPlaying )
            {
                GameObject.Find ("Manager").GetComponent<AudioSource> ( ).Play ( );
            }
            
        }
        else
        {
            GameObject.Find ("Manager").GetComponent<AudioSource> ( ).clip = fight;
            if ( !GameObject.Find ("Manager").GetComponent<AudioSource> ( ).isPlaying )
            {
                GameObject.Find ("Manager").GetComponent<AudioSource> ( ).Play ( );
            }
        }

        //如果玩家进入宝箱房，播放对应视频
        if ( Player_Info.instance.present_room == chest_room&& !IsGoChest )
        {
            GameObject.Find ("Canvas").transform.Find ("Chest_Audio").gameObject.SetActive (true);
            int i = Random.Range (1, 7);;
            if ( i == 1 )
            {
                Player_Info.Player_Attack_num+=1;
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/Attack_Up") as MovieTexture;
            }
            else if ( i == 2 )
            {
                //扣一颗心
                if ( Player_Info.Player_Hp>2 )
                {
                    for ( int j = 0; j < 2; j++ )
                    {
                        if ( ( Player_Info.Player_Hp != 0 ) && ( Player_Info.Player_Hp % 2 == 0 ) )
                        {
                            //Debug.Log ("HP" + ( Player_Hp / 2 ).ToString ( ));
                            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( Player_Info.Player_Hp / 2 ).ToString ( )).GetComponent<Image> ( ).sprite = Player_Info.instance.half_HP;
                            Player_Info.instance.time_wudi = 0;
                            Player_Info.IsWuDi = true;
                            Player_Info.Player_Hp--;
                        }
                        else if ( ( Player_Info.Player_Hp != 0 ) && ( Player_Info.Player_Hp % 2 != 0 ) )
                        {
                            //Debug.Log ("HP" + ( (Player_Hp / 2) + 0.5 ).ToString ( ));
                            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP" + ( ( Player_Info.Player_Hp / 2 ) + 0.5 ).ToString ( )).GetComponent<Image> ( ).sprite = Player_Info.instance.empty_HP;
                            Player_Info.instance.time_wudi = 0;
                            Player_Info.IsWuDi = true;
                            Player_Info.Player_Hp--;
                        }
                    }
                }
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/Hp_Down") as MovieTexture;
            }
            else if ( i == 3 )
            {
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP1").GetComponent<Image> ( ).sprite = Player_Info.instance.full_HP;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP2").GetComponent<Image> ( ).sprite = Player_Info.instance.full_HP;
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Player_Hp").Find ("HP3").GetComponent<Image> ( ).sprite = Player_Info.instance.full_HP;

                Player_Info.Player_Hp = 6;
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/HP_Up") as MovieTexture;
            }
            else if ( i == 4 )
            {
                Player_Info.Shoot_Speed = 0.2f; 
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/Shoot_Up") as MovieTexture;
            }
            else if ( i == 5 )
            {
                Player_Info.Move_Speed = 6;
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/Speed_Up") as MovieTexture;
            }
            else if ( i == 6 )
            {
                if ( Player_Info.Player_Attack_num>=0 )
                {
                    Player_Info.Player_Attack_num -= 0.5f;
                }
                GameObject.Find ("Canvas").transform.Find ("Chest_Audio").GetComponent<Chest_Audio> ( ).movie = Resources.Load ("Chest_Audio/Attack_Down") as MovieTexture;
            }
            IsGoChest = true;
        }
    }

    void Create_Room ( )//生成地图
    {
        room_id.Add (new Vector2 (2, 2));

        while ( room_id.Count < 7 )
        {
            int x = Random.Range (0, 5);
            int y = Random.Range (0, 5);

            if ( !room_id.Contains (new Vector2 (x, y)) && ( x != 2 && ( y != 2 ) ) ) //如果之前跟之前选取的房间不存在重复，则添加进roomid集合中
            {
                room_id.Add (new Vector2 (x, y));
            }
        }
        foreach ( var item in room_id )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + item.x.ToString ( ) + item.y.ToString ( )).gameObject.SetActive (true);
        }

        //将每个房间 相互连接
        for ( int i = 0; i < 7; i++ )
        {
            SeekNextRoom (room_id [ i ]);
        }

        //房间已经生成完毕，接下来给每个房间安装对应的门
        for ( int i = 0; i < room_id.Count; i++ )
        {
            install_door (room_id [ i ]);
        }
        //选定Boss房,
        boss_room = all_chest_room [ Random.Range (0, all_chest_room.Count - 1) ];        //选定boss房
        all_chest_room.Remove (boss_room);//将选定的boss房从集合中删去
        install_chest_door (boss_room, "boss_room");

        //如果还有候选房间的话
        if ( all_chest_room.Count != 0 )
        {
            //因为已经移除了boss房的候选，所以 这里要减2
            chest_room = all_chest_room [ Random.Range (0, all_chest_room.Count - 2) ];
            install_chest_door (chest_room, "chest_room");
        }


        //给每个房间分配怪物数量(除去宝箱房和初始房间以及Boss房,后面记得再添加上)
        room_id.Remove (new Vector2 (2, 2));//除去初始房间
        room_id.Remove (chest_room);//将boss房和宝箱房从普通房间中删除
        room_id.Remove (boss_room);

        enemy_num.Add (new Vector2 (2, 2), 0);
        if ( chest_room != null )
        {
            enemy_num.Add (chest_room, 0);
        }
        enemy_num.Add (boss_room, 1);
        for ( int i = 0; i < room_id.Count; i++ )
        {
            int num = Random.Range (2, 6);//每个房间可能出现的怪物数量
            enemy_num.Add (room_id [ i ], num);
        }

        //显示初始房间的新手指南图片
        GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_22").Find ("All_Kuang").Find ("Tip").gameObject.SetActive (true);

        //再将之前删除的房间添加进集合，不然会出错
        room_id.Add (new Vector2 (2, 2));
        room_id.Add (chest_room);
        room_id.Add (boss_room);

        //将刚开始要显示的房间添加进集合
        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( Room_Quan (room_id [ i ]) == 1 )
            {
                room_allShow.Add (room_id [ i ]);
            }
        }

        Map_Show ( );
    }

    /// <summary>
    /// 计算该房间到初始房间要经过的房间
    /// //存储所有符合条件的房间,然后判断已有的房间中是否已经存在，
    /// 如果两个符合条件的房间都存在（或者都不存在），则随机选取一个，
    /// 然后 继续选取下一个房间，如果有一个房间已经存在，则进行下一轮房间选取
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    void SeekNextRoom ( Vector2 v )
    {
        List<Vector2> all_GoodRoom = new List<Vector2> ( );
        //Debug.Log ("origonal room is:" + v);
        for ( int j = 0; j < 5; j++ )
        {
            for ( int k = 0; k < 5; k++ )
            {
                if ( ( Room_Quan (new Vector2 (j, k)) == ( Room_Quan (v) - 1 ) ) && ( Room_Distance (v, new Vector2 (j, k)) == 1 ) )//接下来的房间要比目标房间的权值小一，而且要紧挨着目标房间
                {
                    //Debug.Log ("seek room is:" + j + " , " + k);
                    all_GoodRoom.Add (new Vector2 (j, k));
                }
            }
        }


        if ( ( v == new Vector2 (2, 2) ) || all_GoodRoom [ 0 ] == new Vector2 (2, 2) )
        {
            //Debug.Log ("over!");
            return;
        }
        //说明该房间的坐标 为（0,2）、（4,2）、（2,0）、（2,4），这些房间到权位减一的房间只有一个房间可以选择
        else if ( all_GoodRoom.Count == 1 )
        {
            if ( room_id.Contains (all_GoodRoom [ 0 ]) )//如果这个唯一的房间存在的话 
            {
                //Debug.Log ("new room is:" + all_GoodRoom [ 0 ]);
            }
            else//如果这个唯一的房间不存在的话
            {
                //Debug.Log ("new room is:" + all_GoodRoom [ 0 ]);

                room_id.Add (all_GoodRoom [ 0 ]);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + all_GoodRoom [ 0 ].x.ToString ( ) + all_GoodRoom [ 0 ].y.ToString ( )).gameObject.SetActive (true);
            }
        }
        //如果符合条件的两个房间都已经存在,则从中随机选取一个
        else if ( room_id.Contains (all_GoodRoom [ 0 ]) && room_id.Contains (all_GoodRoom [ 1 ]) )
        {
            int i = Random.Range (0, 2);

            //Debug.Log ( "new room is:" +   all_GoodRoom [ i ] );

            if ( Room_Quan (v) != 1 )
            {
                SeekNextRoom (all_GoodRoom [ i ]);
            }
        }
        //如果符合条件的两个房间都不 存在,则从中随机选取一个，并且存储到room_id 集合中
        else if ( ( !room_id.Contains (all_GoodRoom [ 0 ]) ) && ( !room_id.Contains (all_GoodRoom [ 1 ]) ) )
        {
            int i = Random.Range (0, 2);

            //Debug.Log ("new room is:" + all_GoodRoom [ i ]);

            room_id.Add (all_GoodRoom [ i ]);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + all_GoodRoom [ i ].x.ToString ( ) + all_GoodRoom [ i ].y.ToString ( )).gameObject.SetActive (true);

            if ( Room_Quan (v) != 1 )
            {
                SeekNextRoom (all_GoodRoom [ i ]);
            }
        }
        else//如果符合条件的房间中有已经存在的
        {
            if ( room_id.Contains (all_GoodRoom [ 0 ]) )
            {
                SeekNextRoom (all_GoodRoom [ 0 ]);
            }
            else
            {
                SeekNextRoom (all_GoodRoom [ 1 ]);
            }
        }
    }

    /// <summary>
    /// 计算该房间的权值(权：需要多少步才能到达初始房间)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    float Room_Quan ( Vector2 v )
    {
        return Mathf.Abs (v.x - 2) + Mathf.Abs (v.y - 2);
    }

    /// <summary>
    /// 计算两个房间的距离
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    float Room_Distance ( Vector2 v1, Vector2 v2 )
    {
        return Mathf.Abs (v1.x - v2.x) + Mathf.Abs (v1.y - v2.y);
    }

    //给该房间安装对应的普通门
    void install_door ( Vector2 v )
    {
        int door_num = 0;//该房间的门的数量

        //如果该房间上面还有房间 的话，则给该房间装上上门，
        if ( room_id.Contains (new Vector2 (v.x, v.y + 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").gameObject.SetActive (true);
            door_num++;
        }
        else//如果没有房间的话，将门的collider显示
        {
            //如果上面没有房间的话，将门隐藏
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up_content").gameObject.SetActive (false);
            //墙的collider显示
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_up").gameObject.SetActive (true);
        }

        //如果该房间下面还有房间 的话，则给该房间装上下门，
        if ( room_id.Contains (new Vector2 (v.x, v.y - 1)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down_content").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_down").gameObject.SetActive (true);
        }

        //如果该房间左面还有房间 的话，则给该房间装上左门，
        if ( room_id.Contains (new Vector2 (v.x - 1, v.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left_content").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_left").gameObject.SetActive (true);
        }

        //如果该房间右面还有房间 的话，则给该房间装上右门，
        if ( room_id.Contains (new Vector2 (v.x + 1, v.y)) )
        {
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").gameObject.SetActive (true);
            door_num++;
        }
        else
        {
            //将门隐藏
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").gameObject.SetActive (false);
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right_content").gameObject.SetActive (false);
            //打开门的碰撞体
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Wall").Find ("door_right").gameObject.SetActive (true);
        }

        //将所有只有一个门的房间加入宝箱房 Boss房的集合中
        if ( door_num == 1 )
        {
            all_chest_room.Add (v);
        }
    }

    /// <summary>
    /// 复制房间（2,2）
    /// </summary>
    void CopyRoom ( )
    {
        for ( int j = 0; j < 5; j++ )
        {
            for ( int k = 0; k < 5; k++ )
            {
                if ( ( j != 2 ) || ( k != 2 ) )
                {
                    GameObject go = ( GameObject )Instantiate (GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_22").gameObject);
                    go.transform.parent = GameObject.Find ("Canvas").transform.Find ("All_Map");
                    go.transform.localScale = new Vector3 (1, 1, 1);
                    go.transform.GetComponent<RectTransform> ( ).anchoredPosition3D = new Vector3 (( j - 2 ) * 1350, ( k - 2 ) * 750, 0);
                    go.transform.name = "Room_" + j.ToString ( ) + k.ToString ( );
                    go.SetActive (false);
                }
            }
        }
    }

    /// <summary>
    /// 安装宝箱房、boss房的门
    /// </summary>
    void install_chest_door ( Vector2 v, string room_type )
    {
        if ( room_type == "chest_room" )
        {
            //安装宝箱房(boss房)内的门
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = chest_door_image;
            GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = chest_door_image;

            //安装宝箱房（boss房）外面的门
            if ( room_id.Contains (v + new Vector2 (0, 1)) )//如果特殊房上面有房间  
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v - new Vector2 (0, 1)) )//如果特殊房间 下面 有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v + new Vector2 (1, 0)) )//如果特殊房间 右边 有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = chest_door_image;
            }
            else if ( room_id.Contains (v - new Vector2 (1, 0)) )//如果特殊房间 左边有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = chest_door_image;
            }
        }
        else if ( room_type == "boss_room" )//特殊房间为Boss房的话
        {
            //安装宝箱房（boss房）外面的门
            if ( room_id.Contains (v + new Vector2 (0, 1)) )//如果特殊房上面有房间  
            {
                //安装boss房内的门
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = boss_door_image;
                //更换boss房内的门里面的图片
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_up_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                //安装boss房外面的门
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = boss_door_image;
                //更换boss房外面门里面的图片
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                //将boss房外面的门前面的血光显示
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down").Find ("light").gameObject.SetActive (true);
                //将boss房对外的两扇门图片更换
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y + 1 ).ToString ( )).Find ("All_Door").Find ("door_down_content").Find ("door_down_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v - new Vector2 (0, 1)) )//如果特殊房间 下面 有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_down_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + ( v.y - 1 ).ToString ( )).Find ("All_Door").Find ("door_up_content").Find ("door_up_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v + new Vector2 (1, 0)) )//如果特殊房间 右边 有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x + 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_left_content").Find ("door_left_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
            else if ( room_id.Contains (v - new Vector2 (1, 0)) )//如果特殊房间 左边有房间
            {
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + v.x.ToString ( ) + v.y.ToString ( )).Find ("All_Door").Find ("door_left").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").GetComponent<Image> ( ).sprite = boss_door_in_image;

                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").GetComponent<Image> ( ).sprite = boss_door_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").GetComponent<Image> ( ).sprite = boss_door_in_image;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right").Find ("light").gameObject.SetActive (true);
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_left").GetComponent<Image> ( ).sprite = boss_door_left;
                GameObject.Find ("Canvas").transform.Find ("All_Map").Find ("Room_" + ( v.x - 1 ).ToString ( ) + ( v.y ).ToString ( )).Find ("All_Door").Find ("door_right_content").Find ("door_right_right").GetComponent<Image> ( ).sprite = boss_door_right;
            }
        }

    }

    /// <summary>
    /// 显示小地图
    /// </summary>
    public void Map_Show ( )
    {
        room_close.Clear ( );//每次清除集合，更新和玩家相邻的房间集合
        room_never_been.Clear ( );//每次未去过的房间会实时更新变化，所以要先清除集合内容
        for ( int i = 0; i < room_id.Count; i++ )
        {
            //存储和玩家相邻的房间
            if ( Room_Quan (room_id [ i ]) == 1 )
            {
                room_close.Add (room_id [ i ]);
            }
        }
        //在Player_Info脚本的OnTriggerEnter2D方法里面已经将去过的房间（room_been）中进行更新
        //通过去过的房间可以获得玩家没有去过得房间上
        for ( int i = 0; i < room_id.Count; i++ )
        {
            //如果去过的房间里面没有该房间，则证明该房间属于没有去过
            if ( !room_been.Contains (room_id [ i ]) )
            {
                room_never_been.Add (room_id [ i ]);
            }
        }

        //接下来进行房间的显示
        //先显示玩家未去过的房间
        for ( int i = 0; i < room_never_been.Count; i++ )
        {
            if ( Room_Quan (room_never_been [ i ]) == 1 )
            {
                GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_never_been [ i ].x.ToString ( ) + room_never_been [ i ].y.ToString ( )).GetComponent<Image> ( ).sprite = room_never_been_image;
            }
        }

        //接下来显示玩家去过的房间
        for ( int i = 0; i < room_been.Count; i++ )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_been [ i ].x.ToString ( ) + room_been [ i ].y.ToString ( )).GetComponent<Image> ( ).sprite = room_been_image;
        }

        //接下来显示玩家当前所在的房间
        GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (Player_Info.instance.present_room.x.ToString ( ) + Player_Info.instance.present_room.y.ToString ( )).GetComponent<Image> ( ).sprite = room_present_image;

        //根据room_allshow集合，显示房间
        for ( int i = 0; i < room_allShow.Count; i++ )
        {
            GameObject.Find ("Canvas").transform.Find ("All_UI").Find ("Map").Find (room_allShow [ i ].x.ToString ( ) + room_allShow [ i ].y.ToString ( )).gameObject.SetActive (true);
        }
    }

    /// <summary>
    /// 计算要显示的房间集合 
    /// </summary>
    public void Show_Room ( )
    {
        for ( int i = 0; i < room_id.Count; i++ )
        {
            if ( Room_Distance (Player_Info.instance.present_room, room_id [ i ]) == 1 )
            {
                if ( !room_allShow.Contains (room_id [ i ]) )
                {
                    room_allShow.Add (room_id [ i ]);
                }
            }
        }
    }

}
