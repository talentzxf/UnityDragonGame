The player controller is controlled by detatching the rigidbody sphere, and adding force to move and roll it around the scene, the controller then follows this rigidbody. This system helps to add more realistic movement with momentum

handly return speed: how quickly control is returned to the character after a boost is applied

ground layers: which layers are the ground

max speed: our max walking speed
speed clamp: the max speed our character can move at before it is limited
acceleration: how quickly we build speed 
MovementAcceleration: how quickly we adjust to new speeds, acting on our rigidbody
SlowDownAcceleration: how quickly we slow down
turnSpeed: how quickly we turn on the ground
   
AirAcceleration: how quickly we adjust to new speeds when in air
turnSpeedInAir: how much we can turn when in mid air
FallingDirectionSpeed: how quickly we will return to a normal direction

FlyingDirectionSpeed: how much influence our direction relative to the camera will influence our flying
FlyingRotationSpeed: how fast we turn in air overall
FlyingUpDownSpeed: how fast we rotate up and down
FlyingLeftRightSpeed: how fast we rotate left and right
FlyingRollSpeed: how fast we roll when flying

FlyingAcceleration: how much we accelerate to max speed
FlyingDecelleration: how quickly we slow down when flying
FlyingSpeed: our max flying speed
FlyingMinSpeed: our flying slow down speed, when we are not holding the fly button

FlyingAdjustmentSpeed: how quickly our velocity adjusts to the flying speed
 
FlyingGravityAmt /how much gravity will pull us down when flying
GlideGravityAmt /how much gravity affects us when just gliding
FlyingGravBuildSpeed /how much our gravity is lerped when stopping flying

FlyingVelocityGain /how much velocity we gain for flying downwards
FlyingVelocityLoss /how much velocity we lose for flying upwards
FlyingLowerLimit /how much we fly down before a boost
FlyingUpperLimit /how much we fly up before a boost; 
GlideTime /how long we glide for when not flying before we start to fall

JumpAmt /how much we jump upwards 
JumpGravityBoost /how much our gravity is reduced to when we jump (This is minused
GroundedTimerBeforeJump /how long we have to be on the floor before an action can be made
JumpForwardAmount /how much our regular jumps move us forward

SpeedLimitBeforeCrash /how fast we have to be going to crash
StunPushBack /how much we are pushed back
StunnedTime /how long we are stunned for
StunTimer /the in use stun timer

  
    