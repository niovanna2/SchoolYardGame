﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField] float speed = 500.0f;
    public bool seeking;
    public bool ready = false;
    ExampleClient clientEx;
    ClientNetwork clientNet;
    Rigidbody rb;

    private void Start()
    {
        clientEx = FindObjectOfType<ExampleClient>();
        clientNet = FindObjectOfType<ClientNetwork>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {        
        float dt = Time.deltaTime;
        if (GetComponent<NetworkSync>().owned)
        {
            //camera stuff
            if (CameraScript.instance.transform.parent != transform)
            {
                CameraScript.instance.transform.SetParent(transform);
                CameraScript.instance.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            //movement
            Vector3 addForce = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical"));
            if (addForce.magnitude > 1.0f)
                addForce.Normalize();
            rb.AddRelativeForce(addForce * speed * dt);

            //looking left/right
            transform.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X"), 0);

            //slappage
            if(Input.GetButtonDown("Slap") && seeking == true)
            {
                clientEx.clientNet.CallRPC("Slap", UCNetwork.MessageReceiver.ServerOnly, -1);
            }
            Debug.DrawRay(transform.position, transform.rotation * Vector3.forward * 2);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity *= 1.0f - Time.fixedDeltaTime;
    }

    public void PlayerIsSeeker(int networkId) //The player is now a seeker
    {
        seeking = true;
        speed = 75;
    }

    public void PlayerIsNotSeeker(int networkId) //The player is now a seeker
    {
        seeking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "ResourceNode")
        {
            clientNet.CallRPC("PlayerIsReady", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "ResourceNode")
        {
            ready = false;
        }
    }
}
