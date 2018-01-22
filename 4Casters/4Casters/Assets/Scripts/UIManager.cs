using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    GameManager _manager;
    SpellManager _spell;

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
        _manager = GetComponent<GameManager>();
        _spell = GetComponent<SpellManager>();
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
        List<string> sentence = _manager.Players[CastingWindowFilterId].SentenceInventory;
        List<string> keyword = _manager.Players[CastingWindowFilterId].KeywordsInventory;


        Button[] buttons = CastingSentence.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();
        Button[] keywordButtons = CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();

        foreach (string key in sentence)
        {
            int index = keyword.IndexOf(key);
            keywordButtons[index].enabled = false;
        }

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

                switch (_spell.KeywordDictionary[sentence[i]])
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
        _manager.Players[CastingWindowFilterId].SentenceInventory.Add(text);
        b.enabled = false;
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
        List<string> keys = _manager.Players[CastingWindowFilterId].KeywordsInventory;
       
        Button[] buttons = CastingKeywords.transform.Find("Viewport/Content").GetComponentsInChildren<Button>();

        Debug.Log(keys.Count);

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
                else {

                    buttons[i].enabled = true;

                    buttons[i].GetComponentInChildren<Text>().text = keys[i];

                    Image img = buttons[i].GetComponent<Image>();
                    Text txt = buttons[i].GetComponentInChildren<Text>();

                    switch (_spell.KeywordDictionary[keys[i]]){
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

   

}
