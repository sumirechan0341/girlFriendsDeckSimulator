namespace GirlFriendDeckSimulator

open Girl
open AttributeType
open SkillType
open FSharp.Data
open System.IO



module Card =
    type Rarity =
        | N
        | HN
        | R
        | HR
        | SR
        | SSR
        | UR

    type Rarity with
        member this.getId =
            match this with
            | N -> 1
            | HN -> 2
            | R -> 3
            | HR -> 4
            | SR -> 5
            | SSR -> 6
            | UR -> 7

    type CardType =
        | Common
        | Switch
        | Kira
        | Mirror
        | Friends
        | Birthday

    type Settings = JsonProvider<".\Setting.json">

    type Card =
        { girl: Girl
          eventName: option<string>
          attribute: AttributeType
          rarity: Rarity
          attack: int
          defence: int
          cardType: CardType
          isEXed: bool
          cost: int
          skillType: option<SkillType>
          skillLevel: int
          cardJson: JsonValue }
        // ボーナス基礎値を返すメソッド
        member this.giveBaseBonusPercentageTo(other: Card) =
            // ボーナス計算するために設定ファイル読み込み
            let settings =
                Settings.Parse(File.ReadAllText(".\Setting.json"))

            match this.skillType with
            | None -> 0.0
            | Some (skill) ->
                match skill.skillMode with
                | SkillMode.Down -> 0.0
                | SkillMode.Up ->
                    match skill.target with
                    // 主センバツ系の声援
                    | SkillTarget.Front
                    | SkillTarget.FrontAndBack1 ->
                        if
                            skill.attribute.isAppliableOn (other.attribute)
                            && not (skill.attribute = SkillAttributeType.All)
                        then
                            match skill.effect with
                            | ExtraSuper ->
                                // リファクタ skillEffectToBonusPercentageというメソッドか関数を定義してskill.effectの分岐を消す
                                // 引数にはskillTargetとskillEffect skillModeも？ skill全部でいいかも skillのメソッドに生やす
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Super ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | ExtraLarge ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    if skill.target = SkillTarget.FrontAndBack1 then
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1
                                        + skill.skillEnchantLevel
                                        |> float
                                    else
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                            | Large ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Middle ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Small ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Random (skillMin, skillMax) ->
                                match skill.mode with
                                | Mode.Attack ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                                // 大から特大もいる あとで実装
                                | Mode.Defence ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                                | Mode.AttackAndDefence ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                            | _ -> failwith ("属性系声援の未実装タイプです")
                        elif skill.attribute.isAppliableOn (other.attribute)
                             && skill.attribute = SkillAttributeType.All then
                            match skill.effect with
                            | ExtraSuper ->
                                // リファクタ skillEffectToBonusPercentageというメソッドか関数を定義してskill.effectの分岐を消す
                                // 引数にはskillTargetとskillEffect skillModeも？ skill全部でいいかも skillのメソッドに生やす
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Super ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | ExtraLarge ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Large ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Middle ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Small ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence ->
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                            | Random (skillMin, skillMax) ->
                                match skill.mode with
                                | Mode.Attack ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                                // 大から特大もいる あとで実装
                                | Mode.Defence ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                                | Mode.AttackAndDefence ->
                                    match skillMin with
                                    | Middle -> // 中から特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的に大Upに
                                    | Large -> // 大からスーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    // 暫定的に特大Upに
                                    | ExtraLarge -> // 特大から超スーパー特大
                                        settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp
                                        + skill.skillEnchantLevel
                                        |> float
                                    //暫定的にスーパー特大Upに
                                    | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                            | _ -> failwith ("全属性声援の未実装タイプです")
                        else
                            0.0
                    | Back (_) ->
                        if
                            skill.attribute.isAppliableOn (other.attribute)
                            && not (skill.attribute = SkillAttributeType.All)
                        then
                            match skill.effect with
                            | ExtraLarge ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Large ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Middle ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Small ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | _ -> failwith ("副センバツ声援の未実装タイプです")
                        elif skill.attribute.isAppliableOn (other.attribute)
                             && skill.attribute = SkillAttributeType.All then
                            match skill.effect with
                            | ExtraLarge ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Large ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Middle ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceMiddleUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | Small ->
                                match skill.mode with
                                | Mode.Attack ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.Defence ->
                                    settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceSmallUp
                                    + skill.skillEnchantLevel
                                    |> float
                                | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                            | _ -> failwith ("副センバツ声援の未実装タイプです")
                        else
                            0.0
                    | SameGirl ->
                        match skill.effect with
                        | ExtraSuper ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                        | Super ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp
                                + skill.skillEnchantLevel
                                |> float
                        | ExtraLarge ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                        | Large ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp
                                + skill.skillEnchantLevel
                                |> float
                        | Middle ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                        | Small ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSmallUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSmallUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSmallUp
                                + skill.skillEnchantLevel
                                |> float
                        | Random (skillMin, skillMax) ->
                            match skill.mode with
                            | Mode.Attack ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                            // 大から特大もいる あとで実装
                            | Mode.Defence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                            | Mode.AttackAndDefence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                        | _ -> failwith ("同種ガール声援の未実装タイプです")
                    | MySelf ->
                        match skill.effect with
                        | Ultra ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackUltraUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceUltraUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceUltraUp
                                + skill.skillEnchantLevel
                                |> float
                        | ExtraSuper ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraSuperUp
                                + skill.skillEnchantLevel
                                |> float
                        | Super ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp
                                + skill.skillEnchantLevel
                                |> float
                        | ExtraLarge ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp
                                + skill.skillEnchantLevel
                                |> float
                        | Large ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp
                                + skill.skillEnchantLevel
                                |> float
                        | Middle ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceMiddleUp
                                + skill.skillEnchantLevel
                                |> float
                        | Small ->
                            match skill.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackSmallUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceSmallUp
                                + skill.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSmallUp
                                + skill.skillEnchantLevel
                                |> float
                        | Random (skillMin, skillMax) ->
                            match skill.mode with
                            | Mode.Attack ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                // 本当は大から特大もいる あとで実装
                                | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                            | Mode.Defence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                            | Mode.AttackAndDefence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp
                                    + skill.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp
                                    + skill.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                        | _ -> failwith ("自分自身声援の未実装タイプです")
                    | _ -> failwith ("未実装の声援です")
