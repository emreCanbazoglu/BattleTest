﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSMStateBehaviour : StateMachineBehaviour
{
    public FSMType FSMType;
    public FSMStateID StateID;

    public bool IsDebugEnabled;

    FSMController _fsmController;
    FSMBehaviourController _bc;

    float _lastEnteredFrame;
    float _lastExitedFrame;

    #region Events

    public Action<FSMStateID> OnStateEntered;

    void FireOnStateEntered()
    {
        if (OnStateEntered != null)
            OnStateEntered(StateID);
    }

    public Action<FSMStateID> OnStateExited;

    void FireOnStateExited()
    {
        if (OnStateExited != null)
            OnStateExited(StateID);
    }

    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*if (_lastEnteredFrame == Time.frameCount)
        {
            if(IsDebugEnabled)
                Debug.Log("tried to enter, failed: " + _lastEnteredFrame);

            return;
        }*/

        _lastEnteredFrame = Time.frameCount;

        if (IsDebugEnabled)
            Debug.Log("on state entered: " + StateID + " " + _lastEnteredFrame);

        FireOnStateEntered();

        if (_fsmController == null)
            InitFSMController(animator);

        if (_bc == null)
            InitEnterBT(animator);

        if (_bc != null)
            _bc.Execute();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*if (_lastExitedFrame == Time.frameCount)
            return;*/

        _lastExitedFrame = Time.frameCount;

        /*if (IsDebugEnabled)
            Debug.Log("on state exit: " + StateID);*/

        FireOnStateExited();

        if (_fsmController == null)
            InitFSMController(animator);

        if (_bc != null)
            _bc.Exit();
    }

    void InitFSMController(Animator animator)
    {
        _fsmController = animator.transform.parent.GetComponent<FSMController>();
    }

    void InitEnterBT(Animator animator)
    {
        _bc = _fsmController.GetBehaviourControllerOfState(FSMType, StateID);
    }
}