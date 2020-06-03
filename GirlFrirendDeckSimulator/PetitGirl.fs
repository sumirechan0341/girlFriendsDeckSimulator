namespace GirlFriendDeckSimulator
open SkillAttributeType
open Mode
open Club
open Grade
open SelectionBonus
open AttributeType

module PetitGirl =
    type EffectDegree = Large | Middle | Small
    type PetitCheerType = 
        | AttributeCheerType of skillAttrType: SkillAttributeType * mode: Mode
        | GradeCheerType of grade: Grade * mode: Mode
        | FavoriteCheerType of mode: Mode
        | DatingCheerType of mode: Mode
        | SameGirlCheerType of mode: Mode
        | TouchCheerType
        | BirthdayCheerType of mode: Mode
        | FacilityBuildUpType of targetFacility: Facility
        | GalIncomeUpType
        | LikeabilityUpType
        | DeckCostDownType
        | ExpIncomeUpType

    type PetitCheerEffect = {
        petitCheerType: PetitCheerType;
        effectNum: float
    }

    type PetitSkillType =
        | AttributeSkillType of skillAttrType: SkillAttributeType * mode: Mode
    
    type PetitSkillEffect = 
        {
            petitSkillType: PetitSkillType;
            effectNum: float
        }       

    type PetitGirl = {
        girlName: string
        eventName: option<string>
        attribute: AttributeType
        attack: int
        defence: int
        petitCheerEffects: PetitCheerEffect[]
        petitSkillEffect: option<PetitSkillEffect>
        selectionBonus: option<SelectionBonus>
    }
        
        