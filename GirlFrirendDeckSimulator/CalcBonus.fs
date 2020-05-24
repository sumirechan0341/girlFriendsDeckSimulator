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

module CalcBonus =
    type PlayerParameterTab = XAML<"PlayerParameterTab.xaml">
    type BonusSettings = JsonProvider<"./BonusSetting.json">
    let calcBonus(card: Card, playerParametersViewModel: PlayerParameterViewModel) = 
        let bonusSeetings = BonusSettings.Parse(File.ReadAllText("..\..\BonusSetting.json"))
        let calcDifference (originalValue: int) (bonusPercentage: float) = 
            (originalValue |> float) * bonusPercentage / 100.0 |> ceil |> int
        let attrBonus = if playerParametersViewModel.SelectedAttributeType = card.attribute 
                            then bonusSeetings.BasicBonusSettings.AttributeBonus 
                            else 0
        let clubTypeBonus = if playerParametersViewModel.SelectedClubType = card.girl.club 
                                then bonusSeetings.ClubBonusSettings.ClubTypeBonus
                                else 0
        let colonBonus = 
            match card.attribute with
            | AttributeType.Cool -> playerParametersViewModel.CoolColon
            | AttributeType.Pop -> playerParametersViewModel.PopColon
            | AttributeType.Sweet -> playerParametersViewModel.SweetColon
        let whiteBoardBonus = if playerParametersViewModel.ExistWhiteBoard && card.attribute = AttributeType.Cool
                                then bonusSeetings.ClubBonusSettings.FacilityBonus
                                else 0
        let televisionBonus = if playerParametersViewModel.ExistTelevision && card.attribute = AttributeType.Pop
                                then bonusSeetings.ClubBonusSettings.FacilityBonus
                                else 0
        let lockerBonus = if playerParametersViewModel.ExistLocker && card.attribute = AttributeType.Sweet
                                then bonusSeetings.ClubBonusSettings.FacilityBonus
                                else 0

        let clubRoleAttackBonus = 
            match playerParametersViewModel.SelectedClubRoleType with
            | Role.President -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.PresidenBonus.Attack
            | Role.VisePresident -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.VicePresidenBonus.Attack
            | Role.AttackCaptain -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.AttackCaptainBonus.Attack
            | Role.DefenceCaptain -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.DefenceCaptainBonus.Attack
            | Role.Member -> 0
        let clubRoleDefenceBonus = 
            match playerParametersViewModel.SelectedClubRoleType with
            | Role.President -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.PresidenBonus.Defence
            | Role.VisePresident -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.VicePresidenBonus.Defence
            | Role.AttackCaptain -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.AttackCaptainBonus.Defence
            | Role.DefenceCaptain -> bonusSeetings.ClubBonusSettings.RoleBonusSettings.DefenceCaptainBonus.Defence
            | Role.Member -> 0
        let birthdayBonus = 
            match playerParametersViewModel.BirhtdaySettingGirl with
            | "指定なし" -> 0
            | girlName -> if card.girl.birthday = getGirlByName(girlName).birthday
                            then bonusSeetings.BasicBonusSettings.BirthdayBonus
                            else 0
        // 同じ誕生日のガールはどちらか設定すれば両方有効になる
        let attackBonusArray = [|float attrBonus; float clubTypeBonus; colonBonus; float whiteBoardBonus; float televisionBonus; float lockerBonus; float clubRoleAttackBonus; float birthdayBonus|]
        let defenceBonusArray = [|float attrBonus; float clubTypeBonus; colonBonus; float whiteBoardBonus; float televisionBonus; float lockerBonus; float clubRoleDefenceBonus; float birthdayBonus|]

        let correctedAttack = Array.fold (fun (acc) (bonus) -> acc + calcDifference card.attack bonus) card.attack attackBonusArray
        let correctedDefence = Array.fold (fun (acc) (bonus) -> acc + calcDifference card.defence bonus) card.defence defenceBonusArray
        {|
            CorrectedAttack = correctedAttack;
            CorrectedDefence = correctedDefence;
        |}
        // datebonus
        // petitGirl bonus