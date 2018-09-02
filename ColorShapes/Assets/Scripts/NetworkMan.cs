using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMan : NetworkManager {



	// Use this for initialization
	void Start ()
    {
        GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(() => StartupHost());
        GameObject.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() => JoinGame());
    }

    public void StartupHost()
    {
        //chagne quitbutton Functionality to online quit
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitGame());

        SetPort();
        NetworkManager.singleton.StartHost();

        //changeing menus
        GameController.GameControllerSingle.startOnlineButton();
    }
    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();

        //changeing menus
        GameController.GameControllerSingle.startOnlineButton();
    }
    public void SetIPAddress()
    {
        string ipAddress = GameObject.Find("IPAdressInputFieldText").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }
    public void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
    public void QuitGame()
    {
        NetworkManager.singleton.StopHost();
        GameController.GameControllerSingle.QuitButton();
    }
}
