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
    }

    //shouldnt make copies in two places
    public int[] GetListOfColors()
    {
        gameBoard = new GameObject[width, height];
        Colors = new int[width*height];
        //makes grid; Instantiate color shape
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int color = Random.Range(0, numberColors);
                //cant pass multidimentional arrays with UNET
                Colors[x + y*height] = color;
            }
        }
        
        //StartConitions();
        return Colors;
    }

    public void DrawShapesForGame(int[] newColors)
    {
        //need to set colors for StartLocations method
        Colors = newColors;
        GameObject shapePrefab;
        GameController.GameControllerSingle.turn = 0;

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
                tempBox.GetComponent<SpriteRenderer>().color = PickColor(newColors[x+y*height]);

                //pattern to place based pn shape
                if (shape == 3)
                {
                    //triangle pattern; had to change center of triangle in sprite editor; 1-(4/.5 = 3/x) = .625
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
    }
    
    public void StartLocations()
    {
        //need to reset player variables
        GameController.GameControllerSingle.Players = new List<GameController.Player>();

        startLocations = new List<Vector2>();
        //possible player starts
        startLocations.Add(new Vector2(0, 0));
        startLocations.Add(new Vector2(width - 1, height - 1));
        startLocations.Add(new Vector2(0, height - 1));
        startLocations.Add(new Vector2(width - 1, 0));

        for (int x = 0; x < numPlayers; x++)
        {
            //int ranStart = Random.Range(0, startLocations.Count);

            GameController.GameControllerSingle.AddPlayer(startLocations[x]);
            gameBoard[(int)startLocations[x].x, (int)startLocations[x].y].GetComponent<SpriteRenderer>().color = Color.black;
            Colors[(int)startLocations[x].x + (int)startLocations[x].y*height] = -1;
            //startLocations.RemoveAt(ranStart);
        }
    }

    public void placeButtons()
    {
        //reset buttons
        GameController.GameControllerSingle.buttons = new List<GameObject>();

        for (int y = 0; y < numberColors; y++)
        {
            var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
            tempBox.GetComponent<Image>().color = PickColor(y);
            //set button method to color of button of the local player
            tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.localPlayer.GetComponent<Player>().PlayerButtonClick(tempBox.GetComponent<Image>().color));
            GameController.GameControllerSingle.buttons.Add(tempBox);
        }
        
    }

    //public void RestartGame(int[] newColors)
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            gameBoard[x, y].GetComponent<SpriteRenderer>().color = PickColor(newColors[x + y * height]);
    //        }
    //    }
    //}

    public void NewGame()
    {
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
