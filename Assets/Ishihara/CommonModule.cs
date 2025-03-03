/**
 * @file CommonModule.cs
 * @brief ���p���W���[��
 * @author yao
 * @date 2025/1/9
 */

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonModule {

	/// <summary>
	/// ���X�g�̏�����
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="capacity"></param>
	public static void InitializeList<T>(ref List<T> list, int capacity = -1) {
		if (list == null) {
			if (capacity < 0) {
				list = new List<T>();
			} else {
				list = new List<T>(capacity);
			}
		} else {
			if (list.Capacity < capacity) list.Capacity = capacity;

			list.Clear();
		}
	}

	/// <summary>
	/// �z�񂪋󂩔ۂ�
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>(T[] array) {
		return array == null || array.Length == 0;
	}

	/// <summary>
	/// �z��ɑ΂��ėL���ȃC���f�N�X���ۂ�
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsEnableIndex<T>(T[] array, int index) {
		if (IsEmpty(array)) return false;

		return array.Length > index && index >= 0;
	}

	/// <summary>
	/// ���X�g���󂩔ۂ�
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>(List<T> list) {
		return list == null || list.Count == 0;
	}

	/// <summary>
	/// ���X�g�ɑ΂��ėL���ȃC���f�N�X���ۂ�
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsEnableIndex<T>(List<T> list, int index) {
		if (IsEmpty(list)) return false;

		return list.Count > index && index >= 0;
	}

	/// <summary>
	/// ���X�g���d���Ȃ��Ń}�[�W
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="main"></param>
	/// <param name="sub"></param>
	public static void MeargeList<T>(ref List<T> main, List<T> sub) {
		if (IsEmpty(sub)) return;

		if (main == null) main = new List<T>();

		for (int i = 0, max = sub.Count; i < max; i++) {
			if (main.Exists(mainElem => mainElem.Equals(sub[i]))) continue;

			main.Add(sub[i]);
		}
	}

	/// <summary>
	/// �����̃^�X�N�̏I���҂�
	/// </summary>
	/// <param name="taskList"></param>
	/// <returns></returns>
	public static async UniTask WaitTask(List<UniTask> taskList) {
		while (!IsEmpty(taskList)) {
			for (int i = taskList.Count - 1; i >= 0; i--) {
				if (!taskList[i].Status.IsCompleted()) continue;

				taskList.RemoveAt(i);
			}
			await UniTask.DelayFrame(1);
		}
	}
}
