***************************************
Name and Contact Info
***************************************
(in)Finite State Machine
Charles Corbin 2015
http://www.charlesgcorbin.com/
https://github.com/cgcorbin/Public-Code

***************************************
Overview:
***************************************
The (in)Finite State Machine is a Finite State Machine that can be flayed in 
any number of sub states, each sub state is capable of being flayed into more 
sub states, and so on etc.

The iFSM functions by using actions (delegates) which are functions that are 
given to the iFSM  by the object it is meant to be controlling.

the iFSM was born from a need for a reusable FSM class that did not need a 
reference to the object it needed to control, or API calls for every member variable
in a class that I might want the FSM to have access to.

The iFSM was built to simply be a vehicle for functions within the subject class, so that
object behaviors could be fully controlled without messy reference passing or encapsulation
threatening APIs.


***************************************
Example:
***************************************

Given a simple Enemy, desired states might be:

- Patrol
  - Aimless
  - Waypoint
- Hunt
  - Direct
  - Flank
- Attacking
  - Melee Attacking
  - Ranged Attacking
- Fleeing

You then write your functions to perform those actions in your Enemy class:

Enemy::wanderAimless() - To be used by the Patrol->Aimless State
Enemy::patrolWayPoints() - To be used by the Patrol->Waypoints State
Enemy::huntForPlayer() - To be used by the Hunt->Direct State
Enemy::flankPlayer() - To be used by the Hunt->Flank State
Enemy::attackSword() - To be used by the Attacking->Melee Attacking State
Enemy::attackGun() - To be used by the Attacking->Ranged Attacking State
Enemy::runAway() - To be used by the Fleeing State

Writing these functions in your Enemy class gives them full access to your 
Enemy's member variables, so there's no passing around references and messy 
function signitures.

To add states your simply:

fsm.AddState("key")
or
fsm.AddState("key", defaultAction)

for the first, or root states of our example you would:

fsm.AddState("patrol")
fsm.AddState("hunt")
fsm.AddState("attacking")

Fleeing doesn't have sub states acording to our spec above, so we set a 
default action:
fsm.AddState("fleeing", runAway)

For the other states, we add our sub states:

fsm.GetState("patrol").AddState("aimless", wanderAimless)
fsm.GetState("patrol").AddState("waypoint", patrolWayPoints)

fsm.GetState("hunt").AddState("direct", huntForPlayer)
fsm.GetState("hunt").AddState("flank", flankPlayer)

fsm.GetState("attacking").AddState("melee attack", attackSword)
fsm.GetState("attacking").AddState("ranged attack", attackGun)

Now, if we were to draw a diagram of your iFSM it would look something like:






              aimless
            /
      Patrol
    /       \
              waypoint


            direct
          /
    - Hunt
          \
            flank

fsm
                 melee attack
               /
    - Attacking
               \
                 ranged attack

    \
      Fleeing


To change root states, it's as simple as calling fsm.ChangeState("key")

There is also ChangeState("Key"), transisionAction), the transision action 
is another function like our other Action functions, but it will be called 
once as soon as the state changes and never again.

To change sub states, we retreive the current state from the fsm and then call the 
change state from above:

fsm.currentState.ChangeState("sub state key")

Depending on how deep you want to build your state tree (they can get unruley 
past a certain point) you could have a line like this:
fsm.currentState.currentState.currentState.currentState.ChangeState("sub state key")

For situations like that I recommend creating another iFSM reference and using 
it like this:
iFSM wayDeepState = fsm.currentState.currentState.currentState.currentState

***************************************
Issues and Concerns:
***************************************
The iFSM has been used successfully in production of a small FPS XNA game and showed no 
obvious signs of weakness HOWEVER it has NOT been stress tested, for example I have no idea 
it would handle an extremely deep state tree, so be aware of that.