using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickManager : MonoBehaviour {

	[SerializeField]
	int stickSize = 40;
	string selectName;

    public void OnPointerDown(Selectable s)
    {
        selectName = s.name;
        if(selectName == "JoystickBody")
        {
            Debug.Log("Body was pressed");
			RectTransform stick = (RectTransform)s.transform.GetChild(0);
            stick.sizeDelta = new Vector2(stickSize, stickSize);
        }
        return;
		
    }

    public void OnPointerUp(Selectable s)
    {
        selectName = s.name;
        if (selectName == "JoystickBody")
        {
            Debug.Log("Body was released");
            RectTransform c = (RectTransform)s.transform.GetChild(0);
            c.sizeDelta = new Vector2(0, 0);
            c.position = new Vector3(0, 0, 0);
        }
        return;
    }

	void Start ()
    {

    }

    void Update()
    {

    }
}
