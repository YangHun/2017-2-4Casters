using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class IVUIManager : MonoBehaviour
{

    LobbyManager _lobby;
    int playerCount;

    IVHostServer _hostserver;
    IVGameManager _game;
    IVVoiceManager _voice;

    [SerializeField]
    public Canvas _loading;
	[SerializeField]
	public Canvas _win;

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

    [SerializeField]
    List<Button> monsters = new List<Button>();

    // Use this for initialization
    void Start()
    {
        //initialization
        _hostserver = GameObject.Find("Host Server").GetComponent<IVHostServer>();
        _lobby = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        _game = GameObject.Find("Manager").GetComponent<IVGameManager>();
        _voice = GameObject.Find("Manager").GetComponent<IVVoiceManager>();
        playerCount = _hostserver.playerNum;

        MonsterButton[] ms = GameObject.Find("Canvas").GetComponentsInChildren<MonsterButton>();

        foreach (MonsterButton b in ms)
        {
            monsters.Add(b.GetComponent<Button>());
        }


        UpdatePlayerUI();
    }

    public void OnClickVoiceButton(Button b)
    {
        if (_voice.isRecording)
        {
            _voice.StopRecording();
            b.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            b.transform.Find("Image").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _voice.isRecording = false;
        }
        else
        {
            _voice.StartRecording();
            b.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            b.transform.Find("Image").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _voice.isRecording = true;
        }
    }

    public void ReMapMonsterButton(List<IVMonster> m)
    {
        foreach (Button b in monsters)
        {
            b.gameObject.SetActive(true);
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            if (i >= m.Count)
            {
                monsters[i].gameObject.SetActive(false);
            }
            else
            {
                monsters[i].GetComponentInChildren<Text>().text = m[i].Keyword;
                monsters[i].GetComponentInChildren<MonsterButton>().Monster = m[i];
                monsters[i].GetComponentInChildren<MonsterButton>().type = m[i].Type;

                monsters[i].GetComponentInChildren<MonsterButton>().keyword = m[i].Keyword;
                monsters[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            }
        }
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

	public void EndGame(string Playername)
	{
		_win.gameObject.SetActive(true);
		_win.transform.GetComponentInChildren<Text>().text = Playername + " is win!!";
		GameObject.Find("Canvas").SetActive(false);
	}

    void UpdatePlayerUI()
    {
        for (int i = playerCount; i < 4; i++)
        {
            Players[i].gameObject.SetActive(false);
            PlayerFilter[i].gameObject.SetActive(false);

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

    public void InitMonsterButtons()
    {
        if (_hostserver.CurrentState == State.MonsterPhase)
        {
            foreach (Button b in monsters)
            {
                b.GetComponent<MonsterButton>().InitMonsterPhase();
            }
        }
        else
        {
            foreach (Button b in monsters)
            {
                b.GetComponent<MonsterButton>().InitCastPhase(_game.myPlayer.GetComponent<IVPlayer>());
            }
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
            List<IVPlayer> plyrs = _game.Players;

            foreach (IVPlayer p in plyrs)
            {
                p.BasicAttack();
            }
        }

        else if (_hostserver.CurrentState == State.CastPhase)
        {
			List<string> sentence = _game.myPlayer.GetComponent<IVPlayer>().SentenceInventory;
            List<string> keyword = _game.myPlayer.GetComponent<IVPlayer>().KeywordsInventory;

			foreach (IVPlayer p in _game.Players)
			{
				try					//try while syntax is legal.
				{
					if(!p.Cast(sentence)) continue;			// Casting function by client player
					string str = "";
					foreach (string s in sentence)
					{
					    str += s;
						keyword.Remove(s);
					}
                    
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
		List<string> sentence = _game.myPlayer.GetComponent<IVPlayer>().SentenceInventory;

        Button[] buttons = CastingSentence.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();

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
