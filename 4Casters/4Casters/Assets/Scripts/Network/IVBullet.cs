using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVBullet : NetworkBehaviour {

	[SerializeField]
    IVPlayer player;

    float timer = 0.0f;
    const float lifetime = 4.0f;
    // Use this for initialization
    void Start()
    {

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

	public void SetOwner(IVPlayer player)
	{
		this.player = player;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            collision.gameObject.GetComponent<IVMonster>().CmdDamaged(2, player.identity);
        }

        Destroy(this.gameObject);
    }
}
