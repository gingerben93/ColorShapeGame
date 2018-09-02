using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayer : MonoBehaviour {

    void Start()
    {
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

    public void ChangeWidth()
    {
        int newWidth;
        bool temp = int.TryParse(GameObject.Find("WidthInputField").GetComponent<InputField>().textComponent.text, out newWidth);
        if (temp)
        {
            GenerateGame.GenerateGameSingle.width = newWidth;
        }
    }
    public void ChangeHeight()
    {
        int newHeight;
        bool temp = int.TryParse(GameObject.Find("HeightInputField").GetComponent<InputField>().textComponent.text, out newHeight);
        if (temp)
        {
            GenerateGame.GenerateGameSingle.height = newHeight;
        }
    }

    public void ChangeNumberColors()
    {
        int newNumberColors;
        bool temp = int.TryParse(GameObject.Find("ColorInputField").GetComponent<InputField>().textComponent.text, out newNumberColors);
        if (temp)
        {
            GenerateGame.GenerateGameSingle.numberColors = newNumberColors;
        }
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
        GenerateGame.GenerateGameSingle.shape = shape;
    }

    public void NewGame()
    {
        GameController.GameControllerSingle.NewGame();
    }

    public void StartNewGame()
    {
        int[] colors;
        colors = GenerateGame.GenerateGameSingle.GetListOfColors();
        GenerateGame.GenerateGameSingle.DrawShapesForGame(colors);
        GenerateGame.GenerateGameSingle.StartLocations();
        GenerateGame.GenerateGameSingle.placeButtons();
    }

    public void RestartGame()
    {
        NewGame();
        StartNewGame();
    }

    public void PlayerButtonClick(Color color)
    {
        GameController.GameControllerSingle.ButtonClick(color);
    }
}
