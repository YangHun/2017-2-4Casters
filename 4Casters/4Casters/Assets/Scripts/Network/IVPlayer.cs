using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVPlayer : NetworkBehaviour
{
	[SerializeField]
	public int id;
    public NetworkIdentity identity;
    public bool myPlayer= false;

	[SerializeField]
	int HP;


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

	IVArrow Arrow;			//the arrow whose parent is a player object
	[SerializeField]
	GameObject Bullet;
	const float bulletspeed = 300.0f;
    

	//called on PlayerSpawner
    //Abandoned.
	public void initializeTransform(Transform Parent, Vector3 pos)		//called at start frame to initialize player's position and parent
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
        Debug.Log("enter? "+gameObject.name);
        base.OnStartClient();
        identity = GetComponent<NetworkIdentity>();
        Arrow = transform.Find("Arrow").GetComponent<IVArrow>();
        //	Bullet = transform.Find("Bullet").gameObject;
        Bullet = Resources.Load("Prefabs/Bullet") as GameObject;
        Bullet.SetActive(false);
        
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

    }


    [Command]
    public void CmdClientReady(NetworkIdentity p)
    {

        GameObject.Find("Manager").GetComponent<IVGameManager>().RpcUpdatePlayerList(p);

    }

    // Update is called once per frame
    void Update()
	{

	}

	GameObject SpawnBasicAttack(Vector3 dir, Vector3 pos, NetworkInstanceId i)
	{
		GameObject b = Instantiate((Object)Bullet, pos, Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.transform.parent = GameObject.Find("Bullets").transform;
		b.SetActive(true);
		b.GetComponent<IVBullet>().SetOwner(ClientScene.FindLocalObject(i).GetComponent<IVPlayer>());
		b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed);
		return b;
	}

	public void BasicAttack()
	{
		Vector3 dir = Arrow.GetDir();
		Vector3 pos = transform.position;
		NetworkInstanceId i = GetComponent<NetworkIdentity>().netId;
		CmdBasicAttack(dir, pos, i);
	}

	[Command]
	public void CmdBasicAttack(Vector3 dir, Vector3 pos, NetworkInstanceId i)
	{
		RpcBasicAttack(dir, pos, i);
	}

	[ClientRpc]
	public void RpcBasicAttack(Vector3 dir, Vector3 pos, NetworkInstanceId i)
	{
		SpawnBasicAttack(dir, pos, i);
	}
	//called when this player kills a monster on Dead() in Monster component
	public void Loot(string keyword, SkillType type)
	{
		KeywordsInventory.Add(keyword);
		SkillTypeInventory[type] += 1;
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
