namespace GirlFriendDeckSimulator
open PetitGirl
open System.Text.RegularExpressions
open System

module PetitCheerTypeConverter =

    let toString(petitCheerEffect) = 
        match petitCheerEffect with
        | AttributeCheerType(attr, mode) -> SkillAttributeTypeConverter.toString(attr) + "の" + ModeConverter.toString(mode) + "UP"
        | GradeCheerType(grade, mode) -> GradeConverter.toString(grade) + "の" + ModeConverter.toString(mode) + "UP"
        | FavoriteCheerType(mode) -> "本命ガールの" + ModeConverter.toString(mode) + "UP"
        | DatingCheerType(mode) -> "デート中のガールの" + ModeConverter.toString(mode) + "UP"
        | SameGirlCheerType(mode) -> "同ガールの" + ModeConverter.toString(mode) + "UP"
        | TouchCheerType -> "ﾀｯﾁﾎﾞｰﾅｽの効果UP"
        | BirthdayCheerType(mode) -> "誕生日のガールの" + ModeConverter.toString(mode) + "UP"
        | FacilityBuildUpType(targetFacility) -> FacilityConverter.toString(targetFacility) + "の効果" + "UP"
        | GalIncomeUpType -> "獲得ガルUP"
        | LikeabilityUpType -> "ガールの獲得好感度UP"
        | DeckCostDownType -> "使用攻コストDOWN"
        | ExpIncomeUpType -> "獲得経験値UP"


    let fromString(petitCheerEffectStr: string) = 
        let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (List.tail [ for x in m.Groups -> x.Value ])
            else None

        let attributeCheerPattern = "((?:COOL|POP|SWEET|全)ﾀｲﾌﾟ)の(攻援|守援|攻守)(?:大|小)?UP"
        let gradeCheerPattern = "(1年生|2年生|3年生)の(攻援|守援|攻守)(?:大|小)?UP"
        let favoriteCheerPattern = "本命ｶﾞｰﾙの(攻援|守援|攻守)UP"
        let datingCheerPattern = "ﾃﾞｰﾄ中のｶﾞｰﾙの(攻援|守援|攻守)UP"
        let sameGirlCheerPattern = "同ｶﾞｰﾙの(攻援|守援|攻守)UP"
        let touchCheerPattern = "ﾀｯﾁﾎﾞｰﾅｽの効果UP"
        let birhtdayCheerPattern = "誕生日のｶﾞｰﾙの(攻援|守援|攻守)UP"
        let facilityBuildUpPattern = "(ﾎﾜｲﾄﾎﾞｰﾄﾞ|ﾃﾚﾋﾞ|ﾛｯｶｰ)の効果UP"
        let galIncomeUpPattern = "獲得ｶﾞﾙ(?:大)?UP"
        let likeabilityUpPattern = "ｶﾞｰﾙの獲得好感度(?:大)?UP"
        let deckCostDownPattern = "使用攻ｺｽﾄDOWN"
        let expIncomeUpPattern = "獲得経験値(?:大)?UP"
        
        match petitCheerEffectStr with
        | ParseRegex attributeCheerPattern [skillAttr; mode] -> AttributeCheerType(SkillAttributeTypeConverter.fromString(skillAttr), ModeConverter.fromString(mode))
        | ParseRegex gradeCheerPattern [grade; mode] -> GradeCheerType(GradeConverter.fromString(grade), ModeConverter.fromString(mode))
        | ParseRegex favoriteCheerPattern [mode] -> FavoriteCheerType(ModeConverter.fromString(mode))
        | ParseRegex datingCheerPattern [mode] -> DatingCheerType(ModeConverter.fromString(mode))
        | ParseRegex sameGirlCheerPattern [mode] -> SameGirlCheerType(ModeConverter.fromString(mode))
        | ParseRegex touchCheerPattern [] -> TouchCheerType
        | ParseRegex birhtdayCheerPattern [mode] -> BirthdayCheerType(ModeConverter.fromString(mode))
        | ParseRegex facilityBuildUpPattern [facility] -> FacilityBuildUpType(FacilityConverter.fromString(facility))
        | ParseRegex galIncomeUpPattern [] -> GalIncomeUpType
        | ParseRegex likeabilityUpPattern [] -> LikeabilityUpType
        | ParseRegex deckCostDownPattern [] -> DeckCostDownType
        | ParseRegex expIncomeUpPattern [] -> ExpIncomeUpType

module PetitSkillTypeConverter =
    let toString(petitSkill) = 
        match petitSkill with
        | AttributeSkillType(skillAttr, mode) -> SkillAttributeTypeConverter.toString(skillAttr) + "のぷちガール" + ModeConverter.toString(mode) + "UP"


    let fromString(petitSkillStr) = 
        let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (List.tail [ for x in m.Groups -> x.Value ])
            else None
            
        let attributeSkillPattern = "((?:COOL|POP|SWEET|全)ﾀｲﾌﾟ)のぷちｶﾞｰﾙ(攻援|守援|攻守)UP"
        
        match petitSkillStr with
        | ParseRegex attributeSkillPattern [skillAttr; mode] -> AttributeSkillType(SkillAttributeTypeConverter.fromString(skillAttr), ModeConverter.fromString(mode))