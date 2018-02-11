using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVPlayer : NetworkBehaviour
{
	[SerializeField]
	public int id;
	[SyncVar]
	public string playerName;
	public NetworkIdentity identity;
	public bool myPlayer = false;

	IVHostServer _hostserver;
	float timer = 0.0f;
	const float sendRPCrate = 0.5f;

	SkillType playerType = SkillType.Null;      // Temperature assignment; Synchronizing needs.

	[SerializeField]
	[SyncVar]
	int HP;
	[SerializeField]
	Dictionary<SkillType, int> power = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 2 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};
	[SerializeField]
	Dictionary<SkillType, int> shield = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 1 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	public List<string> KeywordsInventory = new List<string>();
	public List<string> SentenceInventory = new List<string>();

	Dictionary<SkillType, int> SkillTypeInventory = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 0 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	IVArrow Arrow;          //the arrow whose parent is a player object
	[SerializeField]
	GameObject Bullet;
	const float bulletspeed = 300.0f;


	//called on PlayerSpawner
	//Abandoned.
	public void initializeTransform(Transform Parent, Vector3 pos)      //called at start frame to initialize player's position and parent
	{
		transform.SetParent(Parent);
		transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
		transform.position = pos;
	}

	//called on JoystickManager
	[Command]
	public void CmdUpdateArrow(float theta)
	{
		Arrow.CmdRotateArrow(theta);
		if (isServer)
			RpcUpdateArrow(theta);
	}

	[ClientRpc]
	public void RpcUpdateArrow(float theta)
	{
		Arrow.CmdRotateArrow(theta);
	}

	// Use this for initialization
	public override void OnStartClient()
	{
		Debug.Log("enter? " + gameObject.name);
		base.OnStartClient();

		identity = GetComponent<NetworkIdentity>();
		Arrow = transform.Find("Arrow").GetComponent<IVArrow>();
		//	Bullet = transform.Find("Bullet").gameObject;
		Bullet = Resources.Load("Prefabs/Bullet") as GameObject;
		Bullet.SetActive(false);
		gameObject.name = playerName;
	}


	// Use this for initialization; local player only
	public override void OnStartLocalPlayer()
	{
		Debug.Log("enter? " + gameObject.name);

		base.OnStartLocalPlayer();
		if (identity == null)
			identity = GetComponent<NetworkIdentity>();
		GetComponent<SpriteRenderer>().material.color = Color.blue;
		myPlayer = true;

		CmdConfigStatus(3, (SkillType)id, 0);
		CmdConfigStatus(0, playerType, 1);
		CmdConfigStatus(1, playerType, 1);
	}

	public void ConfigStatus(int code, SkillType type, int delta)
	{
		switch (code)
		{
			case 0:
				HP += delta;
				break;
			case 1:
				power[type] += delta;
				if (power[type] < 0) power[type] = 0;
				break;
			case 2:
				shield[type] += delta;
				if (shield[type] < 0) shield[type] = 0;
				break;
			case 3:
				playerType = type;
				break;
			default:
				break;
		}
	}
	[Command]
	void CmdConfigStatus(int code, SkillType type, int delta)   // 0 : HP, 1 : power, 2 : shield, 3 : playerType
	{
		ConfigStatus(code, type, delta);
	}

	private void Start()
	{



	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;

		if (_hostserver == null)
		{
			_hostserver = GameObject.Find("Host Server").GetComponent<IVHostServer>();
		}

		if (_hostserver.isLoading)
		{
			if (timer >= sendRPCrate)
			{
				if (_hostserver.playerLoading.Contains(false))
				{
					if (isServer)
						RpcClientLoading();

					return;
				}
				else
				{
					_hostserver.isLoading = false;
				}
				timer -= sendRPCrate;
			}
		}
	}

	//------------- about loading 

	[ClientRpc]
	public void RpcClientLoading()
	{
		if (_hostserver.playerLoading.Contains(false) && isLocalPlayer)
		{
			CmdClientLoading(id);
		}

		//       Debug.Log(id + " " + hasAuthority + " " + _hostserver.playerLoading.Contains(false));

	}

	[Command]
	public void CmdClientLoading(int i)
	{
		_hostserver.playerLoading[i] = true;
		RpcUpdateClientLoadingStatus(i);
	}

	[ClientRpc]
	void RpcUpdateClientLoadingStatus(int i)
	{
		_hostserver.playerLoading[i] = true;
		GameObject.Find("Manager").GetComponent<IVUIManager>().UpdateLoadingStatus(i, "Ready");
	}

	//-----------Bullet Spawn------------

	public void BasicAttack()
	{
		Vector3 dir = Arrow.GetDir();
		Vector3 pos = transform.position;
		NetworkInstanceId i = GetComponent<NetworkIdentity>().netId;
		if (isLocalPlayer)
			CmdBasicAttack(dir, pos, i);
	}

	[Command]
	public void CmdBasicAttack(Vector3 dir, Vector3 pos, NetworkInstanceId i)
	{
		GameObject b = Instantiate((Object)Bullet, pos, Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.transform.parent = GameObject.Find("Bullets").transform;
		b.SetActive(true);
		NetworkServer.Spawn(b);

		RpcBasicAttack(b, dir, pos, i);
	}

	[ClientRpc]
	public void RpcBasicAttack(GameObject b, Vector3 dir, Vector3 pos, NetworkInstanceId i)
	{
		//SpawnBasicAttack(dir, pos, i);

		b.GetComponent<IVBullet>().SetOwner(ClientScene.FindLocalObject(i).GetComponent<IVPlayer>());
		b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed);

	}

	//---------Spell Checker-------------

	public bool Cast(List<string> sentence)
	{
		if (!isLocalPlayer) return false;
		else if (!IVSpellManager.SyntaxCheck(sentence)) return false;
		else
		{
			Dictionary<SkillType, int> force = power;
			foreach (string s in sentence)
				force[IVSpellManager.KeywordDictionary[s]] += 1;
			SkillAttack(force);
			return true;
		}
	}

	//---------Skill Attack----------------

	void SkillAttack(Dictionary<SkillType, int> force)
	{
		int[] f = new int[]			//
		{
			force[SkillType.neutral],
			force[SkillType.holy],
			force[SkillType.evil],
			force[SkillType.lightness],
			force[SkillType.darkness]
		};
		CmdSkillAttack(f, id);
	}

	[Command]
	void CmdSkillAttack(int[] force, int id)
	{
		//TODO
		RpcSkillAttack(force, id);
		return;
	}

	[ClientRpc]
	void RpcSkillAttack(int[] force, int id)
	{
		//TODO
		Debug.Log("Skill attack has been casted while caster's id is " + id +
				", and its type is " + _hostserver.players[id].GetComponent<IVPlayer>().playerType.ToString());
		return;
	}
    //---------Bullet Collision-------------

    [Command]
    public void CmdAttackMonster(NetworkIdentity m, int index)
    {
        if (index == id)
        {
            bool isdead = m.GetComponent<IVMonster>().Damaged(2);
            RpcAttackMonster(m.GetComponent<IVMonster>().Keyword);
            if (isdead)
            {
                string keyword = m.gameObject.GetComponent<IVMonster>().Keyword;
                SkillType type = m.gameObject.GetComponent<IVMonster>().Type;
                NetworkServer.Destroy(m.gameObject);

                RpcMonsterDead(identity, keyword, type);
                
            }
        }
    }

    [ClientRpc]
    void RpcAttackMonster(string s)
    {
        Debug.Log(gameObject.name + " damaged " + s);
    }

    //------------Monster Kill--------------
    //ToDo

    [ClientRpc]
    void RpcMonsterDead( NetworkIdentity p, string keyword, SkillType type)
    {
        Debug.Log(gameObject.name + " killed " + keyword);
        
        Loot(p, keyword, type);
    }
    
    //called when this player kills a monster on Dead() in Monster component
    public void Loot(NetworkIdentity p, string keyword, SkillType type)
	{
        Debug.Log("Looting : ID is " + id);

        IVPlayer target = p.GetComponent<IVPlayer>();
        
        target.KeywordsInventory.Add(keyword);
        target.SkillTypeInventory[type] += 1;
		GameObject.Find("Manager").GetComponent<IVUIManager>().UpdatePlayerKeywordText(id, type, SkillTypeInventory[type]);
		GameObject.Find("Manager").GetComponent<IVUIManager>().OnClickCastingWindowFilter(id);
	}
    
	//called when phase is changed (cast --> monster)
	public void ResetPlayers()
	{
		//reset inventories
		KeywordsInventory.Clear();
		SentenceInventory.Clear();
		SkillTypeInventory[SkillType.holy] = 0;
		SkillTypeInventory[SkillType.evil] = 0;
		SkillTypeInventory[SkillType.lightness] = 0;
		SkillTypeInventory[SkillType.darkness] = 0;
	}

}
