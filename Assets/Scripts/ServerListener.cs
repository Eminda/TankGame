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
    int countG = 0;

    //Intelligent System
    public String[,] map = new String[10,10];
    private String[,] mapCopy = new String[10, 10];
    public static ServerListener serverListener;
    void Start()
    {
        serverListener = this;
        //Console.WriteLine("Thread is comming on the way");
        initializingRotation = transform.rotation;
        //UnityEngine.Debug.logger.Log("Thread is comming on the way");
        var thread = new System.Threading.Thread(listen);
        thread.Start();
        Automation.setServerListener(this);
    }
    float time = 0;int count = 0;bool start = false;
    void Update()
    {
        
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
            mapCopy= map.Clone() as string[,];
            bordInitialized = true;
        }
        while (coinsToDraw.Count > 0)
        {
            CoinObject c = coinsToDraw[0];
            coinsToDraw.RemoveAt(0);
            GameObject game = Instantiate(coin, c.getPosition(), initializingRotation) as GameObject;
            //Send data to coin. So that it can work independently later
            game.SendMessage("setValues", new int[] { c.getTimeLeft(), c.getCoinValue() });
            map[(int)c.getX(), -(int)c.getY()] = "C:" + (Time.time + c.getTimeLeft()) + ":" + c.getCoinValue() ;
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
            //UnityEngine.Debug.logger.Log("Prediction Start");
            clearPredictions();
            string[,] mapCopy = serverListener.map.Clone() as string[,];
            //UnityEngine.Debug.logger.Log("Prediction END");
            foreach (Tank t in tanks)
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
                        Vector3 pos = tank1Game.transform.position;

                        map[(int)pos.x, -(int)pos.y] = null;
                        map[(int)t.getPosition().x, -(int)t.getPosition().y] = "T0"+getDirection(t.getRotation());
                        tank1Game.transform.position = t.getPosition();
                        tank1Game.transform.rotation = t.getRotation();
                        if (playerId != playerID)
                        {
                            fillPredictions(t.getPosition(), getDirection(t.getRotation()));
                        }
                        
                        if (t.getShoot())
                        {
                            tank1Game.SendMessage("fireBullet");
                            t.setShoot(false);
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
                        Vector3 pos = tank1Game.transform.position;

                        map[(int)pos.x, -(int)pos.y] = null;
                        map[(int)t.getPosition().x, -(int)t.getPosition().y] = "T1";
                        tank2Game.transform.position = t.getPosition();
                        tank2Game.transform.rotation = t.getRotation();
                        if (playerId != playerID)
                        {
                            fillPredictions(t.getPosition(), getDirection(t.getRotation()));
                        }
                        if (t.getShoot())
                        {
                            tank2Game.SendMessage("fireBullet"); t.setShoot(false);
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
                        Vector3 pos = tank1Game.transform.position;

                        map[(int)pos.x, -(int)pos.y] = null;
                        map[(int)t.getPosition().x, -(int)t.getPosition().y] = "T2";
                        tank3Game.transform.position = t.getPosition();
                        tank3Game.transform.rotation = t.getRotation();
                        if (playerId != playerID)
                        {
                            fillPredictions(t.getPosition(), getDirection(t.getRotation()));
                        }
                        if (t.getShoot())
                        {
                            tank3Game.SendMessage("fireBullet"); t.setShoot(false);
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
                        Vector3 pos = tank1Game.transform.position;

                        map[(int)pos.x, -(int)pos.y] = null;
                        map[(int)t.getPosition().x, -(int)t.getPosition().y] = "T3";
                        tank4Game.transform.position = t.getPosition();
                        tank4Game.transform.rotation = t.getRotation();
                        if (playerId != playerID)
                        {
                            fillPredictions(t.getPosition(), getDirection(t.getRotation()));
                        }
                        if (t.getShoot())
                        {
                            tank4Game.SendMessage("fireBullet"); t.setShoot(false);
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
                        Vector3 pos = tank1Game.transform.position;

                        map[(int)pos.x, -(int)pos.y] = null;
                        map[(int)t.getPosition().x, -(int)t.getPosition().y] = "T4";
                        tank5Game.transform.position = t.getPosition();
                        tank5Game.transform.rotation = t.getRotation();
                        if (playerId != playerID)
                        {
                            fillPredictions(t.getPosition(), getDirection(t.getRotation()));
                        }
                        if (t.getShoot())
                        {
                            tank5Game.SendMessage("fireBullet"); t.setShoot(false);
                        }
                    }
                }
            }
            }
        //Automation part of the tank

        if (b)
        {
            for (int x1 = 0; x1 < serverListener.map.GetLength(0); x1 += 1)
            {
                for (int y1 = 0; y1 < serverListener.map.GetLength(1); y1 += 1)
                {
                    if(mapCopy[x1,y1]!=null && mapCopy[x1, y1].Substring(0, 1).Equals("W"))
                    {
                        map[x1, y1] = "W";
                    }
                    if (mapCopy[x1, y1] != null && mapCopy[x1, y1].Substring(0, 1).Equals("S"))
                    {
                        map[x1, y1] = "S";
                    }


                }
            }
            time += Time.deltaTime;
            if (b && time > 1.2 && (!start))
            {
                //UnityEngine.Debug.logger.Log("Moving right" + Time.time + "   " + (++count));
                Tank ta = getTankObject(playerId);
                Automation.GiveNextCommand(ta.getPosition(), ta.getDirection(), playerId, (int)ta.getHealth());
                // ServerConnect.serverConnect.right();
                time = 0;
                start = true;
                b = false;
            }
            else if (b && start)
            {
                //UnityEngine.Debug.logger.Log("Moving right" + Time.time + "   " + (++count));
                //ServerConnect.serverConnect.right();
                Tank ta = getTankObject(playerId);
                Automation.GiveNextCommand(ta.getPosition(), ta.getDirection(), playerId, (int)ta.getHealth());
                b = false;
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
                if (value.Substring(0, 8).Equals("OBSTACLE"))
                {
                    UnityEngine.Debug.logger.Log("Obstacle  ");
                    GameObject tank = getTankByID(playerId);
                    int x = (int)tank.transform.position.x;
                    int y = -(int)tank.transform.position.y;
                    Vector3 vec = getPositionToGo(getCommand(tank.transform.rotation), x, y);
                    map[x, y] = "S";
                    mapCopy[x, y] = "S";

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
                    countG++;
                    bool x = false; ;
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
                            if (countG < 2)
                            {
                                if (t.getX() != position.x || t.getY() != position.y || t.getRotationZ() != rotation.eulerAngles.z) { x = true; }
                            }
                            else
                            {
                                x = true;
                            }
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
                    b = x;
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
    private static Vector3 getPositionToGo(String command, int x, int y)
    {
        if (command.Equals("UP"))
        {
            return new Vector3(x, y - 1);
        }
        else if (command.Equals("DOWN"))
        {
            return new Vector3(x, y + 1);
        }
        else if (command.Equals("LEFT"))
        {
            return new Vector3(x - 1, y);
        }
        else if (command.Equals("RIGHT"))
        {
            return new Vector3(x + 1, y);
        }
        return new Vector3(0, 0);
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
        public void setShoot(bool shoot)
        {
            this.shoot = shoot;
        }
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
        public int getDirection()
        {
            if ((int)getRotationZ() == 0)
            {
                return 0;
            }else if ((int)getRotationZ() == 180)
            {
                return 2;
            }else if ((int)getRotationZ() == 90)
            {
                return 3;
            }
            else
            {
                return 1;
            }
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
                //UnityEngine.Debug.logger.Log("hekk" + id);
                return tanks[i];
            }
        }
        //UnityEngine.Debug.logger.Log("Tank Creating"+id);
        Tank t = new Tank(id);
        tanks.Add(t);
        return t;
        
    }
    private int getDirection(Quaternion rotation)
    {
        if (transform.rotation.eulerAngles.z == 90)
        {
            return 3;
        }else if(transform.rotation.eulerAngles.z == 180)
        {
            return 2;
        }else if(transform.rotation.eulerAngles.z == 270)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    private string getCommand(Quaternion rotation)
    {
        if (transform.rotation.eulerAngles.z == 90)
        {
            return "LEFT";
        }
        else if (transform.rotation.eulerAngles.z == 180)
        {
            return "DOWN";
        }
        else if (transform.rotation.eulerAngles.z == 270)
        {
            return "RIGHT";
        }
        else
        {
            return "UP";
        }
    }
    private void clearPredictions()
    {
        //UnityEngine.Debug.logger.Log("ADDDDDDDDDDDDDDDDD");
        for (int x = 0; x < map.GetLength(0); x += 1)
        {
            for (int y = 0; y < map.GetLength(1); y += 1)
            {
                //UnityEngine.Debug.logger.Log("QDDDDDDDDDDDDDDDDD");
                if (map[x, y] != null) {
                    if (map[x, y].Substring(0, 1).ToUpper().Equals("P"))
                    {

                        map[x, y] = null;
                    }
                }
                
            }
        }
    }
    private void fillPredictions(Vector3 pos,int direction)
    {
        int x = (int)pos.x;
        int y = -(int)pos.y;
        try {
            if (direction == 0)
            {
                if (map[x, y - 1] == null) {
                    map[x, y - 1] = "P1";
                }
            } else if (direction == 2)
            {
                if (map[x, y + 1] == null)
                {
                    map[x, y + 1] = "P1";
                }
            } else if (direction == 1)
            {
                if (map[x + 1, y] == null)
                {
                    map[x + 1, y] = "P1";
                }
            } else if (direction == 3)
            {
                if (map[x - 1, y] == null)
                {
                    map[x - 1, y] = "P1";
                }
            }
        }catch(Exception e)
        {

        }
    }
}

 