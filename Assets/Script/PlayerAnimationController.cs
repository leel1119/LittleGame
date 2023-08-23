using BasicCode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationControler : MonoBehaviour
{
    private PlayerMovementController playerMovementController;
    private Animator animator;
    private int velXHash, velYHash;
    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = GetComponentInParent<PlayerMovementController>();
        animator = GetComponent<Animator>();
        velXHash = Animator.StringToHash("VelX");
        velYHash = Animator.StringToHash("VelY"); ;
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        //animator.SetFloat("VelX", 0f);
        Vector3 localMoveDirection = transform.InverseTransformDirection(playerMovementController.DeltaPos);
        animator.SetFloat(velXHash, localMoveDirection.x * playerMovementController.MoveSpeed * 10f);
        animator.SetFloat(velYHash, localMoveDirection.z * playerMovementController.MoveSpeed * 10f);
    }
}
