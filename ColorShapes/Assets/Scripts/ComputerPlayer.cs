using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerPlayer : MonoBehaviour {


    //passing by ref not value; cant be a stuct
    public class Computer
    {
        public HashSet<Vector2> playerEdge;
        public HashSet<Vector2> playerReachable;
        public HashSet<Vector2> playerNotEdge;
        public Color currentColor;
        public float score; 

        public Computer(HashSet<Vector2> pv, HashSet<Vector2> pr, HashSet<Vector2> pne, Color cc, float s)
        {
            playerEdge = new HashSet<Vector2>(pv);
            playerReachable = new HashSet<Vector2>(pr);
            playerNotEdge = new HashSet<Vector2>(pne);
            currentColor = cc;
            score = s;
        }
    }

    bool hardComputer = false;

    // Use this for initialization
    void Start ()
    {
        

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //EasyComputer(GameController.GameControllerSingle.Players[GameController.GameControllerSingle.turn]);
            hardComputer = true;
            //HardComputerStart();
        }
        //if (easyComputer && GameController.GameControllerSingle.turn != -1)
        //{
        //    MediumComputer(GameController.GameControllerSingle.Players[GameController.GameControllerSingle.turn]);
        //}
     if (hardComputer && GameController.GameControllerSingle.turn != -1)
        {
            HardComputerStart();
        }
    }

    public void HardComputerStart()
    {
        List<Color> pickOptions = new List<Color>();
        List<Computer> Computers = new List<Computer>();

        foreach (GameObject button in GameController.GameControllerSingle.buttons)
        {
            if (button.activeSelf)
            {
                pickOptions.Add(button.GetComponent<Image>().color);
                //give computer a chioce for first turn; nothing in reachable yet
            }
        }
        
        Color[,] tempGameBoard = new Color[GenerateGame.GenerateGameSingle.width, GenerateGame.GenerateGameSingle.height];
        Computers = new List<Computer>();

        for (int x = 0; x < GenerateGame.GenerateGameSingle.width; x++)
        {
            for (int y = 0; y < GenerateGame.GenerateGameSingle.height; y++)
            {
                tempGameBoard[x, y] = GenerateGame.GenerateGameSingle.gameBoard[x, y].GetComponent<SpriteRenderer>().color;
                //print(tempGameBoard[x, y]);
                //print(GenerateGame.GenerateGameSingle.gameBoard[x, y].GetComponent<SpriteRenderer>().color);
            }
        }

        foreach (GameController.Player player in GameController.GameControllerSingle.Players)
        {
            Computer comp = new Computer(player.playerEdge, player.playerReachable, player.playerNotEdge, player.CurrentColor, 0f);
            Computers.Add(comp);
        }

        List<Color> offColors = new List<Color>();
        List<Color> onColors = new List<Color>(pickOptions);

        foreach (GameController.Player player in GameController.GameControllerSingle.Players)
        {
            offColors.Add(player.CurrentColor);
        }

        Computers = HardComputer(4, GameController.GameControllerSingle.turn, 0, tempGameBoard, Computers, onColors, offColors);

        GameController.GameControllerSingle.ButtonClick(Computers[GameController.GameControllerSingle.turn].currentColor);
    }

    //need ro remove nodes that are surronded on all sides by same color nodes from visited to make run faster; then need to redo how i calculate score
    public List<Computer> HardComputer(int SearchDepth, int turn, float Score, Color[,] gameState, List<Computer> comps, List<Color> onColors, List<Color> offColors)
    {
        int TotalAmountOneColor = 0;
        Color bestColor = onColors[0];
        List<Computer> newComps;
        List<Computer> bestComps = new List<Computer>(comps);
        foreach (Color currentColor in onColors)
        {
            TotalAmountOneColor = 0;
            //med ai stuff
            Queue<Vector2> playerQ = new Queue<Vector2>();
            HashSet<Vector2> playerEdge = new HashSet<Vector2>(comps[turn].playerEdge);
            HashSet<Vector2> playerReachable = new HashSet<Vector2>(comps[turn].playerReachable);
            HashSet<Vector2> playerNotEdge = new HashSet<Vector2>(comps[turn].playerNotEdge);

            //place correct color squares in queue
            foreach (Vector2 vec in playerEdge)
            {
                playerQ.Enqueue(vec);
            }

            //check all correct color squares for their neighbors that are same color //dijstras
            while (playerQ.Count != 0)
            {

                Vector2 currentSquare = playerQ.Dequeue();
                Vector2 temp;
                int tempCountSides = 0;

                bool right = currentSquare.x + 1 < GenerateGame.GenerateGameSingle.width;
                bool left = currentSquare.x - 1 >= 0;
                bool up = currentSquare.y + 1 < GenerateGame.GenerateGameSingle.height;
                bool down = currentSquare.y - 1 >= 0;

                //right
                if (right)
                {
                    temp = new Vector2(currentSquare.x + 1, currentSquare.y);
                    //print(gameState[(int)currentSquare.x + 1, (int)currentSquare.y]);
                    //color correct
                    if (playerEdge.Contains(temp))
                    {
                        tempCountSides += 1;
                        //do nothing if visited already 
                    }
                    else if (gameState[(int)currentSquare.x + 1, (int)currentSquare.y] == currentColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Computer player in comps)
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
                            playerQ.Enqueue(temp);
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

                //left
                if (left)
                {
                    temp = new Vector2(currentSquare.x - 1, currentSquare.y);
                    if (playerEdge.Contains(temp))
                    {
                        tempCountSides += 1;
                        //do nothing if visited already 
                    }
                    else if (gameState[(int)currentSquare.x - 1, (int)currentSquare.y] == currentColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Computer player in comps)
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
                            playerQ.Enqueue(temp);
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

                //up
                if (up && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 1))
                {
                    temp = new Vector2(currentSquare.x, currentSquare.y + 1);
                    //print(gameState[(int)currentSquare.x, (int)currentSquare.y + 1]);
                    if (playerEdge.Contains(temp))
                    {
                        tempCountSides += 1;
                        //do nothing if visited already 
                    }
                    else if (gameState[(int)currentSquare.x, (int)currentSquare.y + 1] == currentColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Computer player in comps)
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
                            playerQ.Enqueue(temp);
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

                //down
                if (down && (GenerateGame.GenerateGameSingle.shape != 3 || (currentSquare.x + currentSquare.y) % 2 == 0))
                {
                    temp = new Vector2(currentSquare.x, currentSquare.y - 1);
                    if (playerEdge.Contains(temp))
                    {
                        tempCountSides += 1;
                        //do nothing if visited already 
                    }
                    else if (gameState[(int)currentSquare.x, (int)currentSquare.y - 1] == currentColor)
                    {
                        //if not in visited then put there
                        bool visit = true;
                        foreach (Computer player in comps)
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
                            playerQ.Enqueue(temp);
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

                //if hexagon shape
                if (GenerateGame.GenerateGameSingle.shape == 6)
                {
                    //if even x; up right; left up
                    if (currentSquare.x % 2 == 0)
                    {
                        if (up && right)
                        {
                            temp = new Vector2(currentSquare.x + 1, currentSquare.y + 1);
                            if (playerEdge.Contains(temp))
                            {
                                tempCountSides += 1;
                                //do nothing if visited already 
                            }
                            else if (gameState[(int)currentSquare.x + 1, (int)currentSquare.y + 1] == currentColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Computer player in comps)
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
                                    playerQ.Enqueue(temp);
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
                        if (left && up)
                        {
                            temp = new Vector2(currentSquare.x - 1, currentSquare.y + 1);
                            if (playerEdge.Contains(temp))
                            {
                                tempCountSides += 1;
                                //do nothing if visited already 
                            }
                            else if (gameState[(int)currentSquare.x - 1, (int)currentSquare.y + 1] == currentColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Computer player in comps)
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
                                    playerQ.Enqueue(temp);
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
                    }
                    //if odd x; down left; right down
                    else
                    {
                        if (down && left)
                        {
                            temp = new Vector2(currentSquare.x - 1, currentSquare.y - 1);
                            if (playerEdge.Contains(temp))
                            {
                                tempCountSides += 1;
                                //do nothing if visited already 
                            }
                            else if (gameState[(int)currentSquare.x - 1, (int)currentSquare.y - 1] == currentColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Computer player in comps)
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
                                    playerQ.Enqueue(temp);
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
                        if (right && down)
                        {
                            temp = new Vector2(currentSquare.x + 1, currentSquare.y - 1);
                            if (playerEdge.Contains(temp))
                            {
                                tempCountSides += 1;
                                //do nothing if visited already 
                            }
                            else if (gameState[(int)currentSquare.x + 1, (int)currentSquare.y - 1] == currentColor)
                            {
                                //if not in visited then put there
                                bool visit = true;
                                foreach (Computer player in comps)
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
                                    playerQ.Enqueue(temp);
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
                    }
                }
                if (tempCountSides == GenerateGame.GenerateGameSingle.shape)
                {
                    playerEdge.Remove(currentSquare);
                    playerNotEdge.Add(currentSquare);
                }
            }

            float tempScore = (playerEdge.Count + playerNotEdge.Count) / ((float)GenerateGame.GenerateGameSingle.width * (float)GenerateGame.GenerateGameSingle.height);

            //cant clone list of non generic types
            //newComps = new List<Computer>(comps);
            newComps = new List<Computer>();
            foreach (Computer player in comps)
            {
               Computer newComp = new Computer(player.playerEdge, player.playerReachable, player.playerNotEdge, player.currentColor, player.score);
               newComps.Add(newComp);
            }
            newComps[turn].playerEdge = playerEdge;
            newComps[turn].playerReachable = playerReachable;
            newComps[turn].playerNotEdge = playerNotEdge;
            newComps[turn].currentColor = currentColor;
            newComps[turn].score += TotalAmountOneColor;

            //game is over
            if (tempScore >= 1f / GenerateGame.GenerateGameSingle.numPlayers)
            {
                //ComputerColor = currentColor;
                return newComps;
            }
            //look another move deep
            else if (SearchDepth > 0)
            {
                if(TotalAmountOneColor != 0)
                {
                    List<Color> newOnColors = new List<Color>(onColors);
                    List<Color> newOffColors = new List<Color>(offColors);

                    newOnColors.Remove(currentColor);

                    if(comps[turn].currentColor!= Color.black)
                        newOffColors.Remove(comps[turn].currentColor);

                    if (comps[turn].currentColor != Color.black)
                        newOnColors.Add(comps[turn].currentColor);

                    newOffColors.Add(currentColor);

                    Color[,] tempGameBoard = new Color[GenerateGame.GenerateGameSingle.width, GenerateGame.GenerateGameSingle.height];

                    for (int x = 0; x < GenerateGame.GenerateGameSingle.width; x++)
                    {
                        for (int y = 0; y < GenerateGame.GenerateGameSingle.height; y++)
                        {
                            tempGameBoard[x, y] = gameState[x, y];
                        }
                    }
                    //change color all squares player owns
                    foreach (Vector2 vec in playerEdge)
                    {
                        tempGameBoard[(int)vec.x, (int)vec.y] = currentColor;
                    }


                    //print(tempScore);
                    int nextTurn = (turn + 1) % comps.Count;

                    newComps = HardComputer(SearchDepth - 1, nextTurn, -Score, tempGameBoard, newComps, newOnColors, newOffColors);
                }
            }
            //update current choice
            if (newComps[turn].score > bestComps[turn].score)
            {
                bestComps = newComps;
                bestColor = currentColor;
            }
        }

        bestComps[turn].currentColor = bestColor;
        return bestComps;
    }

    void printColor(Color cc)
    {
        if(cc == Color.red)
        {
            print("red");
        }
        else if (cc == Color.yellow)
        {
            print("yellow");
        }
        else if (cc == Color.green)
        {
            print("green");
        }
        else if (cc == Color.cyan)
        {
            print("cyan");
        }
        else if (cc == Color.blue)
        {
            print("blue");
        }
        else
        {
            print(cc);
        }
    }

}
