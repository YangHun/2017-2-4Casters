using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVMonsterSpawner : NetworkBehaviour {

    IVGameManager _game;

    [SerializeField]
    GameObject Base;

    [SerializeField]
    List<int> spawnCount = new List<int>();
    // neutral - holy - evil - lightness - darkness

    bool isClientSpawned = false;

    Dictionary<string, SkillType> KeywordDictionary;
    Dictionary<SkillType, List<string>> SkillTypeDictionary;

    // Use this for initialization
    void Start()
    {
		Base = Resources.Load ("Prefabs/Monster") as GameObject;
//			transform.Find("Monster").GetComponent<IVMonster>();
//        Base.gameObject.SetActive(false);

        IVSpellManager _spell = GameObject.Find("Manager").GetComponent<IVSpellManager>();
        KeywordDictionary = _spell.KeywordDictionary;
        SkillTypeDictionary = _spell.SkillTypeDictionary;
        _game = GameObject.Find("Manager").GetComponent<IVGameManager>();


    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Base = Resources.Load("Prefabs/Monster") as GameObject;
        if (Base != null)
        {
            ClientScene.RegisterPrefab(Base);
        }
        else
            Debug.Log("base monster prefab is null");


    }

    // Update is called once per frame
    void Update()
    {
        if (!isClientSpawned)
        {
            
        }
    }


    public void SetClientSpawnedFalse()
    {
        isClientSpawned = false;
    }

    [Command]
    void CmdReadyToSpawnMonster(NetworkIdentity p)
    {
        Debug.Log("Player " + p.netId + " is ready!");
        //RpcMonsterSpawnInit();
    }
/*
    [ClientRpc]
    void RpcReadyToSpawnMonster()
    {
        CmdReadyToSpawnMonster();
        isClientSpawned = true;
    }
*/
	[ClientRpc]
	void RpcMonsterSpawnInit(){
		Debug.Log ("Like this?");

        isClientSpawned = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
        if (player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        for (int i = 0; i < spawnCount.Count; i++)
        {
            for (int j = 0; j < spawnCount[i]; j++)
            {

                Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
                pos.Normalize();
                pos *= Random.Range(0.1f, 6.0f);
                pos.y = 0.5f;

                GameObject obj = Instantiate(Base, pos, Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
                obj.SetActive(true);

                SkillType type = (SkillType)i;

                List<string> keys = SkillTypeDictionary[type];
                string keyword = keys[Random.Range(1, keys.Count) - 1];

                //obj.GetComponent<IVMonster>().Initialization(keyword, type);
                
                NetworkServer.SpawnWithClientAuthority(obj, player);

                IVMonster m = obj.GetComponent<IVMonster>();

                if (m == null)
                {
                    Debug.Log(obj + " does not have monster component");
                    return;
                }

                m.Initialization(keyword, type);


            }
        }
	}

    public void Spawn()
    {
	
        if (!isServer)
			return;
        /*
        while (!isClientSpawned)
        {
            Debug.Log("enter?");
            RpcReadyToSpawnMonster();
        }
        */
    }

    public void Release()
    {
		IVMonster[] monsters = transform.GetComponentsInChildren<IVMonster>();

        foreach (IVMonster m in monsters)
        {
            Destroy(m.gameObject);
        }

    }
}
