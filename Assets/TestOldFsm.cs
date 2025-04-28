using System;
using System.Collections;
using System.Collections.Generic;
using ilsFramework;
using ilsFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestOldFsm : MonoBehaviour
{
    bool changeState = false;
    private bool fastChangeState1 = false;
    [ShowInInspector]
    private OldFSM<string> _oldFsm;
    // Start is called before the first frame update
    void Start()
    {
        _oldFsm = new OldFSM<string>();

        _oldFsm.AddState("state1", new OldState().AddOnEnterAction(()=>{Debug.Log("State1");})).AddTranslation("state1",new CommenTranslation(()=>changeState,()=>changeState = false),"state2");
        _oldFsm.AddState("state2", new OldState().AddOnEnterAction(()=>Debug.Log("State2"))).AddTranslation("state2",new CommenTranslation(()=>changeState,()=>changeState = false),"state3");
        _oldFsm.AddState("state3", new OldState().AddOnEnterAction(()=>Debug.Log("State3"))).AddTranslation("state3",new CommenTranslation(()=>changeState,()=>changeState = false),"state1");
        
        _oldFsm.AddAnyStateTranslation(new CommenTranslation(()=>fastChangeState1,()=>fastChangeState1 = false),"state1");
        _oldFsm.SetDefaultState("state1");
        _oldFsm.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            changeState = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            fastChangeState1 = true;
        }
        _oldFsm.Update();
       // Debug.Log(changeState);
    }
    
    
}
