using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bug.SceneManagement
{
	public class SceneLoader : MonoBehaviour
	{
		[SerializeField] private string[] _scenes;

		[SerializeField] private bool _removeLoaderScene = true;


		private IEnumerator Start()
		{
			if (_scenes.Length == 0)
				yield break;

			Scene loaderScene = SceneManager.GetActiveScene();

			List<AsyncOperation> operations = new List<AsyncOperation>(_scenes.Length);

			string mainSceneName = _scenes[0];
			AsyncOperation mainSceneTask = SceneManager.LoadSceneAsync(mainSceneName, LoadSceneMode.Additive);
			mainSceneTask.allowSceneActivation = false;
			mainSceneTask.completed += operation => {
				Scene mainScene = SceneManager.GetSceneByName(mainSceneName);
				SceneManager.SetActiveScene(mainScene);
				if (_removeLoaderScene)
					SceneManager.UnloadSceneAsync(loaderScene);
			};
			operations.Add(mainSceneTask);

			for (int i = 1; i < _scenes.Length; i++)
			{
				string scene = _scenes[i];
				AsyncOperation task = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
				task.allowSceneActivation = false;
				operations.Add(task);
			}

			foreach (AsyncOperation operation in operations)
				yield return new WaitUntil(() => operation.progress >= 0.9f);

			foreach (AsyncOperation operation in operations)
				operation.allowSceneActivation = true;
		}
	}
}
