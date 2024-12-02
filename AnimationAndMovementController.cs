using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isCrouchingHash;
    int isCrouchWalkingHash;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 cameraRelativeMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isCrouchingPressed;
    bool isCrouchWalkingPressed;
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 8.0f;

    
    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();   

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isCrouchWalkingHash = Animator.StringToHash("isCrouchWalking");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Crouch.started += onCrouch;
        playerInput.CharacterControls.Crouch.canceled += onCrouch;
        
    }
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    void onCrouch(InputAction.CallbackContext context)
    {
        isCrouchingPressed = context.ReadValueAsButton();
    }
     
    void handleRotation()
    {
        Vector3 positionToLookAt;
        
        positionToLookAt.x = cameraRelativeMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = cameraRelativeMovement.z;

        Quaternion currentRotation =  transform.rotation;
        if(isMovementPressed)
        {
        Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame*Time.deltaTime);
        }
    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isCrouching = animator.GetBool(isCrouchingHash);
        bool isCrouchWalking = animator.GetBool(isCrouchWalkingHash);

        if(isMovementPressed && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        if((isMovementPressed)&& isRunPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
            animator.SetBool(isCrouchingHash, false);
            animator.SetBool(isCrouchWalkingHash, false);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
        if(isMovementPressed && isCrouchingPressed && !isCrouching)
        {
            animator.SetBool(isCrouchWalkingHash, true);
        }
        else if((!isMovementPressed && isCrouchingPressed) && !isCrouching)
        {
            animator.SetBool(isCrouchingHash, true);
            animator.SetBool(isCrouchWalkingHash, false);
        }
        else if(!isMovementPressed && !isCrouchingPressed && isCrouching)
        {
            animator.SetBool(isCrouchingHash, false);
        }
        else if(isMovementPressed && !isCrouchingPressed)
        {
            animator.SetBool(isCrouchWalkingHash, false);
        }
        else if(isMovementPressed && isCrouchingPressed && isCrouching)
        {
            animator.SetBool(isCrouchWalkingHash, true);
            animator.SetBool(isCrouchingHash, false);
        }
        
    }
    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void handleGravity()
    {
        if(characterController.isGrounded)
        {
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity; 
        }
        else{
            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;    
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();
        handleGravity();
        if(isRunPressed)
        {
            cameraRelativeMovement= ConvertToCameraSpace(currentRunMovement);
            characterController.Move(cameraRelativeMovement * Time.deltaTime);
        }
        else 
        {
            cameraRelativeMovement= ConvertToCameraSpace(currentMovement);
            characterController.Move(cameraRelativeMovement * Time.deltaTime);
            }
        
    }
    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        //Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        //Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

          

       Vector3 vectorRotatedToCameraSpace= vectorToRotate.x * cameraRight + vectorToRotate.z * cameraForward;
       vectorRotatedToCameraSpace.y =  currentYValue;
       return vectorRotatedToCameraSpace;
    
    }
}
