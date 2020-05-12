namespace GirlFriendDeckSimulator
open AttributeType
open Mode

module PetitGirl =
    type PetitCheerType =
        | AttributeCheerType of attrType: AttributeType * mode: Mode * effectNum: int
        | AllAttributeCheerType of mode: Mode * effectNum: int
        | GradeCheerType of grade: int * effectNum: int
        | FavoriteCheerType of effectNum: int
        | DatingCheerType of effectNum: int
        | SelfCheerType of effectNum: int
        | TouchCheerType of effectNum: int
        | BirthDayCheerType of effectNum: int
        | FacilityBuildUpType of effectNum: int

    type PetitSkillType =
        | AttributeSkillType of attrType: AttributeType * mode: Mode * skillNum: int
        | AllAttributeSkillType of mode: Mode * skillNum: int

    type PetitGirl(girlName, petitCheerTypes, petitSkillType, ?selectionBonus, ?eventName) =
        let girlName: string = girlName
        let eventName: Option<string> = eventName
        let petitCheerTypes: PetitCheerType[] = petitCheerTypes
        let petitSkillType: PetitSkillType = petitSkillType
        let selectionBonus: Option<string> = selectionBonus
        
        
