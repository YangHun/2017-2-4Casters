using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVArrow : MonoBehaviour {

	public float theta;

	// Use this for initialization
	void Start () {
		//			NetworkServer.Spawn(gameObject);
	}


	public void CmdRotateArrow(float theta)
	{
		transform.rotation = Quaternion.Euler(90, 0, theta);
		this.theta = theta;
	}

	public Vector3 GetDir()			//need to be improved
	{
		return transform.up;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
