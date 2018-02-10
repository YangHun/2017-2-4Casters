using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterButton : MonoBehaviour {

    RectTransform rect;
    Vector3 dir = Vector3.zero;

    public IVMonster Monster;

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

    public void OnClickButton()
    {
        Debug.Log(Monster.Keyword);

        if ( Monster != null)
        {
            if (!isSelected)
            {
                Monster.GetComponent<Renderer>().materials[0].SetFloat("_MKGlowPower", 1.5f);

                GetComponent<Image>().color = Monster.GetComponent<Renderer>().materials[0].GetColor("_Color");
                
                isSelected = true;
            }
            else
            {
                Monster.GetComponent<Renderer>().materials[0].SetFloat("_MKGlowPower", 0.2f);

                GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.2f);

                isSelected = false;
            }
        }
    }
}
