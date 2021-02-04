namespace GirlFriendDeckSimulator
open FSharp.Data
open PetitGirl
open System.IO
open SelectionBonus
open System


module PetitGirlFactory =
    type PetitGirlElements = JsonProvider<""" 
        {
            "girlName":"上条るい",
            "eventName":"猫コス18",
            "rarity":"SR",
            "attribute":"COOL",
            "attack":13674,
            "defence":11603,
            "petitCheerEffects":[
                {"CheerType":"COOLﾀｲﾌﾟの攻援UP","CheerEffectNum":9.5,"CheerEffectMode":"UP"},
                {"CheerType":"本命ｶﾞｰﾙの攻守UP","CheerEffectNum":11,"CheerEffectMode":"UP"},
                {"CheerType":"誕生日のｶﾞｰﾙの攻守UP","CheerEffectNum":19,"CheerEffectMode":"UP"},
                {"CheerType":"ﾎﾜｲﾄﾎﾞｰﾄﾞの効果UP","CheerEffectNum":120,"CheerEffectMode":"UP"}
            ],
            "petitSkillEffect":{
                "petitSkillEffectType":"COOLﾀｲﾌﾟのぷちｶﾞｰﾙ攻守UP",
                "petitSkillEffectNum":17,"petitSkillEffectMode":"UP"
            },
            "selectionBonus":["↑スレンダーズボルケーノ↑"]
        } """>
    let petitGirlFactoryFromJson(petitGirlJson) = 
        let petitGirlElements = PetitGirlElements.Parse(petitGirlJson)
        {
            girlName = petitGirlElements.GirlName
            eventName = 
                match petitGirlElements.EventName with
                    | "" -> None
                    | _ -> Some(petitGirlElements.EventName)
            attribute = AttributeTypeConverter.fromString(petitGirlElements.Attribute)
            rarity = PetitGirlRarityConverter.fromString(petitGirlElements.Rarity)
            attack = petitGirlElements.Attack
            defence = petitGirlElements.Defence
            petitCheerEffects = Array.map(fun (cheerEffect: PetitGirlElements.PetitCheerEffect) ->
                {
                    petitCheerType = PetitCheerTypeConverter.fromString(cheerEffect.CheerType);
                    effectNum = cheerEffect.CheerEffectNum |> float;
                    //targetPetitGirlName = 
                    //    match PetitCheerTypeConverter.fromString(cheerEffect.CheerType) with
                    //    | SameGirlCheerType(_) -> Some(petitGirlElements.GirlName)
                    //    | _ -> None
                }
                ) petitGirlElements.PetitCheerEffects
            petitSkillEffect = 
                match petitGirlElements.PetitSkillEffect.PetitSkillEffectType with
                | "" -> None
                | _ -> Some(
                    {
                        petitSkillType = PetitSkillTypeConverter.fromString(petitGirlElements.PetitSkillEffect.PetitSkillEffectType);
                        effectNum = petitGirlElements.PetitSkillEffect.PetitSkillEffectNum |> float;
                    })
            selectionBonus = 
                Array.map (SelectionBonus) petitGirlElements.SelectionBonus
            petitGirlJson = petitGirlElements.JsonValue
        }
        
    let petitGirlList = 
        Seq.map (petitGirlFactoryFromJson) <| 
        if File.Exists(".\PetitGirl.txt")
        then
            Seq.filter (String.IsNullOrWhiteSpace >> not) (File.ReadLines(".\PetitGirl.txt"))
        else Seq.empty