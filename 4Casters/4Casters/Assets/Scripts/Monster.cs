using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { holy, evil, lightness, darkness }

public class Monster : MonoBehaviour {

    [SerializeField]
    List<string> Keyword;
    [SerializeField]
    SkillType type;

    float timer = 0.0f;
    float resetTime;

    [SerializeField]
    Vector3 dir = Vector3.zero;
    float walkspeed = 2.0f;



	// Use this for initialization
	void Start () {
        resetTime = Random.Range(1.0f, 5.0f);
        type = (SkillType)Random.Range(0, 3);
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

    void SetColor()
    {
        Renderer r = GetComponent<Renderer>();
        switch (type)
        {
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
