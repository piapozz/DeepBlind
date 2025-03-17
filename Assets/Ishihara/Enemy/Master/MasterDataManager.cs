using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataManager {
	private static readonly string _DATA_PATH = "MasterData/";

	public static List<List<Entity_EnemyData.Param>> enemyData = null;
    public static List<List<Entity_StageData.Param>> stageData = null;

    public static void LoadAllData()
	{
        enemyData = Load<Entity_EnemyData, Entity_EnemyData.Sheet, Entity_EnemyData.Param>("EnemyData");
        stageData = Load<Entity_StageData, Entity_StageData.Sheet, Entity_StageData.Param>("StageData");
    }

	private static List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject
	{
		// データを読み込む
		T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
		// 名称指定でシートを取得
		var sheetField = typeof(T1).GetField("sheets");
		List<T2> listData = sheetField.GetValue(sourceData) as List<T2>;

		// 名称指定でフィールドを取得
		var listField = typeof(T2).GetField("list");
		List<List<T3>> paramList = new List<List<T3>>();
		foreach (var elem in listData)
		{
			List<T3> param = listField.GetValue(elem) as List<T3>;
			paramList.Add(param);
		}
		return paramList;
	}
}
