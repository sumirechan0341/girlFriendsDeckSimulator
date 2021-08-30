namespace GirlFriendDeckSimulator
open FsXaml
open Card
open Club
open AttributeType
open FSharp.Data
open System.IO
open GirlFactory
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
open FSharpx.Collections

module CalcBonus =
    type PlayerParameterTab = XAML<"PlayerParameterTab.xaml">
    type Settings = JsonProvider<".\Setting.json">

    // 声援ボーナスを各カードに設定する関数
    // 声援がかかるカードに声援効果をセットする
    let applySkillBonus(frontDeck: ResizeArray<CardViewWithStrap>, backDeck: ResizeArray<CardView>, cardList: ResizeArray<CardView>) =
        // 声援発揮ガールリスト
        let skillTriggeredCards = [||] |> ResizeArray
        // 初期化
        let mutable frontSkillTriggeredGirlNum = 0
        let mutable switchSkillTriggeredGirlNum = 0

        for frontCard in frontDeck do
            // 主センバツカードにかかっている声援効果をすべて外す
            frontCard.AppliedAttackSkillBonus.RemoveAll(fun c -> true) |> ignore
            frontCard.AppliedDefenceSkillBonus.RemoveAll(fun c -> true) |> ignore
            // 声援発揮対象に選択済み
            // 声援発揮ガールに追加できるのは主センバツからは5枚まで
            if frontCard.IsTriggeredSkillBonus && frontSkillTriggeredGirlNum < 5 then
                skillTriggeredCards.Add(frontCard :> CardView)
                frontSkillTriggeredGirlNum <- frontSkillTriggeredGirlNum + 1
            else 
                0 |> ignore
        for backCard in backDeck do
            // 副センバツのカードにかかっている声援効果をすべて外す
            backCard.AppliedAttackSkillBonus.RemoveAll(fun c -> true) |> ignore
            backCard.AppliedDefenceSkillBonus.RemoveAll(fun c -> true) |> ignore
            // 声援発揮対象に選択済み
            // 声援発揮ガールに追加できるのは副センバツから2枚まで
            if backCard.Card.cardType = Card.CardType.Switch && backCard.IsTriggeredSkillBonus && switchSkillTriggeredGirlNum < 2 then
                skillTriggeredCards.Add(backCard)
                switchSkillTriggeredGirlNum <- switchSkillTriggeredGirlNum + 1
            else 
                0 |> ignore

        for card in cardList do
            // センバツ外のカードも計算からすべて外す
            card.AppliedAttackSkillBonus.RemoveAll(fun c -> true) |> ignore
            card.AppliedDefenceSkillBonus.RemoveAll(fun c -> true) |> ignore
        
        // 声援発揮ガールリストの声援を発揮させる
        for skillRaiseFrontCard in skillTriggeredCards do
            match skillRaiseFrontCard.Card.skillType with
            | None -> 0 |> ignore
            | Some(skill) ->
                match skill.skillMode with
                | SkillType.SkillMode.Down -> 0 |> ignore // 未実装!
                | SkillType.SkillMode.Up ->
                    let targets = match skill.target with
                    | SkillTarget.Front ->
                        ResizeArray.map (fun (frontCard: CardViewWithStrap) -> frontCard :> CardView) frontDeck
                    | SkillTarget.FrontAndBack1 ->
                        let backTop = backDeck.ToArray() |> Array.tryFind (fun cardView -> skill.attribute.isAppliableOn(cardView.Card.attribute))
                        let targets = (Array.map (fun cardWithStrap -> cardWithStrap :> CardView) <| frontDeck.ToArray()) |> ResizeArray
                        if backTop.IsSome
                        then 
                            targets.Add(backTop.Value)
                        else
                            0 |> ignore
                        targets
                    | SkillTarget.Back(n) ->
                        backDeck.ToArray() |> Array.sortByDescending (fun card -> card.Attack) |> Array.filter(fun cardView -> skill.attribute.isAppliableOn(cardView.Card.attribute)) |> Array.truncate(n) |> ResizeArray                      
                    | SkillTarget.SameGirl ->
                        let targetGirl = skillRaiseFrontCard.Card.girl.name
                        let backDeckTargets = Array.filter (fun (cardView: CardView) -> cardView.Card.girl.name = targetGirl) <| backDeck.ToArray()
                        let frontDeckTargets = 
                            Array.filter (fun (cardView: CardViewWithStrap) -> cardView.Card.girl.name = targetGirl) (frontDeck.ToArray()) 
                            |> Array.map (fun (cardView: CardViewWithStrap) -> cardView :> CardView)
                        Array.append frontDeckTargets backDeckTargets |> ResizeArray
                    | SkillTarget.MySelf ->
                        [skillRaiseFrontCard] |> ResizeArray
                    
                    for target in targets do
                        match skill.mode with
                        | Mode.Attack ->
                            target.AppliedAttackSkillBonus.Add(skillRaiseFrontCard.Card.giveBaseBonusPercentageTo(target.Card) + (skillRaiseFrontCard.Card.skillLevel |> float) - 1.0)
                        | Mode.Defence ->
                            target.AppliedDefenceSkillBonus.Add(skillRaiseFrontCard.Card.giveBaseBonusPercentageTo(target.Card) + (skillRaiseFrontCard.Card.skillLevel |> float) - 1.0)
                        | Mode.AttackAndDefence ->
                            target.AppliedAttackSkillBonus.Add(skillRaiseFrontCard.Card.giveBaseBonusPercentageTo(target.Card) + (skillRaiseFrontCard.Card.skillLevel |> float) - 1.0)
                            target.AppliedDefenceSkillBonus.Add(skillRaiseFrontCard.Card.giveBaseBonusPercentageTo(target.Card) + (skillRaiseFrontCard.Card.skillLevel |> float) - 1.0)
                                                                  

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
        let exedGirlNum = deckEditViewModel.FrontDeck.FindAll(fun cardView -> cardView.Card.isEXed).Count + deckEditViewModel.BackDeck.FindAll(fun cardView -> cardView.Card.isEXed).Count
        let backDeckGirls = [||] |> ResizeArray
        let specialBonusGirl1 = specialBonusEditViewModel.MemorialStorySpecialGirl1
        let specialBonusGirl2 = specialBonusEditViewModel.MemorialStorySpecialGirl2
        let petitGirlLeaders = Array.choose (fun (petitDeck: ResizeArray<PetitGirlView>) -> if petitDeck.Count > 1 then Some(petitDeck.Item(0).petitGirl.girlName) else None) [|petitDeckEditViewModel.PetitGirlDeck1; petitDeckEditViewModel.PetitGirlDeck2; petitDeckEditViewModel.PetitGirlDeck3|]
        let backDeckGirls = ResizeArray.distinctBy (fun (c: CardView) -> c.Card.girl.name) deckEditViewModel.BackDeck
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
                    [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleDefenceBonus; petitCheerEffectDefenceBonus|] 
                    |> Array.append selectionDefenceBonusVals
                else 
                    [|colonBonus; petitCheerEffectAttackBonus|] 
                    |> Array.append selectionDefenceBonusVals
            | otherwise ->
                [|attrBonus; clubTypeBonus; colonBonus; whiteboardBonus; televisionBonus; lockerBonus; clubRoleDefenceBonus; petitCheerEffectAttackBonus|] 
        
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
                            | SpecificGirl limitDistinctGirlNum ->
                                let mutable distinctGirlsNum = 0
                                match scene.PreciousScene.sceneEffect.sceneTarget with
                                | All -> 
                                    let distinctGirlsArray = ResizeArray.distinctBy (fun (c: CardView) -> c.Card.girl.name) <| ResizeArray.append (ResizeArray.map (fun (c: CardViewWithStrap) -> c :> CardView) deckEditViewModel.FrontDeck) deckEditViewModel.BackDeck
                                    distinctGirlsNum <- (distinctGirlsArray.FindAll(fun (c: CardView) -> scene.PreciousScene.sceneEffect.sceneTargetAttribute.isAppliableOn(c.Attribute))).Count
                                | FrontDeck ->
                                    distinctGirlsNum <- (ResizeArray.distinctBy (fun (c: CardViewWithStrap) -> c.Card.girl.name) deckEditViewModel.FrontDeck).Count
                                | BackDeck ->
                                    distinctGirlsNum <- (ResizeArray.distinctBy (fun (c: CardView) -> c.Card.girl.name) deckEditViewModel.BackDeck).Count
                                let coefficient = 
                                    match (scene.PreciousScene.level, scene.PreciousScene.effectMaxNum) with
                                    | (3, 4.5) | (4, 5.5) | (5, 6.0) -> 
                                        Math.Pow((limitDistinctGirlNum |> float) / (Math.Max(girlsNum, limitDistinctGirlNum) |> float), 0.4)
                                    | _ -> 
                                        Math.Pow((limitDistinctGirlNum |> float) / (Math.Max(girlsNum, limitDistinctGirlNum) |> float), 0.2)
                                bonus <- coefficient * scene.PreciousScene.effectMaxNum
                            | Uniform ->
                                bonus <- scene.PreciousScene.effectMaxNum
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                        | WholeExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                            | WholeExedGirlNum maxGirlNum ->
                                bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                        | WholeExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                        | WholeExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                        | WholeExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                        | WholeExedGirlNum maxGirlNum ->
                            bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
                    | WholeExedGirlNum maxGirlNum ->
                        bonus <- Math.Pow((Math.Min(exedGirlNum, maxGirlNum) |> float) / (maxGirlNum |> float), 0.2) * scene.PreciousScene.effectMaxNum
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
        