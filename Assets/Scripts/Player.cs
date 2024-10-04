using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public float thrustForce = 2000f;
    public float rotationSpeed = 1200f;
    public GameObject gun, bulletPrefab, bullet;

    private Rigidbody _rigid;
    private int hp = 3;
    public static int SCORE = 0;
    private Vector3 originalPos;
    public static float yBorderLimit, xBorderLimit;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        originalPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        yBorderLimit = Camera.main.orthographicSize;
        xBorderLimit = (Camera.main.orthographicSize) * Screen.width / Screen.height;
    }
    
    // Update is called once per frame
    void Update()
    {
        var newPos = transform.position;
        if(newPos.x > xBorderLimit) newPos.x = -xBorderLimit;
        else if(newPos.x < -xBorderLimit) newPos.x = xBorderLimit;
        if(newPos.y > yBorderLimit) newPos.y = -yBorderLimit;
        else if(newPos.y < -yBorderLimit) newPos.y = yBorderLimit;
        transform.position = newPos;

        float rotation = Input.GetAxis("Horizontal") * Time.deltaTime;
        float thrust = Input.GetAxis("Vertical") * Time.deltaTime;
        Vector3 thrustDirection = transform.right;

        _rigid.AddForce(thrustDirection * thrust * thrustForce);
        transform.Rotate(Vector3.forward, -rotation * rotationSpeed);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hp--;
            switch (hp)
            {
                case 2:
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.transform.position = originalPos;
                    Destroy(collision.gameObject);
                    Destroy(GameObject.FindGameObjectWithTag("HP2"));
                    break;
                case 1:
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.transform.position = originalPos;
                    Destroy(GameObject.FindGameObjectWithTag("HP2"));
                    Destroy(GameObject.FindGameObjectWithTag("HP1"));
                    Destroy(collision.gameObject);
                    break;
                case 0:
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Destroy(collision.gameObject);
                    SCORE = 0;
                    hp = 3;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
            }
        }
        else
        {
            Debug.Log("He colisionado con otro objeto");
        }
    }
}
