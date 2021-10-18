using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] Camera orthographicCamera;
    
    public delegate void RotatePlayerCallback();
    public event RotatePlayerCallback OnRotatePlayer;

    public delegate void MovePlayerCallback();
    public event MovePlayerCallback OnMovePlayer;


    [SerializeField] private Transform player;

    //[SerializeField] private GameObject startText;

    private Vector3 mouseStartPos;
    private Vector3 mouseCurrentPos;
    public Vector3 dragDirection;
    public float angle;
    
    // public float angleValue;
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
    // Start is called before the first frame update
    void Start()
    {        
        player = PlayerManager.instance.currentPlayer;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = orthographicCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseStartPos.y = player.position.y;
        }

        else if (Input.GetMouseButton(0))
        {
            mouseCurrentPos = orthographicCamera.ScreenToWorldPoint(Input.mousePosition);

            mouseCurrentPos.y = player.position.y;
            dragDirection = mouseCurrentPos - mouseStartPos;
           
            if (dragDirection.magnitude > 0.2f)
            {                
                angle = Mathf.Atan2(dragDirection.x, dragDirection.z) * Mathf.Rad2Deg;
                OnMovePlayer?.Invoke();
                OnRotatePlayer?.Invoke();
            }
        }
    }

}
 