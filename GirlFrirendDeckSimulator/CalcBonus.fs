namespace GirlFriendDeckSimulator
open FsXaml
open Card
open Club
open AttributeType
open FSharp.Data
open System.IO
open GirlFactory
open System.Windows
open System
open PetitGirl
open EventType
open Player
open PreciousScene
open PreciousSceneView
open SkillType
open SelectionBonus
open CardView
open Girl
open PetitGirlView


module CalcBonus =
    type PlayerParameterTab = XAML<"PlayerParameterTab.xaml">
    type Settings = JsonProvider<".\Setting.json">

    let applySkillBonus(frontDeck: ResizeArray<CardViewWithStrap>, backDeck: ResizeArray<CardView>) =
        let checkSkillTriggeredCards = [||] |> ResizeArray
        for frontCard in frontDeck do
            frontCard.AppliedAttackSkillBonus.RemoveAll(fun c -> true) |> ignore
            frontCard.AppliedDefenceSkillBonus.RemoveAll(fun c -> true) |> ignore
            checkSkillTriggeredCards.Add(frontCard :> CardView)
        for backCard in backDeck do
            backCard.AppliedAttackSkillBonus.RemoveAll(fun c -> true) |> ignore
            backCard.AppliedDefenceSkillBonus.RemoveAll(fun c -> true) |> ignore
            if backCard.Card.cardType = Card.CardType.Switch
            then
                checkSkillTriggeredCards.Add(backCard)
            else 
                0 |> ignore
        

        let settings = Settings.Parse(File.ReadAllText(".\Setting.json"))
        for skillRaiseFrontCard in checkSkillTriggeredCards do
            if skillRaiseFrontCard.IsTriggeredSkillBonus
            then
                match skillRaiseFrontCard.Card.skillType with
                | None -> 0 |> ignore
                | Some(skill) ->
                    match skill.skillMode with
                    | SkillType.SkillMode.Down -> 0 |> ignore // 未実装!
                    | SkillType.SkillMode.Up ->
                        match skill.target with
                        | SkillTarget.Front ->
                            for frontCard in frontDeck do
                                match skill.attribute with
                                | SkillAttributeType.Cool ->
                                    if frontCard.Card.attribute = AttributeType.Cool
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大Upに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大Upに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大Upに
                                    else 0 |> ignore
                                | SkillAttributeType.Pop ->
                                    if frontCard.Card.attribute = AttributeType.Pop
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.Sweet ->
                                    if frontCard.Card.attribute = AttributeType.Sweet
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.Defence ->
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            | Mode.AttackAndDefence ->
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                    //暫定的にスーパー特大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.All ->                              
                                    match skill.effect with
                                    | SkillType.Effect.ExtraSuper ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    | SkillType.Effect.Super ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float) 
                                    | SkillType.Effect.ExtraLarge ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    | SkillType.Effect.Large ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    | SkillType.Effect.Middle ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    | SkillType.Effect.Small ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.Defence ->
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        | Mode.AttackAndDefence ->
                                            frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                            frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    | SkillType.Effect.Random(skillMin, skillMax) ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的にスーパー特大UPに
                                            // 大から特大もいる あとで実装
                                        | Mode.Defence ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的にスーパー特大UPに
                                        | Mode.AttackAndDefence ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                frontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                frontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                                //暫定的にスーパー特大UPに
                                    
                        | SkillTarget.FrontAndBack1 ->
                            let backTop = backDeck.ToArray() |> Array.tryFind (fun cardView -> skill.attribute.isAppliableOn(cardView.Card.attribute))
                            let targets = (Array.map (fun cardWithStrap -> (true, cardWithStrap :> CardView)) <| frontDeck.ToArray()) |> ResizeArray
                            if backTop.IsSome
                            then 
                                targets.Add((false, backTop.Value))
                            else
                                0 |> ignore
                            for targetPair in targets do
                                let target = snd targetPair
                                //let isInFrontDeck = fst targetPair
                                //let backDeckFactor = if isInFrontDeck then 1.0 else 0.8
                                let backDeckFactor = 1.0 // ここではなくボーナス計算時にデートとともに0.8倍の補正をかける
                                match skill.attribute with
                                | SkillAttributeType.Cool ->
                                    if target.Card.attribute = AttributeType.Cool
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大Upに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大Upに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大Upに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大Upに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大Upに
                                    else 0 |> ignore
                                | SkillAttributeType.Pop ->
                                    if target.Card.attribute = AttributeType.Pop
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.Sweet ->
                                    if target.Card.attribute = AttributeType.Sweet
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraSuper ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Super ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor) 
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.AttackAndDefence ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                                // 大から特大もいる あとで実装
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                            | Mode.AttackAndDefence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                                | Large -> // 大からスーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1 + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    // 暫定的に特大UPに
                                                | ExtraLarge -> // 特大から超スーパー特大
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的にスーパー特大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.All ->                              
                                    match skill.effect with
                                    | SkillType.Effect.ExtraSuper ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Super ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor) 
                                    | SkillType.Effect.ExtraLarge ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Large ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Middle ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Small ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.AttackAndDefence ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Random(skillMin, skillMax) ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的にスーパー特大UPに
                                            // 大から特大もいる あとで実装
                                        | Mode.Defence ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的にスーパー特大UPに
                                        | Mode.AttackAndDefence ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的に大UPに
                                            | Large -> // 大からスーパー特大
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                // 暫定的に特大UPに
                                            | ExtraLarge -> // 特大から超スーパー特大
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的にスーパー特大UPに
                        | SkillTarget.Back(n) ->
                            let backTopN = backDeck.ToArray() |> Array.sortBy(fun cardView -> cardView.Attack) |> Array.filter(fun cardView -> skill.attribute.isAppliableOn(cardView.Card.attribute)) |> Array.truncate(n)                         
                            //let backDeckFactor = 0.8
                            let backDeckFactor = 1.0
                            for target in backTopN do
                                match skill.attribute with
                                | SkillAttributeType.Cool ->
                                    if target.Card.attribute = AttributeType.Cool
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.Pop ->
                                    if target.Card.attribute = AttributeType.Pop
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.Sweet ->
                                    if target.Card.attribute = AttributeType.Sweet
                                    then
                                        match skill.effect with
                                        | SkillType.Effect.ExtraLarge ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Large ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Middle ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Small ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            | Mode.Defence ->
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | SkillType.Effect.Random(skillMin, skillMax) ->
                                            match skill.mode with
                                            | Mode.Attack ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                            | Mode.Defence ->
                                                match skillMin with
                                                | Middle -> // 中から特大    
                                                    target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                    //暫定的に大UPに
                                    else 0 |> ignore
                                | SkillAttributeType.All ->                              
                                    match skill.effect with
                                    | SkillType.Effect.ExtraLarge ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Large ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Middle ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Small ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        | Mode.Defence ->
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | SkillType.Effect.Random(skillMin, skillMax) ->
                                        match skill.mode with
                                        | Mode.Attack ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的に大UPに
                                        | Mode.Defence ->
                                            match skillMin with
                                            | Middle -> // 中から特大    
                                                target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                                //暫定的に大UPに
                        | SkillTarget.SameGirl ->
                            let targetGirl = skillRaiseFrontCard.Card.girl.name
                            let backDeckTargets = Array.filter (fun (cardView: CardView) -> cardView.Card.girl.name = targetGirl) <| backDeck.ToArray()
                            let frontDeckTargets = 
                                Array.filter (fun (cardView: CardViewWithStrap) -> cardView.Card.girl.name = targetGirl) (frontDeck.ToArray()) 
                                |> Array.map (fun (cardView: CardViewWithStrap) -> cardView :> CardView)
                            let targets = Array.append frontDeckTargets backDeckTargets
                            for target in targets do
                                //let backDeckFactor = if Array.isEmpty target.Straps then 1.0 else 0.8
                                let backDeckFactor = 1.0
                                match skill.effect with
                                | SkillType.Effect.ExtraSuper ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                | SkillType.Effect.Super ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor) 
                                | SkillType.Effect.ExtraLarge ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                | SkillType.Effect.Large ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                | SkillType.Effect.Middle ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                | SkillType.Effect.Small ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.Defence ->
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                    | Mode.AttackAndDefence ->
                                        target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                        target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                | SkillType.Effect.Random(skillMin, skillMax) ->
                                    match skill.mode with
                                    | Mode.Attack ->
                                        match skillMin with
                                        | Middle -> // 中から特大    
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的に大Upに
                                        | Large -> // 大からスーパー特大
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            // 暫定的に特大Upに
                                        | ExtraLarge -> // 特大から超スーパー特大
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的にスーパー特大Upに
                                        // 大から特大もいる あとで実装
                                    | Mode.Defence ->
                                        match skillMin with
                                        | Middle -> // 中から特大    
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的に大Upに
                                        | Large -> // 大からスーパー特大
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            // 暫定的に特大Upに
                                        | ExtraLarge -> // 特大から超スーパー特大
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的にスーパー特大Upに
                                    | Mode.AttackAndDefence ->
                                        match skillMin with
                                        | Middle -> // 中から特大    
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的に大Upに
                                        | Large -> // 大からスーパー特大
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            // 暫定的に特大Upに
                                        | ExtraLarge -> // 特大から超スーパー特大
                                            target.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            target.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float |> (*) backDeckFactor)
                                            //暫定的にスーパー特大Upに
                        | SkillTarget.MySelf ->
                            match skill.effect with
                            | SkillType.Effect.Ultra ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackUltraUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceUltraUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceUltraUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceUltraUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.ExtraSuper ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.Super ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float) 
                            | SkillType.Effect.ExtraLarge ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.Large ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.Middle ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceMiddleUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.Small ->
                                match skill.mode with
                                | Mode.Attack ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.Defence ->
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                | Mode.AttackAndDefence ->
                                    skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                    skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSmallUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                            | SkillType.Effect.Random(skillMin, skillMax) ->
                                match skill.mode with
                                | Mode.Attack ->
                                    match skillMin with
                                    | Middle -> // 中から特大    
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的にスーパー特大Upに
                                    // 大から特大もいる あとで実装
                                | Mode.Defence ->
                                    match skillMin with
                                    | Middle -> // 中から特大    
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的にスーパー特大Upに
                                | Mode.AttackAndDefence ->
                                    match skillMin with
                                    | Middle -> // 中から特大    
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        skillRaiseFrontCard.AppliedAttackSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        skillRaiseFrontCard.AppliedDefenceSkillBonus.Add(settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp + skillRaiseFrontCard.Card.skillLevel - 1 + skill.skillEnchantLevel |> float)
                                        //暫定的にスーパー特大Upに                                                 
            else
                0 |> ignore

    let calcBonus(cardView: CardView, player: Player, deckEditViewModel: DeckEditViewModel, petitDeckEditViewModel: PetitDeckEditViewModel, specialBonusEditViewModel: SpecialBonusEditViewModel) = 
        //strapsがempty => 副センカード
        let card = cardView.Card
        let petitCheerEffects = petitDeckEditViewModel.ActivationCheerEffects
        let ev = deckEditViewModel.SelectedEventType
        let isFavoriteCard = cardView.IsFavoriteCard
        let isDatingCard = cardView.IsDatingCard
        let birthdaySettingGirl = deckEditViewModel.BirthdaySettingGirl
        let isTouched = cardView.IsTouched
        let straps = cardView.Straps
        let preciousScenes = deckEditViewModel.SelectedPreciousSceneList
        let exedGirlNum = deckEditViewModel.FrontDeck.FindAll(fun cardView -> cardView.Card.isEXed).Count
        let backDeckGirls = [||] |> ResizeArray
        let specialBonusGirl1 = specialBonusEditViewModel.MemorialStorySpecialGirl1
        let specialBonusGirl2 = specialBonusEditViewModel.MemorialStorySpecialGirl2
        let petitGirlLeaders = Array.choose (fun (petitDeck: ResizeArray<PetitGirlView>) -> if petitDeck.Count > 1 then Some(petitDeck.Item(0).petitGirl.girlName) else None) [|petitDeckEditViewModel.PetitGirlDeck1; petitDeckEditViewModel.PetitGirlDeck2; petitDeckEditViewModel.PetitGirlDeck3|]

        for cardView in deckEditViewModel.BackDeck do
            if not <| backDeckGirls.Contains(cardView.Card.girl.name)
            then backDeckGirls.Add(cardView.Card.girl.name)
            else 0 |> ignore
        let girlsNum = backDeckGirls.Count
        let triggeredAttackSkillBonus = cardView.AppliedAttackSkillBonus.ToArray()
        let triggeredDefenceSkillBonus = cardView.AppliedDefenceSkillBonus.ToArray()
        let mutable selectionBonusLevelMap = deckEditViewModel.getSelectionBonusLevelMap
        let mutable selectionBonusMaxLevelMap: Map<SelectionBonus, int> = Map.empty
        for selectionBonus in getAllSelectionBonus do
            if selectionBonus.SelectionBonusName = "Precious★Friend" // Precious Friend
            then 
                selectionBonusMaxLevelMap <- selectionBonusMaxLevelMap.Add(selectionBonus, 3)
            else 
                selectionBonusMaxLevelMap <- selectionBonusMaxLevelMap.Add(selectionBonus, 5)
        
        for selectionBonus in petitDeckEditViewModel.ActivatedSelectionBonus do
            match selectionBonusLevelMap.TryFind(selectionBonus) with
            | Some(level) -> 
                selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, level + 1)
            | None -> selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)
            if selectionBonus.SelectionBonusName = "Precious★Friend"
            then 
                0 |> ignore
            else
                selectionBonusMaxLevelMap <- selectionBonusMaxLevelMap.Add(selectionBonus, selectionBonusMaxLevelMap.Item(selectionBonus) + 1)

        let settings = Settings.Parse(File.ReadAllText(".\Setting.json"))
        let eventFactor: float =
            if Array.isEmpty straps
            then
                match ev with
                    | EventType.Raid -> settings.EventFactorSetting.Raid |> float
                    | EventType.CoolTrio -> settings.EventFactorSetting.Raid |> float
                    | EventType.PopTrio -> settings.EventFactorSetting.Raid |> float
                    | EventType.SweetTrio -> settings.EventFactorSetting.Raid |> float
                    | EventType.Hunters -> settings.EventFactorSetting.Hunters |> float
                    | EventType.CoolMega -> 0.0 // 副センバツは関係ない
                    | EventType.PopMega -> 0.0
                    | EventType.SweetMega -> 0.0
                    | EventType.Charisma -> 1.0 // 後で外側から副センバツ係数をかける
                    | EventType.Battle -> settings.EventFactorSetting.Battle |> float
                    | EventType.MemorialStory -> 1.0
            else 1.0
        let backDeckFactor = 
            if Array.isEmpty(straps) then 0.8 else 1.0
        let calcDifference (originalValue: int) (bonusPercentage: float) = 
            (originalValue |> float) * bonusPercentage / 100.0 |> ceil |> int
        
        let attrBonus = 
            if player.AttributeType = card.attribute 
            then settings.BasicBonusSettings.AttributeBonus |> float
            else 0.0
        let clubTypeBonus = 
            match ev with
            | EventType.Charisma | EventType.Battle ->
                if player.ClubType = card.girl.club && backDeckFactor = 1.0 // 主センのみ適用  
                then settings.ClubBonusSettings.ClubTypeBonus |> float
                else 0.0
            | EventType.MemorialStory ->
                0.0
            | otherwise -> 
                if player.ClubType = card.girl.club
                then settings.ClubBonusSettings.ClubTypeBonus |> float
                else 0.0
            
        let colonBonus = 
            match card.attribute with
            | AttributeType.Cool -> player.CoolColon
            | AttributeType.Pop -> player.PopColon
            | AttributeType.Sweet -> player.SweetColon
        
        let whiteboardBonus = 
            if player.ExistWhiteboard && card.attribute = AttributeType.Cool
                then settings.ClubBonusSettings.FacilityBonus |> float
                else 0.0
        let televisionBonus = 
            if player.ExistTelevision && card.attribute = AttributeType.Pop
                then settings.ClubBonusSettings.FacilityBonus |> float
                else 0.0
        let lockerBonus = 
            if player.ExistLocker && card.attribute = AttributeType.Sweet
                then settings.ClubBonusSettings.FacilityBonus |> float
                else 0.0

        let clubRoleAttackBonus = 
            match player.AssignedRole with
            | Role.President -> settings.ClubBonusSettings.RoleBonusSettings.PresidenBonus.Attack |> float
            | Role.VisePresident -> settings.ClubBonusSettings.RoleBonusSettings.VicePresidenBonus.Attack |> float
            | Role.AttackCaptain -> settings.ClubBonusSettings.RoleBonusSettings.AttackCaptainBonus.Attack |> float
            | Role.DefenceCaptain -> settings.ClubBonusSettings.RoleBonusSettings.DefenceCaptainBonus.Attack |> float
            | Role.Member -> 0.0
        let clubRoleDefenceBonus = 
            match player.AssignedRole with
            | Role.President -> settings.ClubBonusSettings.RoleBonusSettings.PresidenBonus.Defence |> float
            | Role.VisePresident -> settings.ClubBonusSettings.RoleBonusSettings.VicePresidenBonus.Defence |> float
            | Role.AttackCaptain -> settings.ClubBonusSettings.RoleBonusSettings.AttackCaptainBonus.Defence |> float
            | Role.DefenceCaptain -> settings.ClubBonusSettings.RoleBonusSettings.DefenceCaptainBonus.Defence |> float
            | Role.Member -> 0.0
        let birthdayBonus = 
            match birthdaySettingGirl with
            | "指定なし" -> 0.0
            | girlName -> if card.girl.birthday = getGirlByName(girlName).birthday
                            then settings.BasicBonusSettings.BirthdayBonus |> float
                            else 0.0
        // 同じ誕生日のガールはどちらか設定すれば両方有効になる
        let touchBonus =
            if isTouched
                then settings.BasicBonusSettings.TouchBonus |> float
                else 0.0

        let datingBonus =
            if isDatingCard
                then match card.rarity with
                    | Card.Rarity.N -> settings.BasicBonusSettings.DateBonus.N |> float
                    | Card.Rarity.HN -> settings.BasicBonusSettings.DateBonus.Hn |> float
                    | Card.Rarity.R -> settings.BasicBonusSettings.DateBonus.R |> float
                    | Card.Rarity.HR -> settings.BasicBonusSettings.DateBonus.Hr |> float
                    | Card.Rarity.SR -> settings.BasicBonusSettings.DateBonus.Sr |> float
                    | Card.Rarity.SSR -> settings.BasicBonusSettings.DateBonus.Ssr |> float
                    | Card.Rarity.UR -> settings.BasicBonusSettings.DateBonus.Ur |> float
                else 0.0

        let mutable petitCheerEffectAttackBonus = 0.0
        let mutable petitCheerEffectDefenceBonus = 0.0
        
        for petitCheerEffect in petitCheerEffects do
            match petitCheerEffect.petitCheerType with
            | AttributeCheerType(skillAttr, mode) -> 
                match skillAttr with
                | SkillAttributeType.Cool ->
                    match mode with
                    | Mode.Attack -> 
                        if card.attribute = AttributeType.Cool
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                0 |> ignore
                    | Mode.Defence ->
                        if card.attribute = AttributeType.Cool
                            then 
                                0 |> ignore
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    | Mode.AttackAndDefence -> 
                        if card.attribute = AttributeType.Cool
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                | SkillAttributeType.Pop ->
                    match mode with
                    | Mode.Attack -> 
                        if card.attribute = AttributeType.Pop
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                0 |> ignore
                    | Mode.Defence ->
                        if card.attribute = AttributeType.Pop
                            then 
                                0 |> ignore
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    | Mode.AttackAndDefence -> 
                        if card.attribute = AttributeType.Pop
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                | SkillAttributeType.Sweet ->
                    match mode with
                    | Mode.Attack -> 
                        if card.attribute = AttributeType.Sweet
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                0 |> ignore
                    | Mode.Defence ->
                        if card.attribute = AttributeType.Sweet
                            then 
                                0 |> ignore
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    | Mode.AttackAndDefence -> 
                        if card.attribute = AttributeType.Sweet
                            then 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            else 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                | SkillAttributeType.All ->
                    match mode with
                    | Mode.Attack -> 
                        petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                    | Mode.Defence ->
                        petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    | Mode.AttackAndDefence -> 
                        petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
            | GradeCheerType(grade, mode) ->
                match mode with
                | Mode.Attack ->
                    if grade = card.girl.grade
                        then petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        else 0 |> ignore
                | Mode.Defence ->
                    if grade = card.girl.grade
                        then petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                        else 0 |> ignore
                | Mode.AttackAndDefence ->
                    if grade = card.girl.grade
                        then 
                            petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                        else 
                            0 |> ignore
            | FavoriteCheerType(mode) ->
                if isFavoriteCard
                    then match mode with
                        | Mode.Attack -> 
                            petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        | Mode.Defence -> 
                            petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                        | Mode.AttackAndDefence ->
                            petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    else 0 |> ignore        
            | DatingCheerType(mode) ->
                if isDatingCard
                    then match mode with
                        | Mode.Attack -> 
                            petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        | Mode.Defence -> 
                            petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                        | Mode.AttackAndDefence ->
                            petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    else 0 |> ignore    
            | SameGirlCheerType(mode) ->
                if Array.contains card.girl.name petitGirlLeaders
                    then match mode with
                    | Mode.Attack -> 
                        petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                    | Mode.Defence -> 
                        petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    | Mode.AttackAndDefence ->
                        petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    else 0 |> ignore   
            | TouchCheerType ->
                if isTouched
                    then
                        petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                        petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                    else 0 |> ignore
            | BirthdayCheerType(mode) ->
                match birthdaySettingGirl with
                | "指定なし" -> 0 |> ignore
                | girlName ->
                    if card.girl.birthday = getGirlByName(girlName).birthday
                        then match mode with
                            | Mode.Attack -> 
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                            | Mode.Defence -> 
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                            | Mode.AttackAndDefence ->
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + petitCheerEffect.effectNum
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + petitCheerEffect.effectNum
                        else 0 |> ignore
            | FacilityBuildUpType(facility) ->
                if backDeckFactor = 0.8 && (deckEditViewModel.SelectedEventType = EventType.Charisma || deckEditViewModel.SelectedEventType = EventType.Battle || deckEditViewModel.SelectedEventType = EventType.MemorialStory)
                then 0 |> ignore
                else
                    match facility with
                    | Facility.Whiteboard ->
                        // カリスマ副みたいに設備ボーナスがない場所でも発生している 修正済み
                        if player.ExistWhiteboard && card.attribute = AttributeType.Cool
                            then 
                                let bonus = (settings.ClubBonusSettings.FacilityBonus |> float) * petitCheerEffect.effectNum / 100.0
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + bonus
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + bonus
                            else 0 |> ignore
                    | Facility.Television ->
                        if player.ExistTelevision && card.attribute = AttributeType.Pop
                            then 
                                let bonus = (settings.ClubBonusSettings.FacilityBonus |> float) * petitCheerEffect.effectNum / 100.0
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + bonus
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + bonus
                            else 0 |> ignore
                    | Facility.Locker ->
                        if player.ExistLocker && card.attribute = AttributeType.Sweet
                            then 
                                let bonus = (settings.ClubBonusSettings.FacilityBonus |> float) * petitCheerEffect.effectNum / 100.0
                                petitCheerEffectAttackBonus <- petitCheerEffectAttackBonus + bonus
                                petitCheerEffectDefenceBonus <- petitCheerEffectDefenceBonus + bonus
                            else 0 |> ignore
            | GalIncomeUpType -> 0 |> ignore
            | LikeabilityUpType -> 0 |> ignore
            | DeckCostDownType -> 0 |> ignore
            | ExpIncomeUpType -> 0 |> ignore
        

        let selectionAttackBonusVals =
            [| for pair in selectionBonusLevelMap ->
                if pair.Key.getSelectionBonusMode = Mode.Attack || pair.Key.getSelectionBonusMode = Mode.AttackAndDefence
                then 
                    let selectionBonusLevel = min pair.Value <| selectionBonusMaxLevelMap.Item(pair.Key)
                    match pair.Key.SelectionBonusName with
                    | "シャイニング★スプラッシュ" ->
                        0.0 // 後で追加
                    | "Precious★Friend" ->
                        0.0 // 後で追加
                    | otherwise ->
                        match selectionBonusLevel with
                        | 1 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level3 |> float
                        | 4 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level4 |> float
                        | 5 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level5 |> float
                        | 6 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level6 |> float
                        | 7 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level7 |> float
                        | 8 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level8 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                else 
                    0.0
            |]
            |> Array.sortDescending 
            |> Array.truncate(5) 
            |> Array.append ([|
                    match selectionBonusLevelMap.TryFind(SelectionBonus("シャイニング★スプラッシュ")) with 
                    | Some(level) -> 
                        match level with
                        | 1 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level3 |> float
                        | 4 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level4 |> float
                        | 5 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level5 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                    | None -> 0.0
                |])
            |> Array.append ([|
                    match selectionBonusLevelMap.TryFind(SelectionBonus("Precious★Friend")) with 
                    | Some(level) -> 
                        match level with
                        | 1 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level3 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                    | None -> 0.0
                |]
            )
        let selectionDefenceBonusVals =
            [| for pair in selectionBonusLevelMap ->
                if pair.Key.getSelectionBonusMode = Mode.Defence || pair.Key.getSelectionBonusMode = Mode.AttackAndDefence
                then 
                    let selectionBonusLevel = min pair.Value <| selectionBonusMaxLevelMap.Item(pair.Key)
                    match pair.Key.SelectionBonusName with
                    | "シャイニング★スプラッシュ" ->
                        0.0 // 後で追加
                    | "Precious★Friend" ->
                        0.0 // 後で追加
                    | otherwise ->
                        match selectionBonusLevel with
                        | 1 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level3 |> float
                        | 4 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level4 |> float
                        | 5 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level5 |> float
                        | 6 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level6 |> float
                        | 7 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level7 |> float
                        | 8 -> 
                            settings.SelectionBonusSettings.NormalSelectionBonusSettings.Level8 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                else 
                    0.0
            |]
            |> Array.sortDescending 
            |> Array.truncate(5) 
            |> Array.append ([|
                    match selectionBonusLevelMap.TryFind(SelectionBonus("シャイニング★スプラッシュ")) with 
                    | Some(level) -> 
                        match level with
                        | 1 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level3 |> float
                        | 4 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level4 |> float
                        | 5 -> 
                            settings.SelectionBonusSettings.ShiningSplashBonusSettings.Level5 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                    | None -> 0.0
                |])
            |> Array.append ([|
                    match selectionBonusLevelMap.TryFind(SelectionBonus("Precious★Friend")) with 
                    | Some(level) -> 
                        match level with
                        | 1 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level1 |> float
                        | 2 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level2 |> float
                        | 3 -> 
                            settings.SelectionBonusSettings.PreciousFriendBonusSettings.Level3 |> float
                        | otherwise -> failwith "センボの計算で失敗しました。"
                    | None -> 0.0
                |]
            )

        //Console.WriteLine("")
        //Console.WriteLine("")
        //Console.WriteLine("---------------------------------")
        let attackBonusWithStrapsAndSceneAndBackDeckFactor =
            //Console.WriteLine("声援")
            //for v in triggeredAttackSkillBonus do
            //    Console.WriteLine(v.ToString())
            Array.append [|datingBonus|] triggeredAttackSkillBonus
        let defenceBonusWithStrapsAndSceneAndBackDeckFactor = Array.append [|datingBonus|] triggeredDefenceSkillBonus
        let attackBonusWithStraps = [|touchBonus|]
        let defenceBonusWithStraps = [|touchBonus|]
        let attackBonusWithStrapsAndBackDeckFactor = [|birthdayBonus|]
        let defenceBonusWithStrapsAndBackDeckFactor = [|birthdayBonus|]
        let attackBonusWithStrapsAndScene =  
            match ev with
            | MemorialStory -> 
                //Console.WriteLine(card.girl.name + ": " + Option.defaultValue "" card.eventName)
                //printfn "コロン: %f, ホワイトボード: %f, テレビ: %f, ロッカー: %f, ぷち: %f" colonBonus whiteboardBonus televisionBonus lockerBonus petitCheerEffectAttackBonus
                //Console.WriteLine("センボ(precious, シャイニング, ...)")
                //for v in selectionAttackBonusVals do
                //    Console.Write(v.ToString() + ", ")
                [|colonBonus; whiteboardBonus; televisionBonus; lockerBonus; petitCheerEffectAttackBonus|] 
                |> Array.append selectionAttackBonusVals
            | Battle | Charisma -> 
                if backDeckFactor = 1.0
                then 
                    //Console.WriteLine(card.girl.name + ": " + Option.defaultValue "" card.eventName)
                    //printfn "属性: %f, 部一致: %f, コロン: %f, ホワイトボード: %f, テレビ: %f, ロッカー: %f, 役職: %f, ぷち: %f" attrBonus clubTypeBonus colonBonus whiteboardBonus televisionBonus lockerBonus clubRoleAttackBonus petitCheerEffectAttackBonus
                    //Console.WriteLine("センボ(precious, シャイニング, ...)")
                    //for v in selectionAttackBonusVals do
                    //    Console.Write(v.ToString() + ", ")
                    [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleAttackBonus; petitCheerEffectAttackBonus|] 
                    |> Array.append selectionAttackBonusVals
                else 
                    //Console.WriteLine(card.girl.name + ": " + Option.defaultValue "" card.eventName)
                    //printfn "コロン: %f, ぷち: %f" colonBonus petitCheerEffectAttackBonus
                    //Console.WriteLine("センボ(precious, シャイニング, ...)")
                    //for v in selectionAttackBonusVals do
                    //    Console.Write(v.ToString() + ", ")
                    [|colonBonus; petitCheerEffectAttackBonus|] 
                    |> Array.append selectionAttackBonusVals
            | otherwise ->
                //Console.WriteLine(card.girl.name + ": " + Option.defaultValue "" card.eventName)
                //printfn "属性: %f, 部一致: %f, コロン: %f, ホワイトボード: %f, テレビ: %f, ロッカー: %f, 役職: %f, ぷち: %f" attrBonus clubTypeBonus colonBonus whiteboardBonus televisionBonus lockerBonus clubRoleAttackBonus petitCheerEffectAttackBonus
                [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleAttackBonus; petitCheerEffectAttackBonus|] 
                
        let defenceBonusWithStrapsAndScene = 
            match ev with
            | MemorialStory -> 
                [|colonBonus; whiteboardBonus; televisionBonus; lockerBonus; petitCheerEffectDefenceBonus|]
                |> Array.append selectionDefenceBonusVals
            | Battle | Charisma -> 
                if backDeckFactor = 1.0
                then
                    [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleAttackBonus; petitCheerEffectDefenceBonus|] 
                    |> Array.append selectionDefenceBonusVals
                else 
                    [|colonBonus; petitCheerEffectAttackBonus|] 
                    |> Array.append selectionDefenceBonusVals
            | otherwise ->
                [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleAttackBonus; petitCheerEffectAttackBonus|] 
        
        //外からかける係数
        let outerFactor = 
            match ev with
            | EventType.Raid -> 1.0
            | EventType.CoolTrio -> 
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.CoolTrio.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.CoolTrio.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.CoolTrio.Sweet |> float
            | EventType.PopTrio -> 
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.PopTrio.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.PopTrio.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.PopTrio.Sweet |> float
            | EventType.SweetTrio -> 
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.SweetTrio.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.SweetTrio.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.SweetTrio.Sweet |> float
            | EventType.Hunters -> 1.0
            | EventType.CoolMega ->
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.CoolMega.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.CoolMega.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.CoolMega.Sweet |> float
            | EventType.PopMega ->
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.PopMega.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.PopMega.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.PopMega.Sweet |> float
            | EventType.SweetMega ->
                match card.attribute with
                | AttributeType.Cool -> settings.EventFactorSetting.SweetMega.Cool |> float
                | AttributeType.Pop -> settings.EventFactorSetting.SweetMega.Pop |> float
                | AttributeType.Sweet -> settings.EventFactorSetting.SweetMega.Sweet |> float
            | EventType.Charisma -> 
                if backDeckFactor = 0.8
                then
                    settings.EventFactorSetting.Charisma |> float
                else 1.0
            | EventType.Battle -> 1.0
            | EventType.MemorialStory -> 
                let specialBonusFactor = 
                    if card.girl.name = specialBonusGirl1 || card.girl.name = specialBonusGirl2
                    then
                        let rarity = card.rarity
                        let skillLevel = card.skillLevel
                        match rarity with
                        | Rarity.UR -> 
                            (settings.SpecialBonus.MemorialStory.Ur.BaseFactor |> float) + ((skillLevel - 1) |> float) * (settings.SpecialBonus.MemorialStory.Ur.Gradient |> float)
                        | Rarity.SSR ->
                            (settings.SpecialBonus.MemorialStory.Ssr.BaseFactor |> float) + ((skillLevel - 1) |> float) * (settings.SpecialBonus.MemorialStory.Ssr.Gradient |> float)
                        | otherwise -> 1.0 // SR以下は暫定的に特攻倍率なし
                    else 1.0
                if backDeckFactor = 0.8
                then
                    (settings.EventFactorSetting.MemorialStory |> float) * specialBonusFactor
                else specialBonusFactor
            | otherEvent -> 1.0


        let percentagePreciousBonusAttackList = [||] |> ResizeArray
        let percentagePreciousBonusDefenceList = [||] |> ResizeArray
        let valuePreciousBonusAttackList = [||] |> ResizeArray
        let valuePreciousBonusDefenceList = [||] |> ResizeArray
        // Exガールの数副センバツバージョン未実装
        // 最適化が必要
        for scene in preciousScenes do
            match scene.PreciousScene.sceneEffect.sceneTarget with
            | FrontDeck ->
                if backDeckFactor = 1.0 
                then
                    match scene.PreciousScene.sceneEffect.sceneTargetAttribute with
                    | SkillAttributeType.Cool -> 
                        if card.attribute = AttributeType.Cool 
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore
                    | SkillAttributeType.Pop -> 
                        if card.attribute = AttributeType.Pop 
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore  
                    | SkillAttributeType.Sweet -> 
                        if card.attribute = AttributeType.Sweet
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore
                    | SkillAttributeType.All -> 
                        let mutable bonus = 0.0
                        match scene.PreciousScene.sceneEffect.sceneEffectType with
                        | CostType maxCost -> 
                            bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                        | ExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                        | SkillLevel maxSkillLevel ->
                            bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                        | Rarity maxRarity ->
                            bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                        | SpecificGirl maxSameGirlNum ->
                            if girlsNum = 0 
                            then
                                0 |> ignore
                            else
                                bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                        | Uniform ->
                            bonus <- scene.PreciousScene.effectMaxNum
                        match scene.PreciousScene.sceneEffect.mode with
                        | Mode.Attack -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                        | Mode.Defence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                        | Mode.AttackAndDefence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> 
                                percentagePreciousBonusAttackList.Add(bonus)
                                percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> 
                                valuePreciousBonusAttackList.Add(bonus)
                                valuePreciousBonusDefenceList.Add(bonus)       
                else
                    0 |> ignore
            | BackDeck ->
                if backDeckFactor = 0.8
                then
                    match scene.PreciousScene.sceneEffect.sceneTargetAttribute with
                    | SkillAttributeType.Cool -> 
                        if card.attribute = AttributeType.Cool 
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore
                    | SkillAttributeType.Pop -> 
                        if card.attribute = AttributeType.Pop 
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore  
                    | SkillAttributeType.Sweet -> 
                        if card.attribute = AttributeType.Sweet
                        then
                            let mutable bonus = 0.0
                            match scene.PreciousScene.sceneEffect.sceneEffectType with
                            | CostType maxCost -> 
                                bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                            | ExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                            | SkillLevel maxSkillLevel ->
                                bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                            | Rarity maxRarity ->
                                bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                            | SpecificGirl maxSameGirlNum ->
                                if girlsNum = 0 
                                then
                                    0 |> ignore
                                else
                                    bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            match scene.PreciousScene.sceneEffect.mode with
                            | Mode.Attack -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                            | Mode.Defence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                            | Mode.AttackAndDefence -> 
                                match scene.PreciousScene.effectNumType with
                                | EffectNumType.Percentage -> 
                                    percentagePreciousBonusAttackList.Add(bonus)
                                    percentagePreciousBonusDefenceList.Add(bonus)
                                | EffectNumType.Value -> 
                                    valuePreciousBonusAttackList.Add(bonus)
                                    valuePreciousBonusDefenceList.Add(bonus)
                        else
                            0 |> ignore
                    | SkillAttributeType.All -> 
                        let mutable bonus = 0.0
                        match scene.PreciousScene.sceneEffect.sceneEffectType with
                        | CostType maxCost -> 
                            bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                        | ExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                        | SkillLevel maxSkillLevel ->
                            bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                        | Rarity maxRarity ->
                            bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                        | SpecificGirl maxSameGirlNum ->
                            if girlsNum = 0 
                            then
                                0 |> ignore
                            else
                                bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                        | Uniform ->
                            bonus <- scene.PreciousScene.effectMaxNum
                        match scene.PreciousScene.sceneEffect.mode with
                        | Mode.Attack -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                        | Mode.Defence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                        | Mode.AttackAndDefence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> 
                                percentagePreciousBonusAttackList.Add(bonus)
                                percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> 
                                valuePreciousBonusAttackList.Add(bonus)
                                valuePreciousBonusDefenceList.Add(bonus)       
                else
                    0 |> ignore
            | All ->
                match scene.PreciousScene.sceneEffect.sceneTargetAttribute with
                | SkillAttributeType.Cool -> 
                    if card.attribute = AttributeType.Cool 
                    then
                        let mutable bonus = 0.0
                        match scene.PreciousScene.sceneEffect.sceneEffectType with
                        | CostType maxCost -> 
                            bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                        | ExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                        | SkillLevel maxSkillLevel ->
                            bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                        | Rarity maxRarity ->
                            bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                        | SpecificGirl maxSameGirlNum ->
                            if girlsNum = 0 
                            then
                                0 |> ignore
                            else
                                bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                        | Uniform ->
                            bonus <- scene.PreciousScene.effectMaxNum
                        match scene.PreciousScene.sceneEffect.mode with
                        | Mode.Attack -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                        | Mode.Defence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                        | Mode.AttackAndDefence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> 
                                percentagePreciousBonusAttackList.Add(bonus)
                                percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> 
                                valuePreciousBonusAttackList.Add(bonus)
                                valuePreciousBonusDefenceList.Add(bonus)
                    else
                        0 |> ignore
                | SkillAttributeType.Pop -> 
                    if card.attribute = AttributeType.Pop 
                    then
                        let mutable bonus = 0.0
                        match scene.PreciousScene.sceneEffect.sceneEffectType with
                        | CostType maxCost -> 
                            bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                        | ExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                        | SkillLevel maxSkillLevel ->
                            bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                        | Rarity maxRarity ->
                            bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                        | SpecificGirl maxSameGirlNum ->
                            if girlsNum = 0 
                            then
                                0 |> ignore
                            else
                                bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                        | Uniform ->
                            bonus <- scene.PreciousScene.effectMaxNum
                        match scene.PreciousScene.sceneEffect.mode with
                        | Mode.Attack -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                        | Mode.Defence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                        | Mode.AttackAndDefence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> 
                                percentagePreciousBonusAttackList.Add(bonus)
                                percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> 
                                valuePreciousBonusAttackList.Add(bonus)
                                valuePreciousBonusDefenceList.Add(bonus)
                    else
                        0 |> ignore  
                | SkillAttributeType.Sweet -> 
                    if card.attribute = AttributeType.Sweet
                    then
                        let mutable bonus = 0.0
                        match scene.PreciousScene.sceneEffect.sceneEffectType with
                        | CostType maxCost -> 
                            bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                        | ExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                        | SkillLevel maxSkillLevel ->
                            bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                        | Rarity maxRarity ->
                            bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                        | SpecificGirl maxSameGirlNum ->
                            if girlsNum = 0 
                            then
                                0 |> ignore
                            else
                                bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                        | Uniform ->
                            bonus <- scene.PreciousScene.effectMaxNum
                        match scene.PreciousScene.sceneEffect.mode with
                        | Mode.Attack -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                        | Mode.Defence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                        | Mode.AttackAndDefence -> 
                            match scene.PreciousScene.effectNumType with
                            | EffectNumType.Percentage -> 
                                percentagePreciousBonusAttackList.Add(bonus)
                                percentagePreciousBonusDefenceList.Add(bonus)
                            | EffectNumType.Value -> 
                                valuePreciousBonusAttackList.Add(bonus)
                                valuePreciousBonusDefenceList.Add(bonus)
                    else
                        0 |> ignore
                | SkillAttributeType.All -> 
                    let mutable bonus = 0.0
                    match scene.PreciousScene.sceneEffect.sceneEffectType with
                    | CostType maxCost -> 
                        bonus <- Math.Pow((Math.Min(card.cost, maxCost) |> float) / (maxCost |> float) , 1.5) * scene.PreciousScene.effectMaxNum
                    | ExedGirlNum maxGirlNum ->
                        bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.47) * scene.PreciousScene.effectMaxNum                                
                    | SkillLevel maxSkillLevel ->
                        bonus <- Math.Pow((Math.Min(card.skillLevel, maxSkillLevel) |> float) / (maxSkillLevel |> float), 0.5) * scene.PreciousScene.effectMaxNum
                    | Rarity maxRarity ->
                        bonus <- Math.Pow((Math.Min(card.rarity.getId, maxRarity.getId) |> float) / (maxRarity.getId |> float), 0.18) * scene.PreciousScene.effectMaxNum
                    | SpecificGirl maxSameGirlNum ->
                        if girlsNum = 0 
                        then
                            0 |> ignore
                        else
                            bonus <- Math.Pow((maxSameGirlNum |> float) / (Math.Max(girlsNum, maxSameGirlNum) |> float), 0.2) * scene.PreciousScene.effectMaxNum
                    | Uniform ->
                        bonus <- scene.PreciousScene.effectMaxNum
                    match scene.PreciousScene.sceneEffect.mode with
                    | Mode.Attack -> 
                        match scene.PreciousScene.effectNumType with
                        | EffectNumType.Percentage -> percentagePreciousBonusAttackList.Add(bonus)
                        | EffectNumType.Value -> valuePreciousBonusAttackList.Add(bonus)
                    | Mode.Defence -> 
                        match scene.PreciousScene.effectNumType with
                        | EffectNumType.Percentage -> percentagePreciousBonusDefenceList.Add(bonus)
                        | EffectNumType.Value -> valuePreciousBonusDefenceList.Add(bonus)
                    | Mode.AttackAndDefence -> 
                        match scene.PreciousScene.effectNumType with
                        | EffectNumType.Percentage -> 
                            percentagePreciousBonusAttackList.Add(bonus)
                            percentagePreciousBonusDefenceList.Add(bonus)
                        | EffectNumType.Value -> 
                            valuePreciousBonusAttackList.Add(bonus)
                            valuePreciousBonusDefenceList.Add(bonus)       

        let correctedAttack = 
            let baseAttackWithStraps =
                Array.fold(fun (acc) (bonus) -> acc + calcDifference card.attack bonus) card.attack straps // タッチボーナス
            let baseAttackWithStrapsAndScene = 
                (valuePreciousBonusAttackList.ToArray() |> Array.map (fun b -> b |> ceil |> int) |> Array.sum) + 
                (Array.fold(fun (acc) (bonus) -> acc + calcDifference baseAttackWithStraps bonus) baseAttackWithStraps (percentagePreciousBonusAttackList.ToArray()))
            let baseAttackWithStrapsAndSceneAndBackDeckFactor = (baseAttackWithStrapsAndScene |> float) * backDeckFactor |> ceil |> int // デートボーナス
            let baseAttackWithStrapsAndBackDeckFactor = (baseAttackWithStraps |> float) * backDeckFactor |> ceil |> int // 誕生日
                
            let baseAttack = (eventFactor * (card.attack |> float)) |> ceil |> int 
            let strapBonus = Array.fold(fun (acc) (bonus) -> acc + calcDifference card.attack bonus) 0 straps
            let sceneBonus = 
                (valuePreciousBonusAttackList.ToArray() |> Array.map (fun b -> b |> ceil |> int) |> Array.sum) + 
                (Array.fold(fun (acc) (bonus) -> acc + calcDifference baseAttackWithStraps bonus) 0 (percentagePreciousBonusAttackList.ToArray()))
                
            (outerFactor * 
                ((
                    baseAttack
                    + strapBonus
                    + sceneBonus
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseAttackWithStrapsAndScene bonus) 0 attackBonusWithStrapsAndScene
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseAttackWithStrapsAndSceneAndBackDeckFactor bonus) 0 attackBonusWithStrapsAndSceneAndBackDeckFactor
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseAttackWithStraps bonus) 0 attackBonusWithStraps 
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseAttackWithStrapsAndBackDeckFactor bonus) 0 attackBonusWithStrapsAndBackDeckFactor
                ) |> float)
            ) |> ceil |> int
        let correctedDefence = 
            let baseDefenceWithStraps = Array.fold(fun (acc) (bonus) -> acc + calcDifference card.defence bonus) card.defence straps // タッチボーナス
            let baseDefenceWithStrapsAndScene = 
                (valuePreciousBonusDefenceList.ToArray() |> Array.map (fun b -> b |> ceil |> int) |> Array.sum) + 
                (Array.fold(fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStraps bonus) baseDefenceWithStraps (percentagePreciousBonusDefenceList.ToArray()))
            let baseDefenceWithStrapsAndSceneAndBackDeckFactor = (baseDefenceWithStrapsAndScene |> float) * backDeckFactor |> ceil |> int // デートボーナス
            let baseDefenceWithStrapsAndBackDeckFactor = (baseDefenceWithStraps |> float) * backDeckFactor |> ceil |> int // 誕生日
                
            let baseDefence = (eventFactor * (card.defence |> float)) |> ceil |> int 
            let strapBonus = Array.fold(fun (acc) (bonus) -> acc + calcDifference card.defence bonus) 0 straps
            let sceneBonus = 
                (valuePreciousBonusDefenceList.ToArray() |> Array.map (fun b -> b |> ceil |> int) |> Array.sum) + 
                (Array.fold(fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStraps bonus) 0 (percentagePreciousBonusDefenceList.ToArray()))
                
            (outerFactor *
                ((
                    baseDefence
                    + strapBonus
                    + sceneBonus
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStrapsAndScene bonus) 0 defenceBonusWithStrapsAndScene
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStrapsAndSceneAndBackDeckFactor bonus) 0 defenceBonusWithStrapsAndSceneAndBackDeckFactor
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStraps bonus) 0 defenceBonusWithStraps 
                    + Array.fold (fun (acc) (bonus) -> acc + calcDifference baseDefenceWithStrapsAndBackDeckFactor bonus) 0 defenceBonusWithStrapsAndBackDeckFactor) |> float
                )
            ) |> ceil |> int
        
        {|
            CorrectedAttack = correctedAttack;
            CorrectedDefence = correctedDefence;
        |}
        