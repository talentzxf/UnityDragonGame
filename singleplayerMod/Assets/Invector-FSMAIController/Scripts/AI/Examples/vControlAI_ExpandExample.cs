using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    /// <summary>
    /// This is a simple example on how to expand the AI Controller by creating your own methods, you don't need to modify the core scripts
    /// Simple expand the Interface and the Controller here, and you can create new FSM Custom Actions & Decitions to call your custom methods
    /// </summary>    

    // First create a Partial Interface vIControlAI to expand access to the FSM with new methods and variables
    public partial interface vIControlAI
    {
        // Here we created a variable that can be access from my New Action
        GameObject customTarget { get; }
        bool _moveToTarget { get; }

        // Declaring the new Method that will be implemented below
        void MoveToTargetExample(vAIMovementSpeed speed = vAIMovementSpeed.Running);
    }

    // Now create a Partial class vControlAI and implement the new methods and variables from our Interface above 
    public partial class vControlAI
    {
        // The vEditorToolbar will display a new tab in your AI Controller Inspector
        [vEditorToolbar("Custom Properties")]

        // This variable will appear in the new Tab 'Custom Properties'
        [vHelpBox("Check the script 'vControlAI_ExpandExample' to see how you can expand the controller by creating your own methods and variables", vHelpBoxAttribute.MessageType.Info)]
        public GameObject myTarget;

        // Just a bool so we can test the method, check true via inspector to make the controller MoveTo the target assigned
        public bool moveToTarget;

        // We need to create the bool again so we can pass this information to the FSM 
        public bool _moveToTarget { get => moveToTarget; }

        // This is a simple example for the FSM to have access to the variable myTarget
        public GameObject customTarget { get => myTarget; }

        /// <summary>
        /// Make it public so you can call it when you create a new FSM CustomAction
        /// To create a new FSM CustomAction open the FSM Window > FSM Component click on the '+' button > Action > New Action Script
        /// Then simple call your method in the DoAction like this:
        /// fsmBehaviour.aiController.MyNewMethod();
        /// </summary>
        /// <param name="speed">Set the speed of the locomotion</param>
        public void MoveToTargetExample(vAIMovementSpeed speed = vAIMovementSpeed.Running)
        {
            // Simple example to move the controller to a specific position set via inspector     
            
            if(myTarget != null)
            {                
                // Here we simple call the method MoveTo from the original vControlAI script

                MoveTo(myTarget.transform.position, speed);
                Debug.Log("Controller Calling 'MoveTo' ");
            }
            else
            {
                Debug.Log("Controller Calling 'MoveTo' but a 'myTarget' is not assigned");
            }
        }

        /// <summary>
        /// Public method so we can set the variavel 'moveToTarget' using Inspector Events
        /// </summary>
        public void SetMoveToTarget_Example()
        {
            moveToTarget = !moveToTarget;
        }
    }
}