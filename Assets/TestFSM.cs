using System;
using System.Collections;
using System.Collections.Generic;
using ilsFramework;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestFSM : MonoBehaviour
{
    bool changeState = false;
    private bool fastChangeState1 = false;
    [ShowInInspector]
    private FSM<string> fsm;
    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSM<string>();

        fsm.AddState("state1", new State().AddOnEnterAction(()=>{Debug.Log("State1");})).AddTranslation("state1",new CommenTranslation(()=>changeState,()=>changeState = false),"state2");
        fsm.AddState("state2", new State().AddOnEnterAction(()=>Debug.Log("State2"))).AddTranslation("state2",new CommenTranslation(()=>changeState,()=>changeState = false),"state3");
        fsm.AddState("state3", new State().AddOnEnterAction(()=>Debug.Log("State3"))).AddTranslation("state3",new CommenTranslation(()=>changeState,()=>changeState = false),"state1");
        
        fsm.AddAnyStateTranslation(new CommenTranslation(()=>fastChangeState1,()=>fastChangeState1 = false),"state1");
        fsm.SetDefaultState("state1");
        fsm.Start();
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
        fsm.Update();
       // Debug.Log(changeState);
    }
    
    
}
