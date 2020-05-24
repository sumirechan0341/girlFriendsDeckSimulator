namespace GirlFriendDeckSimulator
open AttributeType
open Mode

module PetitGirl =
    type PetitCheerType =
        | AttributeCheerType of attrType: AttributeType * mode: Mode * effectNum: float
        | AllAttributeCheerType of mode: Mode * effectNum: float
        | GradeCheerType of grade: int * effectNum: float
        | FavoriteCheerType of effectNum: float
        | DatingCheerType of effectNum: float
        | SelfCheerType of effectNum: float
        | TouchCheerType of effectNum: float
        | BirthdayCheerType of effectNum: float
        | FacilityBuildUpType of effectNum: float
        | GalIncomeUpType of effectNum: float
        | LikeabilityUpType of effectNum: float
        | DeckCostDownType of effectNum: float

    type PetitSkillType =
        | AttributeSkillType of attrType: AttributeType * mode: Mode * skillNum: int
        | AllAttributeSkillType of mode: Mode * skillNum: int

    type PetitGirl(girlName, petitCheerTypes, petitSkillType, ?selectionBonus, ?eventName) =
        let girlName: string = girlName
        let eventName: Option<string> = eventName
        let petitCheerTypes: PetitCheerType[] = petitCheerTypes
        let petitSkillType: PetitSkillType = petitSkillType
        let selectionBonus: Option<string> = selectionBonus
        
        
