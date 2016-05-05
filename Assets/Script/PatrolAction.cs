using UnityEngine;
using System;
using System.Collections.Generic;

/*Python

update(sensor):
    active: detect enemy
    passive: hit door, hit food -> hit smart object

-->  

detect_enemy = check_detect_enemy(sensor)
if detect_enemy:
    handle_enemy()  # attack, quite interesting part
    return

hit_door = check_hit_door(sensor)
if hit_door:
    handle_door()  # open the door
    return

find_food = check_find_food(sensor)
if find_food:
    handle_food()  # pick and eat
    return

find_bullet = check_find_bullet(sensor)
if find_bullet:
    handle_bullet  # pick
    return

(...)

goto_next_patrolpoint();

-->

if handle_enemy():
    return;

if handle_door():
    return

if handle_food():
    return

if handle_bullet():
   return

(...) :
    handle_other_actions
    make_other_desicions

handle_goto_next_patrolpoint()


Q: which is door failed to open?

# other actions

if handle_jump_obstacle():
    return;

if handle_jump_fall():
    return;

if handle_crouch():
    return;

if handle_crouch():
    return;
**/

[Serializable]
public class PatrolAction {
    public Bot Bot;
    public Sensor Sensor;

    private AttackAction _attackAction;
    private DoorAction _doorAction;
    private InspectAction _inspectAction;
    private CrouchAction _crouchAction;

    public bool Start(Task task_) { 
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            // not important
            Bot._patrolStatus = "inspecting";
            _attackAction = new AttackAction(){
                Bot = Bot,
            };
            _doorAction = new DoorAction(){
                Bot = Bot,
            };
            _inspectAction = new InspectAction(){
                Bot = Bot,
            };
            _inspectAction.patrolTask = patrolTask;
            _crouchAction = new CrouchAction(){
                Bot = Bot,
            };
            return true;
        }else{
            return false;
        }
    }

    public bool Update(Task task_){
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            // Is shooting
            if (Bot.status == "shoot") return true;

            // TODO: 
            // (1) interrupt
            // (2) animation
            if (Bot.status == "crouch") return true;
            // patrolStatus == "crouch"
            if (Bot.status == "open_door") return true;
            // patrolStatus == smartdoor
            //
            if (Bot.status == "standup") return true;

            // _attackAction.AttackTarget = Sensor.AttackTarget;
            if (_attackAction.Handle()){
                return true;
            }

            // _doorAction.Door = Sensor.Door;
            if (_doorAction.Handle()){
                return true;
            }

            // _crouchAction.Obstacle = Sensor.Obstacle;
            if (_crouchAction.Handle()){
                return true;
            }

            // do nothing but set status
            if (_inspectAction.Handle()){
                // return;
            }
            return true;
        }else{
            return false;
        }
    }
}
