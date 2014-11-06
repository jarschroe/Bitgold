// Author: Jared Schroeder 6-11-14
// Comments: Based on code in 'credits.cs' created by Ellen Castle 2013

using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	bool PanStop = false;
	bool BackButtonPressed = false;
	float Timer = 0.0f;
	float PanTime = 2.0f;
	Vector3 MenuPosition = new Vector3(0.0f, 1.0f, -10.0f);
	Vector3 ShopPosition = new Vector3(8.0857f, 1.2506f, -5.9832f);
	float ItemsStartX = Screen.width/16.0f;
	float ItemsStartY = Screen.height/4.0f;
	static float ItemButtonWidth = 200.0f;
	static float ItemButtonHeight = 80.0f;
	float ItemsXSpace = ItemButtonWidth + 20.0f;
	float ItemsYSpace = ItemButtonHeight + 30.0f;

	public string[][] ItemNames = new string[][]
	{
		new string[] { "item1", "item2", "item3", "item4" },
		new string[] { "item5", "item6", "item7", "item8" }
	};

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (BackButtonPressed)
		{
			// travel to menu position
			Timer += Time.deltaTime;
			transform.position = Vector3.Lerp(ShopPosition, MenuPosition, Timer / PanTime);

			if (transform.position == MenuPosition)
			{
				Application.LoadLevel("menu");
			}
		}
		else
		{
			// travel to shop position
			Timer += Time.deltaTime;
			transform.position = Vector3.Lerp(MenuPosition, ShopPosition, Timer / PanTime);

			if (transform.position == ShopPosition)
			{
				PanStop = true;
			}
		}
	}

	void OnGUI()
	{
		// Author: Ellen Castle 2013
		GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
		myButtonStyle.fontSize = 20;
		
		Color colour = new Color(0.1F,1, 0, 1);
		
		myButtonStyle.normal.textColor = Color.white; 	 			
		myButtonStyle.hover.textColor = colour;
		
		// Load and set Font
		Font myFont = (Font)Resources.Load("NEUROPOL", typeof(Font));
		myButtonStyle.font = myFont;
		// \Author

		// display buttons when camera has reached shop position
		if (PanStop == true)
		{
			// shop buttons
			for (int i = 0; i < ItemNames.Length; ++i)
			{
				for (int j = 0; j < ItemNames[i].Length; ++j)
				{
					if (GUI.Button(new Rect(ItemsStartX+j*ItemsXSpace, ItemsStartY+i*ItemsYSpace, ItemButtonWidth, ItemButtonHeight), ItemNames[i][j], myButtonStyle))
					{
						// do Bitgold stuff
					}
				}
			}

			// back button
			if (GUI.Button(new Rect(Screen.width/16.0f, Screen.height - Screen.height/8.0f, 100, 40), "Back", myButtonStyle))
			{				
				BackButtonPressed = true;
				PanStop = false;
				Timer = 0.0f;
			}			
		}
	}
}
