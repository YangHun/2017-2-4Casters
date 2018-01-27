using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster : MonoBehaviour {

    [SerializeField]
    string Keyword = "";
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
    int HP = 10;

	// Use this for initialization
	void Start () {
        resetTime = Random.Range(1.0f, 5.0f);
        SetColor();
	}
	
	// Update is called once per frame
	void Update () {

        if (timer >= resetTime || dir == Vector3.zero)
        {
            timer = 0.0f;
            dir = new Vector3 ( Random.Range(-1.0f, 1.0f) , 0.0f, Random.Range(-1.0f, 1.0f));
            dir.Normalize();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(dir * walkspeed);
        }

        timer += Time.deltaTime;

	}

    public void Damaged (int damage, Player p)
    {
        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            Dead(p);
        }
    }

    public void Dead(Player p)
    {
        Debug.Log(gameObject.name + " is dead! (type,key) = (" + type + ", " + Keyword + ")");
        Debug.Log(p.gameObject.name + " killed "+gameObject.name);
        p.Loot(Keyword, type);
        gameObject.SetActive(false);
    }

    public void Initialization (string key, SkillType t)
    {
        if (Keyword == "")
            Keyword = key;

        if (type == SkillType.Null && t != SkillType.Null)
            type = t;

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
