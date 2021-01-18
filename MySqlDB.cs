using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using MySql.Data.MySqlClient;

public class MySqlDB : MonoBehaviour {
	private string connectSql = "server=37.59.55.185;user=3UWttL9edk;database=3UWttL9edk;password=B7WdnSUtrj";
	bool fr;
/*Username: VBu6ICLn7z
Database name: VBu6ICLn7z
Password: PjcbbdK5EO
Server: remotemysql.com
Port: 3306*/
	public bool addClient (string login, string pass)
	{
		bool flag = true;
		string maxIDclient = "SELECT MAX(ID) FROM clientInfo"; // id last client in DB sql
		MySqlConnection conn = new MySqlConnection (connectSql);
		conn.Open ();
		int i = 0;
		while(flag == true){
			string selectLogin = "SELECT Login FROM clientInfo WHERE ID = " + i;
			string selectPasswd = "SELECT Passwd FROM clientInfo WHERE ID = " + i;
			MySqlCommand getLogin = new MySqlCommand (selectLogin, conn);
			MySqlCommand getPasswd = new MySqlCommand (selectPasswd, conn);
			MySqlCommand getID = new MySqlCommand (maxIDclient, conn);
			string idClient = getID.ExecuteScalar ().ToString();
			string LoginClient = getLogin.ExecuteScalar ().ToString ();
			string PasswdClient = getPasswd.ExecuteScalar ().ToString ();
			if (login != LoginClient) { 
				if (i == System.Int32.Parse (idClient)) {
					i++;
					string addNewCl = "INSERT INTO clientInfo VALUES ('" + i + "', '" + login + "', '" + pass + "');"; // for create new client
					MySqlCommand com = new MySqlCommand (addNewCl, conn);
					com.ExecuteNonQuery ();
					flag = false;
					conn.Close ();
					fr = true;
				}
			} else if (login == LoginClient && pass == PasswdClient) {
				flag = false;
				conn.Close ();
				fr = true;
			} else {
				flag = false;
				conn.Close ();
				fr = false;
			}
			i++;
		}
		Debug.LogError (flag);
		if (fr == true)
			return true;
		else
			return false;
	}
	public void addLog(string nameCl, string mess){
		int id;
		string maxIDclient = "SELECT MAX(ID) FROM logDB";
		string time = DateTime.Now.ToString ();
		MySqlConnection conn = new MySqlConnection (connectSql);
		conn.Open ();
		MySqlCommand getID = new MySqlCommand (maxIDclient, conn);
		string s = getID.ExecuteScalar ().ToString ();
		if (s == "") {
			id = 0;	
		}
		else
		id = System.Int32.Parse(getID.ExecuteScalar ().ToString()) + 1;
		
		string sql = "INSERT INTO logDB VALUES ('"+ id + "', '" + nameCl + "', '" + mess + "', '" + time + "');";
		MySqlCommand com = new MySqlCommand (sql, conn);
		com.ExecuteNonQuery ();
		conn.Close ();
	}
}
