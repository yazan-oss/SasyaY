﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Purgatory
{
    public class MonitorInteractingAnimation : StateAction
    {
        CharacterStateManager states;
        string targetBool;
        string targetState;

        public MonitorInteractingAnimation(CharacterStateManager characterStateManager, string targetBool,string targetState)
        {
            states = characterStateManager;
            this.targetBool = targetBool;
            this.targetState = targetState;
        }
        public override bool Execute()
        {
            bool isInteracting = states.anim.GetBool(targetBool);
            states.isInteracting = isInteracting;

            if (isInteracting)
            {
                return false;
            }
            else
            {
                states.ChangeState(targetState);

                return true;
            }
        }
    }
}

