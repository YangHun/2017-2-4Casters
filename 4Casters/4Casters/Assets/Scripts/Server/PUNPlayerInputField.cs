using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PUNPlayerInputField : MonoBehaviour {

    static string playerNamePrefKey = "PlayerName";

	// Use this for initialization
	void Start () {

        string defaultName = "";
        InputField _field = this.GetComponent<InputField>();
        if (_field != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _field.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;

	}

    public void SetPlayerName(string val)
    {
        PhotonNetwork.playerName = val + " ";
        PlayerPrefs.SetString(playerNamePrefKey, val);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
