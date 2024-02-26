using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggle : MonoBehaviour {

	public GameObject myobject;
	public bool activateme;

	void Start()
	{
		myobject.SetActive (true);
		activateme = true;;
	}

	// Update is called once per frame
	void Update () {
		if (activateme == true)
		{
			myobject.SetActive (false);
	        activateme = false;

		}

		if (activateme == false)
		{
			myobject.SetActive (true);
			activateme = true;
		}
			
	}
}
