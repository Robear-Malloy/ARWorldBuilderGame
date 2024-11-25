using UnityEngine;

public class SelectionBehavior : MonoBehaviour
{
    public GameObject player;

    public GameObject currentTree;
    public GameObject hiddenTree;

    public GameObject currentHouse;
    public GameObject hiddenHouse;

    public PaddleBehavior paddleBehavior;
    public GoblinBehavior goblinBehavior;

    private PlayerMovement playerMovement;
    private bool treeToggle = true;
    private bool houseToggle;

    private GameObject selectedTarget; 
    private float rotationSpeed = 100f;

    public GameObject light;
    private day_night_cycle currentTime;

    private void Start()
    {
        hiddenTree.SetActive(false);
        hiddenHouse.SetActive(false);

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        currentTime = light.GetComponent<day_night_cycle>();
    }

    void Update()
    {
        if (getUserTap())
        {
            GameObject mainCamera = Camera.main.gameObject;
            Vector3 origin = mainCamera.transform.position;
            Vector3 direction = mainCamera.transform.forward;
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;
            bool isThereAHit = Physics.Raycast(ray, out hit);

            if (isThereAHit)
            {
                Debug.Log("There's a hit");
                if (hit.collider.gameObject.name == "Tree" && !hit.collider.gameObject.CompareTag("Target"))
                {
                    treeToggle = SwitchObjects(currentTree, hiddenTree, treeToggle);
                    paddleBehavior.ToggleTreePrefab();
                }
                else if (hit.collider.gameObject.name == "House" && !hit.collider.gameObject.CompareTag("Target"))
                {
                    houseToggle = SwitchObjects(currentHouse, hiddenHouse, houseToggle);
                    paddleBehavior.ToggleHousePrefab();
                }
                else if (hit.collider.gameObject.name == "PlayField")
                {
                    MovePlayerToPosition(hit.point);
                }
                else if (hit.collider.gameObject.CompareTag("Target"))
                {
                    selectedTarget = hit.collider.gameObject;
                }
            }
        }

        if (selectedTarget != null)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RotateTarget(Vector3.up);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                RotateTarget(-Vector3.up); 
            }
        }

        if (Input.touchCount > 0 && selectedTarget != null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotationDelta = touch.deltaPosition.x * Time.deltaTime * rotationSpeed;
                selectedTarget.transform.Rotate(Vector3.up, -rotationDelta);
            }
        }

        if (Input.GetKey(KeyCode.G) 
            || currentTime.GetHours() % 24 == 23
            )
        {
            goblinBehavior.SpawnGoblin();
        }
}

    private bool getUserTap()
    {
        bool isTap = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 p = touch.position;
                float fractScreenBorder = 0.3f;
                if (p.x > fractScreenBorder * Screen.width && p.x < (1 - fractScreenBorder) * Screen.width &&
                    p.y > fractScreenBorder * Screen.height && p.y < (1 - fractScreenBorder) * Screen.height)
                {
                    isTap = true;
                }
            }
        }
        else
        {
            isTap = Input.anyKeyDown && Input.GetKey(KeyCode.Space);
        }
        return isTap;
    }

    private bool SwitchObjects(GameObject currentObject, GameObject hiddenObject, bool toggle)
    {
        if (currentObject == null || hiddenObject == null) return toggle;

        currentObject.SetActive(!toggle);
        hiddenObject.SetActive(toggle);

        return !toggle;
    }

    private void MovePlayerToPosition(Vector3 targetPosition)
    {
        StartCoroutine(playerMovement.MovePlayer(targetPosition));
    }

    private void RotateTarget(Vector3 rotationAxis)
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        selectedTarget.transform.Rotate(rotationAxis, rotationAmount);
    }
}
