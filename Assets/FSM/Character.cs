﻿using UnityEngine;
using System.Collections.Generic;
using FSM;

public enum MovementAnimation
{
    Idle,
    Run
}

[RequireComponent(typeof(StateMachine))]
public class Character : MonoBehaviour 
{
    [SerializeField] private Animator animator;

    [SerializeField] private Transform target;
    [SerializeField] private AStar pathFinding;

    [SerializeField] private float singleMoveTime = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;

    private StateMachine fsm;

    private const string idleTrigger = "Idle";
    private const string runTrigger = "Run";

    private void Start()
    {
        fsm = GetComponent<StateMachine>();

        fsm.Initialize(this);

        Messenger.OnCheckNode += OnCheckNode;
        Messenger.OnSelectedTarget += OnSelectedTarget;
    }

    private void OnDestroy()
    {
        Messenger.OnCheckNode -= OnCheckNode;
        Messenger.OnSelectedTarget -= OnSelectedTarget;
    }

    private void MoveToTarget(Node targetNode)
    {
        Queue<Node> path = GetPath(targetNode);

        fsm.ChangeState(new FollowState(fsm, path, singleMoveTime, rotationSpeed));
    }

    private Queue<Node> GetPath(Node targetNode)
    {
        Queue<Node> path = null;

        Node startNode = GridManager.GetClosestNodeToWorldPosition(Transform.position);

        List<Node> nodes = pathFinding.FindPath(startNode, targetNode);

        if (nodes != null && nodes.Count > 0)
        {
            path = new Queue<Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                path.Enqueue(nodes[i]);
            }
        }

        return path;
    }

    private void OnCheckNode(Node node)
    {
        GetPath(node);
    }

    private void OnSelectedTarget(Node node)
    {
        MoveToTarget(node);
    }

    public void PlayMovementAnimation(MovementAnimation movementAnimation)
    {
        switch (movementAnimation)
        {
            case MovementAnimation.Idle:
                animator.ResetTrigger(runTrigger);
                animator.SetTrigger(idleTrigger);
                break;
            case MovementAnimation.Run:
                animator.ResetTrigger(idleTrigger);
                animator.SetTrigger(runTrigger);
                break;
        }
    }

    private Transform myTransform;

    public Transform Transform
    {
        get
        {
            if (myTransform == null)
            {
                myTransform = GetComponent<Transform>();
            }

            return myTransform;
        }

        set
        {
            myTransform = value;
        }
    }
}
