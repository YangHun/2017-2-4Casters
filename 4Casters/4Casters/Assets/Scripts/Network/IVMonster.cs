using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVMonster : NetworkBehaviour {
    [SerializeField]
    public string Keyword = "";
    TextMesh mesh;

    [SerializeField]
    SkillType type = SkillType.Null;
    public SkillType Type
    {
        get { return type; }
    }


    float timer = 0.0f;
    float resetTime;

    [SerializeField]
    Vector3 dir = Vector3.zero;
    float walkspeed = 2.0f;

    [SerializeField]
    [SyncVar]
    int HP = 10;

    // Use this for initialization
    public override void OnStartClient()
    {
      
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;

        if (timer >= resetTime || dir == Vector3.zero)
        {
            timer = 0.0f;
            dir = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            dir.Normalize();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            //GetComponent<Rigidbody>().AddForce(dir * walkspeed);
            CmdUpdateMonsterDirection(dir * walkspeed);
        }

        timer += Time.deltaTime;

    }

    [Command]
    void CmdUpdateMonsterDirection(Vector3 dir)
    {
        RpcUpdateMonsterDirection(dir);

    }

    [ClientRpc]
    void RpcUpdateMonsterDirection (Vector3 dir)
    {
        Rigidbody r = GetComponent<Rigidbody>();
        r.velocity = Vector3.zero;
        r.AddForce(dir * walkspeed);
    }



    public bool Damaged(int damage)
    {

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    [Command]
    void CmdDead(NetworkIdentity p)
    {
        RpcDead(p);
    }

    [ClientRpc]
    public void RpcDead(NetworkIdentity p)
    {
        //Debug.Log(gameObject.name + " is dead! (type,key) = (" + type + ", " + Keyword + ")");
        Debug.Log(p.gameObject.name + " killed " + gameObject.name);
        if (p.GetComponent<IVPlayer>() == null)
            return;
        p.GetComponent<IVPlayer>().Loot(Keyword, type);
        NetworkServer.UnSpawn(this.gameObject);
        gameObject.SetActive(false);
    }

    public void Initialization(string key, SkillType t)
    {
        transform.SetParent(GameObject.Find("Monster").transform);

        if (Keyword == "")
            Keyword = key;

        if (type == SkillType.Null && t != SkillType.Null)
            type = t;

        resetTime = Random.Range(1.0f, 5.0f);
        SetColor();

        if (mesh == null)
            mesh = transform.Find("Text").GetComponent<TextMesh>();

        if (mesh != null)
            mesh.text = key;

    }

    void SetColor()
    {
        Renderer r = GetComponent<Renderer>();
        switch (type)
        {
            case SkillType.neutral:
                r.material = Resources.Load("Material/Monster-neutral", typeof(Material)) as Material;
                break;
            case SkillType.darkness:
                r.material = Resources.Load("Material/Monster-darkness", typeof(Material)) as Material;
                break;
            case SkillType.evil:
                r.material = Resources.Load("Material/Monster-evil", typeof(Material)) as Material;
                break;
            case SkillType.holy:
                r.material = Resources.Load("Material/Monster-holy", typeof(Material)) as Material;
                break;
            case SkillType.lightness:
                r.material = Resources.Load("Material/Monster-lightness", typeof(Material)) as Material;
                break;
        }
    }
}
