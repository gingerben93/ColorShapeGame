using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public List<GameObject> buttons = new List<GameObject>();

    public List<Player> Players = new List<Player>();

    public struct Player
    {
        public HashSet<Vector2> playerVisited;
        public List<Vector2> playerReachable;
        public Queue<Vector2> playerQ;

        public Player(HashSet<Vector2> pv, List<Vector2> pr, Queue<Vector2> pq)
        {
            playerVisited = pv;
            playerReachable = pr;
            playerQ = pq;
        }
    }

    public void AddPlayer()
    {
        HashSet<Vector2> pv = new HashSet<Vector2>();
        List<Vector2> pr = new List<Vector2>();
        Queue<Vector2> pq = new Queue<Vector2>();
        Player newPlayer = new Player(pv, pr, pq);
        Players.Add(newPlayer);
    }

    int turn = 0;

    public static GameController GameControllerSingle;

    //create singleton of class
    void Awake()
    {
        if (GameControllerSingle == null)
        {
            DontDestroyOnLoad(gameObject);
            GameControllerSingle = this;
        }
        else if (GameControllerSingle != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {

        if(turn != -1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ButtonClick(Color.red, Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ButtonClick(Color.yellow, Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ButtonClick(Color.green, Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ButtonClick(Color.cyan, Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ButtonClick(Color.blue, Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ);
            }
        }
    }

    void ButtonClick(Color buttonColor, HashSet<Vector2> visited, List<Vector2> reachable, Queue<Vector2> playerQ)
    {
        List<Vector2> tempReach = new List<Vector2>();

        //check all reachable squares for correct color
        foreach (Vector2 vec in reachable)
        {
            //or black for first square
            if (GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color == buttonColor || GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color == Color.black)
            {
                tempReach.Add(vec);
                visited.Add(vec);
            }
        }

        //place correct color squares in queue
        foreach (Vector2 vec in tempReach)
        {
            playerQ.Enqueue(vec);
        }

        //check all correct color squares for their neighbors that are same color //dijstras
        while (playerQ.Count != 0)
        {
            Vector2 currentSquare = playerQ.Dequeue();
            Vector2 temp;

            //right
            if (currentSquare.x + 1 < GenerateGame.GenerateGameSingle.width)
            {
                temp = new Vector2(currentSquare.x + 1, currentSquare.y);
                //color correct
                if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                {
                    //if not in visited then put there
                    if (!visited.Contains(temp))
                    {
                        playerQ.Enqueue(temp);
                        visited.Add(temp);
                        reachable.Remove(temp);
                    }
                }
                //if color not correct add to reachable 
                else if (!reachable.Contains(temp))
                {
                    reachable.Add(temp);
                }
            }

            //left
            if (currentSquare.x - 1 >= 0)
            {
                temp = new Vector2(currentSquare.x - 1, currentSquare.y);
                if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                {
                    if (!visited.Contains(temp))
                    {
                        playerQ.Enqueue(temp);
                        visited.Add(temp);
                        reachable.Remove(temp);
                    }
                }
                else if (!reachable.Contains(temp))
                {
                    reachable.Add(temp);
                }
            }

            //up
            if (currentSquare.y + 1 < GenerateGame.GenerateGameSingle.height)
            {
                temp = new Vector2(currentSquare.x, currentSquare.y + 1);
                if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                {
                    if (!visited.Contains(temp))
                    {
                        playerQ.Enqueue(temp);
                        visited.Add(temp);
                        reachable.Remove(temp);
                    }
                }
                else if (!reachable.Contains(temp))
                {
                    reachable.Add(temp);
                }
            }

            //down
            if (currentSquare.y - 1 >= 0)
            {
                temp = new Vector2(currentSquare.x, currentSquare.y - 1);
                if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                {
                    if (!visited.Contains(temp))
                    {
                        playerQ.Enqueue(temp);
                        visited.Add(temp);
                        reachable.Remove(temp);
                    }
                }
                else if (!reachable.Contains(temp))
                {
                    reachable.Add(temp);
                }
            }
        }

        //change color all squares player owns
        foreach (Vector2 vec in visited)
        {
            GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color = buttonColor;
        }

        //next turn
        turn += 1;
        turn = turn % GenerateGame.GenerateGameSingle.numPlayers;
    }
}
