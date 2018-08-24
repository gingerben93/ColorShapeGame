using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    //[SyncVar]
    //public string playerId;
    //private NetworkInstanceId playerNetId;

    //public InputField Height;

    //public override void OnStartLocalPlayer()
    //{
    //    GetNetI();
    //    if (!isLocalPlayer)
    //    {
    //        transform.name = playerId;
    //    }
    //    else
    //    {
    //        name = "Player " + playerNetId.ToString();
    //    }
    //}

    //[Client]
    //void GetNetI()
    //{
    //    playerNetId = GetComponent<NetworkIdentity>().netId;
    //    CmdTellServer("Player " + playerNetId.ToString());
    //}

    //[Command]
    //void CmdTellServer(string name)
    //{
    //    playerId = name;
    //}

    //Use this for initialization

   int myTurn = 0;

   void Start()
    {
        //find beter way to sync turns
        myTurn = GameController.GameControllerSingle.turn;
        GameController.GameControllerSingle.turn += 1;

        if (isLocalPlayer)
        {
            //search for player tag to find local player
            tag = ("Player");
            GameController.GameControllerSingle.localPlayer = gameObject;
            GameObject.Find("NewGameButton").GetComponent<Button>().onClick.AddListener(() => NewGame());
            GameObject.Find("RestartGameButton").GetComponent<Button>().onClick.AddListener(() => RestartGame());
            GameObject.Find("StartNewGameButton").GetComponent<Button>().onClick.AddListener(() => StartNewGame());
            GameObject.Find("WidthInputField").GetComponent<InputField>().onEndEdit.AddListener(delegate { ChangeWidth(); });
            GameObject.Find("HeightInputField").GetComponent<InputField>().onEndEdit.AddListener(delegate { ChangeHeight(); });
            GameObject.Find("ColorInputField").GetComponent<InputField>().onEndEdit.AddListener(delegate { ChangeNumberColors(); });
            GameObject.Find("ShapeDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(delegate { ChangeShape(); });

        }
    }

    public void ChangeWidth()
    {
        //server client sync
        int newWidth;
        bool temp = int.TryParse(GameObject.Find("WidthInputField").GetComponent<InputField>().textComponent.text, out newWidth);
        if (temp)
        {
            if (isServer)
            {
                RpcChangeWidth(newWidth);
            }
            else
            {
                CmdChangeWidth(newWidth);
            }
        }
    }

    [Command]
    public void CmdChangeWidth(int w)
    {
        RpcChangeWidth(w);
    }

    [ClientRpc]
    public void RpcChangeWidth(int w)
    {
        GenerateGame.GenerateGameSingle.width = w;
    }

    public void ChangeHeight()
    {
        //server client sync
        int newHeight;
        bool temp = int.TryParse(GameObject.Find("HeightInputField").GetComponent<InputField>().textComponent.text, out newHeight);
        if (temp)
        {
            if (isServer)
            {
                RpcChangeHeight(newHeight);
            }
            else
            {
                CmdChangeHeight(newHeight);
            }
        }
    }

    [Command]
    public void CmdChangeHeight(int h)
    {
        RpcChangeHeight(h);
    }

    [ClientRpc]
    public void RpcChangeHeight(int h)
    {
        GenerateGame.GenerateGameSingle.height = h;
    }

    public void ChangeNumberColors()
    {
        //server client sync
        int newNumberColors;
        bool temp = int.TryParse(GameObject.Find("ColorInputField").GetComponent<InputField>().textComponent.text, out newNumberColors);
        if (temp)
        {
            if (isServer)
            {
                RpcChangeNumberColors(newNumberColors);
            }
            else
            {
                CmdChangeNumberColors(newNumberColors);
            }
        }
    }

    [Command]
    public void CmdChangeNumberColors(int c)
    {
        RpcChangeNumberColors(c);
    }

    [ClientRpc]
    public void RpcChangeNumberColors(int c)
    {
        GenerateGame.GenerateGameSingle.numberColors = c;
    }

    public void ChangeShape()
    {

        int shape = 3;
        var lableString = GameObject.Find("ShapeDropdown").GetComponent<Dropdown>().value;

        //order object are in the dropdown menu
        if (lableString == 0)
        {
            shape = 3;
        }
        else if (lableString == 1)
        {
            shape = 4;
        }
        else if (lableString == 2)
        {
            shape = 6;
        }

        //server client sync
        if (isServer)
        {
            RpcChangeShape(shape);
        }
        else
        {
            CmdChangeShape(shape);
        }
    }

    [Command]
    public void CmdChangeShape(int s)
    {
        RpcChangeShape(s);
    }

    [ClientRpc]
    public void RpcChangeShape(int s)
    {
        GenerateGame.GenerateGameSingle.shape = s;
    }

    public void NewGame()
    {
        //server client sync
        if (isServer)
        {
            RpcNewGame();
        }
        else
        {
            CmdNewGame();
        }
    }

    [Command]
    public void CmdNewGame()
    {
        RpcNewGame();
    }

    [ClientRpc]
    public void RpcNewGame()
    {
        GameController.GameControllerSingle.NewGame();
    }

    public void StartNewGame()
    {
        int[] colors;
        colors = GenerateGame.GenerateGameSingle.GetListOfColors();
        //server client sync
        if (isServer)
        {
            RpcStartNewGame(colors);
        }
        else 
        {
            CmdStartNewGame(colors);
        }
    }

    [Command]
    public void CmdStartNewGame(int[] colors)
    {
        RpcStartNewGame(colors);
    }

    [ClientRpc]
    public void RpcStartNewGame(int[] colors)
    {
        GenerateGame.GenerateGameSingle.DrawShapesForGame(colors);
        GenerateGame.GenerateGameSingle.StartLocations();
        GenerateGame.GenerateGameSingle.placeButtons();
    }

    public void RestartGame()
    {
        if (isServer)
        {
            NewGame();
            StartNewGame();
        }
        else
        {
            NewGame();
            StartNewGame();
        }
    }
    
    public void PlayerButtonClick(Color color)
    {
        //check if my turn
        //if(myTurn == GameController.GameControllerSingle.turn)
        //{
            //server client sync
            if (isServer)
            {
                RpcPlayerButtonClick(color);
            }
            else
            {
                CmdPlayerButtonClick(color);
            }
        //}
    }

    [Command]
    public void CmdPlayerButtonClick(Color color)
    {
        RpcPlayerButtonClick(color);
    }

    [ClientRpc]
    public void RpcPlayerButtonClick(Color color)
    {
        //GenerateGame.GenerateGameSingle.StartGameGeneration(colors);
        GameController.GameControllerSingle.ButtonClick(color);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    transform.position = new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z);
        //}
        //else if (Input.GetKeyDown(KeyCode.H))
        //{
        //    transform.position = new Vector3(transform.position.x + .5f, transform.position.y, transform.position.z);
        //}

        //if (transform.name == "Player(Clone)")
        //{
        //    if (!isLocalPlayer)
        //    {
        //        transform.name = playerId;
        //    }
        //    else
        //    {
        //        name = "Player " + playerNetId.ToString();
        //    }
        //}

    }
}
