using UnityEngine;

public class GoblinBehavior : MonoBehaviour
{
    private bool isAlive;
    private Transform originalPosition;
    public float speed = 2f; 
    private GameObject target; 
    private float destroyDelay = 5f; 
    private PlayerMovement playerMovement;
    private Animator animator;

    void Start()
    {
        target = null;
        originalPosition = transform; 
        animator = GetComponent<Animator>();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isAlive)
        {
            if (target == null)
            {
                FindClosestTarget();
            }
            else
            {
                MoveTowardTarget();
            }
        }
    }

    public void KillGoblin()
    {
        isAlive = false; 
        transform.position = originalPosition.position;
        this.gameObject.SetActive(false);
        target = null; 
    }

    public void SpawnGoblin()
    {
        this.gameObject.SetActive(true);

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.OnGoblinSpawned(this.gameObject);
        isAlive = true;
    }

    private void FindClosestTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Target");
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        if (potentialTargets.Length == 0) return;

        foreach (GameObject potentialTarget in potentialTargets)
        {
            float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = potentialTarget;
            }
        }

        target = closestTarget; 
        Debug.Log(target);

        if (target != null)
        {
            Debug.Log($"Closest target found: {target.name}");
        }
        else
        {
            Debug.LogWarning("No targets available. Goblin will remain idle.");
        }
    }

    private void MoveTowardTarget()
    {


        if (target == null) return;

        animator.SetBool("Walk", true);

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            animator.SetBool("Attack", true);
            StartCoroutine(DestroyTargetAfterDelay());
            animator.SetBool("Attack", false);
        }

        animator.SetBool("Walk", false);

    }

    private System.Collections.IEnumerator DestroyTargetAfterDelay()
    {
        GameObject currentTarget = target;
        target = null;
        yield return new WaitForSeconds(destroyDelay);

        if (currentTarget != null)
        {
            Destroy(currentTarget);
        }
    }
}
