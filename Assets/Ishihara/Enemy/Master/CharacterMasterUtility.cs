/**
 * @file CharacterMasterUtility.cs
 * @brief キャラクターマスターデータ実行処理
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_CharacterData;

public class CharacterMasterUtility {

	/// <summary>
	/// キャラクターマスターデータ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public static Param GetCharacterMaster(int ID) {
		List<Param> characterMasterList = MasterDataManager.characterData[0];
		for (int i = 0, max = characterMasterList.Count; i < max; i++) {
			if (characterMasterList[i].ID != ID) continue;

			return characterMasterList[i];
		}
		return null;
	}

}
