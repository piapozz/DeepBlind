using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_EnemyData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int ID;
		public float Speed;
		public float SpeedDiameter;
		public float ThreatRange;
		public float ViewLength;
		public float FieldOfView;
		public string Search;
		public string Vigilance;
		public string Tracking;
		public string Skill;
	}
}

