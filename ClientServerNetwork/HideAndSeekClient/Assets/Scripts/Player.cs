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
    GameManager gameManager;
    Rigidbody rb;

    private void Start()
    {
        clientEx = FindObjectOfType<ExampleClient>();
        clientNet = FindObjectOfType<ClientNetwork>();
        gameManager = FindObjectOfType<GameManager>();
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

            if(Input.GetButtonDown("Ready"))
            {
                ReadyUp();
            }

            //slappage
            if(Input.GetButtonDown("Slap") && seeking == true)
            {
                Vector3 slapPos = transform.position + (transform.rotation * Vector3.forward * 2);
                clientEx.clientNet.CallRPC("Slap", UCNetwork.MessageReceiver.ServerOnly, -1, slapPos);
            }
            //Jumping
            if(Input.GetButtonDown("Jump"))
            {
                if(Physics.Raycast(transform.position, Vector3.down, 1))
                {
                    rb.AddRelativeForce(new Vector3(0, 7, 0), ForceMode.Impulse);
                }
            }
            Debug.DrawRay(transform.position, (transform.rotation * Vector3.forward) * 10);
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
            gameManager.playerStatusText.text = "Seeker";
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
            ReadyUp();
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

    private void ReadyUp()
    {
        ready = true;
        clientNet.CallRPC("PlayerIsReady", UCNetwork.MessageReceiver.ServerOnly, -1);
    }
}