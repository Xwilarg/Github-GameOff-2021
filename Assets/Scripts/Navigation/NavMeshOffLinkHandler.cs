using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Bug.Navigation
{
	/// <summary>
	/// Handles a NavMeshAgent navigating across a NaveMeshLink with uniform speed.
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent))]
	public class NavMeshOffLinkHandler : MonoBehaviour
	{
		[SerializeField]
		private float _traverseSpeed = 0.2f;

		private NavMeshAgent _agent;

		private bool _handlingOffLink;


		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			if (_agent.isOnOffMeshLink && !_handlingOffLink)
			{
				StartCoroutine(HandleOffLink());
				_handlingOffLink = true;
			}
		}

		private IEnumerator HandleOffLink()
		{
			OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
			Vector3 startPosition = transform.position;
			Quaternion startRotation = transform.rotation;

			_agent.updateRotation = false;
			_agent.updatePosition = false;
			float originalSpeed = _agent.speed;
			_agent.speed = 0f;
			_agent.velocity = Vector3.zero;

			_agent.CompleteOffMeshLink();

			if (_agent.isOnNavMesh && _agent.navMeshOwner is NavMeshSurface surface)
			{
				Vector3 targetPosition = _agent.nextPosition;
				float delta = (targetPosition - startPosition).magnitude;

				Quaternion targetRotation = Quaternion.LookRotation((linkData.endPos - linkData.startPos).normalized, surface.transform.up);

				float speed = _traverseSpeed * originalSpeed;
				float duration = delta / speed;
				float time = Time.deltaTime;

				while (time < duration)
				{
					float ratio = time / duration;
					transform.position = Vector3.Lerp(startPosition, targetPosition, ratio);
					transform.rotation = Quaternion.Lerp(startRotation, targetRotation, ratio);
					yield return null;
					time += Time.deltaTime;
				}

				transform.rotation = targetRotation;
				transform.position = targetPosition;
				_agent.velocity = targetRotation * Vector3.forward * speed;
			}

			_agent.updateRotation = true;
			_agent.updatePosition = true;
			_agent.speed = originalSpeed;

			_handlingOffLink = false;
		}
	}
}
