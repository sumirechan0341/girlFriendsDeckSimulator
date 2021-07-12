namespace GirlFriendDeckSimulator

open SkillAttributeType
open Mode
open Card
open FSharp.Data
open System.IO

module SkillType =
    type Settings = JsonProvider<".\Setting.json">

    type SkillTarget =
        | Front
        | Back of targetNum: int
        | FrontAndBack1
        | MySelf
        | SameGirl
        | OpponentFront

    type Effect =
        | Ultra
        | ExtraSuper
        | Super
        | ExtraLarge
        | Large
        | Middle
        | Small
        | Random of min: Effect * max: Effect

    type SkillMode =
        | Up
        | Down

    type SkillType =
        { attribute: SkillAttributeType
          target: SkillTarget
          effect: Effect
          mode: Mode
          skillMode: SkillMode
          skillEnchantLevel: int }
        // ボーナス基礎値を返すメソッド
        member this.getBaseBonusPercentage(card: Card) =
            // ボーナス計算するために設定ファイル読み込み
            let settings =
                Settings.Parse(File.ReadAllText(".\Setting.json"))

            match this.skillMode with
            | SkillMode.Down -> 0.0
            | SkillMode.Up ->
                match this.target with
                // 主センバツ系の声援
                | SkillTarget.Front
                | SkillTarget.FrontAndBack1 ->
                    if
                        this.attribute.isAppliableOn (card.attribute)
                        && not (this.attribute = SkillAttributeType.All)
                    then
                        match this.effect with
                        | ExtraSuper ->
                            // リファクタ skillEffectToBonusPercentageというメソッドか関数を定義してskill.effectの分岐を消す
                            // 引数にはskillTargetとskillEffect skillModeも？ skill全部でいいかも skillのメソッドに生やす
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                        | Super ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                        | ExtraLarge ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                if this.target = SkillTarget.FrontAndBack1 then
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUpPlus1
                                    + this.skillEnchantLevel
                                    |> float
                                else
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                        | Large ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                        | Middle ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                        | Small ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                        | Random (skillMin, skillMax) ->
                            match this.mode with
                            | Mode.Attack ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                            // 大から特大もいる あとで実装
                            | Mode.Defence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeDefenceSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                            | Mode.AttackAndDefence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AttributeSkill.AttributeAttackAndDefenceSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("属性系ランダム声援の未実装タイプです")
                        | _ -> failwith ("属性系声援の未実装タイプです")
                    elif this.attribute.isAppliableOn (card.attribute)
                         && this.attribute = SkillAttributeType.All then
                        match this.effect with
                        | ExtraSuper ->
                            // リファクタ skillEffectToBonusPercentageというメソッドか関数を定義してskill.effectの分岐を消す
                            // 引数にはskillTargetとskillEffect skillModeも？ skill全部でいいかも skillのメソッドに生やす
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraSuperUp
                                + this.skillEnchantLevel
                                |> float
                        | Super ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                        | ExtraLarge ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                        | Large ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                        | Middle ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                        | Small ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence ->
                                settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                        | Random (skillMin, skillMax) ->
                            match this.mode with
                            | Mode.Attack ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                            // 大から特大もいる あとで実装
                            | Mode.Defence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeDefenceSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                            | Mode.AttackAndDefence ->
                                match skillMin with
                                | Middle -> // 中から特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的に大Upに
                                | Large -> // 大からスーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceExtraLargeUp
                                    + this.skillEnchantLevel
                                    |> float
                                // 暫定的に特大Upに
                                | ExtraLarge -> // 特大から超スーパー特大
                                    settings.SkillBonusSettings.AllAttributeSkill.AllAttributeAttackAndDefenceSuperUp
                                    + this.skillEnchantLevel
                                    |> float
                                //暫定的にスーパー特大Upに
                                | _ -> failwith ("全属性系ランダム声援の未実装タイプです")
                        | _ -> failwith ("全属性声援の未実装タイプです")
                    else
                        0.0
                | Back (_) ->
                    if
                        this.attribute.isAppliableOn (card.attribute)
                        && not (this.attribute = SkillAttributeType.All)
                    then
                        match this.effect with
                        | ExtraLarge ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Large ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Middle ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Small ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckAttackSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AttributeBackDeckDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | _ -> failwith ("副センバツ声援の未実装タイプです")
                    elif this.attribute.isAppliableOn (card.attribute)
                         && this.attribute = SkillAttributeType.All then
                        match this.effect with
                        | ExtraLarge ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Large ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Middle ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceMiddleUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | Small ->
                            match this.mode with
                            | Mode.Attack ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckAttackSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.Defence ->
                                settings.SkillBonusSettings.BackDeckSkill.AllAttributeBackDeckDefenceSmallUp
                                + this.skillEnchantLevel
                                |> float
                            | Mode.AttackAndDefence -> failwith ("副センバツ声援の攻守タイプは未実装です")
                        | _ -> failwith ("副センバツ声援の未実装タイプです")
                    else
                        0.0
                | SameGirl ->
                    match this.effect with
                    | ExtraSuper ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                    | Super ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp
                            + this.skillEnchantLevel
                            |> float
                    | ExtraLarge ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                    | Large ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp
                            + this.skillEnchantLevel
                            |> float
                    | Middle ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackMiddleUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceMiddleUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceMiddleUp
                            + this.skillEnchantLevel
                            |> float
                    | Small ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSmallUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSmallUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSmallUp
                            + this.skillEnchantLevel
                            |> float
                    | Random (skillMin, skillMax) ->
                        match this.mode with
                        | Mode.Attack ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackSuperUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的にスーパー特大Upに
                            | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                        // 大から特大もいる あとで実装
                        | Mode.Defence ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的にスーパー特大Upに
                            | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                        | Mode.AttackAndDefence ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                            // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SameGirlSkill.SameGirlAttackAndDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                            //暫定的にスーパー特大Upに
                            | _ -> failwith ("同種ガール系のランダム声援の未実装タイプです")
                    | _ -> failwith ("同種ガール声援の未実装タイプです")
                | MySelf ->
                    match this.effect with
                    | Ultra ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackUltraUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceUltraUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceUltraUp
                            + this.skillEnchantLevel
                            |> float
                    | ExtraSuper ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraSuperUp
                            + this.skillEnchantLevel
                            |> float
                    | Super ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp
                            + this.skillEnchantLevel
                            |> float
                    | ExtraLarge ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp
                            + this.skillEnchantLevel
                            |> float
                    | Large ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp
                            + this.skillEnchantLevel
                            |> float
                    | Middle ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackMiddleUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceMiddleUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceMiddleUp
                            + this.skillEnchantLevel
                            |> float
                    | Small ->
                        match this.mode with
                        | Mode.Attack ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackSmallUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.Defence ->
                            settings.SkillBonusSettings.SelfSkill.SelfDefenceSmallUp
                            + this.skillEnchantLevel
                            |> float
                        | Mode.AttackAndDefence ->
                            settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSmallUp
                            + this.skillEnchantLevel
                            |> float
                    | Random (skillMin, skillMax) ->
                        match this.mode with
                        | Mode.Attack ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackLargeUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                                // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackSuperUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的にスーパー特大Upに
                            // 本当は大から特大もいる あとで実装
                            | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                        | Mode.Defence ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                                // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的にスーパー特大Upに
                            | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                        | Mode.AttackAndDefence ->
                            match skillMin with
                            | Middle -> // 中から特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceLargeUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的に大Upに
                            | Large -> // 大からスーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceExtraLargeUp
                                + this.skillEnchantLevel
                                |> float
                                // 暫定的に特大Upに
                            | ExtraLarge -> // 特大から超スーパー特大
                                settings.SkillBonusSettings.SelfSkill.SelfAttackAndDefenceSuperUp
                                + this.skillEnchantLevel
                                |> float
                                //暫定的にスーパー特大Upに
                            | _ -> failwith ("自分自身系のランダム声援の未実装タイプです")
                    | _ -> failwith ("自分自身声援の未実装タイプです")
                | _ -> failwith("未実装の声援です")