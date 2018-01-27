using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVPlayer : NetworkBehaviour
{
	[SerializeField]
	int id;

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

	Transform Arrow;			//the arrow whose parent is a player object
	[SerializeField]
	GameObject Bullet;
	const float bulletspeed = 300.0f;

	//called on PlayerSpawner
	public void InitializeTransform(Transform Parent, Vector3 pos)		//called at start frame to initialize player's position and parent
	{
		transform.SetParent(Parent);
		transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
		transform.position = pos;
		Debug.Log(this.name + "is initialized.");
	}

	//called on JoystickManager
	public void UpdateArrow(float theta)
	{
		if (!isLocalPlayer) return;             //If not a local player, it halts
		else
			Arrow.transform.rotation = Quaternion.Euler(90, 0, theta);
	}

	// Use this for initialization
	void Start()
	{
		Arrow = transform.Find("Arrow");
		Bullet = transform.Find("Bullet").gameObject;
		Bullet.SetActive(false);
	}

	// Use this for initialization; local player only
	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		GetComponent<SpriteRenderer>().material.color = Color.blue;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void BasicAttack()
	{
		Vector3 dir = Arrow.up;
		Vector3 pos = Arrow.position;

		if (!isLocalPlayer) return;

		GameObject b = Instantiate((Object)Bullet, pos, Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
		b.transform.parent = transform;
		b.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
		b.SetActive(true);

		b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed);

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
