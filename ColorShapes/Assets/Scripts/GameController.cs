using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public List<GameObject> buttons;

    public List<Player> Players;
    public int numberActive = 0;
    public int turn = 0;

    //passing by ref not value; cant be a stuct
    public class Player
    {
        public HashSet<Vector2> playerVisited;
        public List<Vector2> playerReachable;
        public Queue<Vector2> playerQ;
        public Color CurrentColor;

        public Player(HashSet<Vector2> pv, List<Vector2> pr, Queue<Vector2> pq, Color cc)
        {
            playerVisited = pv;
            playerReachable = pr;
            playerQ = pq;
            CurrentColor = cc;
        }
    }

    public void AddPlayer()
    {
        HashSet<Vector2> pv = new HashSet<Vector2>();
        List<Vector2> pr = new List<Vector2>();
        Queue<Vector2> pq = new Queue<Vector2>();
        Color StartColor = Color.black;
        Player newPlayer = new Player(pv, pr, pq, StartColor);
        Players.Add(newPlayer);
    }

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
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    ButtonClick(Color.red, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    ButtonClick(Color.yellow, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    ButtonClick(Color.green, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    ButtonClick(Color.cyan, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    ButtonClick(Color.blue, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha6))
            //{
            //    ButtonClick(Color.magenta, Players[turn]);
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha7))
            //{
            //    ButtonClick(Color.white, Players[turn]);
            //}
        }
    }

    public void ButtonClick(Color buttonColor)
    {
        Player CurPlayer = Players[turn];

        //make sure another play doesnt have that color
        bool colorPickValid = true;
        foreach(Player aPlayer in Players)
        {
            if(buttonColor == aPlayer.CurrentColor)
            {
                colorPickValid = false;
                break;
            }
        }
        if (colorPickValid)
        {
            List<Vector2> tempReach = new List<Vector2>();

            //check all reachable squares for correct color
            foreach (Vector2 vec in CurPlayer.playerReachable)
            {
                //or black for first square
                if (GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color == buttonColor || GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color == Color.black)
                {
                    tempReach.Add(vec);
                    CurPlayer.playerVisited.Add(vec);
                }
            }

            //place correct color squares in queue
            foreach (Vector2 vec in tempReach)
            {
                CurPlayer.playerQ.Enqueue(vec);
            }

            //check all correct color squares for their neighbors that are same color //dijstras
            while (CurPlayer.playerQ.Count != 0)
            {
                Vector2 currentSquare = CurPlayer.playerQ.Dequeue();
                Vector2 temp;

                bool right = currentSquare.x + 1 < GenerateGame.GenerateGameSingle.width;
                bool left = currentSquare.x - 1 >= 0;
                bool up = currentSquare.y + 1 < GenerateGame.GenerateGameSingle.height;
                bool down = currentSquare.y - 1 >= 0;

                //right
                if (right)
                {
                    temp = new Vector2(currentSquare.x + 1, currentSquare.y);
                    //color correct
                    if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach(Player player in Players)
                        {
                            if (player.playerVisited.Contains(temp))
                            {
                                visit = false;
                                break;
                            }
                        }
                        if (visit)
                        {
                            CurPlayer.playerQ.Enqueue(temp);
                            CurPlayer.playerVisited.Add(temp);
                            CurPlayer.playerReachable.Remove(temp);
                        }

                    }
                    //if color not correct add to reachable 
                    else if (!CurPlayer.playerReachable.Contains(temp))
                    {
                        CurPlayer.playerReachable.Add(temp);
                    }
                }

                //left
                if (left)
                {
                    temp = new Vector2(currentSquare.x - 1, currentSquare.y);
                    if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Player player in Players)
                        {
                            if (player.playerVisited.Contains(temp))
                            {
                                visit = false;
                                break;
                            }
                        }
                        if (visit)
                        {
                            CurPlayer.playerQ.Enqueue(temp);
                            CurPlayer.playerVisited.Add(temp);
                            CurPlayer.playerReachable.Remove(temp);
                        }
                    }
                    else if (!CurPlayer.playerReachable.Contains(temp))
                    {
                        CurPlayer.playerReachable.Add(temp);
                    }
                }

                //up
                if (up && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 1))
                {
                    temp = new Vector2(currentSquare.x, currentSquare.y + 1);
                    if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Player player in Players)
                        {
                            if (player.playerVisited.Contains(temp))
                            {
                                visit = false;
                                break;
                            }
                        }
                        if (visit)
                        {
                            CurPlayer.playerQ.Enqueue(temp);
                            CurPlayer.playerVisited.Add(temp);
                            CurPlayer.playerReachable.Remove(temp);
                        }
                    }
                    else if (!CurPlayer.playerReachable.Contains(temp))
                    {
                        CurPlayer.playerReachable.Add(temp);
                    }
                }

                //down
                if (down && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 0))
                {
                    temp = new Vector2(currentSquare.x, currentSquare.y - 1);
                    if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Player player in Players)
                        {
                            if (player.playerVisited.Contains(temp))
                            {
                                visit = false;
                                break;
                            }
                        }
                        if (visit)
                        {
                            CurPlayer.playerQ.Enqueue(temp);
                            CurPlayer.playerVisited.Add(temp);
                            CurPlayer.playerReachable.Remove(temp);
                        }
                    }
                    else if (!CurPlayer.playerReachable.Contains(temp))
                    {
                        CurPlayer.playerReachable.Add(temp);
                    }
                }

                //if hexagon shape
                if (GenerateGame.GenerateGameSingle.shape == 6)
                {
                    //if even x; up right; left up
                    if(currentSquare.x % 2 == 0)
                    {
                        if (up && right)
                        {
                            temp = new Vector2(currentSquare.x + 1, currentSquare.y + 1);
                            if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Player player in Players)
                                {
                                    if (player.playerVisited.Contains(temp))
                                    {
                                        visit = false;
                                        break;
                                    }
                                }
                                if (visit)
                                {
                                    CurPlayer.playerQ.Enqueue(temp);
                                    CurPlayer.playerVisited.Add(temp);
                                    CurPlayer.playerReachable.Remove(temp);
                                }
                            }
                            else if (!CurPlayer.playerReachable.Contains(temp))
                            {
                                CurPlayer.playerReachable.Add(temp);
                            }
                        }
                        if (left && up)
                        {
                            temp = new Vector2(currentSquare.x - 1, currentSquare.y + 1);
                            if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Player player in Players)
                                {
                                    if (player.playerVisited.Contains(temp))
                                    {
                                        visit = false;
                                        break;
                                    }
                                }
                                if (visit)
                                {
                                    CurPlayer.playerQ.Enqueue(temp);
                                    CurPlayer.playerVisited.Add(temp);
                                    CurPlayer.playerReachable.Remove(temp);
                                }
                            }
                            else if (!CurPlayer.playerReachable.Contains(temp))
                            {
                                CurPlayer.playerReachable.Add(temp);
                            }
                        }
                    }
                    //if odd x; down left; right down
                    else
                    {
                        if (down && left)
                        {
                            temp = new Vector2(currentSquare.x - 1, currentSquare.y - 1);
                            if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Player player in Players)
                                {
                                    if (player.playerVisited.Contains(temp))
                                    {
                                        visit = false;
                                        break;
                                    }
                                }
                                if (visit)
                                {
                                    CurPlayer.playerQ.Enqueue(temp);
                                    CurPlayer.playerVisited.Add(temp);
                                    CurPlayer.playerReachable.Remove(temp);
                                }
                            }
                            else if (!CurPlayer.playerReachable.Contains(temp))
                            {
                                CurPlayer.playerReachable.Add(temp);
                            }
                        }
                        if (right && down)
                        {
                            temp = new Vector2(currentSquare.x + 1, currentSquare.y - 1);
                            if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Player player in Players)
                                {
                                    if (player.playerVisited.Contains(temp))
                                    {
                                        visit = false;
                                        break;
                                    }
                                }
                                if (visit)
                                {
                                    CurPlayer.playerQ.Enqueue(temp);
                                    CurPlayer.playerVisited.Add(temp);
                                    CurPlayer.playerReachable.Remove(temp);
                                }
                            }
                            else if (!CurPlayer.playerReachable.Contains(temp))
                            {
                                CurPlayer.playerReachable.Add(temp);
                            }
                        }
                    }
                }
            }




            //change color all squares player owns
            foreach (Vector2 vec in CurPlayer.playerVisited)
            {
                GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color = buttonColor;
            }

            //next turn
            turn += 1;
            turn = turn % GenerateGame.GenerateGameSingle.numPlayers;
            //set currPlayer Color
            CurPlayer.CurrentColor = buttonColor;

            ChangeButtonOptions();
        }
    }

    void ChangeButtonOptions()
    {
        numberActive = 0;
        foreach (GameObject cb in buttons)
        {
            cb.SetActive(true);
            foreach (Player cp in Players)
            {
                if (cb.GetComponent<Image>().color == cp.CurrentColor)
                {
                    cb.SetActive(false);
                    break;
                }
            }

            //cb.transform.localPosition = new Vector3(0,GenerateGame.GenerateGameSingle.ColorButtonPanel.GetComponent<SpriteRenderer>().size.y / 2f - .5f - numberActive, 0);
            //if (cb.activeSelf)
            //    numberActive += 1;
        }
    }

    public void RestartGame()
    {
        //GenerateGame.GenerateGameSingle.StartGameCondition(10, 10, 7, 4, 6);
        //Players = new List<Player>();
        GenerateGame.GenerateGameSingle.RestartGame();
        turn = Random.Range(0, GenerateGame.GenerateGameSingle.numPlayers);
        foreach (GameObject cb in buttons)
        {
            cb.SetActive(true);
        }
    }

    public void NewGame()
    {
        turn = -1;
        GenerateGame.GenerateGameSingle.NewGame();
    }
}
