using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    float speed = 5;
    public bool seeking;
    public bool ready = false;

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkSync>().owned)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime, 0);
            transform.position += movement;
            CameraScript.instance.transform.SetPositionAndRotation(
                transform.position,
                transform.rotation);
        }
    }

    public void PlayerIsSeeker(int networkId) //The player is now a seeker
    {
        seeking = true;
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
