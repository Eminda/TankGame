using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    // Use this for initialization
    float timeLeft;
    bool b = true;
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!b)
        
        {
            timeLeft -= Time.deltaTime * 1000;
            if (timeLeft < 0)
            {
                UnityEngine.Debug.logger.Log("Health  vanished "  + "  time" + Time.time);
                Destroy(gameObject);
            }
        }
    }
    public void setValues(int timeLeft)
    {
        this.timeLeft = timeLeft;
        b = false;
        Debug.logger.Log("HValues set"+Time.time);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Tank")
        {
            col.gameObject.SendMessage("healthGained");
            Vector3 pos = transform.position;
            int x = (int)pos.x;
            int y = -(int)pos.y;
            ServerListener.serverListener.map[x, y] = null;
            Destroy(gameObject);
        }
    }
}
