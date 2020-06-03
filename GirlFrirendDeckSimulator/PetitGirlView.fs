namespace GirlFriendDeckSimulator
open PetitGirl
open System.Collections

module PetitGirlView =
    type PetitGirlView(petitGirl: PetitGirl) =
        member val petitGirl = petitGirl
        member val PetitGirlName = 
            (match petitGirl.eventName with
            | None -> petitGirl.girlName
            | Some(evName) -> "[" + evName + "]" + petitGirl.girlName) with get, set
        member val AttributeType = petitGirl.attribute
        member val Attack = petitGirl.attack with get, set
        member val Defence = petitGirl.defence with get, set
        member val PetitCheerEffect1 = 
            (if petitGirl.petitCheerEffects.Length < 1 
                then "" 
                else PetitCheerTypeConverter.toString(petitGirl.petitCheerEffects.[0].petitCheerType) + " : " + petitGirl.petitCheerEffects.[0].effectNum.ToString() + "%") with get, set
        member val PetitCheerEffect2 = 
            (if petitGirl.petitCheerEffects.Length < 2
                then "" 
                else PetitCheerTypeConverter.toString(petitGirl.petitCheerEffects.[1].petitCheerType) + " : " + petitGirl.petitCheerEffects.[1].effectNum.ToString() + "%") with get, set
        member val PetitCheerEffect3 = 
            (if petitGirl.petitCheerEffects.Length < 3 
                then "" 
                else PetitCheerTypeConverter.toString(petitGirl.petitCheerEffects.[2].petitCheerType) + " : " + petitGirl.petitCheerEffects.[2].effectNum.ToString() + "%") with get, set
        member val PetitCheerEffect4 =
            (if petitGirl.petitCheerEffects.Length < 4 
                then "" 
                else PetitCheerTypeConverter.toString(petitGirl.petitCheerEffects.[3].petitCheerType) + " : " + petitGirl.petitCheerEffects.[3].effectNum.ToString() + "%") with get, set
        member val PetitSkillEffect =
            (match petitGirl.petitSkillEffect with
            | None -> ""
            | Some(petitSkillEffect) -> PetitSkillTypeConverter.toString(petitSkillEffect.petitSkillType) + " : " + petitSkillEffect.effectNum.ToString() + "%") with get, set
        member val PetitSelectionBonus =
            match petitGirl.selectionBonus with
            | None -> ""
            | Some(selectionBonus) -> selectionBonus.SelectionBonusName
        member val Attribute = petitGirl.attribute