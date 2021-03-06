﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVSkill : NetworkBehaviour {

	[SerializeField]
	IVPlayer player;

	bool isEscaped = false;
	[SerializeField]
	float timer = 0.0f;

	enum _Type { BulletAttack , LaserAttack , ReflectionAttack , Buff , Debuff};
	[SerializeField]
	_Type type;


	[SerializeField]
	float lifetime;
	[SerializeField]
	float escapetime;

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

	public static SortedDictionary<SkillType, int> DicToSort(Dictionary<SkillType, int> origin)
	{
		return new SortedDictionary<SkillType, int>(origin);
	}
	public static KeyValuePair<SkillType[], int[]> DicToPair(Dictionary<SkillType, int> origin)
	{
		int len = (int)SkillType.Null + 1;
		SkillType[] s = new SkillType[len];
		int[] i = new int[len];
		SortedDictionary<SkillType, int> sort = new SortedDictionary<SkillType, int>(origin);
		origin.Keys.CopyTo(s, 0);
		origin.Values.CopyTo(i, 0);
		return new KeyValuePair<SkillType[], int[]>(s, i);
	}

	public static Dictionary<SkillType, int> PairToDic(SkillType[] key, int[] value)
	{
		Dictionary<SkillType, int> force = new Dictionary<SkillType, int>();
		for (int i = 0; i < key.Length; i++) 
			force[key[i]] = value[i];
		return force;
	}

	public static Dictionary<SkillType, int> ListToDic(List<SkillType> forcekey, List<int> forcevalue)
	{
		Dictionary<SkillType, int> force = new Dictionary<SkillType, int>();
		for(int i =0; i<forcekey.Count; i++)
			force[forcekey[i]] = forcevalue[i];
		return force;
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
            foreach (ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>())
            {
                p.Stop();
            }
            Destroy(gameObject);
            

            //transform.DetachChildren();
        }

		if (type == _Type.BulletAttack && !isEscaped && timer >= escapetime)
		{
			GetComponent<SphereCollider>().enabled = true;
			isEscaped = true;
		}

		if (type == _Type.Buff && !isEscaped && timer >= escapetime)
		{
			player.AddBuff(force);
		}

	}

	private void OnDestroy()
	{
		if (!isServer) return;
		if (type == _Type.Buff)
			player.AddBuff(force);
		else if (type == _Type.Debuff)
			player.AddDebuff(force);
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
