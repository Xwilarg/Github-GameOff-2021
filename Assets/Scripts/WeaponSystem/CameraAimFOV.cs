using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bug.WeaponSystem
{
	public class CameraAimFOV : MonoBehaviour
	{
		[SerializeField] private Camera[] _cameras;
		[Min(0f)]
		[SerializeField] private float _aimAnimDefaultDuration = 2f;


		private Dictionary<Camera, float> _fovPerCamera;
		private float _currentZoom = 1f;
		private Coroutine _animCoroutine;


		private void Start()
		{
			_fovPerCamera = _cameras.ToDictionary(cam => cam, cam => cam.fieldOfView);
		}

		public void SetZoom(float value)
		{
			StopAnim();
			ApplyZoom(value);
		}

		public void SetZoomAnimated(float value) => SetZoomAnimated(value, _aimAnimDefaultDuration);

		public void SetZoomAnimated(float value, float duration)
		{
			StopAnim();
			_animCoroutine = StartCoroutine(AnimateZoom(value, duration));
		}

		public void RestoreZoom()
		{
			SetZoom(1f);
		}

		public void RestoreZoomAnimated() => RestoreZoomAnimated(_aimAnimDefaultDuration);

		public void RestoreZoomAnimated(float duration)
		{
			SetZoomAnimated(1f, duration);
		}

		private IEnumerator AnimateZoom(float value, float duration)
		{
			float startValue = _currentZoom;
			float time = Time.deltaTime;
			while (time < duration)
			{
				float ratio = time / duration;
				float factor = Mathf.Lerp(startValue, value, ratio);
				ApplyZoom(factor);

				yield return null;
				time += Time.deltaTime;
			}
			ApplyZoom(value);
		}

		private void ApplyZoom(float value)
		{
			_currentZoom = value;

			foreach ((Camera cam, float originalFov) in _fovPerCamera)
				cam.fieldOfView = originalFov * (1f / value);
		}

		private void StopAnim()
		{
			if (_animCoroutine != null)
			{
				StopCoroutine(_animCoroutine);
				_animCoroutine = null;
			}
		}

		private void Reset()
		{
			_cameras = GetComponentsInChildren<Camera>();
		}
	}
}
