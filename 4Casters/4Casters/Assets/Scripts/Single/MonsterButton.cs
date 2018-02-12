using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterButton : MonoBehaviour {

    RectTransform rect;
    Vector3 dir = Vector3.zero;

    public IVMonster Monster;
    public SkillType type;
    public string keyword;

    List<Vector3> dirs = new List<Vector3>();
    int index = 0;

    float movespeed = 1f;
    float updatespeed;
    float timer = 0.0f;
    public bool isSelected = false;

    // Use this for initialization
    void Start () {
        rect = GetComponent<RectTransform>();

        updatespeed = Random.Range(0.2f, 0.4f);

        for (int i = 0; i < 5; i++)
        {
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            dirs.Add(dir.normalized);

        }

        for (int i = 0; i < 5; i++)
        {
            dirs.Add(dirs[i] * (-1.0f));
        }

    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if (timer < updatespeed)
        {
            rect.position = Vector3.Lerp(rect.position, rect.position + dirs[index % 10] * movespeed, timer);

        }
        else
        {

            index++;
            timer -= updatespeed;
        }

        if(index == 100)
        {
            index = 0;
        }

       // Debug.Log(rect.position);
        
	}

    public void InitCastPhase(IVPlayer p)
    {
        Color c = Color.white;

        switch (type)
        {
            case SkillType.neutral:
                c = (Resources.Load("Material/Monster-neutral", typeof(Material)) as Material).color;
                break;
            case SkillType.darkness:
                c = (Resources.Load("Material/Monster-darkness", typeof(Material)) as Material).color;
                break;
            case SkillType.evil:
                c = (Resources.Load("Material/Monster-evil", typeof(Material)) as Material).color;
                break;
            case SkillType.holy:
                c = (Resources.Load("Material/Monster-holy", typeof(Material)) as Material).color;
                break;
            case SkillType.lightness:
                c = (Resources.Load("Material/Monster-lightness", typeof(Material)) as Material).color;
                break;
        }

        GetComponent<Image>().color = c;

        if (!p.KeywordsInventory.Contains(keyword))
        {
            gameObject.SetActive(false);
        }
        if (isSelected)
        {
            isSelected = false;
        }
    }

    public void InitMonsterPhase()
    {

        GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.2f);

        gameObject.SetActive(true);
        isSelected = false;
    }

    public void OnClickButton()
    {
        Debug.Log(Monster.Keyword);

        if ( Monster != null)
        {
            if (!isSelected)
            {
                SkillType type = Monster.Type;
                Color c = Color.white;

                switch (type)
                {
                    case SkillType.neutral:
                        c = (Resources.Load("Material/Monster-neutral", typeof(Material)) as Material).color;
                        Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.30f);
                        break;
                    case SkillType.darkness:
                        c = (Resources.Load("Material/Monster-darkness", typeof(Material)) as Material).color;
                        Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.80f);
                        break;
                    case SkillType.evil:
                        c = (Resources.Load("Material/Monster-evil", typeof(Material)) as Material).color;
                        Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.80f);
                        break;
                    case SkillType.holy:
                        c = (Resources.Load("Material/Monster-holy", typeof(Material)) as Material).color;
                        Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.20f);
                        break;
                    case SkillType.lightness:
                        c = (Resources.Load("Material/Monster-lightness", typeof(Material)) as Material).color;
                        Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.20f);
                        break;
                }

                GetComponent<Image>().color = c;

                

                isSelected = true;
            }
            else
            {
                Monster.GetComponentInChildren<MeshRenderer>().materials[1].SetFloat("_MKGlowPower", 0.0f);

                GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.2f);

                isSelected = false;
            }
        }
    }
}
