/*
The MIT License (MIT)

Copyright (c) <year> <copyright holders>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Charles G Corbin 2015
http://www.charlesgcorbin.com/

Ver. 1: March 2014
Ver. 2: February 2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cgc.DataStructures
{
    /// <summary>
    /// (in)Finite State Machine. An FSM that can be flayed into multiple sub states, which can in turn be flayed into sub states, etc etc.
    /// Each state also has a list of functions that are called on Run
    /// </summary>
    class iFSM
    {
        public delegate void StateAction(object sender);
        public delegate void TransitionAction(object sender);

        List<StateAction> actions;

        Dictionary<String, iFSM> states;
        String currentStateID = String.Empty;
        iFSM currentState;

        iFSM myParent;

        bool runMyActionsAndSubStateActions = false;
        public enum ActionRunOrder { RunFirst, RunLast };
        ActionRunOrder myRunOrder = ActionRunOrder.RunFirst;

        #region Properties
        /// <summary>
        /// Get the current (sub)state object.
        /// To set the current state use ChangeState and it's overloads
        /// </summary>
        public iFSM CurrentState
        {
            get { return currentState; }
        }

        /// <summary>
        /// Get the current (sub)State ID string.
        /// </summary>
        public String CurrentStateID
        {
            get { return currentStateID; }
        }

        /// <summary>
        /// Get *this* state's Parent state object.
        /// </summary>
        public iFSM Parent
        {
            get { return myParent; }
        }

        /// <summary>
        /// Flag that allows a state's actions to run if the state has sub states
        /// </summary>
        public bool RunMyActionsAndSubStateActions
        {
            get { return runMyActionsAndSubStateActions; }
            set { runMyActionsAndSubStateActions = value; }
        }

        /// <summary>
        /// Sets the run order of the state's actions.
        /// Run first will run *this* state's actions first, then execute current sub state's actions.
        /// Run last will do the opposite.
        /// </summary>
        ActionRunOrder MyRunOrder
        {
            get { return myRunOrder; }
            set { myRunOrder = value; }
        }
        #endregion

        /// <summary>
        /// Default iFSM Constructor
        /// </summary>
        public iFSM()
        {
            actions = new List<StateAction>();
            states = new Dictionary<String, iFSM>();
        }

        private iFSM(iFSM parent)
        {
            actions = new List<StateAction>();
            states = new Dictionary<String, iFSM>();
            this.myParent = parent;
        }

        /// <summary>
        /// Run the state's actions
        /// </summary>
        public void Run()
        {
            if (currentState == null && !runMyActionsAndSubStateActions)
            {
                foreach (StateAction sa in actions)
                {
                    sa(this);
                }
            }
            else
            {
                if (myRunOrder == ActionRunOrder.RunLast)
                {
                    currentState.Run();
                }

                foreach (StateAction sa in actions)
                {
                    sa(this);
                }

                if (myRunOrder == ActionRunOrder.RunFirst)
                {
                    currentState.Run();
                }
            }
        }

        /// <summary>
        /// Add an Action(function) to the state
        /// </summary>
        /// <param name="action">The function matching the StateAction delegate signature</param>
        public void AddAction(StateAction action)
        {
            actions.Add(action);
        }

        /// <summary>
        /// Change the current state to the state at stateID
        /// </summary>
        /// <param name="stateID">The key of the desired state</param>
        public void ChangeState(String stateID)
        {
            if (states.ContainsKey(stateID) & currentStateID != stateID)
            {
                currentState = states[stateID];
                currentStateID = stateID;
            }
        }

        /// <summary>
        /// Change the current state to the state at stateID and calls the transition functions
        /// </summary>
        /// <param name="stateID">The key of the desired state</param>
        /// /// <param name="transitionAction">The function to call as the state changes</param>
        public void ChangeState(String stateID, TransitionAction transitionAction)
        {
            if (states.ContainsKey(stateID) & currentStateID != stateID)
            {
                currentState = states[stateID];
                currentStateID = stateID;
                transitionAction(this);
            }
        }

        /// <summary>
        /// Retrieve the state at StateID
        /// </summary>
        /// <param name="stateID">The key of the desired state</param>
        /// <returns>The desired state</returns>
        public iFSM GetState(String stateID)
        {
            if (states.ContainsKey(stateID))
            {
                return states[stateID];
            }

            return null;
        }

        /// <summary>
        /// Add a state to the states.
        /// </summary>
        /// <param name="stateID">The desired state</param>
        public void AddState(String stateID)
        {
            if (!states.ContainsKey(stateID))
            {
                states.Add(stateID, new iFSM(this));
            }
            else
            {
                throw new ArgumentException("Key: " + stateID + " Already present in states dictionary");
            }
        }

        /// <summary>
        /// Add a state to the states with a default action.
        /// </summary>
        /// <param name="stateID">The desired state</param>
        /// <param name="action">The default action of the state</param>
        public void AddState(String stateID, StateAction action)
        {
            AddState(stateID);
            states[stateID].AddAction(action);
        }
    }
}
