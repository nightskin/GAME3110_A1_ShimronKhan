﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerScript : MonoBehaviour
{
    public string id;
    public float speed = 3;
    Vector3 movement;
    private void Start()
    {
        movement = new Vector3(0, 0, 0);
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            movement.x = speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement.z = speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.z = speed * Time.deltaTime;
        }
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W))
        {
            movement = Vector3.zero;
        }

        transform.position += movement;
    }
}
