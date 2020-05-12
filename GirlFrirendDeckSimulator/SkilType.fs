namespace GirlFriendDeckSimulator
open AttributeType
open Mode
module SkillType =
    type SkillTarget = 
        Front 
        | Back of targetNum: int
        | FrontAndBack1 
        | MySelf 
        | SameGirl 
        | OpponentFront

    type Effect = 
        Ultra
        | Super 
        | ExtraLarge 
        | Large 
        | Middle 
        | Small
        | Random of min: Effect * max: Effect
    
    type SkillMode = Up | Down

    type SkillType = {
        attribute: AttributeType
        target: SkillTarget
        effect: Effect
        mode: Mode
        skillMode: SkillMode
    }
    