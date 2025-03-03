/**
 * @file MasterDataManager.cs
 * @brief マスターデータ管理
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataManager {
	//private static readonly string _DATA_PATH = "MasterData/";

	//public static List<List<Entity_FloorData.Param>> floorData = null;
	//public static List<List<Entity_CharacterData.Param>> characterData = null;
	//public static List<List<Entity_ActionData.Param>> actionData = null;
 //   public static List<List<Entity_MessageData.Param>> messageData = null;

 //   public static void LoadAllData() {
	//	floorData = Load<Entity_FloorData, Entity_FloorData.Sheet, Entity_FloorData.Param>("FloorData");
	//	characterData = Load<Entity_CharacterData, Entity_CharacterData.Sheet, Entity_CharacterData.Param>("CharacterData");
	//	actionData = Load<Entity_ActionData, Entity_ActionData.Sheet, Entity_ActionData.Param>("ActionData");
 //       messageData = Load<Entity_MessageData, Entity_MessageData.Sheet, Entity_MessageData.Param>("MessageData");
 //   }

	//private static List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject {
	//	// データを読み込む
	//	T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
	//	// 名称指定でシートを取得
	//	var sheetField = typeof(T1).GetField("sheets");
	//	List<T2> listData = sheetField.GetValue(sourceData) as List<T2>;

	//	// 名称指定でフィールドを取得
	//	var listField = typeof(T2).GetField("list");
	//	List<List<T3>> paramList = new List<List<T3>>();
	//	foreach (var elem in listData) {
	//		List<T3> param = listField.GetValue(elem) as List<T3>;
	//		paramList.Add(param);
	//	}
	//	return paramList;
	//}

}
