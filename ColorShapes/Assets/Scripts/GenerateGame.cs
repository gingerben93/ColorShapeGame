using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GenerateGame : NetworkBehaviour
{

    public GameObject[,] gameBoard;
    public int[] Colors;
    public int width;
    public int height;

    public int numberColors;
    public int numPlayers;
    public int shape;

    private List<Vector2> startLocations;

    public GameObject BoxPrefab;
    public GameObject HexagonPrefab;
    public GameObject TriabglePrefab;

    public GameObject ButtonSquarePrefab;

    public GameObject ColorButtonPanel;
    CanvasGroup EndGamePanel;
    CanvasGroup StartGamePanel;

    public static GenerateGame GenerateGameSingle;
    //create singleton of class
    void Awake()
    {
        if (GenerateGameSingle == null)
        {
            //DontDestroyOnLoad(gameObject);
            GenerateGameSingle = this;
        }
        else if (GenerateGameSingle != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start()
    {
        ColorButtonPanel = GameObject.Find("ColorButtonPanel");
        EndGamePanel = GameObject.Find("EndGamePanel").GetComponent<CanvasGroup>();
        StartGamePanel = GameObject.Find("StartGamePanel").GetComponent<CanvasGroup>();

        EndGamePanel.alpha = 1;
        EndGamePanel.interactable = true;
        EndGamePanel.blocksRaycasts = true;
        StartGamePanel.alpha = 0;
        StartGamePanel.interactable = false;
        StartGamePanel.blocksRaycasts = false;

        numPlayers = 2;

        // width , height, number colors, number players, number of sides on shape
        //StartGameCondition(10, 10, 7, 2, 6);
        //StartGameGeneration();



        if (isServer)
        {
            //StartGameCondition(ServerController.ServerControllerSingle.width, ServerController.ServerControllerSingle.height, ServerController.ServerControllerSingle.numberColors, ServerController.ServerControllerSingle.numPlayers, ServerController.ServerControllerSingle.shape);

            //StartGameCondition(10, 10, 7, 2, 6);
            //clear if this is game reset
            //StartGameGeneration();
        }
        else
        {
            //CmdStartGameCondition();
            //clear if this is game reset
        }

    }

    public void StartGameCondition(int w, int h, int numCol, int numPlay, int s)
    {
        //start variables
        width = w;
        height = h;
        numberColors = numCol;
        numPlayers = numPlay;
        shape = s;

    }

    //[Command]
    //public void CmdStartGameCondition()
    //{
    //    RpcStartGameCondition(width, height, numberColors, numPlayers, shape);
    //}

    //// width , height, number colors, number players, number of sides on shape
    //[ClientRpc]
    //public void RpcStartGameCondition(int w, int h, int numCol, int numPlay, int s)
    //{
    //    //start variables
    //    width = w;
    //    height = h;
    //    numberColors = numCol;
    //    numPlayers = numPlay;
    //    shape = s;
    //}

    public int[] StartGameGeneration()
    {
        GameObject shapePrefab;
        GameController.GameControllerSingle.turn = 0;
        GameController.GameControllerSingle.Players = new List<GameController.Player>();

        EndGamePanel.alpha = 1;
        EndGamePanel.interactable = true;
        EndGamePanel.blocksRaycasts = true;
        StartGamePanel.alpha = 0;
        StartGamePanel.interactable = false;
        StartGamePanel.blocksRaycasts = false;

        //sets shape prefab
        if (shape == 3)
        {
            shapePrefab = TriabglePrefab;
        }
        else if (shape == 4)
        {
            shapePrefab = BoxPrefab;
        }
        else if (shape == 6)
        {
            shapePrefab = HexagonPrefab;
        }
        else
        {
            shapePrefab = BoxPrefab;
        }

        gameBoard = new GameObject[width, height];
        Colors = new int[width*height];
        //makes grid; Instantiate color shape
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int color = Random.Range(0, numberColors);
                Colors[x + y*height] = color;

                var tempBox = Instantiate(shapePrefab, transform);
                gameBoard[x, y] = tempBox;
                tempBox.GetComponent<SpriteRenderer>().color = PickColor(color);

                if (shape == 3)
                {
                    //triangle pattern; had to change center of triangle in sprite editor
                    if ((x + y) % 2 == 1)
                    {
                        tempBox.transform.localEulerAngles = new Vector3(tempBox.transform.eulerAngles.x, tempBox.transform.eulerAngles.y, 180);
                        tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                    }
                    else
                        tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                }
                else if (shape == 4)
                {
                    //square
                    tempBox.transform.localPosition = new Vector3(x - width / 2, y - height / 2, 0);
                }
                else if (shape == 6)
                {
                    //hexagon pattern
                    if (x % 2 == 1)
                        tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f - .25f - (height / 2 * .75f) - 1.5f, 0);
                    else
                        tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f + .25f - (height / 2 * .75f) - 1.5f, 0);
                }
            }
        }

        //reset buttons
        GameController.GameControllerSingle.buttons = new List<GameObject>();

        //for (int y = 0; y < numberColors; y++)
        //{
        //    var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
        //    tempBox.GetComponent<Image>().color = PickColor(y);
        //    //set button method to color of button
        //    tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.ButtonClick(tempBox.GetComponent<Image>().color));
        //    GameController.GameControllerSingle.buttons.Add(tempBox);
        //    //tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
        //    //tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
        //    ////set position from panel not world
        //    //tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
        //}

        //GameController.GameControllerSingle.turn = 0;
        print("players");
        StartConitions();

        //if (isServer)
        //{
        //    RpcStartGameGeneration();
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            RpcChangeColor(x, y, Colors[x, y]);
        //        }
        //    }
        //}
        //else
        //{
        //    CmdStartGameGeneration();
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            CmdChangeColor(x, y, Colors[x, y]);
        //        }
        //    }
        //}

        return Colors;
    }

    public void StartGameGeneration(int[] newColors)
    {
        GameObject shapePrefab;
        GameController.GameControllerSingle.turn = 0;
        GameController.GameControllerSingle.Players = new List<GameController.Player>();

        EndGamePanel.alpha = 1;
        EndGamePanel.interactable = true;
        EndGamePanel.blocksRaycasts = true;
        StartGamePanel.alpha = 0;
        StartGamePanel.interactable = false;
        StartGamePanel.blocksRaycasts = false;

        //sets shape prefab
        if (shape == 3)
        {
            shapePrefab = TriabglePrefab;
        }
        else if (shape == 4)
        {
            shapePrefab = BoxPrefab;
        }
        else if (shape == 6)
        {
            shapePrefab = HexagonPrefab;
        }
        else
        {
            shapePrefab = BoxPrefab;
        }

        gameBoard = new GameObject[width, height];
        //makes grid; Instantiate color shape
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tempBox = Instantiate(shapePrefab, transform);
                gameBoard[x, y] = tempBox;
                //print(newColors[x, y]);
                tempBox.GetComponent<SpriteRenderer>().color = PickColor(newColors[x+y*height]);

                if (shape == 3)
                {
                    //triangle pattern; had to change center of triangle in sprite editor
                    if ((x + y) % 2 == 1)
                    {
                        tempBox.transform.localEulerAngles = new Vector3(tempBox.transform.eulerAngles.x, tempBox.transform.eulerAngles.y, 180);
                        tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                    }
                    else
                        tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                }
                else if (shape == 4)
                {
                    //square
                    tempBox.transform.localPosition = new Vector3(x - width / 2, y - height / 2, 0);
                }
                else if (shape == 6)
                {
                    //hexagon pattern
                    if (x % 2 == 1)
                        tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f - .25f - (height / 2 * .75f) - 1.5f, 0);
                    else
                        tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f + .25f - (height / 2 * .75f) - 1.5f, 0);
                }
            }
        }

        ////reset buttons
        //GameController.GameControllerSingle.buttons = new List<GameObject>();

        //for (int y = 0; y < numberColors; y++)
        //{
        //    var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
        //    tempBox.GetComponent<Image>().color = PickColor(y);
        //    //set button method to color of button
        //    tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.ButtonClick(tempBox.GetComponent<Image>().color));
        //    GameController.GameControllerSingle.buttons.Add(tempBox);
        //    //tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
        //    //tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
        //    ////set position from panel not world
        //    //tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
        //}

        //GameController.GameControllerSingle.turn = 0;
        //StartConitions();

        //if (isServer)
        //{
        //    RpcStartGameGeneration();
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            RpcChangeColor(x, y, Colors[x, y]);
        //        }
        //    }
        //}
        //else
        //{
        //    CmdStartGameGeneration();
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            CmdChangeColor(x, y, Colors[x, y]);
        //        }
        //    }
        //}
    }

    //[Command]
    //public void CmdStartGameGeneration()
    //{
    //    RpcStartGameGeneration();
    //}


    //[ClientRpc]
    //public void RpcStartGameGeneration()
    //{
    //    GameObject shapePrefab;
    //    GameController.GameControllerSingle.turn = 0;

    //    EndGamePanel.alpha = 1;
    //    EndGamePanel.interactable = true;
    //    EndGamePanel.blocksRaycasts = true;
    //    StartGamePanel.alpha = 0;
    //    StartGamePanel.interactable = false;
    //    StartGamePanel.blocksRaycasts = false;

    //    //sets shape prefab
    //    if (shape == 3)
    //    {
    //        shapePrefab = TriabglePrefab;
    //    }
    //    else if (shape == 4)
    //    {
    //        shapePrefab = BoxPrefab;
    //    }
    //    else if (shape == 6)
    //    {
    //        shapePrefab = HexagonPrefab;
    //    }
    //    else
    //    {
    //        shapePrefab = BoxPrefab;
    //    }

    //    gameBoard = new GameObject[width, height];
    //    //makes grid; Instantiate color shape
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            var tempBox = Instantiate(shapePrefab, transform);
    //            gameBoard[x, y] = tempBox;
    //            //tempBox.GetComponent<SpriteRenderer>().color = PickColor(color);

    //            if (shape == 3)
    //            {
    //                //triangle pattern; had to change center of triangle in sprite editor
    //                if ((x + y) % 2 == 1)
    //                {
    //                    tempBox.transform.localEulerAngles = new Vector3(tempBox.transform.eulerAngles.x, tempBox.transform.eulerAngles.y, 180);
    //                    tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
    //                }
    //                else
    //                    tempBox.transform.localPosition = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
    //            }
    //            else if (shape == 4)
    //            {
    //                //square
    //                tempBox.transform.localPosition = new Vector3(x - width / 2, y - height / 2, 0);
    //            }
    //            else if (shape == 6)
    //            {
    //                //hexagon pattern
    //                if (x % 2 == 1)
    //                    tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f - .25f - (height / 2 * .75f) - 1.5f, 0);
    //                else
    //                    tempBox.transform.localPosition = new Vector3(x * .8f - width / 2, y * .9f + .25f - (height / 2 * .75f) - 1.5f, 0);
    //            }
    //        }
    //    }

    //    //reset buttons
    //    GameController.GameControllerSingle.buttons = new List<GameObject>();

    //    for (int y = 0; y < numberColors; y++)
    //    {
    //        var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
    //        tempBox.GetComponent<Image>().color = PickColor(y);
    //        //set button method to color of button
    //        tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.ButtonClick(tempBox.GetComponent<Image>().color));
    //        GameController.GameControllerSingle.buttons.Add(tempBox);
    //        //tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
    //        //tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
    //        ////set position from panel not world
    //        //tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
    //    }
    //}

    ////[Command]
    ////private void CmdChangeColor(int x, int y, int c)
    ////{
    ////    RpcChangeColor(x, y, c);
    ////}

    ////[ClientRpc]
    ////private void RpcChangeColor(int x, int y, int c)
    ////{
    ////    gameBoard[x, y].GetComponent<SpriteRenderer>().color = PickColor(c);
    ////}

    private void StartConitions()
    {
        startLocations = new List<Vector2>();

        //possible player starts
        startLocations.Add(new Vector2(0, 0));
        startLocations.Add(new Vector2(width - 1, height - 1));
        startLocations.Add(new Vector2(0, height - 1));
        startLocations.Add(new Vector2(width - 1, 0));

        for (int x = 0; x < numPlayers; x++)
        {
            int ranStart = Random.Range(0, startLocations.Count);

            GameController.GameControllerSingle.AddPlayer(startLocations[ranStart]);
            gameBoard[(int)startLocations[ranStart].x, (int)startLocations[ranStart].y].GetComponent<SpriteRenderer>().color = Color.black;
            Colors[(int)startLocations[ranStart].x + (int)startLocations[ranStart].y*height] = -1;
            //stop player from being in same spot
            //if (isServer)
            //{
            //    RpcStartConitions(startLocations[ranStart]);
            //}
            //else
            //{
            //    CmdStartConitions(startLocations[ranStart]);
            //}

            startLocations.RemoveAt(ranStart);
        }
        print(GameController.GameControllerSingle.Players.Count);
    }

    [Command]
    private void CmdStartConitions(Vector2 location)
    {
        RpcStartConitions(location);
    }

    [ClientRpc]
    private void RpcStartConitions(Vector2 location)
    {
        GameController.GameControllerSingle.AddPlayer(location);
    }

    //public void ClientGetGame(GameObject [,] ServerGame)
    //{
    //    GameObject shapePrefab;
    //    GameController.GameControllerSingle.turn = 0;

    //    EndGamePanel.alpha = 1;
    //    EndGamePanel.interactable = true;
    //    EndGamePanel.blocksRaycasts = true;
    //    StartGamePanel.alpha = 0;
    //    StartGamePanel.interactable = false;
    //    StartGamePanel.blocksRaycasts = false;

    //    //sets shape prefab
    //    if (shape == 3)
    //    {
    //        shapePrefab = TriabglePrefab;
    //    }
    //    else if (shape == 4)
    //    {
    //        shapePrefab = BoxPrefab;
    //    }
    //    else if (shape == 6)
    //    {
    //        shapePrefab = HexagonPrefab;
    //    }
    //    else
    //    {
    //        shapePrefab = BoxPrefab;
    //    }

    //    gameBoard = new GameObject[width, height];
    //    //makes grid; Instantiate color shape
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            var tempBox = Instantiate(shapePrefab, transform);
    //            gameBoard[x, y] = tempBox;
    //            tempBox.GetComponent<SpriteRenderer>().color = ServerGame[x, y].GetComponent<SpriteRenderer>().color;
    //            tempBox.transform.localEulerAngles = ServerGame[x, y].transform.localEulerAngles;
    //            tempBox.transform.localPosition = ServerGame[x, y].transform.localPosition;
    //        }
    //    }

    //    //reset buttons
    //    GameController.GameControllerSingle.buttons = new List<GameObject>();

    //    for (int y = 0; y < numberColors; y++)
    //    {
    //        var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
    //        tempBox.GetComponent<Image>().color = PickColor(y);
    //        //set button method to color of button
    //        tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.ButtonClick(tempBox.GetComponent<Image>().color));
    //        GameController.GameControllerSingle.buttons.Add(tempBox);
    //        //tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
    //        //tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
    //        ////set position from panel not world
    //        //tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
    //    }
    //}

    //public void RestartGame()
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            int color = Random.Range(0, numberColors);
    //            gameBoard[x,y].GetComponent<SpriteRenderer>().color = PickColor(color);
    //        }
    //    }
    //    StartConitions();

    //    ////server client sync
    //    //if (isServer)
    //    //{
    //    //    RpcRestartGame(gameBoard);
    //    //}
    //    //else
    //    //{
    //    //    CmdRestartGame(gameBoard);
    //    //}
    //}

    //[Command]
    //private void CmdRestartGame(GameObject [,] board)
    //{
    //    RpcRestartGame(board);
    //}

    //[ClientRpc]
    //private void RpcRestartGame(GameObject[,] board)
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            //int color = Random.Range(0, numberColors);
    //            gameBoard[x, y].GetComponent<SpriteRenderer>().color = board[x, y].GetComponent<SpriteRenderer>().color;
    //        }
    //    }
    //    StartConitions();
    //}

    public void NewGame()
    {
        ////server client sync
        //if (isServer)
        //{
        //    RpcNewGame();
        //}
        //else
        //{
        //    CmdNewGame();
        //}

        //remove all old shapes
        foreach (Transform child in GameObject.Find("Floor").transform)
        {
            Destroy(child.gameObject);
        }


        //remove all old button colors
        foreach (Transform button in ColorButtonPanel.transform)
        {
            Destroy(button.gameObject);
        }

        //stop reading inputs
        GameController.GameControllerSingle.turn = -1;

        //panel options after newgame button click
        EndGamePanel.alpha = 0;
        EndGamePanel.interactable = false;
        EndGamePanel.blocksRaycasts = false;
        StartGamePanel.alpha = 1;
        StartGamePanel.interactable = true;
        StartGamePanel.blocksRaycasts = true;
    }

    [Command]
    public void CmdNewGame()
    {
        RpcNewGame();
    }

    [ClientRpc]
    public void RpcNewGame()
    {
        //remove all old shapes
        foreach(Transform child in GameObject.Find("Floor").transform)
        {
            Destroy(child.gameObject);
        }
        

        //remove all old button colors
        foreach (Transform button in ColorButtonPanel.transform)
        {
            Destroy(button.gameObject);
        }

        //stop reading inputs
        GameController.GameControllerSingle.turn = -1;

        //panel options after newgame button click
        EndGamePanel.alpha = 0;
        EndGamePanel.interactable = false;
        EndGamePanel.blocksRaycasts = false;
        StartGamePanel.alpha = 1;
        StartGamePanel.interactable = true;
        StartGamePanel.blocksRaycasts = true;
    }

    public void ChangeWidth()
    {
        int newWidth;
        bool temp = int.TryParse(GameObject.Find("WidthInputField").GetComponent<InputField>().textComponent.text, out newWidth);
        if (temp)
        {
            width = newWidth;
        }

        //server client sync
        if (isServer)
        {
            RpcChangeWidth(width);
        }
        else
        {
            CmdChangeWidth(width);
        }
    }

    [Command]
    private void CmdChangeWidth(int w)
    {
        RpcChangeWidth(w);
    }

    [ClientRpc]
    private void RpcChangeWidth(int w)
    {
        width = w;
        //GameObject.Find("WidthInputField").GetComponent<InputField>().textComponent.text = width.ToString();
    }


    public void ChangeHeight()
    {
        int newHeight;
        bool temp = int.TryParse(GameObject.Find("HeightInputField").GetComponent<InputField>().textComponent.text, out newHeight);
        if (temp)
        {
            height = newHeight;
        }

        //server client sync
        if (isServer)
        {
            RpcChangeHeight(height);
        }
        else
        {
            CmdChangeHeight(height);
        }
    }

    [Command]
    private void CmdChangeHeight(int h)
    {
        RpcChangeWidth(h);
    }

    [ClientRpc]
    private void RpcChangeHeight(int h)
    {
        height = h;
        //GameObject.Find("WidthInputField").GetComponent<InputField>().textComponent.text = width.ToString();
    }

    public void ChangeNumberColors()
    {
        int numColors;
        bool temp = int.TryParse(GameObject.Find("ColorInputField").GetComponent<InputField>().textComponent.text, out numColors);
        if (temp)
        {
            numberColors = numColors;
        }

        //server client sync
        if (isServer)
        {
            RpcChangeNumberColors(numberColors);
        }
        else
        {
            CmdChangeNumberColors(numberColors);
        }
    }

    [Command]
    private void CmdChangeNumberColors(int num)
    {
        RpcChangeNumberColors(num);
    }

    [ClientRpc]
    private void RpcChangeNumberColors(int num)
    {
        numberColors = num;
    }

    public void ChangeShape()
    {
        var lableString = GameObject.Find("ShapeDropdown").GetComponent<Dropdown>().value;

        //order object are in the dropdown menu
        if (lableString == 0)
        {
            shape = 3;
        }
        else if (lableString == 1)
        {
            shape = 3;
        }
        else if (lableString == 2)
        {
            shape = 4;
        }
        else if (lableString == 3)
        {
            shape = 6;
        }

        ////server client sync
        //if (isServer)
        //{
        //    RpcChangeShape(shape);
        //}
        //else
        //{
        //    CmdChangeShape(shape);
        //}

    }

    [Command]
    private void CmdChangeShape(int s)
    {
        RpcChangeShape(s);
    }

    [ClientRpc]
    private void RpcChangeShape(int s)
    {
        shape = s;
    }

    public Color PickColor(int color)
    {
        switch (color)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.yellow;
            case 2:
                return Color.green;
            case 3:
                return Color.cyan;
            case 4:
                return Color.blue;
            case 5:
                return Color.magenta;
            case 6:
                return Color.white;
            default:
                return Color.black;
        }
    }

}
