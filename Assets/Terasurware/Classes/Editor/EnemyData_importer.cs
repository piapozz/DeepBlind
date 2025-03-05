using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EnemyData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Resources/MasterData/EnemyData.xlsx";
	private static readonly string exportPath = "Assets/Resources/MasterData/EnemyData.asset";
	private static readonly string[] sheetNames = { "EnemyData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_EnemyData data = (Entity_EnemyData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_EnemyData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_EnemyData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_EnemyData.Sheet s = new Entity_EnemyData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_EnemyData.Param p = new Entity_EnemyData.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Speed = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.SpeedDiameter = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.ThreatRange = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.ViewLength = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.FieldOfView = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.Seach = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.Vigilance = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(9); p.Tracking = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(10); p.Skill = (cell == null ? "" : cell.StringCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
