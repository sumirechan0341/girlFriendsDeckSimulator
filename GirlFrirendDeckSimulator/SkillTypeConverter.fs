namespace GirlFriendDeckSimulator
open SkillType
open System.Text.RegularExpressions
open AttributeTypeConverter
module SkillTypeConverter =
    module SkillType =
        let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (List.tail [ for x in m.Groups -> x.Value ])
            else None

        let fromString(skillTypeStr) =
            let separater = "の"
            let attributeRegex = "(COOL|COOLタイプ|POP|POPタイプ|SWEET|SWEETタイプ|全タイプ)"
            let skillTargetRegex = "(主センバツ|副センバツ上位(\d+)人|主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人)"
            let modeRegex = "(攻援|守援|攻守)" 
            let skillEffectRegex = "(ウルトラ特大|ｽｰﾊﾟｰ特大|スーパー特大|特大|大|中|小)"
            let skillEffectRandomRegex = skillEffectRegex + "～" + skillEffectRegex
            let skillUpDownRegex = "(UP|DOWN)"
            let skillEnchantLevelRegex = "(\+\d)"

            // pattern1 COOLの主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援ｽｰﾊﾟｰ特大UP
            let pattern1 = 
                attributeRegex 
                + separater 
                + skillTargetRegex 
                + separater 
                + modeRegex 
                + skillEffectRegex 
                + skillUpDownRegex 
            
            // pattern2 pattern1のランダム声援バージョン
            let pattern2 = 
                attributeRegex 
                + separater 
                + skillTargetRegex 
                + separater 
                + modeRegex 
                + skillEffectRandomRegex 
                + skillUpDownRegex 
            
            // pattern3 "主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人の攻援大UP"(attributeが空 => 全タイプ)
            let pattern3 =
                skillTargetRegex
                + separater
                + modeRegex
                + skillEffectRegex
                + skillUpDownRegex
            
           
            // pattern4 pattern3のランダム声援バージョン
            let pattern4 =
                skillTargetRegex
                + separater
                + modeRegex
                + skillEffectRandomRegex 
                + skillUpDownRegex
            
            // pattern5 SWEETタイプの攻援スーパー特大UP(skillTargetが空 => UPの場合主センバツ, DOWNの場合相手主センバツ)
            let pattern5 = 
                attributeRegex 
                + separater 
                + modeRegex
                + skillEffectRegex 
                + skillUpDownRegex
            
            // pattern6 pattern5のランダム声援バージョン
            let pattern6 =
                attributeRegex 
                + separater 
                + modeRegex
                + skillEffectRandomRegex 
                + skillUpDownRegex

            // pattern7 pattern1の+x表記があるもの
            let pattern7 = 
                attributeRegex 
                + separater 
                + skillTargetRegex 
                + separater 
                + modeRegex 
                + skillEffectRegex 
                + skillUpDownRegex 
                + skillEnchantLevelRegex
            
            // pattern8 pattern2の+x表記があるもの
            let pattern8 = 
                attributeRegex 
                + separater 
                + skillTargetRegex 
                + separater 
                + modeRegex 
                + skillEffectRandomRegex 
                + skillUpDownRegex 
                + skillEnchantLevelRegex
            
            // pattern9 pattern3の+x表記があるもの
            let pattern9 =
                skillTargetRegex
                + separater
                + modeRegex
                + skillEffectRegex
                + skillUpDownRegex
                + skillEnchantLevelRegex
            
           
            // pattern10 pattern4の+x表記があるもの
            let pattern10 =
                skillTargetRegex
                + separater
                + modeRegex
                + skillEffectRandomRegex 
                + skillUpDownRegex
                + skillEnchantLevelRegex
            
            // pattern11 pattern5の+x表記があるもの
            let pattern11 = 
                attributeRegex 
                + separater 
                + modeRegex
                + skillEffectRegex 
                + skillUpDownRegex
                + skillEnchantLevelRegex
            
            // pattern12 pattern6の+x表記があるもの
            let pattern12 =
                attributeRegex 
                + separater 
                + modeRegex
                + skillEffectRandomRegex 
                + skillUpDownRegex
                + skillEnchantLevelRegex
            
            
            match skillTypeStr with
            | ParseRegex pattern1 [attribute; skillTarget; mode; skillEffect; skillUpDown] -> null 
                //-> {
                //        attribute = AttributeTypeConverter.fromString(attribute);
                //        target = SkillTarget.fromString(skillTarget);
                //        effect = 
                //    }
            
    module SkillTarget =
        let toString(skillTarget: SkillTarget) =
            match skillTarget with
            | Front -> "主センバツ"
            | Back(n) -> "副センバツ上位" + n.ToString() + "人"
            | FrontAndBack1 -> "主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人"
            | MySelf -> "自分自身"
            | SameGirl -> "同ガール"
            | OpponentFront -> "相手主センバツ"
        let fromString(skillTargetStr: string) =
            if(skillTargetStr.[..5] = "副センバツ") 
            then 
                let targetNum = Regex("\d+").Match(skillTargetStr).Value |> int
                Back(targetNum)
            else match skillTargetStr with
                | "主センバツ" -> Front
                | "主ｾﾝﾊﾞﾂ全員&副ｾﾝﾊﾞﾂ1人" -> FrontAndBack1
                | "自分自身" -> MySelf
                | "同ガール" -> SameGirl
                | _ -> OpponentFront

    module SkillEffect =
        let rec toString(skillEffect) = 
            match skillEffect with
            | Ultra -> "ウルトラ特大"
            | Super -> "スーパー特大"
            | ExtraLarge -> "特大"
            | Large -> "大"
            | Middle -> "中"
            | Small -> "小"
            | Random(minSkill, maxSkill) -> toString(minSkill) + "～" + toString(maxSkill)


