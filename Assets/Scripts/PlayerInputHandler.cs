using System.Linq;

using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset playerControls;
    [Header("Actions Map Name Reference")]
    [SerializeField] private string mapName = "Player";
    [Header("Actions Name Reference")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";

    [Header("Movement Joystick")]
    [SerializeField] private TouchJoystick movementJoystick;
    [SerializeField] private bool isJoystickActive = false;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    public static PlayerInputHandler Instance{get; private set;}

    public Vector2 MoveInput {get; private set;}
    public Vector2 LookInput {get; private set;}
    public bool JumpTriggered {get; private set;}
    public float SprintValue {get; private set;}

    

    void Awake(){
        if(Instance==null){
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else{
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(mapName).FindAction(move);
        lookAction = playerControls.FindActionMap(mapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(mapName).FindAction(jump);
        sprintAction = playerControls.FindActionMap(mapName).FindAction(sprint);
        RegisterInputActions();
    }

    

    void Update(){
        if(isJoystickActive){
            Vector2 joystickInput = movementJoystick.GetJoystickInput();
            if(joystickInput.magnitude>0) MoveInput = joystickInput;
            else MoveInput = Vector2.zero;
        }
    }

    void RegisterInputActions(){
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>(); 
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;
    }

    void OnEnable(){
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
    }
    void OnDisable(){
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
    }

    

    
}
