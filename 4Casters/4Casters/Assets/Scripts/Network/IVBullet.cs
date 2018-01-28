using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVBullet : NetworkBehaviour {
    IVPlayer player;

    float timer = 0.0f;
    const float lifetime = 4.0f;
    // Use this for initialization
    void Start()
    {
        player = transform.parent.gameObject.GetComponent<IVPlayer>();		//An error occur
    }

    // Update is called once per frame
    void Update()
    {

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
        if (collision.gameObject.tag == "Monster")
        {
            collision.gameObject.GetComponent<IVMonster>().Damaged(2, player);
        }

        Destroy(this.gameObject);
    }
}
