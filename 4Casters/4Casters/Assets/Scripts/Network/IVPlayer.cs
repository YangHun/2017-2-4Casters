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

    [SerializeField]
    GameObject models;

	IVHostServer _hostserver;
	float timer = 0.0f;
	const float sendRPCrate = 0.5f;

    [SerializeField]
	SkillType playerType = SkillType.holy;      // Temperature assignment; Synchronizing needs.
    [SyncVar]
    public Color playerColor = Color.white;

	[SerializeField]
	[SyncVar]
	int HP;

	Dictionary<SkillType, int> power = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 2 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	Dictionary<SkillType, int> buff = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 0 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	Dictionary<SkillType, int> shield = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 1 },
		{ SkillType.holy, 0 },
		{ SkillType.evil, 0 },
		{ SkillType.lightness, 0 },
		{ SkillType.darkness, 0 },
	};

	Dictionary<SkillType, int> debuff = new Dictionary<SkillType, int>()
	{
		{ SkillType.neutral, 0 },
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
	GameObject Bullet;
	GameObject SkillBullet;
	GameObject SkillBuff;
	GameObject SkillDebuff;
	const float bulletspeed = 300.0f;

	public Dictionary<SkillType, int> Shield
	{
		get
		{
			return shield;
		}
	}


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

    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    
    void SetType(Color c)
    {
        for (int i = 0; i < models.transform.childCount; i++)
        {
            models.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (c == Color.yellow)
        {
            playerType = SkillType.lightness;
            models.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (c == Color.grey)
        {
            playerType = SkillType.darkness;
            models.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (c == Color.white)
        {
            playerType = SkillType.holy;
            models.transform.GetChild(3).gameObject.SetActive(true);
        }
        else if (c == Color.black)
        {
            playerType = SkillType.evil;
            models.transform.GetChild(2).gameObject.SetActive(true);
        }
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
		SkillBullet = Resources.Load("Prefabs/SkillBullet") as GameObject;
		SkillBuff = Resources.Load("Prefabs/SkillBuff") as GameObject;
		SkillDebuff = Resources.Load("Prefabs/SkillDebuff") as GameObject;
		Bullet.SetActive(false);
		SkillBullet.SetActive(false);
		SkillBuff.SetActive(false);
		gameObject.name = playerName;
        SetType(playerColor);
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

		CmdConfigStatus(1, playerType, 1);
		CmdConfigStatus(2, playerType, 1);
		CmdConfigStatus(3, (SkillType)id, 0);
        
    }


	Dictionary<SkillType, int> Calculate(bool isPower)				// Calculate force of shield related to buff and debuff.
	{
		Dictionary<SkillType, int> ans = new Dictionary<SkillType, int>();
		if(isPower)
		{
			foreach(SkillType type in new List<SkillType>(power.Keys))
			{
				int part = power[type] + buff[type];
				ans[type] = part > 0 ? part : 0;
			}
			return ans;
		}
		else
		{
			foreach(SkillType type in new List<SkillType>(shield.Keys))
			{
				int part = shield[type] - debuff[type];
				ans[type] = part > 0 ? part : 0;
			}
			return ans;
		}
	}
	// 0 : HP, 1 : power, 2 : shield, 3 : playerType, 4 : buff, 5 : debuff
	void ConfigStatus(int code, SkillType type, int delta)
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
			case 4:
				buff[type] += delta;
				break;
			case 5:
				debuff[type] += delta;
				break;
			default:
				break;
		}
	}

	[Command]
	void CmdConfigStatus(int code, SkillType type, int delta)
	{
		RpcConfigStatus(code, type, delta);
	}

	[ClientRpc]
	void RpcConfigStatus(int code, SkillType type, int delta)
	{
		ConfigStatus(code, type, delta);
	}

	public void AddBuff(Dictionary<SkillType, int> delta)
	{
		foreach(SkillType type in new List<SkillType>(delta.Keys))
			CmdConfigStatus(4, type, delta[type]);
	}

	public void AddDebuff(Dictionary<SkillType, int> delta)
	{
		foreach(SkillType type in new List<SkillType>(delta.Keys))
			CmdConfigStatus(5, type, delta[type]);
	}

	// Dead TODO
	void Dead()
	{
		if(HP > 0)
		{
			Debug.Log("Player with id " + id + " is not dead but dead script has been executed");
			return;
		}
		else
		{
			Debug.Log("Player with id " + id + " is dead");
			Destroy(gameObject);
		}
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
            if (GameObject.Find("Host Server") != null)
            _hostserver = GameObject.Find("Host Server").GetComponent<IVHostServer>();
        }

        else if (_hostserver.isLoading)
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
        if (_hostserver == null)
        {
            _hostserver = GameObject.Find("Host Server").GetComponent<IVHostServer>();
        }

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
		b.GetComponent<IVBullet>().SetOwner(ClientScene.FindLocalObject(i).GetComponent<IVPlayer>());
		b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed);

	}

	//---------Cast Section-------------

	public bool Cast(List<string> sentence)
	{
		if (!isLocalPlayer) return false;
		else
		{
			int type = IVSpellManager.SyntaxCheck(sentence);
			if (type == 0) return false;
			Dictionary<SkillType, int> force;
			switch (type)
			{
				case 1:
					force = IVSpellManager.ForceCalculator(sentence, power);
					SkillAttack(force);
					break;
				case 2:
					force = IVSpellManager.ForceCalculator(sentence, new Dictionary<SkillType, int>(IVSpellManager.emptyforce));
					CastBuff(force);
					break;
				case 0:
				case 3:
					force = IVSpellManager.ForceCalculator(sentence, new Dictionary<SkillType, int>(IVSpellManager.emptyforce));
					CastDebuff(force);
					break;
					
			}
			return true;
		}
	}

	//---------Skill Attack----------------

	void SkillAttack(Dictionary<SkillType, int> force)
	{
		Vector3 dir = Arrow.GetDir();
		Vector3 pos = transform.position;
		NetworkInstanceId i = GetComponent<NetworkIdentity>().netId;
		// It ensures this code will be executed on client
		KeyValuePair<SkillType[], int[]> Sforce = IVSkill.DicToPair(force);
		CmdSkillAttack(dir, pos, i, Sforce.Key, Sforce.Value);
	}

	[Command]
	void CmdSkillAttack(Vector3 dir, Vector3 pos, NetworkInstanceId id, SkillType[] key, int[] value)
	{
		GameObject b = Instantiate((Object)SkillBullet, pos, Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.GetComponent<IVSkill>().Force = IVSkill.PairToDic(key, value);
		b.transform.parent = GameObject.Find("Skills").transform;
		b.SetActive(true);
		NetworkServer.Spawn(b);

		RpcSkillAttack(b, dir, pos, id);
		return;
	}

	[ClientRpc]
	void RpcSkillAttack(GameObject b, Vector3 dir, Vector3 pos, NetworkInstanceId id)
	{
		//TODO
		Debug.Log("Skill attack has been casted on the code Player while caster's id is " + id +
				", and its type is " + ClientScene.FindLocalObject(id).GetComponent<IVPlayer>().playerType.ToString());
		b.GetComponent<IVSkill>().SetOwner(ClientScene.FindLocalObject(id).GetComponent<IVPlayer>());

        b.transform.LookAt(b.transform.position + dir);

        ParticleSystem[] ps = b.transform.GetChild(0).GetComponentsInChildren<ParticleSystem>();

        for (int i = 1; i < ps.Length; i++)
        {
            ParticleSystem.EmissionModule em = ps[i].emission;
            em.rateOverTime = b.GetComponent<IVSkill>().Force[(SkillType)i];

        }

		b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed * 3);
		return;
	}

	//------------- Buff and Debuff ----------------
	// On this section, Debuff is called 'CastedBuff'.

	void CastBuff(Dictionary<SkillType, int> force)
	{
		NetworkInstanceId id = GetComponent<NetworkIdentity>().netId;
		KeyValuePair<SkillType[], int[]> Sforce = IVSkill.DicToPair(force);

		CmdCastBuff(id, Sforce.Key, Sforce.Value);
	}

	void CastDebuff(Dictionary<SkillType, int> force)		// cast player --> casted player
	{
		IVPlayer target = _hostserver.GetNearestPlayer(transform.position, Arrow.theta);
		KeyValuePair<SkillType[], int[]> Sforce = IVSkill.DicToPair(force);

		CmdCastedBuff(target.GetComponent<NetworkIdentity>().netId, Sforce.Key, Sforce.Value);
	}

	[Command]
	void CmdCastBuff(NetworkInstanceId id, SkillType[] forcekey, int[] forcevalue)
	{
		GameObject b = Instantiate((Object)SkillDebuff, new Vector3(0,0,0), Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.GetComponent<IVSkill>().Force = IVSkill.PairToDic(forcekey, forcevalue);
		b.transform.parent = GameObject.Find("Skills").transform;
		b.SetActive(true);
		
		NetworkServer.Spawn(b);

		RpcCastBuff(b, id);
	}

	[Command]
	void CmdCastedBuff(NetworkInstanceId id, SkillType[] forcekey, int[] forcevalue)
	{
		GameObject b = Instantiate((Object)SkillDebuff, new Vector3(0,0,0), Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.GetComponent<IVSkill>().Force = IVSkill.PairToDic(forcekey, forcevalue);
		b.transform.parent = GameObject.Find("Skills").transform;
		b.SetActive(true);
		
		NetworkServer.Spawn(b);

		RpcCastedBuff(b, id);
	}

	[ClientRpc]
	void RpcCastBuff(GameObject b, NetworkInstanceId id)
	{
		Debug.Log(ClientScene.FindLocalObject(id).name + " has been buffed.");
		b.GetComponent<IVSkill>().SetOwner(ClientScene.FindLocalObject(id).GetComponent<IVPlayer>());
	}

	[ClientRpc]
	void RpcCastedBuff(GameObject b, NetworkInstanceId id)
	{
		Debug.Log(ClientScene.FindLocalObject(id).name + " has been debuffed.");
		b.GetComponent<IVSkill>().SetOwner(ClientScene.FindLocalObject(id).GetComponent<IVPlayer>());
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

	public void AttackPlayer(NetworkIdentity id, int index, Dictionary<SkillType, int> force)
	{
		if (!isServer) return;
		else if (index == this.id)
		{
			IVPlayer p = id.gameObject.GetComponent<IVPlayer>();
			int dmg = IVSpellManager.DamageCalculator(Calculate(true), p.Calculate(false));
			RpcAttackPlayer(id, index, dmg);
		}
	}

	[ClientRpc]
	void RpcAttackPlayer(NetworkIdentity id, int index, int dmg)
	{
		IVPlayer p = id.gameObject.GetComponent<IVPlayer>();
		p.ConfigStatus(0, SkillType.Null, -dmg);
		Debug.Log("Player" + id + "has attacked player with id " + p.id + "; damage is " + dmg);
		if (p.HP <= 0)
		{
			Debug.Log("Player" + id + "has killed player with id " + p.id);
			p.Dead();
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
    
    //------------Player Kill--------------
    //ToDo

	[ClientRpc]
	void RpcPlayerDead(NetworkIdentity dead, NetworkIdentity killer)
	{
		Debug.Log(killer.gameObject.name + " is dead by " + killer.gameObject.name);

		dead.gameObject.GetComponent<IVPlayer>().Dead();
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
