using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class JoystickManager : NetworkBehaviour {

	[SerializeField]
	int stickSize = 40;
	[SerializeField]
	public float theta = 0;
	[SerializeField]
	public float dist = 0;

	string selectName;
	RectTransform stick;
	RectTransform body;
	Vector3 delta;
	List<Player> players;

    public void OnPointerDown(Selectable s)
	{
		players = GameObject.Find("Manager").GetComponent<GameManager>().Players;
		selectName = s.name;
        if(selectName == "JoystickBody")
        {
            //Debug.Log("Body was pressed");
            stick.sizeDelta = new Vector2(stickSize, stickSize);
        }
        return;
		
    }

    public void OnPointerUp(Selectable s)
    {
        selectName = s.name;
        if (selectName == "JoystickBody")
        {
            //Debug.Log("Body was released");
			stick.sizeDelta = new Vector2(0, 0);
            stick.position = new Vector3(0, 0, 0);
			theta = 0;
			dist = 0;
        }
        return;
    }

	public void OnDrag(Selectable s)
	{
		delta = stick.position - body.position;
		dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y);
		theta = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
		foreach(Player player in players)
			player.UpdateArrow(theta - 90);
		//Debug.Log("Theta : " + theta);
	}
	/*
	public void OnEndDrag(Selectable s)
	{
		foreach (Player player in players)
			player.EndUpdateArrow();
	}
	*/
	void Start ()
    {
		stick = (RectTransform)GameObject.Find("Joystick").transform;
		body = (RectTransform)GameObject.Find("JoystickBody").transform;
		players = GameObject.Find("Manager").GetComponent<GameManager>().Players;
	}

    void Update()
    {
		
    }


}