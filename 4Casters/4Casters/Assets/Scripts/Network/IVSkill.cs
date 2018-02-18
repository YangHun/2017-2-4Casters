using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVSkill : NetworkBehaviour {

	[SerializeField]
	IVPlayer player;

	bool isEscaped = false;

	float timer = 0.0f;

	[SerializeField]
	float lifetime = 2.0f;
	float escapetime = 0.22f;

	Dictionary<SkillType, int> force = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 0 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	public Dictionary<SkillType, int> Force
	{
		get { return force; }
		set { force = value; }
	}

	public void SetOwner(IVPlayer player)
	{
		this.player = player;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.tag == "Player")
		{
			if (collision.gameObject.name == player.name) return;
		    player.AttackPlayer(collision.gameObject.GetComponent<NetworkIdentity>(), player.id, force);
			NetworkServer.Destroy(gameObject);
		}
    }
}
