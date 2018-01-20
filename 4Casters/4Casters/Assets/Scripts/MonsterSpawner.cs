using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

    [SerializeField]
    Monster Base;

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

        }
    }
}
