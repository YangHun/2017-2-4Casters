using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    int HP;
    List<string> KeywordsInventory = new List<string>();
    Transform Arrow;
    [SerializeField]
    GameObject Bullet;
	JoystickManager Joystick;
    const float bulletspeed = 300.0f;

	public void UpdateArrow(float theta)
	{
		Arrow.transform.rotation = Quaternion.Euler(90, 0, theta);
	}
	/*
	public void EndUpdateArrow()
	{
		Arrow.transform.rotation = Quaternion.Euler(90, 0, 0);
	}
	*/
	// Use this for initialization
	void Start () {
        Arrow = transform.Find("Arrow");
        Bullet = transform.Find("Bullet").gameObject;
        Bullet.SetActive(false);
		Joystick = GameObject.Find("JoystickSystem").GetComponent<JoystickManager>();
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate(new Vector3(0, 0, Joystick.theta));

	}

    public void BasicAttack()
    {
        Vector3 dir = Arrow.up;
        Vector3 pos = Arrow.position;

        GameObject b = Instantiate((Object)Bullet, pos, Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f))) as GameObject;
        b.transform.parent = transform;
        b.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        b.SetActive(true);

        b.GetComponent<Rigidbody>().AddForce(dir * bulletspeed);

    }
    
}
