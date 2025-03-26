using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity_EnemyData;

public class CharacterMasterUtility {

	/// <summary>
	/// キャラクターマスターデータ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static Param GetCharacterMaster(int ID)
	{
		List<Param> characterMasterList = MasterDataManager.enemyData[0];
		for (int i = 0, max = characterMasterList.Count; i < max; i++)
		{
			if (characterMasterList[i].ID != ID) continue;

			return characterMasterList[i];
		}
		return null;
	}

}
