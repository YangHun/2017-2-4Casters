using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    GameManager _manager;

    [SerializeField]
    Button Right;
    [SerializeField]
    ScrollRect CastingSentence;

    [SerializeField]
    Text[] Players;

	// Use this for initialization
	void Start () {
        _manager = GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeRightButtonText(string t)
    {
        Right.GetComponentInChildren<Text>().text = t;
    }

    public void UpdatePlayerKeywordText(int id, SkillType t, int n)
    {
        Text targetPlayer = Players[id];

        switch (t)
        {
            case SkillType.neutral:
                targetPlayer.transform.Find("Neutral").GetComponent<Text>().text = "Neutral : " + n;
                break;
            case SkillType.holy:
                targetPlayer.transform.Find("Holy").GetComponent<Text>().text = "Holy : " + n;
                break;
            case SkillType.evil:
                targetPlayer.transform.Find("Evil").GetComponent<Text>().text = "Evil : " + n;
                break;
            case SkillType.lightness:
                targetPlayer.transform.Find("Lightness").GetComponent<Text>().text = "Lightness : " + n;
                break;
            case SkillType.darkness:
                targetPlayer.transform.Find("Darkness").GetComponent<Text>().text = "Darkness : " + n;
                break;
            case SkillType.Null:
                break;
        }

    }

    public void ResetPlayerKeywordText()
    {
        foreach (Text t in Players)
        {
            t.transform.Find("Neutral").GetComponent<Text>().text = "Neutral : " + 0;
            t.transform.Find("Holy").GetComponent<Text>().text = "Holy : " + 0;
            t.transform.Find("Evil").GetComponent<Text>().text = "Evil : " + 0;
            t.transform.Find("Lightness").GetComponent<Text>().text = "Lightness : " + 0;
            t.transform.Find("Darkness").GetComponent<Text>().text = "Darkness : " + 0;
        }
    }

    public void OnClickButtonRight()
    {
        if (_manager.CurrentState == GameManager.State.MonsterPhase)
        {
            List<Player> plyrs = GetComponent<GameManager>().Players;

            foreach (Player p in plyrs)
            {
                p.BasicAttack();
            }
        }

        else if (_manager.CurrentState == GameManager.State.CastPhase)
        {
            //TODO

        }
    }

}
