using UnityEngine;
using System.Collections;

public class PaddleBehavior : MonoBehaviour
{
    public Transform treeParent;
    public Transform playFieldParent; 
    private GameObject carriedObject;

    public GameObject treePrefab1; 
    public GameObject treePrefab2;
    private GameObject currentTreePrefab; 

    public GameObject housePrefab1;
    public GameObject housePrefab2;
    private GameObject currentHousePrefab;

    public GameObject fireEffectPrefab;

    private void Start()
    {
        currentTreePrefab = treePrefab1;
        currentHousePrefab = housePrefab1;
    }

    public void ToggleTreePrefab()
    {
        currentTreePrefab = currentTreePrefab == treePrefab1 ? treePrefab2 : treePrefab1;
    }

    public void ToggleHousePrefab()
    {
        currentHousePrefab = currentHousePrefab == housePrefab1 ? housePrefab2 : housePrefab1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Tree" && carriedObject == null
            && other.transform.parent != playFieldParent && other.gameObject.activeSelf)
        {
            carriedObject = Instantiate(currentTreePrefab, transform.position, transform.rotation);
            carriedObject.transform.SetParent(transform); 
            carriedObject.name = "Tree"; 
            carriedObject.transform.localPosition = Vector3.zero; 
            carriedObject.transform.localRotation = Quaternion.identity;
        }

        else if (other.gameObject.name == "House" && carriedObject == null
            && other.transform.parent != playFieldParent && other.gameObject.activeSelf)
        {
            carriedObject = Instantiate(currentHousePrefab, transform.position, transform.rotation);
            carriedObject.transform.SetParent(transform); 
            carriedObject.name = "House";
            carriedObject.transform.localPosition = Vector3.zero; 
            carriedObject.transform.localRotation = Quaternion.identity;
        }

        else if (other.gameObject.name == "PlayField" && carriedObject != null)
        {
            carriedObject.transform.SetParent(playFieldParent);
            carriedObject.transform.position = other.ClosestPoint(transform.position); 
            carriedObject.transform.localRotation = Quaternion.identity; 
            carriedObject.tag = "Target";

            carriedObject = null; 
        }

        else if (other.gameObject.name == "Goblin")
        {
            StartCoroutine(SummonFireEffect(other.gameObject));
        }

        else if (other.gameObject.CompareTag("Target"))
        {
            PushTarget(other);
        }
    }

    private void PushTarget(Collider target)
    {
        Vector3 paddleToTarget = target.transform.position - transform.position;
        paddleToTarget.y = 0;
        paddleToTarget.Normalize();

        float pushDistance = .01f; 

        Vector3 newPosition = target.transform.position + paddleToTarget * pushDistance;
        target.transform.position = newPosition;
    }


    private IEnumerator SummonFireEffect(GameObject goblin)
    {
        GameObject fireEffect = Instantiate(fireEffectPrefab, goblin.transform.position, Quaternion.identity, goblin.transform);

        yield return new WaitForSeconds(5f);

        Destroy(fireEffect);

        GoblinBehavior goblinBehavior = goblin.GetComponent<GoblinBehavior>();

        goblinBehavior.KillGoblin();
    }
}
