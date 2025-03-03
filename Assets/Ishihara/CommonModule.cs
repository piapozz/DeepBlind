/**
 * @file CommonModule.cs
 * @brief 共用モジュール
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
	/// リストの初期化
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
	/// 配列が空か否か
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="array"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>(T[] array) {
		return array == null || array.Length == 0;
	}

	/// <summary>
	/// 配列に対して有効なインデクスか否か
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
	/// リストが空か否か
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static bool IsEmpty<T>(List<T> list) {
		return list == null || list.Count == 0;
	}

	/// <summary>
	/// リストに対して有効なインデクスか否か
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
	/// リストを重複なしでマージ
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
	/// 複数のタスクの終了待ち
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
