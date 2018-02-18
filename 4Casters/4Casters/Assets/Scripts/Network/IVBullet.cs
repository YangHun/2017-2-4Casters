using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVBullet : NetworkBehaviour {

	[SerializeField]
    IVPlayer player;

	bool isEscaped = false;		// used to disable colider while bullet is inside of owner's rigidbody

    float timer = 0.0f;
	
	[SerializeField]
    float lifetime = 4.0f;
	float escapetime = 0.222f;

	// Use this for initialization
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            timer += Time.deltaTime;
        }
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
		if (!isEscaped && timer >= escapetime)
		{
			GetComponent<SphereCollider>().enabled = true;
			isEscaped = true;
		}
    }
	
	public void SetOwner(IVPlayer player)
	{
		this.player = player;
	}

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Monster")
		{
		    player.CmdAttackMonster(collision.gameObject.GetComponent<NetworkIdentity>(), player.id);
			NetworkServer.Destroy(gameObject);
		}
    }
}
