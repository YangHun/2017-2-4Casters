using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    [SerializeField]
    Monster Base;

    [SerializeField]
    List<int> spawnCount = new List<int>();
    // neutral - holy - evil - lightness - darkness


    Dictionary<string, SkillType> KeywordDictionary;
    Dictionary<SkillType, List<string>> SkillTypeDictionary;

    // Use this for initialization
    void Start () {
        Base = transform.Find("Single Monster").GetComponent<Monster>();
        Base.gameObject.SetActive(false);

        SpellManager _spell = GameObject.Find("Manager").GetComponent<SpellManager>();
        KeywordDictionary = _spell.KeywordDictionary;
        SkillTypeDictionary = _spell.SkillTypeDictionary;


    }

	// Update is called once per frame
	void Update () {
		
	}

    public void Spawn()
    {
        for (int i = 0; i < spawnCount.Count; i++)
        {
            for (int j = 0; j < spawnCount[i]; j++)
            {

                Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
                pos.Normalize();
                pos *= Random.Range(0.1f, 6.0f);
                pos.y = 0.5f;

                GameObject obj = Instantiate((Object)Base.gameObject, pos, Quaternion.identity) as GameObject;
                obj.transform.SetParent(transform);
                obj.SetActive(true);

                SkillType type = (SkillType)i;

                List<string> keys = SkillTypeDictionary[type];
                string keyword = keys[Random.Range(1, keys.Count) - 1];

                obj.GetComponent<Monster>().Initialization(keyword, type);

            }
        }
    }

    public void Release()
    {
        Monster[] monsters = transform.GetComponentsInChildren<Monster>();

        foreach ( Monster m in monsters)
        {
            Destroy(m.gameObject);
        }

    }
}
