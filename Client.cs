using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Client : MonoBehaviour {
	MySqlDB dostupDB = new MySqlDB();

	public Button btnLogIn, btnConnect, btnSend;
	public InputField enterLogin, hostInput, portInput, passwdInput;

	public GameObject chatContainer;
	public GameObject messagePrefab;
	public string nameClient, passwdClient;

	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;

	void Start(){
		 
		btnLogIn.onClick.AddListener (delegate {
			Debug.LogError("Clik");
			nameClient = enterLogin.text.ToString();
			passwdClient = passwdInput.text.ToString();
			bool f = dostupDB.addClient(nameClient, passwdClient);
			if (f == true) {
			hostInput.gameObject.SetActive(true);
			portInput.gameObject.SetActive(true);
			btnConnect.gameObject.SetActive(true);
			btnLogIn.gameObject.SetActive(false);
			enterLogin.gameObject.SetActive(false);
			passwdInput.gameObject.SetActive(false);
			}
		});
	}
		

	public void ConnectToServer(){
		// 
		if (socketReady)
			return;
		//
		string host = "127.0.0.1";
		int port = 6321;

		string h;
		int p;
		h = hostInput.GetComponentInChildren<Text>().text.ToString();
		if (h != "")
			host = h;
		int.TryParse (portInput.GetComponentInChildren<Text>().text, out p);

		if (p != 0)
			port = p;

		//
		try{
			socket = new TcpClient(host, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		}
		catch(Exception e){
			Debug.LogError("Socket error : " + e.Message);
		}
	}

	private void Update(){
		if (socketReady) {
			if (stream.DataAvailable) {
				string data = reader.ReadLine ();
				if (data != null)
					OnIncomingData (data);
			}
		}

	}

	private void OnIncomingData (string data){
		if(data == "%NAME"){

			Send ("&NAME|" + nameClient + "|" + passwdClient);
			return;
		}

		GameObject go = Instantiate (messagePrefab, chatContainer.transform) as GameObject;	
		go.GetComponentInChildren<Text> ().text = data;
	}

	private void Send(string data){

		if (!socketReady)
			return;
		writer.WriteLine (data);
		writer.Flush ();
	}

	public void OnSendButton(){
		string message = GameObject.Find ("SendInput").GetComponent<InputField> ().text;
		Send (message);
		dostupDB.addLog (nameClient, message);
	}

	private void CloseSocket(){
		if (!socketReady)
			return;

		writer.Close ();
		reader.Close ();
		socket.Close();
		socketReady = false;
	}

	private void OnApplicationQuit(){
		CloseSocket ();
	}
	private void OnDisable(){
		CloseSocket ();
	}

}
