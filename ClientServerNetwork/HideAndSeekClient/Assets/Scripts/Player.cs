using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

    [SerializeField] float defaultSpeed = 550.0f;
    [SerializeField] float seekerSpeed = 750.0f;
    float speed = 550.0f;
    [SerializeField] float dragScalar = 1.0f;
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
                //clientEx.clientNet.CallRPC("Slap", UCNetwork.MessageReceiver.ServerOnly, -1);
                RaycastHit hitObject;
                if(Physics.Raycast(Vector3.forward, Vector3.forward * 1.5f, out hitObject))
                {
                    if(hitObject.transform.gameObject.tag == "Player")
                    {
                        clientEx.clientNet.CallRPC("PlayerIsNowSeeking", UCNetwork.MessageReceiver.ServerOnly, -1, hitObject.transform.gameObject.GetComponent<NetworkSync>().GetId());
                    }
                }
            }
            //Jumping
            if(Input.GetButtonDown("Jump"))
            {
                rb.AddRelativeForce(new Vector3(0, 7, 0), ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(
            rb.velocity.x * (1.0f - (Time.fixedDeltaTime * dragScalar)),
            rb.velocity.y,
            rb.velocity.z * (1.0f - (Time.fixedDeltaTime * dragScalar)));
    }

    public void Seeker(int networkId) //The player is now a seeker
    {
        if(GetComponent<NetworkSync>().GetId() == networkId)
        {
            seeking = true;
            speed = seekerSpeed;
            GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            //transform.position = SpawnPoints.instance.seekerSpawnPoint.transform.position;
        }

    }

    public void NotSeeker(int networkId) //The player is not a seeker
    {
        if (GetComponent<NetworkSync>().GetId() == networkId)
        {
            seeking = false;
            speed = defaultSpeed;
            GetComponent<Renderer>().material.color = new Color(0, 1, 0);
            //try
            //{
            //    int slot = networkId % SpawnPoints.instance.spawnPoints.Count;
            //    transform.position = SpawnPoints.instance.spawnPoints[slot].transform.position;
            //}
            //catch(Exception e)
            //{
            //    Debug.Log(e.Message);
            //}
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "ResourceNode")
        {
            ready = true;
            clientNet.CallRPC("PlayerIsReady", UCNetwork.MessageReceiver.ServerOnly, -1);
        }
        else if(collision.collider.tag == "Respawn")
        {
            transform.position = new Vector3(0, 20, 0);
        }
        /*else if(collision.collider.tag == "Player" && seeking)
        {
            clientEx.clientNet.CallRPC("PlayerIsNowSeeking", UCNetwork.MessageReceiver.ServerOnly, -1, collision.gameObject.GetComponent<NetworkSync>().GetId());
        } */
    }
}