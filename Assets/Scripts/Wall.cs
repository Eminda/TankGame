using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    // Use this for initialization
    public GameObject next;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.logger.Log("colided"+ col.gameObject.tag);
        if (col.gameObject.tag == "bullet")
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Destroy(gameObject);
            Vector3 pos = transform.position;
            int x = (int)pos.x;
            int y = -(int)pos.y;
            if (next != null)
            {
                Instantiate(next, position, rotation);
                

                string brick=ServerListener.serverListener.map[x, y];
                int val = int.Parse(brick.Substring(1, 1));
                brick = brick.Substring(0, 1) + (val - 1);
                ServerListener.serverListener.map[x, y] = brick;
                Debug.logger.Log("colided opa");
            }
            else
            {
                ServerListener.serverListener.map[x, y] = null;
            }
        }
    }
}
