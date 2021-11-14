using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bug.WeaponSystem
{
	public class RecoilMotor : MonoBehaviour
	{
		[SerializeField] private float _snapSpeed = 1f;
		[SerializeField] private float _restitution = 1f;
		[SerializeField] private Transform _target;

		private Vector3 targetRotation;
		private Vector3 currentRotation;


		private void LateUpdate()
		{
			targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, _restitution * Time.deltaTime);
			currentRotation = Vector3.Slerp(currentRotation, targetRotation, _snapSpeed * Time.deltaTime);
			_target.localRotation = Quaternion.Euler(currentRotation);
		}

		public void AddRecoil(RecoilData recoilData)
		{
			targetRotation += new Vector3(-recoilData.x, Random.Range(-recoilData.y, recoilData.y), Random.Range(-recoilData.z, recoilData.z));
		}
	}
}
