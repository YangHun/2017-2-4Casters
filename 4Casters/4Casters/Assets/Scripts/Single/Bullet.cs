using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Player player;

    float timer = 0.0f;
    const float lifetime = 4.0f;
	// Use this for initialization
	void Start () {
        player = transform.parent.gameObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {

        if (this.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
        }

        if (timer >= lifetime)
        {
            Destroy(this.gameObject);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster" )
        {
            collision.gameObject.GetComponent<Monster>().Damaged(2, player);
        }

        Destroy(this.gameObject);
    }
}
