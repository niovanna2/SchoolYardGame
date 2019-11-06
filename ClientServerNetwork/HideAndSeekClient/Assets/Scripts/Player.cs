using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    float speed = 5;
    public bool seeking;
    public bool ready = false;
    ExampleClient clientEx;

    private void Start()
    {
        clientEx = FindObjectOfType<ExampleClient>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            //Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime);
            if(Input.GetKey(KeyCode.W))
            {
                Vector3 movement = (transform.rotation * Vector3.forward) * speed * Time.deltaTime;
                transform.position += movement;
            }
            if(Input.GetKey(KeyCode.S))
            {
                Vector3 movement = (transform.rotation * Vector3.back) * speed * Time.deltaTime;
                transform.position += movement;
            }
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, Input.GetAxis("Horizontal") * 2, 0);
            }
            if(Input.GetKeyDown(KeyCode.Space) && seeking == true)
            {
                clientEx.clientNet.CallRPC("Slap", UCNetwork.MessageReceiver.ServerOnly, -1);
            }
            Debug.DrawRay(transform.position, transform.rotation * Vector3.forward * 2);

            CameraScript.instance.transform.SetPositionAndRotation(
                transform.position,
                transform.rotation);
        }
    }

    public void PlayerIsSeeker(int networkId) //The player is now a seeker
    {
        seeking = true;
        speed = 7;
    }

    public void PlayerIsNotSeeker(int networkId) //The player is now a seeker
    {
        seeking = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.tag == "ResourceNode")
        {
            ready = true;
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
