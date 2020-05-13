namespace GirlFriendDeckSimulator
open SkillType
open System.Text.RegularExpressions
open System
module SkillTypeConverter =
    module SkillTargetConverter =
        let toString(skillTarget: SkillTarget) =
            match skillTarget with
            | Front -> "主センバツ"
            | Back(n) -> "副センバツ上位" + n.ToString() + "人"
            | FrontAndBack1 -> "主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人"
            | MySelf -> "自分自身"
            | SameGirl -> "同ガール"
            | OpponentFront -> "相手主センバツ"
        let fromString(skillTargetStr: string) =
            if(skillTargetStr.[..5] = "副センバツ" || skillTargetStr.[..5] = "副ｾﾝﾊﾞﾂ") 
            then 
                let targetNum = Regex("\d+").Match(skillTargetStr).Value |> int
                Back(targetNum)
            else match skillTargetStr with
                | "主センバツ" | "主ｾﾝﾊﾞﾂ" -> Front
                | "主センバツ全員&副センバツ1人" | "主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人" -> FrontAndBack1
                | "自分自身" -> MySelf
                | "同ガール" -> SameGirl
                | _ -> OpponentFront

    module SkillEffectConverter =
        let rec toString(skillEffect) = 
            match skillEffect with
            | Ultra -> "ウルトラ特大"
            | ExtraSuper -> "超スーパー特大"
            | Super -> "スーパー特大"
            | ExtraLarge -> "特大"
            | Large -> "大"
            | Middle -> "中"
            | Small -> "小"
            | Random(minSkill, maxSkill) -> toString(minSkill) + "～" + toString(maxSkill)

        let rec fromString(skillEffectStr) =
            match skillEffectStr with
            | "ウルトラ特大" | "ｳﾙﾄﾗ特大" -> Ultra
            | "超スーパー特大" | "超ｽｰﾊﾟｰ特大" -> ExtraSuper
            | "スーパー特大" | "ｽｰﾊﾟｰ特大" -> Super
            | "特大" -> ExtraLarge
            | "大" -> Large
            | "中" -> Middle
            | "小" -> Small
            | skillEffect when skillEffect.Contains("～")
                -> Effect.Random(fromString(skillEffect.Split('～').[0]), fromString(skillEffect.Split('～').[1]))

    module SkillModeConverter =
        let toString(skillMode) = 
            match skillMode with
            | Up -> "UP"
            | Down -> "DOWN"

        let fromString(skillModeStr) =
            match skillModeStr with
            |"UP" -> Up
            |"DOWN" -> Down
    
    let (|ParseRegex|_|) regex str =
        let m = Regex(regex).Match(str)
        if m.Success
        then Some (List.tail [ for x in m.Groups -> x.Value ])
        else None

    let fromString(skillTypeStr) =
        let separater = "の"
        let attributeRegex = "(COOL|COOLタイプ|POP|POPタイプ|SWEET|SWEETタイプ|全タイプ)"
        let skillTargetRegex = "(主(?:センバツ|ｾﾝﾊﾞﾂ)|副(?:センバツ|ｾﾝﾊﾞﾂ)上位(?:\d+)人|主(?:センバツ|ｾﾝﾊﾞﾂ)全員&副(?:センバツ|ｾﾝﾊﾞﾂ)1人)"
        let modeRegex = "(攻援|守援|攻守)" 
        let skillEffectRegex = "((?:(?:ｳﾙﾄﾗ|ウルトラ)特大|(?:超スーパー|超ｽｰﾊﾟｰ)特大|(?:スーパー|ｽｰﾊﾟｰ)特大|特大|大|中|小)(?:(?:～(?:(?:ｳﾙﾄﾗ|ウルトラ)特大|(?:超スーパー|超ｽｰﾊﾟｰ)特大|(?:スーパー|ｽｰﾊﾟｰ)特大|特大|大|中|小))?))"
        let skillTypeModeRegex = "(UP|DOWN)"
        let skillEnchantLevelRegex = "(\+\d+)"

        // pattern1 COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP
        let pattern1 = 
            "(?:" + attributeRegex + separater + ")" + "?" 
            + "(?:" + skillTargetRegex + separater + ")" + "?" 
            + modeRegex 
            + skillEffectRegex 
            + skillTypeModeRegex 
            + skillEnchantLevelRegex + "?"
//        System.Console.WriteLine(List.tail [ for x in Regex(pattern1).Match("COOLタイプの攻守スーパー特大UP").Groups -> x.Value ])
        match skillTypeStr with
        | ParseRegex pattern1 [attribute; skillTarget; mode; skillEffect; skillTypeMode; skillEnchantLevel] 
                ->  
                    {
                        attribute = if(String.IsNullOrEmpty(attribute) |> not) then SkillAttributeTypeConverter.fromString(attribute) else SkillAttributeType.All;
                        target = if(String.IsNullOrEmpty(attribute) |> not) then SkillTargetConverter.fromString(skillTarget) else if(skillTypeMode = "UP") then SkillTarget.Front else SkillTarget.OpponentFront;
                        effect = SkillEffectConverter.fromString(skillEffect);
                        mode = ModeConverter.fromString(mode);
                        skillMode = SkillModeConverter.fromString(skillTypeMode);
                        skillEnchantLevel = if(String.IsNullOrEmpty(skillEnchantLevel) |> not) then skillEnchantLevel |> int else 0;
                    }
        
        //| ParseRegex pattern2 [attribute; skillTarget; mode; skillEffect; skillUpDown] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = 0;
        //            }
        //| ParseRegex pattern3 [skillTarget; mode; skillEffect; skillUpDown] 
        //        -> {
        //                attribute = SkillAttributeType.All;
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = 0;
        //            }
        //| ParseRegex pattern4 [skillTarget; mode; skillEffect; skillUpDown] 
        //        -> {
        //                attribute = SkillAttributeType.All;
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = 0;
        //            }
        //| ParseRegex pattern5 [attribute; mode; skillEffect; skillUpDown] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = if(skillUpDown = "UP") then SkillTarget.Front else SkillTarget.OpponentFront;
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = 0;
        //            }
        //| ParseRegex pattern6 [attribute; mode; skillEffect; skillUpDown] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = if(skillUpDown = "UP") then SkillTarget.Front else SkillTarget.OpponentFront;
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = 0;
        //            }
        //| ParseRegex pattern7 [attribute; skillTarget; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
        //| ParseRegex pattern8 [attribute; skillTarget; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
        //| ParseRegex pattern9 [skillTarget; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeType.All;
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
        //| ParseRegex pattern10 [skillTarget; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeType.All;
        //                target = SkillTargetConverter.fromString(skillTarget);
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
        //| ParseRegex pattern11 [attribute; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = if(skillUpDown = "UP") then SkillTarget.Front else SkillTarget.OpponentFront;
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
        //| ParseRegex pattern12 [attribute; mode; skillEffect; skillUpDown; skillEnchantLevel] 
        //        -> {
        //                attribute = SkillAttributeTypeConverter.fromString(attribute);
        //                target = if(skillUpDown = "UP") then SkillTarget.Front else SkillTarget.OpponentFront;
        //                effect = SkillEffectConverter.fromString(skillEffect);
        //                mode = ModeConverter.fromString(mode);
        //                skillMode = SkillModeConverter.fromString(skillUpDown);
        //                skillEnchantLevel = skillEnchantLevel |> int
        //            }
    
         



