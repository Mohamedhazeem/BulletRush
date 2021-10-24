using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] Camera orthographicCamera;    

    public delegate void MovePlayerCallback();
    public event MovePlayerCallback OnMovePlayer;

    [SerializeField] private GameObject player;

    private Vector3 mouseStartPos;
    private Vector3 mouseCurrentPos;
    public Vector3 dragDirection;
    public float angle;    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {        
        player = PlayerManager.instance.currentPlayer;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = orthographicCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseStartPos.y = player.transform.position.y;
        }

        else if (Input.GetMouseButton(0))
        {
            mouseCurrentPos = orthographicCamera.ScreenToWorldPoint(Input.mousePosition);

            mouseCurrentPos.y = player.transform.position.y;
            dragDirection = mouseCurrentPos - mouseStartPos;
           
            if (dragDirection.magnitude > 0.2f)
            {                
                angle = Mathf.Atan2(dragDirection.x, dragDirection.z) * Mathf.Rad2Deg;
                OnMovePlayer?.Invoke();
            }
        }
    }

}
 