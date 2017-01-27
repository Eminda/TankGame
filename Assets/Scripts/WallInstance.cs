using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInstance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int h = 4 - damage;
        if (health != h)
        {
            Destroy(brick);
            if (damage == 1)
            {
                brick = Instantiate(listner.wallHit1, position, Quaternion.Euler(0, 0, 0)) as GameObject;
                listner.map[(int)position.x, -(int)position.y] = "B3";
            }
            else if (damage == 2)
            {
                brick = Instantiate(listner.wallHit2, position, Quaternion.Euler(0, 0, 0)) as GameObject;
                listner.map[(int)position.x, -(int)position.y] = "B2";
            }
            else if (damage == 3)
            {
                brick = Instantiate(listner.wallHit3, position, Quaternion.Euler(0, 0, 0)) as GameObject;
                listner.map[(int)position.x, -(int)position.y] = "B1";
            }
            else
            {
                listner.map[(int)position.x, -(int)position.y] = null;
            }
        }
        health = h;
    }
    Vector3 position;
    int health = 4;
    int damage=0;
    GameObject brick;
    ServerListener listner;
    public void setListener(ServerListener listner)
    {
        this.listner = listner;
    }
   
    public void setPosition(Vector3 position)
    {
        this.position = position;
    }
    public void setHealth(int damage)
    {
        this.damage = damage;
        
        

    }
}
