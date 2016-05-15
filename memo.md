(1) Manual Fire/Shoot

Manual control && Auto planning:

-  Plan on actions to take 
-  Movement 

(2) Jump 

栅栏 鸿沟

攀爬 梯子 墙

(3) Bot AI

Enemy/Creep

Buddy


Avatar

-> Bot

   -> goto          ---
   -> shoot            |
   -> crouch            => Motion: Animator (Animation State Machine)
   -> standup          |
   (...)            ---

Note: for other Bots:

-  have different motions, bots take different actions
   e.g. shoot with gun, attack with knife

   attack => animation + weapon:
   choose animation according to the current weapon
 
-  share some motions, e.g. goto 
   even for smae motion, may different (??), e.g. goto

-  dont have some motions, e.g. crouch, standup

-  have other unique motions, climb


-> Sensor

-> AutoMotor

-> TaskPlanner

   -> GotoTask
   -> PatrolTask
