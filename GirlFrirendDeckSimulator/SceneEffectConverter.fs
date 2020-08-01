namespace GirlFriendDeckSimulator
open PreciousScene
open System.Text.RegularExpressions
module SceneEffectConverter =
    module SceneTargetConverter =
        let fromString(sceneTarget) =
            match sceneTarget with
            | "主センバツ" | "主ｾﾝﾊﾞﾂ" -> FrontDeck
            | "副センバツ" | "副ｾﾝﾊﾞﾂ" -> BackDeck
            | _ -> All

        let toString(sceneTarget) =
            match sceneTarget with
            | FrontDeck -> "主センバツ"
            | BackDeck -> "副センバツ"
            | All -> "主センバツと副センバツ"

    let fromString(sceneEffectDescription: string, sceneEffectMaxTerms: string) =
        let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (List.tail [ for x in m.Groups -> x.Value ])
            else None
        let costPattern = "(?:(主センバツ|副センバツ)の)?(COOL|POP|SWEET)ガールのコストが高いほど(攻援|守援|攻守)UP"
        let exedPattern = "(?:(主センバツ|副センバツ)に)?(COOL|POP|SWEET)のEx進展ガールが多いほど(攻援|守援|攻守)UP"
        let skillLevelPattern = "(?:(主センバツ|副センバツ)の)?(COOL|POP|SWEET)ガールの声援Lvが高いほど(攻援|守援|攻守)UP"
        let rarityPattern = "(?:(主センバツ|副センバツ)の)?(COOL|POP|SWEET)ガールのレアリティが高いほど(攻援|守援|攻守)UP"
        let specificGirlPattern = "(?:(主センバツ|副センバツ)を)?特定の(COOL|POP|SWEET)ガールで編成するほど(攻援|守援|攻守)UP"
        let uniformPattern = "(?:(主センバツ|副センバツ)における)?(COOLタイプ|POPタイプ|SWEETタイプ|全タイプ)のガールの(攻援|守援|攻守)UP"   
        let uniformPatternAll = "(COOL|POP|SWEET)ガールの(攻援|守援|攻守)UP"
        match sceneEffectDescription with
        | ParseRegex costPattern [sceneTarget; skillAttribute; mode] ->
            let sceneEffectMaxCost = sceneEffectMaxTerms.Replace("コスト", "") |> int
            {
                sceneEffectType = CostType sceneEffectMaxCost
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex exedPattern [sceneTarget; skillAttribute; mode] ->
            let sceneEffectMaxExedGirl = sceneEffectMaxTerms.Replace("ガール", "") |> int
            {
                sceneEffectType = ExedGirlNum sceneEffectMaxExedGirl
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex skillLevelPattern [sceneTarget; skillAttribute; mode] ->
            let sceneEffectMaxSkillLevel = sceneEffectMaxTerms.Replace("Lv.", "") |> int
            {
                sceneEffectType = SkillLevel sceneEffectMaxSkillLevel;
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex rarityPattern [sceneTarget; skillAttribute; mode] ->
            let sceneEffectMaxRarity = CardConverter.RarityConverter.fromString(sceneEffectMaxTerms)
            {
                sceneEffectType = Rarity sceneEffectMaxRarity;
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex specificGirlPattern [sceneTarget; skillAttribute; mode] ->
            let sceneEffectMaxGirlNum = sceneEffectMaxTerms.Replace("ガール", "") |> int
            {
                sceneEffectType = SpecificGirl sceneEffectMaxGirlNum;
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex uniformPattern [sceneTarget; skillAttribute; mode] ->
            {
                sceneEffectType = Uniform;
                sceneTarget = SceneTargetConverter.fromString(sceneTarget);
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
        | ParseRegex uniformPatternAll [skillAttribute; mode] ->
            {
                sceneEffectType = Uniform;
                sceneTarget = SceneTargetType.All;
                sceneTargetAttribute = SkillAttributeTypeConverter.fromString(skillAttribute);
                mode = ModeConverter.fromString(mode)
            }
    
    let toString(sceneEffect: SceneEffect) =
        match sceneEffect.sceneEffectType with
        | CostType cost -> 
            SceneTargetConverter.toString(sceneEffect.sceneTarget) + 
            "において" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "のコストが高いほど" +
            ModeConverter.toString(sceneEffect.mode) +
            "UP" +
            "(最大発揮コスト: " +
            cost.ToString() + 
            ")"
        | ExedGirlNum num -> 
            SceneTargetConverter.toString(sceneEffect.sceneTarget) + 
            "において" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "のEx進展ガールが多いほど" +
            ModeConverter.toString(sceneEffect.mode) +
            "UP" +
            "(最大発揮人数: " +
            num.ToString() + 
            ")"
        | SkillLevel maxSkillLevel -> 
            SceneTargetConverter.toString(sceneEffect.sceneTarget) + 
            "において" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "の声援レベルが高いほど" +
            ModeConverter.toString(sceneEffect.mode) +
            "UP" +
            "(最大発揮声援レベル: " +
            maxSkillLevel.ToString() + 
            ")"
        | Rarity rarity -> 
            SceneTargetConverter.toString(sceneEffect.sceneTarget) + 
            "において" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "のレアリティが高いほど" +
            ModeConverter.toString(sceneEffect.mode) +
            "UP" +
            "(最大発揮レアリティ: " +
            CardConverter.RarityConverter.toString(rarity) +
            ")"
        | SpecificGirl girlNum ->
            SceneTargetConverter.toString(sceneEffect.sceneTarget) +
            "を特定の" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "ガールで編成するほど" +
            ModeConverter.toString(sceneEffect.mode) + 
            "UP"
        | Uniform ->
            SceneTargetConverter.toString(sceneEffect.sceneTarget) + 
            "において" +
            SkillAttributeTypeConverter.toString(sceneEffect.sceneTargetAttribute) +
            "の" +
            ModeConverter.toString(sceneEffect.mode) +
            "UP" +
            "(均等に効果を発揮)"
