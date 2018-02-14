using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This bullet class works as two types.
// Bullet 1: Monster phase bullet; has no force and normal speed, targets a monster.
// Bullet 2: Cast phase bullet; has force, targets another player.
public class IVBullet : NetworkBehaviour {

	[SerializeField]
    IVPlayer player;

	State state = State.Null;
	Dictionary<SkillType, int> force; 

    float timer = 0.0f;
    const float lifetime = 4.0f;

	public State State
	{
		get
		{
			return state;
		}
	}

	public Dictionary<SkillType, int> Force
	{
		get
		{
			if (State == State.CastPhase)
				return force;
			else
				Debug.Log("Wrong reference of bullet force.");
			return new Dictionary<SkillType, int>();
		}

		set
		{
			if (State == State.CastPhase)
				force = value;
		}
	}

	// Use this for initialization
	void Start()
    {
		state =
			GameObject.Find("Host Server").GetComponent<IVHostServer>().CurrentState;
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
    }
	
	public void SetOwner(IVPlayer player)
	{
		this.player = player;
	}

    private void OnCollisionEnter(Collision collision)
    {
		if(state == State.MonsterPhase)
		{
			if (collision.gameObject.tag == "Monster")
			{
			    Debug.Log("enter");
			    player.CmdAttackMonster(collision.gameObject.GetComponent<NetworkIdentity>(), player.id);
				NetworkServer.Destroy(gameObject);
			}

		}
		else if (state == State.CastPhase)
		{
			if (collision.gameObject.tag == "Player")
			{
				Debug.Log("Player attacked");
				//int damage = IVSpellManager.DamageCalculator(force, collision.gameObject.GetComponent<IVPlayer>().Shield);
				int damage = 100000;		// For Debug
				player.CmdAttackPlayer(collision.gameObject.GetComponent<NetworkIdentity>(), damage);
			}
		}
		else
			Debug.Log("The condition of state of bullet is null. Please see the code.");
    }
}
