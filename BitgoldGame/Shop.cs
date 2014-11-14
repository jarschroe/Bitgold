// Author: Jared Schroeder 6-11-14
// Comments: Based on code in 'credits.cs' created by Ellen Castle 2013

using UnityEngine;
using System.Collections;
using Bitgold;
using System.IO;

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
	// Bitgold
	BgDeveloper developer = new BgDeveloper("18R3k1bCPKmD6oNtE5rBq2pwut8i2d8SEB");

	public class ShopItem
	{
		public string content;
		public float cost;
		bool bought;
		public bool Bought
		{
			get
			{
				return bought;
			}
			set 
			{
				bought = true;
				content += "\nBOUGHT";
				// remove the cost
				cost = 0.0f;
				// unlock the bought effect
				Toggle();
			}
		}
		public bool InEffect { get; private set; }

		public ShopItem(string content, float cost)
		{
			this.content = content;
			this.cost = cost;
			bought = false;
			InEffect = false;
		}

		public void Toggle()
		{
			InEffect = !InEffect;

			// do the effect if the shop item has been toggled on
			// TODO: set this up so each shop item can have unique effects (with delegates maybe?)
			if (InEffect)
			{
				// flip camera upside down
				Vector3 newRotationEular = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, (Camera.main.transform.eulerAngles.z + 180.0f) % 360.0f);
				Camera.main.transform.eulerAngles = newRotationEular;
			}
			else
			{
				// unflip camera
				Vector3 newRotationEular = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, (Camera.main.transform.eulerAngles.z + 180.0f) % 360.0f);
				Camera.main.transform.eulerAngles = newRotationEular;
			}
		}
	};

	ShopItem[][] ShopItems = new ShopItem[][]
	{
		new ShopItem[] { new ShopItem("item1", 0.99f), new ShopItem("item2", 0.99f), new ShopItem("item3", 0.99f), new ShopItem("item4", 0.99f) },
		new ShopItem[] { new ShopItem("item5", 1.99f), new ShopItem("item6", 2.99f), new ShopItem("item7", 4.99f), new ShopItem("item8", 9.99f) }
	};

	string TransactionResultText = string.Empty;

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

		GUIStyle resultButton = new GUIStyle(GUI.skin.button);
		resultButton.fontSize = 25;
		resultButton.wordWrap = true;
		resultButton.normal.textColor = Color.white; 	 			
		resultButton.hover.textColor = colour;
		resultButton.font = myFont;

		// display buttons when camera has reached shop position
		if (PanStop == true)
		{
			// shop buttons
			for (int i = 0; i < ShopItems.Length; ++i)
			{
				for (int j = 0; j < ShopItems[i].Length; ++j)
				{
					if (GUI.Button(new Rect(ItemsStartX+j*ItemsXSpace, ItemsStartY+i*ItemsYSpace, ItemButtonWidth, ItemButtonHeight), ShopItems[i][j].content + "\n$" + ShopItems[i][j].cost.ToString(), myButtonStyle))
					{
						ShopItem clickedItem = ShopItems[i][j];

						if (clickedItem.Bought == true)
						{
							// item has been bought already, so just toggle the item's effect on and off
							clickedItem.Toggle();
						}
						else
						{
							// do Bitgold transaction
							// get player credentials
							string playerAddress = "16q6tj1wCAYbdVg7yWDngtnYmCeAUBAR2Q";
							string key = new StreamReader("E:/New Text Document.txt").ReadToEnd();
							BgPlayer player = new BgPlayer(playerAddress, key, BgCurrency.AUD);
							BgTransaction transaction = new BgTransaction(developer, player, ShopItems[i][j].cost);
							// do transaction (currently just prints the response to the console)
							BgApiController api = new BgApiController();
							BgApiResult result = api.SubmitTransaction(transaction);
							
							// handle transaction result
							switch (result.Type)
							{
							case BgApiResult.ResultType.SUCCESS:
							{
								TransactionResultText = "Success!\nTransaction details: " + result.Message + "\nTransaction hash: " + result.TransactionHash + "\nNote: " + result.Notice;
								// register the button as being bought
								clickedItem.Bought = true;
								break;
							}
							case BgApiResult.ResultType.ERROR:
							{
								TransactionResultText = "Failed!\nError details:\n" + result.Message;
								break;
							}
							default:
							{
								TransactionResultText = "Failed for an unknown reason!";
								break;
							}
							}
						}
					}
				}
			}

			if (TransactionResultText.Length > 0)
			{
				// create a button that shows the transaction result
				if (GUI.Button(new Rect(Screen.width/8.0f, Screen.height/8.0f, Screen.width*0.75f, Screen.height*0.75f), TransactionResultText, resultButton))
				{
					// empty the result string to remove the button when the user clicks it
					TransactionResultText = string.Empty;
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
