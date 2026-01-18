using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string deathTrigger = "Death";

    private int _speedHash;
    private int _attackHash;
    private int _deathHash;

    private void Awake()
    {
        if (animator == null)
        {
            Debug.LogError("UnitAnimator missing Animator reference.", this);
            enabled = false;
            return;
        }

        _speedHash = Animator.StringToHash(speedParameter);
        _attackHash = Animator.StringToHash(attackTrigger);
        _deathHash = Animator.StringToHash(deathTrigger);
    }

    public void SetSpeed(float speed)
    {
        if (!enabled)
            return;

        animator.SetFloat(_speedHash, speed);
    }

    public void PlayAttack()
    {
        if (!enabled)
            return;

        animator.SetTrigger(_attackHash);
    }

    public void PlayDeath()
    {
        if (!enabled)
            return;

        animator.SetTrigger(_deathHash);
    }
}
