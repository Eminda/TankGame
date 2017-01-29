using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

    public GameObject bullet;

    // Use this for initialization
    private static readonly int UP = 0;
    private static readonly int RIGHT = 1;
    private static readonly int DOWN = 2;
    private static readonly int LEFT = 3;

    //float speed = 10;
    //Vector3 position;
    ////preposition stores the position before a movement occurs. if a collision occurs. tank will get this cordinate
    //Vector3 prePosition;
    //bool b = false;
    //float t = 5;
    //Quaternion initRotation;
    ////distance that tank goes for 0,1 seconds
    //float addY = 0;
    //float addX = 0;
    //bool colided = false;
    //int rotation = UP;
    //int coinCollected;

    void Start()
    {
        //    initRotation = transform.rotation;
        //    position = transform.position;
        //    prePosition = position;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //b is used to restrict continues movemen. inputs will be taken one for one sec
    //    if (b)
    //    {
    //        if (t ==5)
    //        {
    //            prePosition.x = position.x;
    //            prePosition.y = position.y;
    //        }
    //        //t gets 10 when a key is pressed. 10 should go to 0 in one sec
    //        t -= Time.deltaTime*10;
    //        //if a collision occurs b will be false.
    //        if (!colided)
    //        {
    //            position.x = transform.position.x + addX*Time.deltaTime*2;
    //            position.y = transform.position.y + addY*Time.deltaTime*2;
    //            transform.position = position;
    //        }
    //        if (t <= 0)
    //        {
    //            b = false;
    //            t = 5;
    //        }
    //    }
    //    else
    //    {
    //        //Subtractions some time get answers like 2.9999 instead of 3. This is to solve that issue
    //        position.x = Mathf.Round(transform.position.x);
    //        position.y = Mathf.Round(transform.position.y);
    //        transform.position = position;
    //        if (Input.GetKey(KeyCode.RightArrow))
    //        {

    //            if (rotation==RIGHT && Mathf.Round(transform.position.x) < 9)
    //            {
    //                addX = 1f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //            else
    //            {
    //                //rotate in direction
    //                transform.rotation = initRotation * Quaternion.Euler(0, 0, -90);
    //                rotation = RIGHT;
    //                addX = 0f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //        }else if (Input.GetKey(KeyCode.LeftArrow))
    //        {
    //            Debug.logger.Log("dfg" + Mathf.Round(transform.position.x) + "  " + (Mathf.Round(transform.position.y)));
                
    //            if (rotation==LEFT && Mathf.Round(transform.position.x )>= 1)
    //            {
    //                addX = -1f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //            else
    //            {
    //                transform.rotation = initRotation * Quaternion.Euler(0, 0, 90);
    //                rotation = LEFT;
    //                addX = 0f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //        }else if (Input.GetKey(KeyCode.DownArrow))
    //        {
    //            Debug.logger.Log("dfg" + Mathf.Round(transform.position.x) + "  " + (Mathf.Round(transform.position.y)));
                
    //            if (rotation==DOWN &&  Mathf.Round(transform.position.y) > -9)
    //            {
    //                addY = -1f;
    //                addX = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //            else
    //            {
    //                transform.rotation = initRotation * Quaternion.Euler(0, 0, 180);
    //                rotation = DOWN;
    //                addX = 0f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //        }else if (Input.GetKey(KeyCode.UpArrow))
    //        {
    //            Debug.logger.Log("dfg" + Mathf.Round(transform.position.x) + "  " + (Mathf.Round(transform.position.y)));
                
    //            if (rotation==UP && Mathf.Round(transform.position.y) <= -1)
    //            {
    //                addX = 0f;
    //                addY = 1f;
    //                b = true;
    //                colided = false;
    //            }
    //            else
    //            {
    //                transform.rotation = initRotation * Quaternion.Euler(0, 0, 0);
    //                rotation = UP;
    //                addX = 0f;
    //                addY = 0f;
    //                b = true;
    //                colided = false;
    //            }
    //        }else if (Input.GetKey(KeyCode.RightShift))
    //        {
    //            Debug.logger.Log("Firing");
    //            Vector3 position1 = transform.position;
    //            Quaternion initialRotation = Quaternion.Euler(0, 0, 0); ;
    //            Debug.logger.Log("Firddd3333ing");
    //            if (rotation == UP)
    //            {
    //                initialRotation = initRotation * Quaternion.Euler(0, 0, 90);
    //                position1.y += 0.9f;
    //            }
    //            else if (rotation == DOWN)
    //            {
    //                initialRotation = initRotation * Quaternion.Euler(0, 0, -90);
    //                position1.y += -0.9f;
    //            }
    //            else if (rotation == LEFT)
    //            {
    //                initialRotation = initRotation * Quaternion.Euler(0, 0, 180);
    //                position1.x -= 0.9f;
    //            }
    //            else
    //            {
    //                position1.x += 0.9f;
    //            }
    //            GameObject game = Instantiate(bullet, position1, initialRotation) as GameObject;
    //            game.SendMessage("setPosition",rotation);
    //            Debug.logger.Log("Fireddd"+bullet);

    //            Debug.logger.Log("Fired");
    //        }
            

    //    }
        
    //}
    void update()
    {

    }
    public void fireBullet()
    {
        Vector3 position1 = transform.position;
        Quaternion initialRotation = Quaternion.Euler(0, 0, 0); ;
        //Debug.logger.Log("Firddd3333ing"+transform.rotation.z);
        int rotation = -1;
        if (transform.rotation.z == 0)
        {
            initialRotation = Quaternion.Euler(0, 0, 90);
            position1.y += 0.9f;
            rotation = 0;
            GameObject game = Instantiate(bullet, position1, initialRotation) as GameObject;
            game.SendMessage("setPosition", rotation);
            //Debug.logger.Log("Fireddd" + bullet);
        }
        //else 
        //Debug.logger.Log("AAAAAAAAAAAAAAAA" + transform.rotation.eulerAngles.z);
        if (transform.rotation.eulerAngles.z == 90)
        {
            initialRotation = Quaternion.Euler(0, 0, 180);
            position1.x += -0.9f;
            rotation = 3;
            GameObject game = Instantiate(bullet, position1, initialRotation) as GameObject;
            game.SendMessage("setPosition", rotation);
            //Debug.logger.Log("Fireddd" + bullet);
        }
        if (transform.rotation.eulerAngles.z == 270)
        {
            initialRotation = Quaternion.Euler(0, 0, 0);
            position1.x += 0.9f;
            rotation = 1;
            GameObject game = Instantiate(bullet, position1, initialRotation) as GameObject;
            game.SendMessage("setPosition", rotation);
            //Debug.logger.Log("Fireddd" + bullet);
        }
        if (transform.rotation.eulerAngles.z == 180)
        {
            initialRotation = Quaternion.Euler(0, 0, -90);
            position1.y -= 0.9f;
            rotation = 2;
            GameObject game = Instantiate(bullet, position1, initialRotation) as GameObject;
            game.SendMessage("setPosition", rotation);
            //Debug.logger.Log("Fireddd" + bullet);
        }

    }
    public void OnCollisionEnter2D(Collision2D col)
     {

        //if (col.gameObject.name == "Wall" || col.gameObject.name == "Stone" || col.gameObject.name == "Water")
        //{
        //    colided = true;
        //    transform.position = position;
        //}
    }

    public void coinAdded(int coinValue)
    {
        //coinCollected += coinValue;
        Debug.logger.Log("Coin gained" + coinValue);
    }
    public void healthGained()
    {
        //Debug.logger.Log("Health restores");
    }
    private void goRight()
    {

    }
    private void goLeft()
    {

    }
    private void goUp()
    {

    }
    private void goDown()
    {

    }
}
