using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	[SerializeField] PlayerController _player;
	[SerializeField] Animator _animator;

	float _moveInput;

	void Update()
	{
		Vector2 moveInput = _player.GetMoveInput();

		_moveInput = math.lerp(_moveInput, moveInput.y, Time.deltaTime * 5);
		_animator.SetFloat("Y", _moveInput);
		_animator.SetBool("Running", _player.MovementState == PlayerMovementState.Running);
		_animator.SetBool("Crouching", _player.BaseState == PlayerBaseState.Crouching);
	}
}
