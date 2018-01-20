using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    [SerializeField]
    Monster Base;

    Dictionary<string, SkillType> KeywordDictionary = new Dictionary<string, SkillType>()
    {
       { "선1" , SkillType.holy },
       { "선2" , SkillType.holy },
       { "선3" , SkillType.holy },
       { "선4" , SkillType.holy },
       { "선5" , SkillType.holy },
       { "악1" , SkillType.evil },
       { "악2" , SkillType.evil },
       { "악3" , SkillType.evil },
       { "악4" , SkillType.evil },
       { "악5" , SkillType.evil },
       { "빛1" , SkillType.lightness },
       { "빛2" , SkillType.lightness },
       { "빛3" , SkillType.lightness },
       { "빛4" , SkillType.lightness },
       { "빛5" , SkillType.lightness },
       { "어둠1" , SkillType.darkness },
       { "어둠2" , SkillType.darkness },
       { "어둠3" , SkillType.darkness },
       { "어둠4" , SkillType.darkness },
       { "어둠5" , SkillType.darkness }
    };

    Dictionary<SkillType, List<string>> SkillTypeDictionary = new Dictionary<SkillType, List<string>>()
    {
        { SkillType.holy, new List<string>() { "선1", "선2", "선3","선4", "선5" }},
        { SkillType.evil, new List<string>() { "악1", "악2", "악3","악4", "악5" }},
        { SkillType.lightness, new List<string>() { "빛1", "빛2", "빛3","빛4", "빛5" }},
        { SkillType.darkness, new List<string>() { "어둠1", "어둠2", "어둠3", "어둠4", "어둠5" }}
    };

	// Use this for initialization
	void Start () {
        Base = transform.Find("Monster").GetComponent<Monster>();
        Base.gameObject.SetActive(false);
  
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Spawn(int count)
    {
        if (count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
            pos.Normalize();
            pos *= Random.Range(0.1f, 6.0f);
            pos.y = 0.5f;

            GameObject obj = Instantiate((Object)Base.gameObject, pos, Quaternion.identity) as GameObject;
            obj.transform.SetParent(transform);
            obj.SetActive(true);
            SetMonsterKeyword(obj.GetComponent<Monster>());
        }
    }

    void SetMonsterKeyword (Monster m)
    {
        List<string> keys = SkillTypeDictionary[m.Type];

        int index = Random.Range(1, keys.Count);

        m.SetKeyword(keys[index - 1]);

    }
}
