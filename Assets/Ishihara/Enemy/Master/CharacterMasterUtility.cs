/**
 * @file CharacterMasterUtility.cs
 * @brief �L�����N�^�[�}�X�^�[�f�[�^���s����
 * @author yao
 * @date 2025/2/4
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_CharacterData;

public class CharacterMasterUtility {

	/// <summary>
	/// �L�����N�^�[�}�X�^�[�f�[�^�擾
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
