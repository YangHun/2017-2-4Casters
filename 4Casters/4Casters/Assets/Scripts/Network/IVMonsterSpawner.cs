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


    // Use this for initialization
    void Start()
    {
		Base = Resources.Load ("Prefabs/Monster") as GameObject;
//			transform.Find("Monster").GetComponent<IVMonster>();
//        Base.gameObject.SetActive(false);

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

    }



	[ClientRpc]
	void RpcMonsterSpawnInit(GameObject obj, SkillType t, string k){
		Debug.Log ("Like this?");

        isClientSpawned = true;
        IVMonster m = obj.GetComponent<IVMonster>();

        if (m == null)
        {
            Debug.Log(obj + " does not have monster component");
            return;
        }

        m.Initialization(k, t);

    }

    public void Spawn()
    {
	
        if (!isServer)
			return;

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

                NetworkServer.Spawn(obj);

                SkillType type = (SkillType)i;

                List<string> keys = IVSpellManager.SkillTypeDictionary[type];
                string keyword = keys[Random.Range(1, keys.Count) - 1];
                
                RpcMonsterSpawnInit(obj, type, keyword);
                
            }
        }

        
    }

    public void Release()
    {
        Debug.Log("enter?");

        if (!isServer)
            return;

        IVMonster[] monsters = GameObject.Find("Monster").GetComponentsInChildren<IVMonster>();

        foreach (IVMonster m in monsters)
        {
            Destroy(m.gameObject);
            NetworkServer.Destroy(m.gameObject);
            //RpcRelease(m.gameObject);
        }
        
    }



    void RpcRelease(GameObject m)
    {
        Destroy(m);
    }

}
