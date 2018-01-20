using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    [SerializeField]
    List<string> Keyword;

    float timer = 0.0f;
    const float resetTime = 3.0f;

    [SerializeField]
    Vector3 dir = Vector3.zero;
    float walkspeed = 2.0f;

	// Use this for initialization
	void Start () {
		
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
}
