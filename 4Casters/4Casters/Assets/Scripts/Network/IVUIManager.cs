using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class IVUIManager : MonoBehaviour {

    LobbyManager _lobby;
	int playerCount;

    IVHostServer _hostserver;
    IVGameManager _game;

    [SerializeField]
    public Canvas _loading;

    [SerializeField]
    Button Right;

    [SerializeField]
    GameObject CastingWindow;
    int CastingWindowFilterId = 0;

    [SerializeField]
    ScrollRect CastingSentence;
    [SerializeField]
    ScrollRect CastingKeywords;
    [SerializeField]
    Button[] PlayerFilter;

    [SerializeField]
    Text[] Players;

    // Use this for initialization
    void Start()
    {
        //initialization
        _hostserver = GameObject.Find("Host Server").GetComponent<IVHostServer>();
        _lobby = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
        _game = GameObject.Find("Manager").GetComponent<IVGameManager>();
        playerCount = _hostserver.playerNum;

        UpdatePlayerUI ();
    }

    // Update is called once per frame
    void Update()
    {

	}


    public void InitLoadingCanvas(List<string> names)
    {
        GameObject prefab = Resources.Load("Prefabs/UI/playerInfo") as GameObject;

        for (int i = 0; i < names.Count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.position = new Vector3(960f, 630f - 30 * i, 0f);
            obj.transform.SetParent(_loading.transform.Find("PlayerInfo"));
            obj.transform.Find("Name").GetComponent<Text>().text = names[i];
            obj.transform.Find("Status").GetComponent<Text>().text = "loading";
        }
    }

    public void UpdateLoadingStatus(int i, string status)
    {
        _loading.transform.Find("PlayerInfo").GetChild(i).Find("Status").GetComponent<Text>().text = status;
    }

    void UpdatePlayerUI(){
		for (int i = playerCount; i < 4; i++) {
			Players [i].gameObject.SetActive (false);
			PlayerFilter [i].gameObject.SetActive (false);

		}
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
        if (_hostserver.CurrentState == State.MonsterPhase)
        {
            foreach (IVPlayer p in _game.Players)
            {
                p.BasicAttack();
            }
        }
        else if (_hostserver.CurrentState == State.CastPhase)
        {
            List<string> sentence = _game.Players[CastingWindowFilterId].SentenceInventory;
            List<string> keyword = _game.Players[CastingWindowFilterId].KeywordsInventory;
            Button[] keywordButtons = CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();

			foreach (IVPlayer p in _game.Players)
			{
				try					//try while syntax is legal.
				{
					p.Cast(sentence);			// Casting function by client player
					string str = "";
					foreach (string s in sentence)
					{
					    str += s;
						keyword.Remove(s);
					}
					Debug.Log(str + "attack has been casted on the code UIManager while Caster's id is " + p.id);
					sentence.Clear();
					OnClickCastingWindowFilter(CastingWindowFilterId);
				}
				catch(BrokenSyntaxException e)			//catch exception when spell syntax is illegal.
				{
					Debug.Log("Spell syntax was broken on the order of " + e.Num);
				}
			}
        }
    }

    public void OnClickCastingWindowButtonExit()
    {
        CastingWindow.SetActive(false);
    }

    public void OnClickCastingWindowButtonOpen()
    {
        CastingWindow.SetActive(true);

        //TODO : Update keywords scroll contents
        CastingWindowFilterId = 0;
        OnClickCastingWindowFilter(CastingWindowFilterId);
    }

    void UpdateCastingWindowSentence()
    {
        List<string> sentence = _game.Players[CastingWindowFilterId].SentenceInventory;
        List<string> keyword = _game.Players[CastingWindowFilterId].KeywordsInventory;

        Button[] buttons = CastingSentence.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();
        Button[] keywordButtons = CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();
        /*
        foreach (string key in sentence)
        {
            int index = keyword.IndexOf(key);
            keywordButtons[index].enabled = false;
        }
		*/
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i >= sentence.Count)
            {
                buttons[i].enabled = false;

                buttons[i].GetComponentInChildren<Text>().text = "";
                buttons[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            else
            {
                buttons[i].GetComponentInChildren<Text>().text = sentence[i];

                Image img = buttons[i].GetComponent<Image>();
                Text txt = buttons[i].GetComponentInChildren<Text>();

                switch (IVSpellManager.KeywordDictionary[sentence[i]])
                {
                    case SkillType.neutral:
                        img.color = new Color(144 / 255.0f, 207 / 255.0f, 238 / 255.0f);
                        txt.color = Color.black;
                        break;
                    case SkillType.holy:
                        img.color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);
                        txt.color = Color.black;
                        break;
                    case SkillType.evil:
                        img.color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f);
                        txt.color = Color.white;
                        break;
                    case SkillType.lightness:
                        img.color = new Color(248 / 255.0f, 220 / 255.0f, 33 / 255.0f);
                        txt.color = Color.black;
                        break;
                    case SkillType.darkness:
                        img.color = new Color(47 / 255.0f, 18 / 255.0f, 49 / 255.0f);
                        txt.color = Color.white;
                        break;
                }
            }
        }

    }

    public void OnClickCastingWindowKeywordButton(Button b)
    {
        string text = b.GetComponentInChildren<Text>().text;
        if (_game.Players[CastingWindowFilterId].SentenceInventory.Contains(text))
            _game.Players[CastingWindowFilterId].SentenceInventory.Remove(text);
        else
            _game.Players[CastingWindowFilterId].SentenceInventory.Add(text);
        UpdateCastingWindowSentence();
    }

    //For debugging
    public void OnClickCastingWindowFilter(int id)
    {
        CastingWindowFilterId = id;

        for (int i = 0; i < PlayerFilter.Length; i++)
        {
            if (i == CastingWindowFilterId)
                PlayerFilter[i].GetComponent<Image>().color = Color.yellow;
            else
                PlayerFilter[i].GetComponent<Image>().color = Color.white;
        }
        List<string> keys = _game.Players[CastingWindowFilterId].KeywordsInventory;

        Button[] buttons = CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();

        //Debug.Log(keys.Count);

        if (buttons == null)
        {
            Debug.Log("null..");
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {


                if (i >= keys.Count)
                {
                    buttons[i].enabled = false;

                    buttons[i].GetComponentInChildren<Text>().text = "";

                    buttons[i].GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

                }
                else
                {

                    buttons[i].enabled = true;

                    buttons[i].GetComponentInChildren<Text>().text = keys[i];

                    Image img = buttons[i].GetComponent<Image>();
                    Text txt = buttons[i].GetComponentInChildren<Text>();

                    switch (IVSpellManager.KeywordDictionary[keys[i]])
                    {
                        case SkillType.neutral:
                            img.color = new Color(144 / 255.0f, 207 / 255.0f, 238 / 255.0f);
                            txt.color = Color.black;
                            break;
                        case SkillType.holy:
                            img.color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f);
                            txt.color = Color.black;
                            break;
                        case SkillType.evil:
                            img.color = new Color(0 / 255.0f, 0 / 255.0f, 0 / 255.0f);
                            txt.color = Color.white;
                            break;
                        case SkillType.lightness:
                            img.color = new Color(248 / 255.0f, 220 / 255.0f, 33 / 255.0f);
                            txt.color = Color.black;
                            break;
                        case SkillType.darkness:
                            img.color = new Color(47 / 255.0f, 18 / 255.0f, 49 / 255.0f);
                            txt.color = Color.white;
                            break;
                    }
                }
            }
        }

        UpdateCastingWindowSentence();

    }

    //make 'public' to be able to be called in the phase manager
    //refresh UI when the phase is changed
    public void RefreshInMonsterPhase()
    {
        foreach (Button b in CastingSentence.transform.Find("Viewport/Content").GetComponentsInChildren<Button>())
            b.enabled = false;
        foreach (Button b in CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>())
            b.enabled = false;
    }

    public void RefreshInCastPhase()
    {
        foreach (Button b in CastingSentence.transform.Find("Viewport/Content").GetComponentsInChildren<Button>())
            b.enabled = true;
        foreach (Button b in CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>())
            b.enabled = true;
    }

}
