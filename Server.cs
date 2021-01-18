using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Server : MonoBehaviour {
	private List<ServerClient> clients;
	private List<ServerClient> disconnectList;
	public int port = 6321;
	private TcpListener server;
	private bool serverStarted;

	private void Start(){
		clients = new List<ServerClient> ();
		disconnectList = new List<ServerClient> ();
		try{                       //IPAddress.Any
			server = new TcpListener(IPAddress.Any, port);
			server.Start();
			StartListening();
			serverStarted = true;
			Debug.LogError("Server has been started on port " + port.ToString());
		}
		catch (Exception e){
			Debug.LogError("Soket error: " + e.Message);
		}
	}
	private void Update(){
		if (!serverStarted)
			return;
		foreach (ServerClient c in clients) {
			if (!IsConnected (c.tcp)) { //check on conected client
				c.tcp.Close ();
				disconnectList.Add (c);
				continue;
			} else {
				NetworkStream s = c.tcp.GetStream ();
				if (s.DataAvailable) {
					StreamReader reader = new StreamReader (s, true);
					string data = reader.ReadLine ();
					if (data != null)
						OnIncomingData (c, data);
				}
			}
		}
		for(int i = 0; i < disconnectList.Count - 1; i++){
			SendClient (disconnectList[i].clientName + " has disconnected", clients);
			clients.Remove (disconnectList[i]);
			disconnectList.RemoveAt (i);
		}
	}
	private bool IsConnected(TcpClient c){
		try{
			if (c != null && c.Client != null){
				if (c.Client.Poll(0, SelectMode.SelectError)){
					return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
				}
				return true;
			}
			else return false;
		}
		catch{
			return false;
		}
	}
	private void StartListening(){
		server.BeginAcceptTcpClient (AcceptTcpClient, server);
	}
	private void AcceptTcpClient(IAsyncResult ar){
		TcpListener listener = (TcpListener)ar.AsyncState;
		clients.Add (new ServerClient (listener.EndAcceptTcpClient (ar)));
		StartListening ();
		SendClient ("%NAME", new List<ServerClient>() {clients[clients.Count - 1]});
	}
	private void OnIncomingData(ServerClient c, string data){
		if (data.Contains("&NAME")){
			c.clientName = data.Split ('|')[1];
			SendClient (c.clientName + " has connected", clients);
			return;
		}
		SendClient (c.clientName + " : " + data, clients); //<-- 
	}
	private void SendClient(string data, List<ServerClient> cl){
		foreach (ServerClient c in cl){
			try{
				StreamWriter writer = new StreamWriter(c.tcp.GetStream());
				writer.WriteLine(data);
				writer.Flush();
			} 
			catch(Exception e){
				Debug.LogError ("Write error : " + e.Message + " to client " + c.clientName);
			}
		}
	}
}
public class ServerClient{
	public TcpClient tcp;
	public string clientName, clientPasswd;

	public ServerClient(TcpClient clientSocet){
		clientName = "Guest";
		clientPasswd = "";
		tcp = clientSocet;
	}
}
