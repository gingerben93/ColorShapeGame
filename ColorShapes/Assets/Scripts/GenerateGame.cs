using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGame : MonoBehaviour {

    public GameObject[,] gameBoard;
    public int width;
    public int height;

    public int numberColors;
    public int numPlayers;

    private List<Vector2> startLocations = new List<Vector2>();

    public GameObject BoxPrefab;

    public static GenerateGame GenerateGameSingle;
    //create singleton of class
    void Awake()
    {
        if (GenerateGameSingle == null)
        {
            DontDestroyOnLoad(gameObject);
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
        //start variables
        width = 50;
        height = 10;
        numberColors = 5;
        numPlayers = 2;
        gameBoard = new GameObject[width, height];

        StartGameGeneration();

    }

    // Update is called once per frame
    void Update () {

		
	}

    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    private void StartGameGeneration()
    {
        //makes grid; Instantiate color square
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int color = Random.Range(0, numberColors);
                
                var tempBox = Instantiate(BoxPrefab);
                gameBoard[x, y] = tempBox;
                tempBox.GetComponent<SpriteRenderer>().color = PickColor(color);
                tempBox.transform.position = new Vector3(x - width / 2, y - height / 2, 0);

                //hexagon pattern
                //if (x % 2 == 1)
                //    tempBox.transform.position = new Vector3(x - width / 2, y - height / 2 + .25f, 0);
                //else
                //    tempBox.transform.position = new Vector3(x - width / 2, y - height / 2 - .25f, 0);

            }
        }

        //make player Buttons
        GameObject ColorButtonPanel = GameObject.Find("ColorButtonPanel");
        for (int y = 0; y < numberColors; y++)
        {
            var tempBox = Instantiate(BoxPrefab, ColorButtonPanel.transform);
            GameController.GameControllerSingle.buttons.Add(tempBox);
            tempBox.GetComponent<SpriteRenderer>().color = PickColor(y);
            tempBox.GetComponent<SpriteRenderer>().size = new Vector2(4, 1);
            //set position from panel not world
            tempBox.transform.localPosition = new Vector3(0, ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - y, 0);
        }

        StartConitions();
    }

    private void StartConitions()
    {
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
