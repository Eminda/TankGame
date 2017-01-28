using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;



public class ServerListener : MonoBehaviour
{

    public bool isconnection;

    Quaternion initializingRotation;
    /*
    Server messages are recived in a thread. but in unity can't draw images as soon as we get data.
    Those images should be created when and only when update method is called.
    Hence we sore server message's data in below. And later in update they are created
    */
    //Initializing details
    private System.Collections.Generic.List<Vector3> walles = new System.Collections.Generic.List<Vector3>();
    private System.Collections.Generic.List<Vector3> stones = new System.Collections.Generic.List<Vector3>();
    private System.Collections.Generic.List<Vector3> waters = new System.Collections.Generic.List<Vector3>();
    private bool bordInitialized = true;
    //Coin emerging details

    System.Collections.Generic.List<CoinObject> coinsToDraw = new System.Collections.Generic.List<CoinObject>();


    //Health emerging details

    System.Collections.Generic.List<HealthObject> healthToDraw = new System.Collections.Generic.List<HealthObject>();

    System.Collections.Generic.List<Tank> tanks = new System.Collections.Generic.List<Tank>();
    System.Collections.Generic.Dictionary<String,GameObject> walls = new System.Collections.Generic.Dictionary<String,GameObject>();

    //Prefabs that need for the scinario
    public GameObject wall;
    public GameObject wallHit1;
    public GameObject wallHit2;
    public GameObject wallHit3;
    public GameObject wallInstance;
    public GameObject stone;
    public GameObject water;
    public GameObject health;
    public GameObject coin;

    public GameObject tank1;
    public GameObject tank2;
    public GameObject tank3;
    public GameObject tank4;
    public GameObject tank5;
    

    private GameObject tank1Game;
    private GameObject tank2Game;
    private GameObject tank3Game;
    private GameObject tank4Game;
    private GameObject tank5Game;

    // Use this for initialization
    int playerId;
    bool b=false;

    //Intelligent System
    public String[,] map = new String[10,10];
    void Start()
    {
        //Console.WriteLine("Thread is comming on the way");
        initializingRotation = transform.rotation;
        //UnityEngine.Debug.logger.Log("Thread is comming on the way");
        var thread = new System.Threading.Thread(listen);
        thread.Start();

    }
    float time = 0;int count = 0;bool start = false;
    void Update()
    {
        if (b)
        {
            time += Time.deltaTime;
            if(b && time > 1.2 && (!start)) {
                UnityEngine.Debug.logger.Log("Moving right"+Time.time+"   "+(++count));
                ServerConnect.serverConnect.right();
                time = 0;
                start = true;
                b = false;
            }else if(b && start)
            {
                ServerConnect.serverConnect.right();
                b = false;
            }
            

        }
        //initialize bord if data had recevied
        if (!bordInitialized)
        {
            foreach (Vector3 vec in walles)
            {
                Instantiate(wall, vec, initializingRotation);
                map[(int)vec.x,-(int)vec.y] = "B4";
                GameObject game = Instantiate(wallInstance, vec, Quaternion.Euler(0,0,0)) as GameObject;
                game.SendMessage("setListener", this);
                game.SendMessage("setPosition", vec);
                walls.Add(""+((int)vec.x)+((int)vec.y),game);
            }
            foreach (Vector3 vec in stones)
            {
                Instantiate(stone, vec, initializingRotation);
                map[(int)vec.x, -(int)vec.y] = "S";
            }
            foreach (Vector3 vec in waters)
            {
                Instantiate(water, vec, initializingRotation);
                map[(int)vec.x, -(int)vec.y] = "W";
            }
            bordInitialized = true;
        }
        while (coinsToDraw.Count > 0)
        {
            CoinObject c = coinsToDraw[0];
            coinsToDraw.RemoveAt(0);
            GameObject game = Instantiate(coin, c.getPosition(), initializingRotation) as GameObject;
            //Send data to coin. So that it can work independently later
            game.SendMessage("setValues", new int[] { c.getTimeLeft(), c.getCoinValue() });
            map[(int)c.getX(), -(int)c.getY()] = "C:" + (Time.time + c.getTimeLeft());
            //UnityEngine.Debug.logger.Log("Coin   " + c.getX() + "," + c.getY() + " " + c.getCoinValue() + "  time" + c.getTimeLeft());
        }
        while (healthToDraw.Count > 0)
        {
            HealthObject c = healthToDraw[0];
            healthToDraw.RemoveAt(0);
            GameObject game = Instantiate(health, c.getPosition(), initializingRotation) as GameObject;
            //Send data to coin. So that it can work independently later
            game.SendMessage("setValues", c.getTimeLeft());
            map[(int)c.getX(), -(int)c.getY()] = "H:" + (Time.time + c.getTimeLeft());
            //UnityEngine.Debug.logger.Log("Health   " + c.getX() + "," + c.getY()  + "  time" + c.getTimeLeft());
        }
        if(tanks.Count>0)
        {
            foreach(Tank t in tanks)
            {
                int playerID = t.getPlayerID();

                if (playerID == 0)
                {
                    if (tank1Game == null)
                    {
                        tank1Game = Instantiate(tank1, t.getPosition(), t.getRotation()) as GameObject;
                    }
                    else
                    {
                        tank1Game.transform.position = t.getPosition();
                        tank1Game.transform.rotation = t.getRotation();
                        if (t.getShoot())
                        {
                            tank1Game.SendMessage("fireBullet");
                        }
                    }
                }
                else if (playerID == 1)
                {
                    if (tank2Game == null)
                    {
                        tank2Game = Instantiate(tank2, t.getPosition(), t.getRotation()) as GameObject;
                    }
                    else
                    {
                        
                        tank2Game.transform.position = t.getPosition();
                        tank2Game.transform.rotation = t.getRotation();
                        if (t.getShoot())
                        {
                            tank2Game.SendMessage("fireBullet");
                        }
                    }
                }
                else if (playerID == 2)
                {
                    if (tank3Game == null)
                    {
                        tank3Game = Instantiate(tank3, t.getPosition(), t.getRotation()) as GameObject;
                    }
                    else
                    {
                        tank3Game.transform.position = t.getPosition();
                        tank3Game.transform.rotation = t.getRotation();
                        if (t.getShoot())
                        {
                            tank3Game.SendMessage("fireBullet");
                        }
                    }
                }
                else if (playerID == 3)
                {
                    if (tank4Game == null)
                    {
                        tank4Game = Instantiate(tank4, t.getPosition(), t.getRotation()) as GameObject;
                    }
                    else
                    {
                        tank4Game.transform.position = t.getPosition();
                        tank4Game.transform.rotation = t.getRotation();
                        if (t.getShoot())
                        {
                            tank4Game.SendMessage("fireBullet");
                        }
                    }
                }
                else if (playerID == 4)
                {
                    if (tank5Game == null)
                    {
                        tank5Game = Instantiate(tank5, t.getPosition(), t.getRotation()) as GameObject;
                    }
                    else
                    {
                        tank5Game.transform.position = t.getPosition();
                        tank5Game.transform.rotation = t.getRotation();
                        if (t.getShoot())
                        {
                            tank5Game.SendMessage("fireBullet");
                        }
                    }
                }
            }
            }

        }
   
    void listen()
    {
        try
        {
            //UnityEngine.Debug.logger.Log("Thread started");
            // Create the listener providing the IP Address and the port to listen on.
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
            listener.Start();

            // Listen in an endless loop. 
            while (true)
            {
                // We will store the textual representation of the bytes we receive. 
                string value;

                // Accept the sender and take message as a stream. 
                using (var networkStream = listener.AcceptTcpClient().GetStream())
                {
                    // Create a memory stream to copy the message. 

                    var bytes = new System.Collections.Generic.List<byte>();

                    int asw = 0;
                    while (asw != -1)
                    {
                        asw = networkStream.ReadByte();
                        bytes.Add((Byte)asw);
                    }

                    // Convert bytes to text. 
                    value = Encoding.UTF8.GetString(bytes.ToArray());

                }
                String[] datas = value.Split(':');
               // UnityEngine.Debug.logger.Log(Time.time);
                UnityEngine.Debug.logger.Log(datas[0]+"   "+value);
                if (datas[0].Equals("I"))
                {
                    String player = datas[1];
                    playerId = Int32.Parse(player.Substring(1));
                    String walls = datas[2];
                    walles = getVecotors(walls);
                    String stone = datas[3];
                    stones = getVecotors(stone);
                    String water = datas[4].Trim();
                    //At the end String there are unwanter  characters. one is # and other is a unicode
                    water = water.Substring(0, water.Length - 2);
                    waters = getVecotors(water);
                    bordInitialized = false;


                }
                else if (datas[0].ToUpper().Equals("C"))
                {
                    String[] cod = datas[1].Split(',');
                    Vector3 coinPosition = new Vector3(Int32.Parse(cod[0]), -Int32.Parse(cod[1]));
                    int coinCountTime = Int32.Parse(datas[2]);
                    datas[3] = datas[3].Trim();
                    datas[3] = datas[3].Substring(0, datas[3].Length - 2);
                    int coinValue = Int32.Parse(datas[3]);
                    CoinObject o = new CoinObject(coinPosition, coinValue, coinCountTime);
                    coinsToDraw.Add(o);
                    //UnityEngine.Debug.logger.Log("Coin   " + coinPosition.x+","+coinPosition.y+" "+coinValue+"  time"+coinCountTime);

                }
                else if (datas[0].ToUpper().Equals("L"))
                {
                    String[] cod = datas[1].Split(',');
                    Vector3 healthPosition = new Vector3(Int32.Parse(cod[0]), -Int32.Parse(cod[1]));
                    datas[2] = datas[2].Trim();
                    datas[2] = datas[2].Substring(0, datas[2].Length - 2);
                    int healthCountTime = Int32.Parse(datas[2]);
                    HealthObject he = new HealthObject(healthPosition, healthCountTime);
                    healthToDraw.Add(he);
                    //UnityEngine.Debug.logger.Log("Health   " + healthPosition.x + "," + healthPosition.y + " " + healthCountTime);
                }
                else if (datas[0].ToUpper().Equals("G"))
                {
                    b = true;
                    for (int i = 1; i < datas.Length; i++)
                    {
                        String[] more = datas[i].Split(';');
                        if (more[0].Substring(0, 1).ToUpper().Equals("P")) {
                            int player = Int32.Parse(more[0].Substring(1));

                            Tank t = getTankObject(player);
                            String[] cod = more[1].Split(',');
                            Vector3 position = new Vector3(Int32.Parse(cod[0]), -Int32.Parse(cod[1]));
                            int direction = Int32.Parse(more[2]);
                            Quaternion rotation = Quaternion.Euler(0, 0, -90 * direction);
                            bool shoot = Int32.Parse(more[3]) == 1;
                            float health = float.Parse(more[4]);
                            float coin = float.Parse(more[5]);
                            float point = float.Parse(more[6]);
                            if(t.getX()!=position.x || t.getY()!=position.y || t.getRotationZ()!= rotation.eulerAngles.z) { b = true; }
                            t.setValues(position, rotation, shoot, coin, health, point);

                        }
                        else
                        {
                            foreach(String s in more)
                            {
                                String s1 = s.Substring(0, 5);
                                String[] cod = s1.Split(',');
                                String key = cod[0] + cod[1];
                                if (walls.ContainsKey(key))
                                {
                                    UnityEngine.Debug.logger.Log("Brick exsit");
                                    GameObject g = walls[key];
                                    g.SendMessage("setHealth", Int32.Parse(cod[2]));
                                }
                                
                            }
                        }
                        
                    }
                    //UnityEngine.Debug.logger.Log("Tank count "+tanks.Count);
                }
                // Call an external function (void) given. 

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occured");
        }
    }
    /*
    A set off cordinates (2,3:2,4:....) will be passed to this method as a string.
    This method will decode them and create list of vectors fromt those cordinates..
    */
    System.Collections.Generic.List<Vector3> getVecotors(String data)
    {
        String[] cod = data.Split(';');
        System.Collections.Generic.List<Vector3> vectors = new System.Collections.Generic.List<Vector3>();

        foreach (String e in cod)
        {
            String[] d = e.Split(',');
            //Y is made minus because our grid system in unity is creates as such.
            Vector3 v = new Vector3(Int32.Parse(d[0]), -Int32.Parse(d[1]), 0);
            vectors.Add(v);
        }
        return vectors;

    }
    class CoinObject
    {
        Vector3 position;
        int coinValue;
        int timeLeft;
        public float getX()
        {
            return position.x;
        }
        public float getY()
        {
            return position.y;
        }
        public int getCoinValue()
        {
            return coinValue;
        }
        public int getTimeLeft()
        {
            return timeLeft;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public CoinObject(Vector3 pos, int coin, int time)
        {
            this.position = pos;
            this.coinValue = coin;
            this.timeLeft = time;
        }
    }
    class HealthObject
    {
        Vector3 position;
        int timeLeft;
        public float getX()
        {
            return position.x;
        }
        public float getY()
        {
            return position.y;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public int getTimeLeft()
        {
            return timeLeft;
        }
        public HealthObject(Vector3 pos, int time)
        {
            this.position = pos;
            this.timeLeft = time;
        }
    }

 

    //G:P0;0,0;0;0;100;0;0:0,3,0;7,1,0;3,6,0;8,6,0;4,2,0;9,8,0;9,3,0;5,2,0#
    class Tank
    {
        int playerID;
        Vector3 position;
        Quaternion rotation;
        float coin;
        float point;
        float health;
        bool shoot;
       public float getCoin()
        {
            return coin;
        }
        public float getPoint()
        {
            return point;
        }
        public float getHealth()
        {
            return health;
        }
        public Quaternion getRotation()
        {
            return rotation;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public int getPlayerID()
        {
            return playerID;
        }
        public bool getShoot()
        {
            return shoot;
        }
        public float getX()
        {
            return position.x;
        }
        public float getY()
        {
            return position.y;
        }
        public float getRotationZ()
        {
            return rotation.eulerAngles.z;
        }
        public Tank(int PlayerID,Vector3 pos,Quaternion rotation,bool shot,float coin,float health,float point)
        {
            this.playerID = PlayerID;
            this.position = pos;
            this.rotation = rotation;
            this.shoot = shot;
            this.coin = coin;
            this.health = health;
            this.point = point;
        }
        public void setValues(Vector3 pos, Quaternion rotation, bool shot, float coin, float health, float point)
        {
            this.position = pos;
            this.rotation = rotation;
            this.shoot = shot;
            this.coin = coin;
            this.health = health;
            this.point = point;
        }
        public Tank(int Player) { this.playerID = Player; }


    }



    private GameObject getTankByID(int id)
    {
        switch (id)
        {
            case 0: return tank1Game;
            case 1: return tank2Game;
            case 2: return tank3Game;
            case 3: return tank4Game;
            default: return tank5Game;
        }
    }
    private Tank getTankObject(int id)
    {
        for(int i = 0; i < this.tanks.Count; i++)
        {
            //UnityEngine.Debug.logger.Log("hekk" + id+"  "+ tanks[i].getPlayerID());
            if (tanks[i].getPlayerID() == id)
            {
                UnityEngine.Debug.logger.Log("hekk" + id);
                return tanks[i];
            }
        }
        //UnityEngine.Debug.logger.Log("Tank Creating"+id);
        Tank t = new Tank(id);
        tanks.Add(t);
        return t;
        
    }
}

 