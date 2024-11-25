using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = .1f; 
    private GameObject targetGoblin; 
    private Animator animator;
    private bool isMovingToGoblin = false;
    private Vector3 originalPosition; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isMovingToGoblin && targetGoblin != null)
        {
            MoveTowardsTarget(targetGoblin.transform.position);

            if (Vector3.Distance(transform.position, targetGoblin.transform.position) < 0.01f)
            {
                StartCoroutine(AttackGoblin());
                isMovingToGoblin = false; 
            }
        }
    }

    public void OnGoblinSpawned(GameObject goblin)
    {
        targetGoblin = goblin;
        originalPosition = transform.position; 

        StartCoroutine(MoveToGoblinAfterDelay());
    }

    private IEnumerator MoveToGoblinAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (targetGoblin != null && targetGoblin.activeSelf)
        {
            isMovingToGoblin = true;
            animator.SetBool("Walk", true);
        }
    }

    private IEnumerator AttackGoblin()
    {
        animator.SetBool("Attack", true); 

        yield return new WaitForSeconds(3f); 

        if (targetGoblin != null)
        {
            GoblinBehavior goblinBehavior = targetGoblin.GetComponent<GoblinBehavior>();
            if (goblinBehavior != null)
            {
                goblinBehavior.KillGoblin();
            }
            targetGoblin = null;
        }

        animator.SetBool("Attack", false);

        StartCoroutine(ReturnToOriginalPosition());
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        animator.SetBool("Walk", true);

        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            MoveTowardsTarget(originalPosition);
            yield return null;
        }

        animator.SetBool("Walk", false);
    }

    public IEnumerator MovePlayer(Vector3 target)
    {
        animator.SetBool("Walk", true);

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            MoveTowardsTarget(target);
            yield return null;
        }

        animator.SetBool("Walk", false);
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {

        Debug.Log($"Moving to {targetPosition}");
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
}
