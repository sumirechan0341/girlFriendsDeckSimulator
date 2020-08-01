namespace GirlFriendDeckSimulator
open SkillAttributeType
open Mode
open Club
open Grade
open SelectionBonus
open AttributeType
open FSharp.Data

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
        effectNum: float;
        targetPetitGirlName: option<string>
    }

    type PetitSkillType =
        | AttributeSkillType of skillAttrType: SkillAttributeType * mode: Mode
    
    type PetitSkillEffect = 
        {
            petitSkillType: PetitSkillType;
            effectNum: float
        }       
    type PetitGirlRarity = 
        SSR
        | SR
        | HR
        | R

    type PetitGirl = {
        girlName: string
        eventName: option<string>
        attribute: AttributeType
        rarity: PetitGirlRarity
        attack: int
        defence: int
        petitCheerEffects: PetitCheerEffect[]
        petitSkillEffect: option<PetitSkillEffect>
        selectionBonus: SelectionBonus[]
        petitGirlJson: JsonValue
    }
        
        