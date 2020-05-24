namespace GirlFriendDeckSimulator
open System

module AttributeType = 
    type AttributeType =
        | Cool = 'C'
        | Pop = 'P'
        | Sweet = 'S'

    let getAllAttributeTypes = Enum.GetValues(typeof<AttributeType>)

module SkillAttributeType =
    type SkillAttributeType =
        | Cool
        | Pop
        | Sweet
        | All
        | Combination of attr1: SkillAttributeType * attr2: SkillAttributeType

