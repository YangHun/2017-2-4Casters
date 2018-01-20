using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickButtonRight()
    {
        List<Player> plyrs = GetComponent<GameManager>().Players;

        foreach (Player p in plyrs)
        {
            p.BasicAttack();
        }

    }

}
