namespace GirlFriendDeckSimulator
open System

module AttributeType = 
    type AttributeType =
        | Cool = 'C'
        | Pop = 'P'
        | Sweet = 'S'

    let getAllAttributeTypes = Enum.GetValues(typeof<AttributeType>)

module SkillAttributeType =
    open AttributeType
    type SkillAttributeType =
        | Cool
        | Pop
        | Sweet
        | All
        | Combination of attr1: SkillAttributeType * attr2: SkillAttributeType
    type SkillAttributeType with
    member this.isAppliableOn(attributeType: AttributeType) =
        match this with
        | Cool -> attributeType = AttributeType.Cool
        | Pop -> attributeType = AttributeType.Pop
        | Sweet -> attributeType = AttributeType.Sweet
        | All -> true
        | Combination(attr1, attr2) -> attr1.isAppliableOn(attributeType) || attr2.isAppliableOn(attributeType)

