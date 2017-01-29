﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automation {

    private static ServerListener serverListener;
    private static Collectable choosenCoin=null;

    public static void setServerListener(ServerListener lis)
    {
        serverListener = lis;
    }

    public static void GiveNextCommand(Vector3 current, int direction, int playerID,int health)
    {
        //UnityEngine.Debug.logger.Log("start 0");
        if (choosenCoin != null && !serverListener.map[choosenCoin.X, choosenCoin.Y].Substring(0, 1).Equals("C"))
        {
            choosenCoin = null;
        }
        List<Collectable> coins = new List<Collectable>();
        List<Tank> tanks = new List<Tank>();
        List<Collectable> healths = new List<Collectable>();
        //UnityEngine.Debug.logger.Log("Automation 0"+ serverListener.map);
        for (int x = 0; x < serverListener.map.GetLength(0); x += 1)
        {
            for (int y = 0; y < serverListener.map.GetLength(1); y += 1)
            {
                //UnityEngine.Debug.logger.Log("Automation Check 00");
                if (serverListener.map[x, y]!=null && serverListener.map[x, y].Substring(0, 1).ToUpper().Equals("C"))
                {
                    //UnityEngine.Debug.logger.Log("Automation Check");
                    string value = serverListener.map[x, y];
                    Collectable c = new Collectable();
                    c.X = x;
                    c.Y = y;
                    string[] data = value.Split(':');
                    c.Disapear = float.Parse(data[1]);
                    c.Value = int.Parse(data[2]);
                    coins.Add(c);
                }
            }
        }
        //UnityEngine.Debug.logger.Log("Automation 1");
        for (int x = 0; x < serverListener.map.GetLength(0); x += 1)
        {
            for (int y = 0; y < serverListener.map.GetLength(1); y += 1)
            {
                if (serverListener.map[x, y] != null && serverListener.map[x, y].Substring(0, 1).ToUpper().Equals("H"))
                {
                    string value = serverListener.map[x, y];
                    Collectable c = new Collectable();
                    c.X = x;
                    c.Y = y;
                    string[] data = value.Split(':');
                    c.Disapear = float.Parse(data[1]);
                    healths.Add(c);
                }
            }
        }
        //UnityEngine.Debug.logger.Log("Automation2");
        for (int x = 0; x < serverListener.map.GetLength(0); x += 1)
        {
            for (int y = 0; y < serverListener.map.GetLength(1); y += 1)
            {
                if (serverListener.map[x, y] != null && serverListener.map[x, y].Substring(0, 1).ToUpper().Equals("T") && !serverListener.map[x, y].Substring(1, 1).Equals(playerID + ""))
                {
                    string value = serverListener.map[x, y];
                    Tank t = new Tank();
                    t.X = x;
                    t.Y = y;
                }
               
            }
        }
        //UnityEngine.Debug.logger.Log("Automation 3");
        
            findCostToTheCollectable(coins,(int) current.x, -(int)current.y, direction, true);
            foreach(Tank t in tanks)
            {
                findCostToTheCollectable(coins, t.X, t.Y, direction, false);
            }
        
       
            findCostToTheCollectable(healths, (int)current.x, -(int)current.y, direction, true);
    
        
        //UnityEngine.Debug.logger.Log("Automation 4");
        Collectable bestCoin = null; ;
        if (coins.Count > 0)
        {
            //UnityEngine.Debug.logger.Log("Automation 4 Coin Count Greater than 0 :"+coins.Count+"  xx "+coins[0].Cost+"  "+coins[0].Command+"  "+coins[0].Value);
            Collectable bestCoinVal;
            Collectable bestCoinCost;
            
            List<Collectable> closerTome = new List<Collectable>();
            foreach(Collectable c in coins)
            {
                if (c.Cost < c.MinCostOther)
                {
                    closerTome.Add(c);
                }
            }
            if (closerTome.Count > 0)
            {
                //UnityEngine.Debug.logger.Log("Automation 4 Closer");
                bestCoinVal = closerTome[0];
                foreach(Collectable c in closerTome)
                {
                    if (bestCoinVal.Value < c.Value)
                    {
                        bestCoinVal = c;
                    }
                }
                bestCoinCost = closerTome[0];
                foreach(Collectable c in closerTome)
                {
                    if (bestCoinCost.Cost > c.Cost)
                    {
                        bestCoinCost = c;
                    }
                }
                if (!bestCoinCost.Equals(bestCoinCost))
                {
                    if (bestCoinVal.Value - bestCoinCost.Value > 200)
                    {
                        if (bestCoinVal.Cost - bestCoinCost.Cost < 2)
                        {
                            bestCoin = bestCoinVal;
                        }else if (bestCoinVal.Value - bestCoinCost.Value > 400)
                        {
                            if (bestCoinVal.Cost - bestCoinCost.Cost < 4)
                            {
                                bestCoin = bestCoinVal;
                            }
                            else
                            {
                                bestCoin = bestCoinCost;
                            }
                        }
                    }
                    else
                    {
                        bestCoin = bestCoinCost;
                    }
                }
                else
                {
                    bestCoin = bestCoinCost;
                }
            }
            else
            {
                int difference = 0;
                bestCoin = coins[0];
                foreach(Collectable c in coins)
                {
                    if (difference < c.MinCostOther)
                    {
                        bestCoin = c;
                    }
                }
            }
        }
        UnityEngine.Debug.logger.Log("Automation 5" + bestCoin+"   "+bestCoin.Command);
        if (bestCoin != null)
        {
            UnityEngine.Debug.logger.Log("Automation 5sssssssssss");
            Vector3 pos = getPositionToGo(bestCoin.Command,(int)current.x,-(int)current.y);
            UnityEngine.Debug.logger.Log("Automation 5swwwwwssswwssss"+pos.x+"  "+pos.y);
            try {
            if (serverListener.map[(int)pos.x,(int)pos.y]!=null && serverListener.map[(int)pos.x, (int)pos.y].Substring(0, 1).Equals("B")){
                UnityEngine.Debug.logger.Log("Automatioqqqqqqqqqqqqqqqqq");
                ServerConnect.serverConnect.shoot();
            }
            else
            {
                UnityEngine.Debug.logger.Log("Automation 5 invoke" + bestCoin.Command);
                invokeCommand(bestCoin.Command);
            }
            UnityEngine.Debug.logger.Log("Automation 5" + bestCoin.Command);
            }catch(Exception e)
            {
                UnityEngine.Debug.logger.Log("Automation 5" + e.Message);
            }
        }

    }
    private static void invokeCommand(String command)
    {
        if (command.Equals("UP"))
        {
            ServerConnect.serverConnect.up();
        }else if (command.Equals("DOWN"))
        {
            ServerConnect.serverConnect.down();
        }else if (command.Equals("LEFT"))
        {
            ServerConnect.serverConnect.left();
        }else if (command.Equals("RIGHT"))
        {
            ServerConnect.serverConnect.right(); 
        }
    }
    private static Vector3 getPositionToGo(String command,int x,int y)
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
            return new Vector3(x-1, y);
        }
        else if (command.Equals("RIGHT"))
        {
            return new Vector3(x +1, y);
        }
        return new Vector3(0, 0);
    }
    private static void findCostToTheCollectable(List<Collectable> collectables, int x, int y, int direction, bool me)
    {
        string[,] map = serverListener.map.Clone() as string[,];
        map[x, y] = "R:0:";
        try
        {
            int t = 1;
            if (direction != 0) { t++; }
            if (map[x, y - 1] == null || map[x, y - 1].Substring(0, 1).Equals("R") || (map[x, y - 1].Equals("P1") && t > 1) || map[x, y - 1].Substring(0, 1).Equals("C") || map[x, y - 1].Substring(0, 1).Equals("H") || map[x, y - 1].Substring(0, 1).Equals("T"))
            {
                //UnityEngine.Debug.logger.Log("Filling calling");
                map[x, y - 1] = "R:" + t + ":" + "UP";
                fillRest(x, y - 1, 0, map);
            }
            else if (map[x, y - 1] != null && map[x, y - 1].Substring(0, 1).Equals("B"))
            {
                int brickHealth = int.Parse(map[x, y - 1].Substring(1, 1));
                t += brickHealth;
                map[x, y - 1] = "R:" + t + ":" + "UP";
                fillRest(x, y - 1, 0, map);

            }
            
        }catch (Exception e) { }
        try
        {
            int t = 1;
            if (direction != 2) { t++; }
            if (map[x, y + 1] == null || map[x, y + 1].Substring(0, 1).Equals("R") || (map[x, y + 1].Equals("P1") && t > 1) || map[x, y + 1].Substring(0, 1).Equals("C") || map[x, y + 1].Substring(0, 1).Equals("H") || map[x, y + 1].Substring(0, 1).Equals("T"))
            {
                //UnityEngine.Debug.logger.Log("Filling calling");
                map[x, y + 1] = "R:" + t + ":" + "DOWN";
                fillRest(x, y + 1, 2, map);
            }
            else if (map[x, y + 1] != null && map[x, y + 1].Substring(0, 1).Equals("B"))
            {
                int brickHealth = int.Parse(map[x, y + 1].Substring(1, 1));
                t += brickHealth;
                map[x, y + 1] = "R:" + t + ":" + "DOWN";
                fillRest(x, y + 1, 2, map);

            }
            
        }
        catch (Exception e) { }
        try
        {
            int t = 1;
            if (direction != 1) { t++; }
            if (map[x + 1, y ] == null || map[x + 1, y].Substring(0, 1).Equals("R") || (map[x + 1, y].Equals("P1") && t > 1) || map[x + 1, y].Substring(0, 1).Equals("C") || map[x + 1, y].Substring(0, 1).Equals("H") || map[x + 1, y].Substring(0, 1).Equals("T"))
            {
                //UnityEngine.Debug.logger.Log("Filling calling");
                map[x + 1, y] = "R:" + t + ":" + "RIGHT";
                fillRest(x + 1, y, 1, map);
            }
            else if (map[x + 1, y] != null && map[x + 1, y].Substring(0, 1).Equals("B"))
            {
                int brickHealth = int.Parse(map[x + 1, y].Substring(1, 1));
                t += brickHealth;
                map[x + 1, y] = "R:" + t + ":" + "RIGHT";
                fillRest(x + 1, y, 1, map);

            }
            
        }
        catch (Exception e) { }
        try
        {
            int t = 1;
            if (direction != 3) { t++; }
            if (map[x - 1, y] == null || map[x - 1, y].Substring(0, 1).Equals("R") || (map[x - 1, y].Equals("P1") && t > 1) || map[x - 1, y].Substring(0, 1).Equals("C") || map[x - 1, y].Substring(0, 1).Equals("H") || map[x - 1, y].Substring(0, 1).Equals("T"))
            {
                //UnityEngine.Debug.logger.Log("Filling calling");
                map[x - 1, y] = "R:" + t + ":" + "LEFT";
                fillRest(x - 1, y, 3, map);
            }
            else if (map[x - 1, y] != null && map[x - 1, y].Substring(0, 1).Equals("B"))
            {
                int brickHealth = int.Parse(map[x - 1, y].Substring(1, 1));
                t += brickHealth;
                map[x - 1, y] = "R:" + t + ":" + "LEFT";
                fillRest(x - 1, y, 3, map);

            }
            
        }
        catch (Exception e) { }
        foreach(Collectable collectable in collectables)
        {
            string last = map[collectable.X, collectable.Y];
            if (last.Substring(0, 1).Equals("R"))
            {
                string[] data = last.Split(':');
                if (me)
                {
                    collectable.Cost = int.Parse(data[1]);
                    collectable.Command = data[2];
                }
                else
                {
                    if (collectable.MinCostOther > int.Parse(data[1]))
                    {
                        collectable.MinCostOther = int.Parse(data[1]);
                    }
                }
            }
        }
        
        for (int x1 = 0; x1 < serverListener.map.GetLength(0); x1 += 1)
        {
            String line = "";
            for (int y1 = 0; y1 < serverListener.map.GetLength(1); y1 += 1)
            {
                line +="C  "+ map[x1, y1];
                

            }
            UnityEngine.Debug.logger.Log(line);
        }
        
    }
    //comes current cell
    private static void fillRest(int x, int y, int direction, string[,] map)
    {
        string[] data = map[x, y].Split(':');
        int value = int.Parse(data[1]);
        string command = data[2];
        callAccordingToDirection(x, y, value, command, map, direction);


    }
    private static void callAccordingToDirection(int x,int y,int value,string command,string[,] map,int direction)
    {
        //UnityEngine.Debug.logger.Log("Called"+x+" "+y+"  "+command+"  "+direction);
        bool b = false;
        if (y - 1 >= 0)
        {
            int t = 1;
            if (direction != 0) { t++; }
            b = checkFillCell(x, y - 1, value + t, command, map);
            if (b)
            {
                fillRest(x, y - 1, 0, map);
            }
        }
        if (y + 1 <= 9)
        {
            int t = 1;
            if (direction != 2) { t++; }
            b = checkFillCell(x, y + 1, value + t, command, map);
            if (b)
            {
                fillRest(x, y + 1, 2, map);
            }
        }
        if (x - 1 >= 0)
        {
            int t = 1;
            if (direction != 3) { t++; }
            b = checkFillCell(x - 1, y, value + t, command, map);
            if (b)
            {
                fillRest(x - 1, y, 3, map);
            }
        }
        if (x + 1 <= 9)
        {
            //UnityEngine.Debug.logger.Log("Right coming");
            int t = 1;
            if (direction != 1) { t++; }
            b = checkFillCell(x + 1, y, value + t, command, map);
            //UnityEngine.Debug.logger.Log("Right coming"+b);
            if (b)
            {
                fillRest(x + 1, y, 1, map);
            }
        }
    }
    //comes next cell
    private static bool checkFillCell(int x, int y, int value, string command, string[,] map)
    {
        if (map[x, y] == null || map[x, y].Substring(0, 1).Equals("R") || (map[x, y].Equals("P1") && value > 1) || map[x,y].Substring(0,1).Equals("C") || map[x, y].Substring(0, 1).Equals("H") || map[x, y].Substring(0, 1).Equals("T"))
        {
            //UnityEngine.Debug.logger.Log("Filling calling");
            return fillCell(x, y, value, map, command);
            
        }else if(map[x, y] != null && map[x, y].Substring(0, 1).Equals("B"))
        {
            int brickHealth = int.Parse(map[x, y].Substring(1, 1));
            value += brickHealth;
            return fillCell(x, y, value, map, command);
        }
        return false;
    }
    //comes next cell
    private static bool fillCell(int x,int y,int value,string[,] map,string command)
    {
        //UnityEngine.Debug.logger.Log("Filling called");
        if (map[x, y]!=null && map[x, y].Substring(0, 1).Equals("R"))
        {
            int preVal = int.Parse(map[x, y].Split(':')[1]);
            if (preVal > value)
            {
                map[x, y] = "R:" + value + ":" + command;
                return true;
            }
        }
        else
        {
            //UnityEngine.Debug.logger.Log("Came to point"+x+"  "+y+"  "+command);
            map[x, y] = "R:" + value + ":" + command;
            return true;
        }
        return false;
    }
    class Collectable
    {
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }


        private float timeDisapear;

        public float Disapear
        {
            get { return timeDisapear; }
            set { timeDisapear = value; }
        }
        private int cost;

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        private int costOther = int.MaxValue;

        public int MinCostOther
        {
            get { return costOther; }
            set { costOther = value; }
        }

        private String command;

        public String Command
        {
            get { return command; }
            set { command = value; }
        }
        private int val;

        public int Value
        {
            get { return val; }
            set { val = value; }
        }
    }

   
    class Tank
    {
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

    }

}
