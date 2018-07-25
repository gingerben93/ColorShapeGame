using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateGame : MonoBehaviour {

    public GameObject[,] gameBoard;
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
    void Start ()
    {
        ColorButtonPanel = GameObject.Find("ColorButtonPanel");
        EndGamePanel = GameObject.Find("EndGamePanel").GetComponent<CanvasGroup>();
        StartGamePanel = GameObject.Find("StartGamePanel").GetComponent<CanvasGroup>();
        // width , height, number colors, number players, number of sides on shape
        StartGameCondition(10, 10, 7, 2, 6);
        StartGameGeneration();

    }

    // Update is called once per frame
    void Update () {

		
	}

    // width , height, number colors, number players, number of sides on shape
    public void StartGameCondition(int w, int h, int numCol, int numPlay, int s)
    {
        //start variables
        width = w;
        height = h;
        numberColors = numCol;
        numPlayers = numPlay;
        shape = s;
    }

    public void StartGameGeneration()
    {
        GameObject shapePrefab;
        GameController.GameControllerSingle.turn = Random.Range(0, numPlayers);

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
        else if(shape == 6)
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
                int color = Random.Range(0, numberColors);
                
                var tempBox = Instantiate(shapePrefab);
                gameBoard[x, y] = tempBox;
                tempBox.GetComponent<SpriteRenderer>().color = PickColor(color);

                if (shape == 3)
                {
                    //triangle pattern; had to change center of triangle in sprite editor
                    if ((x + y) % 2 == 1)
                    {
                        tempBox.transform.eulerAngles = new Vector3(tempBox.transform.eulerAngles.x, tempBox.transform.eulerAngles.y, 180);
                        tempBox.transform.position = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                    }
                    else
                        tempBox.transform.position = new Vector3(x * .5f - width / 2, y * .8f - height / 2, 0);
                }
                else if (shape == 4)
                {
                    //square
                    tempBox.transform.position = new Vector3(x - width / 2, y - height / 2, 0);
                }
                else if(shape == 6)
                {
                    //hexagon pattern
                    if (x % 2 == 1)
                        tempBox.transform.position = new Vector3(x * .8f - width / 2, y * .9f - .25f - (height / 2 * .75f) - 1.5f, 0);
                    else
                        tempBox.transform.position = new Vector3(x * .8f - width / 2, y * .9f + .25f - (height / 2 * .75f) - 1.5f, 0);
                }
            }
        }
        
        //rest buttons
        GameController.GameControllerSingle.buttons = new List<GameObject>();

        for (int y = 0; y < numberColors; y++)
        {
            var tempBox = Instantiate(ButtonSquarePrefab, ColorButtonPanel.transform);
            tempBox.GetComponent<Image>().color = PickColor(y);
            tempBox.GetComponent<Button>().onClick.AddListener(() => GameController.GameControllerSingle.ButtonClick(tempBox.GetComponent<Image>().color));
            GameController.GameControllerSingle.buttons.Add(tempBox);
            //tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
            //tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
            ////set position from panel not world
            //tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
        }

        StartConitions();
    }

    private void StartConitions()
    {
        //clear if this is game reset
        GameController.GameControllerSingle.Players = new List<GameController.Player>();
        startLocations = new List<Vector2>();

        //possible player starts
        startLocations.Add(new Vector2(0, 0));
        startLocations.Add(new Vector2(width - 1, height - 1));
        startLocations.Add(new Vector2(0, height - 1));
        startLocations.Add(new Vector2(width - 1, 0));

        for (int x = 0; x < numPlayers; x++)
        {
            int ranStart = Random.Range(0, startLocations.Count);

            GameController.GameControllerSingle.AddPlayer();
            GameController.GameControllerSingle.Players[x].playerReachable.Add(startLocations[ranStart]);
            gameBoard[(int)startLocations[ranStart].x, (int)startLocations[ranStart].y].GetComponent<SpriteRenderer>().color = Color.black;
            //stop player from being in same spot
            startLocations.RemoveAt(ranStart);
        }
    }

    public void RestartGame()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int color = Random.Range(0, numberColors);
                gameBoard[x,y].GetComponent<SpriteRenderer>().color = PickColor(color);
            }
        }
        StartConitions();
    }

    public void NewGame()
    {
        //remove all old shapes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Destroy(gameBoard[x, y].GetComponent<SpriteRenderer>().gameObject);
            }
        }
        
        //remove all old button colors
        foreach(Transform button in ColorButtonPanel.transform)
        {
            Destroy(button.gameObject);
        }

        //restart game by call start methods
        //StartGameCondition(5, 5, 5, 2, 4);
        //StartGameGeneration();

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
    }


    public void ChangeHeight()
    {
        int newHeight;
        bool temp = int.TryParse(GameObject.Find("HeightInputField").GetComponent<InputField>().textComponent.text, out newHeight);
        if (temp)
        {
            height = newHeight;
        }
    }

    public void ChangeNumberColors()
    {
        int numColors;
        bool temp = int.TryParse(GameObject.Find("ColorInputField").GetComponent<InputField>().textComponent.text, out numColors);
        if (temp)
        {
            numberColors = numColors;
        }
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
