using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public List<GameObject> buttons;
    public GameObject localPlayer;
    public GameObject SinglePlayerPrefab;

    public List<Player> Players;
    public int turn = 0;

    public CanvasGroup CurrentMenu;
    public CanvasGroup Menu1;
    public CanvasGroup Menu2;
    public CanvasGroup Menu3;

    //passing by ref not value; cant be a stuct
    public class Player
    {
        public HashSet<Vector2> playerEdge;
        public HashSet<Vector2> playerReachable;
        public HashSet<Vector2> playerNotEdge;
        public Color CurrentColor;
        public GameObject ScoreObj;
        public int turnOrder;

        public Player(HashSet<Vector2> pe, HashSet<Vector2> pr, HashSet<Vector2> pne, Color cc, GameObject so, int t)
        {
            playerEdge = pe;
            playerReachable = pr;
            playerNotEdge = pne;
            CurrentColor = cc;
            ScoreObj = so;
            turnOrder = t;
        }
    }

    public void AddPlayer(Vector2 Start, GameObject so, int t)
    {
        HashSet<Vector2> pe = new HashSet<Vector2>();
        HashSet<Vector2> pr = new HashSet<Vector2>();
        HashSet<Vector2> pne = new HashSet<Vector2>();
        Color StartColor = Color.black;
        Player newPlayer = new Player(pe, pr, pne, StartColor, so, t);
        newPlayer.playerEdge.Add(Start);
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
        //Players = new List<Player>();
        Menu1 = GameObject.Find("Menu1").GetComponent<CanvasGroup>();
        Menu2 = GameObject.Find("Menu2").GetComponent<CanvasGroup>();
        Menu3 = GameObject.Find("Menu3").GetComponent<CanvasGroup>();
        GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => PlayGameButton());
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitButton());
        GameObject.Find("OnlineButton").GetComponent<Button>().onClick.AddListener(() => PlayOnlineButton());
        GameObject.Find("ReturnToMainMenuButton").GetComponent<Button>().onClick.AddListener(() => BackButton());

        CurrentMenu = Menu1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //cant toggle if main menu or online menu
        if (Input.GetKeyDown(KeyCode.Escape) && (CurrentMenu != Menu1 && CurrentMenu != Menu3))
        {
            //toggle menu on and off
            CurrentMenu.alpha = (CurrentMenu.alpha + 1) % 2;
            CurrentMenu.interactable = !CurrentMenu.interactable;
            CurrentMenu.blocksRaycasts =!CurrentMenu.blocksRaycasts;
        }
    }

    void PlayGameButton()
    {
        //set button to offline response
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitButton());
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => NetworkManager.singleton.StopClient());

        Instantiate(SinglePlayerPrefab);

        //turn off current menu
        CurrentMenu.alpha = 0;
        CurrentMenu.interactable = false;
        CurrentMenu.blocksRaycasts = false;

        //change current menu to quit menu
        CurrentMenu = Menu2;

        //turn on menu to play
        GenerateGame.GenerateGameSingle.EndGamePanel.alpha = 1;
        GenerateGame.GenerateGameSingle.EndGamePanel.interactable = true;
        GenerateGame.GenerateGameSingle.EndGamePanel.blocksRaycasts = true;
    }

    public void QuitButton()
    {
        Destroy(localPlayer);

        //turn off current menu
        CurrentMenu.alpha = 0;
        CurrentMenu.interactable = false;
        CurrentMenu.blocksRaycasts = false;

        //change menu to select menu
        CurrentMenu = Menu1;

        //turn on current menu
        CurrentMenu.alpha = 1;
        CurrentMenu.interactable = true;
        CurrentMenu.blocksRaycasts = true;

        //turn off used game assests
        GenerateGame.GenerateGameSingle.NewGame();

        //turn off menu to play
        GenerateGame.GenerateGameSingle.EndGamePanel.alpha = 0;
        GenerateGame.GenerateGameSingle.EndGamePanel.interactable = false;
        GenerateGame.GenerateGameSingle.EndGamePanel.blocksRaycasts = false;

        //turn off menu to play
        GenerateGame.GenerateGameSingle.StartGamePanel.alpha = 0;
        GenerateGame.GenerateGameSingle.StartGamePanel.interactable = false;
        GenerateGame.GenerateGameSingle.StartGamePanel.blocksRaycasts = false;

    }

    void PlayOnlineButton()
    {
        //turn off current menu
        CurrentMenu.alpha = 0;
        CurrentMenu.interactable = false;
        CurrentMenu.blocksRaycasts = false;

        //set to online menu
        CurrentMenu = Menu3;

        //turn on current menu
        CurrentMenu.alpha = 1;
        CurrentMenu.interactable = true;
        CurrentMenu.blocksRaycasts = true;
    }

    void BackButton()
    {
        //turn off current menu
        CurrentMenu.alpha = 0;
        CurrentMenu.interactable = false;
        CurrentMenu.blocksRaycasts = false;

        //set to online menu
        CurrentMenu = Menu1;

        //turn on current menu
        CurrentMenu.alpha = 1;
        CurrentMenu.interactable = true;
        CurrentMenu.blocksRaycasts = true;
    }

    public void startOnlineButton()
    {
        //turn off current menu
        CurrentMenu.alpha = 0;
        CurrentMenu.interactable = false;
        CurrentMenu.blocksRaycasts = false;

        //change current menu to quit menu
        CurrentMenu = Menu2;

        //turn on menu to play
        GenerateGame.GenerateGameSingle.EndGamePanel.alpha = 1;
        GenerateGame.GenerateGameSingle.EndGamePanel.interactable = true;
        GenerateGame.GenerateGameSingle.EndGamePanel.blocksRaycasts = true;
    }

    public void Neighbor(int xOffSet, int yOffSet, ref int tempCountSides, ref int TotalAmountOneColor, Color buttonColor, Vector2 currentSquare, ref Queue<Vector2> pq, ref HashSet<Vector2> playerEdge, ref HashSet<Vector2> playerReachable)
    {
        Vector2 temp;
        temp = new Vector2(currentSquare.x + xOffSet, currentSquare.y + yOffSet);
        //color correct
        if (playerEdge.Contains(temp))
        {
            tempCountSides += 1;
            //do nothing if visited already 
        }
        else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + xOffSet, (int)currentSquare.y + yOffSet].GetComponent<SpriteRenderer>().color == buttonColor)
        {
            //if not in visited then put there
            bool visit = true;
            foreach (Player player in Players)
            {
                if (player.playerEdge.Contains(temp))
                {
                    visit = false;
                    break;
                }
            }
            if (visit)
            {
                TotalAmountOneColor += 1;
                pq.Enqueue(temp);
                playerEdge.Add(temp);
                playerReachable.Remove(temp);
            }
        }
        //if color not correct add to reachable 
        else if (!playerReachable.Contains(temp))
        {
            playerReachable.Add(temp);
        }
    }

    public void ButtonClick(Color buttonColor)
    {
        if (turn == -1)
            return;
        Player CurPlayer = Players[turn];
        Queue<Vector2> pq = new Queue<Vector2>();
        //Player CurPlayer = new Player(Players[turn].playerVisited, Players[turn].playerReachable, Players[turn].playerQ, Players[turn].CurrentColor, Players[turn].ScoreObj, Players[turn].turnOrder);


        //make sure another play doesnt have that color; kind of useless becuase buttons get turned off
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
            //place correct color squares in queue
            foreach (Vector2 vec in CurPlayer.playerEdge)
            {
                pq.Enqueue(vec);
            }

            int TotalAmountOneColor = 0;
            //check all correct color squares for their neighbors that are same color //dijstras
            while (pq.Count != 0)
            {
                Vector2 currentSquare = pq.Dequeue();
                int tempCountSides = 0;

                bool right = currentSquare.x + 1 < GenerateGame.GenerateGameSingle.width;
                bool left = currentSquare.x - 1 >= 0;
                bool up = currentSquare.y + 1 < GenerateGame.GenerateGameSingle.height;
                bool down = currentSquare.y - 1 >= 0;

                if (GenerateGame.GenerateGameSingle.shape == 3)
                {
                    if (right)
                    {
                        Neighbor(1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (left)
                    {
                        Neighbor(-1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if(up && (currentSquare.x + currentSquare.y) % 2 == 1)
                    {
                        Neighbor(0, 1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (down && (currentSquare.x + currentSquare.y) % 2 == 0)
                    {
                        Neighbor(0, -1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                }
                else if (GenerateGame.GenerateGameSingle.shape == 4)
                {
                    if (right)
                    {
                        Neighbor(1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (left)
                    {
                        Neighbor(-1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (up)
                    {
                        Neighbor(0, 1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (down)
                    {
                        Neighbor(0, -1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                }
                else if (GenerateGame.GenerateGameSingle.shape == 6)
                {
                    if (right)
                    {
                        Neighbor(1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (left)
                    {
                        Neighbor(-1, 0, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (up)
                    {
                        Neighbor(0, 1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (down)
                    {
                        Neighbor(0, -1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                    }
                    if (currentSquare.x % 2 == 0)
                    {
                        if (up && right)
                        {
                            Neighbor(1, 1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                        }
                        if (up && left)
                        {
                            Neighbor(-1, 1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                        }
                    }
                    else if (currentSquare.x % 2 == 1)
                    {
                        if (down && right)
                        {
                            Neighbor(1, -1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                        }
                        if (down && left)
                        {
                            Neighbor(-1, -1, ref tempCountSides, ref TotalAmountOneColor, buttonColor, currentSquare, ref pq, ref CurPlayer.playerEdge, ref CurPlayer.playerReachable);
                        }
                    }
                }

                ////right
                //if (right)
                //{
                //    temp = new Vector2(currentSquare.x + 1, currentSquare.y);
                //    //color correct
                //    if (CurPlayer.playerEdge.Contains(temp))
                //    {
                //        tempCountSides += 1;
                //        //do nothing if visited already 
                //    }
                //    else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                //    {
                //        //if not in visited then put there
                //        bool visit = true;
                //        foreach (Player player in Players)
                //        {
                //            if (player.playerEdge.Contains(temp))
                //            {
                //                visit = false;
                //                break;
                //            }
                //        }
                //        if (visit)
                //        {
                //            TotalAmountOneColor += 1;
                //            pq.Enqueue(temp);
                //            CurPlayer.playerEdge.Add(temp);
                //            CurPlayer.playerReachable.Remove(temp);
                //        }

                //    }
                //    //if color not correct add to reachable 
                //    else if (!CurPlayer.playerReachable.Contains(temp))
                //    {
                //        CurPlayer.playerReachable.Add(temp);
                //    }
                //}

                ////left
                //if (left)
                //{
                //    temp = new Vector2(currentSquare.x - 1, currentSquare.y);
                //    if (CurPlayer.playerEdge.Contains(temp))
                //    {
                //        tempCountSides += 1;
                //        //do nothing if visited already 
                //    }
                //    else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y].GetComponent<SpriteRenderer>().color == buttonColor)
                //    {
                //        //if not in visited then put there
                //        bool visit = true;
                //        foreach (Player player in Players)
                //        {
                //            if (player.playerEdge.Contains(temp))
                //            {
                //                visit = false;
                //                break;
                //            }
                //        }
                //        if (visit)
                //        {
                //            TotalAmountOneColor += 1;
                //            pq.Enqueue(temp);
                //            CurPlayer.playerEdge.Add(temp);
                //            CurPlayer.playerReachable.Remove(temp);
                //        }
                //    }
                //    else if (!CurPlayer.playerReachable.Contains(temp))
                //    {
                //        CurPlayer.playerReachable.Add(temp);
                //    }
                //}

                ////up
                //if (up && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 1))
                //{
                //    temp = new Vector2(currentSquare.x, currentSquare.y + 1);
                //    if (CurPlayer.playerEdge.Contains(temp))
                //    {
                //        tempCountSides += 1;
                //        //do nothing if visited already 
                //    }
                //    else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //    {
                //        //if not in visited then put there
                //        bool visit = true;
                //        foreach (Player player in Players)
                //        {
                //            if (player.playerEdge.Contains(temp))
                //            {
                //                visit = false;
                //                break;
                //            }
                //        }
                //        if (visit)
                //        {
                //            TotalAmountOneColor += 1;
                //            pq.Enqueue(temp);
                //            CurPlayer.playerEdge.Add(temp);
                //            CurPlayer.playerReachable.Remove(temp);
                //        }
                //    }
                //    else if (!CurPlayer.playerReachable.Contains(temp))
                //    {
                //        CurPlayer.playerReachable.Add(temp);
                //    }
                //}

                ////down
                //if (down && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 0))
                //{
                //    temp = new Vector2(currentSquare.x, currentSquare.y - 1);
                //    if (CurPlayer.playerEdge.Contains(temp))
                //    {
                //        tempCountSides += 1;
                //        //do nothing if visited already 
                //    }
                //    else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //    {
                //        //if not in visited then put there
                //        bool visit = true;
                //        foreach (Player player in Players)
                //        {
                //            if (player.playerEdge.Contains(temp))
                //            {
                //                visit = false;
                //                break;
                //            }
                //        }
                //        if (visit)
                //        {
                //            TotalAmountOneColor += 1;
                //            pq.Enqueue(temp);
                //            CurPlayer.playerEdge.Add(temp);
                //            CurPlayer.playerReachable.Remove(temp);
                //        }
                //    }
                //    else if (!CurPlayer.playerReachable.Contains(temp))
                //    {
                //        CurPlayer.playerReachable.Add(temp);
                //    }
                //}


                ////if hexagon shape
                //if (GenerateGame.GenerateGameSingle.shape == 6)
                //{
                //    //if even x; up right; left up
                //    if(currentSquare.x % 2 == 0)
                //    {
                //        if (up && right)
                //        {
                //            temp = new Vector2(currentSquare.x + 1, currentSquare.y + 1);

                //            if (CurPlayer.playerEdge.Contains(temp))
                //            {
                //                //do nothing if visited already 
                //            }
                //            else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //            {
                //                //if not in visited then put there
                //                bool visit = true;
                //                foreach (Player player in Players)
                //                {
                //                    if (player.playerEdge.Contains(temp))
                //                    {
                //                        visit = false;
                //                        break;
                //                    }
                //                }
                //                if (visit)
                //                {
                //                    pq.Enqueue(temp);
                //                    CurPlayer.playerEdge.Add(temp);
                //                    CurPlayer.playerReachable.Remove(temp);
                //                }
                //            }
                //            else if (!CurPlayer.playerReachable.Contains(temp))
                //            {
                //                CurPlayer.playerReachable.Add(temp);
                //            }
                //        }
                //        if (left && up)
                //        {
                //            temp = new Vector2(currentSquare.x - 1, currentSquare.y + 1);
                //            if (CurPlayer.playerEdge.Contains(temp))
                //            {
                //                //do nothing if visited already 
                //            }
                //            else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y + 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //            {
                //                //if not in visited then put there
                //                bool visit = true;
                //                foreach (Player player in Players)
                //                {
                //                    if (player.playerEdge.Contains(temp))
                //                    {
                //                        visit = false;
                //                        break;
                //                    }
                //                }
                //                if (visit)
                //                {
                //                    pq.Enqueue(temp);
                //                    CurPlayer.playerEdge.Add(temp);
                //                    CurPlayer.playerReachable.Remove(temp);
                //                }
                //            }
                //            else if (!CurPlayer.playerReachable.Contains(temp))
                //            {
                //                CurPlayer.playerReachable.Add(temp);
                //            }
                //        }
                //    }
                //    //if odd x; down left; right down
                //    else if(currentSquare.x % 2 == 1)
                //    {
                //        if (down && left)
                //        {
                //            temp = new Vector2(currentSquare.x - 1, currentSquare.y - 1);
                //            if (CurPlayer.playerEdge.Contains(temp))
                //            {
                //                //do nothing if visited already 
                //            }
                //            else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x - 1, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //            {
                //                //if not in visited then put there
                //                bool visit = true;
                //                foreach (Player player in Players)
                //                {
                //                    if (player.playerEdge.Contains(temp))
                //                    {
                //                        visit = false;
                //                        break;
                //                    }
                //                }
                //                if (visit)
                //                {
                //                    pq.Enqueue(temp);
                //                    CurPlayer.playerEdge.Add(temp);
                //                    CurPlayer.playerReachable.Remove(temp);
                //                }
                //            }
                //            else if (!CurPlayer.playerReachable.Contains(temp))
                //            {
                //                CurPlayer.playerReachable.Add(temp);
                //            }
                //        }
                //        if (right && down)
                //        {
                //            temp = new Vector2(currentSquare.x + 1, currentSquare.y - 1);
                //            if (CurPlayer.playerEdge.Contains(temp))
                //            {
                //                //do nothing if visited already 
                //            }
                //            else if (GenerateGame.GenerateGameSingle.gameBoard[(int)currentSquare.x + 1, (int)currentSquare.y - 1].GetComponent<SpriteRenderer>().color == buttonColor)
                //            {
                //                //if not in visited then put there
                //                bool visit = true;
                //                foreach (Player player in Players)
                //                {
                //                    if (player.playerEdge.Contains(temp))
                //                    {
                //                        visit = false;
                //                        break;
                //                    }
                //                }
                //                if (visit)
                //                {
                //                    pq.Enqueue(temp);
                //                    CurPlayer.playerEdge.Add(temp);
                //                    CurPlayer.playerReachable.Remove(temp);
                //                }
                //            }
                //            else if (!CurPlayer.playerReachable.Contains(temp))
                //            {
                //                CurPlayer.playerReachable.Add(temp);
                //            }
                //        }
                //    }
                //}
                if(tempCountSides == GenerateGame.GenerateGameSingle.shape)
                {
                    CurPlayer.playerEdge.Remove(currentSquare);
                    CurPlayer.playerNotEdge.Add(currentSquare);
                }
            }

            //change color all squares player owns
            foreach (Vector2 vec in CurPlayer.playerEdge)
            {
                GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color = buttonColor;
            }
            foreach (Vector2 vec in CurPlayer.playerNotEdge)
            {
                GenerateGame.GenerateGameSingle.gameBoard[(int)vec.x, (int)vec.y].GetComponent<SpriteRenderer>().color = buttonColor;
            }

            float tempScore = (CurPlayer.playerEdge.Count + CurPlayer.playerNotEdge.Count) / ((float)GenerateGame.GenerateGameSingle.width * (float)GenerateGame.GenerateGameSingle.height);
            CurPlayer.ScoreObj.GetComponent<Text>().text = "Player " + turn + " Score: " + (tempScore*100).ToString("00") + "%";

            if (tempScore >= 1f/GenerateGame.GenerateGameSingle.numPlayers)
            {
                GenerateGame.GenerateGameSingle.endGameText.enabled = true;
                GenerateGame.GenerateGameSingle.endGameText.text = "Player " + turn + " Wins!";
                turn = -1;
            }
            else
            {
                //next turn
                turn += 1;
                turn = turn % GenerateGame.GenerateGameSingle.numPlayers;
                //set currPlayer Color
                CurPlayer.CurrentColor = buttonColor;

                ChangeButtonOptions();
            }
        }
    }

    void ChangeButtonOptions()
    {
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
        }
    }

    //public void RestartGame()
    //{
    //    turn = 0;
    //    //turns all buttons on
    //    foreach (GameObject cb in buttons)
    //    {
    //        cb.SetActive(true);
    //    }
    //}

    public void NewGame()
    {
        turn = -1;
        GenerateGame.GenerateGameSingle.NewGame();
    }
}
