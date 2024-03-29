﻿open System
open System.Windows
open System.Windows.Controls
open FsXaml
open GirlFriendDeckSimulator
open CardView
open PetitGirlView
open Microsoft.FSharp.Collections
open System.IO
open PreciousSceneView
open Card
open SkillType
open AttributeType
open FSharp.Data
open EventType
open CalcBonus
open Mode
open SelectionBonus

type PlayerParameterTab = XAML<"PlayerParameterTab.xaml">
type DeckEditTab = XAML<"DeckEditTab.xaml">
type MainWindow = XAML<"MainWindow.xaml">
type PetitDeckEditTab = XAML<"PetitDeckEditTab.xaml">
type SpecialBonusEditTab = XAML<"SpecialBonusEditTab.xaml">
type Settings = JsonProvider<".\Setting.json">
 

[<STAThread>]
[<EntryPoint>]
let main argv =
    //initializeでフォーム生成
    let window = MainWindow()
    let playerParameterTabContent = PlayerParameterTab()
    let deckEditTabContent = DeckEditTab()
    let petitDeckEditTabContent = PetitDeckEditTab()
    let specialBonusEditTabContent = SpecialBonusEditTab()
    
    match PlayerParameterViewModel.loadPlayerParameterViewModel with
    | Some(playerParameterViewModel) -> playerParameterTabContent.DataContext <- playerParameterViewModel
    | None -> 0 |> ignore
    //match DeckEditViewModel.loadDeckEditViewModel(EventType.Raid) with // 初期値はレイド
    //| Some(deckEditViewModel) -> deckEditTabContent.DataContext <- deckEditViewModel
    //| None -> 0 |> ignore
    let deckEditViewModel = DeckEditViewModel()
    deckEditTabContent.DataContext <- deckEditViewModel
    deckEditViewModel.loadDeck()

    match PetitDeckEditViewModel.loadPetitDeckEditViewModel with
    | Some(petitDeckEditViewModel) -> petitDeckEditTabContent.DataContext <- petitDeckEditViewModel
    | None -> 0 |> ignore
    match SpecialBonusEditViewModel.loadSpecialBonusEditViewModel with
    | Some(specialBonusEditViewModel) -> specialBonusEditTabContent.DataContext <- specialBonusEditViewModel
    | None -> 0 |> ignore

    let playerParameterTab = TabItem()
    playerParameterTab.Header <- "プレイヤー情報"
    playerParameterTab.Content <- playerParameterTabContent

    let deckEditTab = TabItem()
    deckEditTab.Header <- "デッキ編集"
    deckEditTab.Content <- deckEditTabContent

    let petitDeckEditTab = TabItem()
    petitDeckEditTab.Header <- "ぷちデッキ編集"
    petitDeckEditTab.Content <- petitDeckEditTabContent

    let specialBonusEditTab = TabItem()
    specialBonusEditTab.Header <- "SP効果編集"
    specialBonusEditTab.Content <- specialBonusEditTabContent


    window.TabControl.Items.Add(playerParameterTab) |> ignore
    window.TabControl.Items.Add(deckEditTab) |> ignore
    window.TabControl.Items.Add(petitDeckEditTab) |> ignore
    window.TabControl.Items.Add(specialBonusEditTab) |> ignore

    
    let indexing(cardArray: ResizeArray<CardView>) =
        for (index, cardView) in Array.indexed <| cardArray.ToArray() do
            cardView.Index <- (index + 1).ToString()
    
    let refreshSelectionBonusInfo() =
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let petitDeckEditViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel

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
            | None -> 
                selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)
            if selectionBonus.SelectionBonusName = "Precious★Friend"
            then 
                0 |> ignore
            else
                selectionBonusMaxLevelMap <- selectionBonusMaxLevelMap.Add(selectionBonus, selectionBonusMaxLevelMap.Item(selectionBonus) + 1)

        //for card in deckEditViewModel.FrontDeck do
        //    for selectionBonus in card.Card.girl.selectionBonuses do
        //        match selectionBonusLevelMap.TryFind(selectionBonus) with
        //        | Some(level) -> 
        //            selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, level + 1)
        //        | None -> 
        //            selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)
        //for card in deckEditViewModel.BackDeck do
        //    for selectionBonus in card.Card.girl.selectionBonuses do
        //        match selectionBonusLevelMap.TryFind(selectionBonus) with
        //        | Some(level) -> 
        //            selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, level + 1)
        //        | None -> 
        //            selectionBonusLevelMap <- selectionBonusLevelMap.Add(selectionBonus, 1)

        deckEditViewModel.SelectionBonusInfoViews <- 
            [|for pair in selectionBonusLevelMap -> 
                let maxLevel = selectionBonusMaxLevelMap.Item(pair.Key)
                SelectionBonusInfoView(pair.Key, pair.Value, maxLevel)
            |] 
            |> Array.filter (fun (sbiv: SelectionBonusInfoView) -> sbiv.SelectionBonus.getSelectionBonusMode.IsAppliable(deckEditViewModel.SelectedMode)) 
            |> Array.sortDescending
            |> ResizeArray

        deckEditTabContent.SelectionBonusBox.Items.Refresh()


    let calcDamage(frontDeck: ResizeArray<CardViewWithStrap>, backDeck: ResizeArray<CardView>, petitDeckTotalAttack: int, petitDeckTotalDefence: int, attackCost: int, eventType: EventType, mode: Mode) =
        let frontTotal =
            match mode with
            | Mode.Attack ->
                Array.sumBy(fun (cardView: CardViewWithStrap) -> cardView.CorrectedAttack) <| frontDeck.ToArray()
            | Mode.Defence -> 
                Array.sumBy(fun (cardView: CardViewWithStrap) -> cardView.CorrectedDefence) <| frontDeck.ToArray()
            | otherwise -> failwith "不正なモード選択です。"
        let backTotal = 
            match mode with
            | Mode.Attack ->
                Array.sumBy(fun (cardView: CardView) -> cardView.CorrectedAttack) <| backDeck.ToArray()
            | Mode.Defence ->
                Array.sumBy(fun (cardView: CardView) -> cardView.CorrectedDefence) <| backDeck.ToArray()
            | otherwise -> failwith "不正なモード選択です。"
        let deckTotal = frontTotal + backTotal
        let petitTotal =
            match mode with
            | Mode.Attack -> petitDeckTotalAttack
            | Mode.Defence -> petitDeckTotalDefence
            | otherwise -> failwith "不正なモード選択です。"

        let attackFactor = (attackCost |> float) / 100.0
        let specialBonus = 
            let specialBonusEditViewModel = specialBonusEditTabContent.DataContext :?> SpecialBonusEditViewModel
            match eventType with
            | EventType.CoolMega -> specialBonusEditViewModel.RaidCoolMega
            | EventType.PopMega -> specialBonusEditViewModel.RaidPopMega
            | EventType.SweetMega -> specialBonusEditViewModel.RaidSweetMega
            | EventType.Raid -> specialBonusEditViewModel.RaidSuperRare
            | EventType.Hunters -> specialBonusEditViewModel.HuntersSuperRare
            | otherwise -> 0
            |> int64
        let maxConsumeUnitNum = 
            match eventType with
            | EventType.Raid | EventType.CoolTrio |  EventType.PopTrio | EventType.SweetTrio | EventType.CoolMega | EventType.PopMega | EventType.SweetMega -> 12L
            | EventType.Hunters -> 36L
            | otherwise -> 1L

        let damage: int64 = 
            match eventType with
            | EventType.CoolMega | EventType.PopMega | EventType.SweetMega ->
                maxConsumeUnitNum * ((((frontTotal |> float) * attackFactor * 4.0) |> ceil |> int64) + (((petitDeckTotalAttack |> float) / 4.0 * attackFactor) |> ceil |> int64) * 2L + specialBonus)
            | EventType.Raid | EventType.CoolTrio | EventType.PopTrio | EventType.SweetTrio | EventType.Hunters ->
                maxConsumeUnitNum * ((((deckTotal |> float) * attackFactor) |> ceil |> int64) + (((petitTotal |> float) * attackFactor) |> ceil |> int64) + specialBonus)
            | EventType.Charisma | EventType.Battle | EventType.MemorialStory ->
                maxConsumeUnitNum * ((deckTotal |> int64) + ((petitTotal |> int64)) + specialBonus)
        damage

    let calcDamageExpectation() =
        let settings = Settings.Parse(File.ReadAllText(".\Setting.json"))
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let petitDeckEditViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        let specialBonusEditViewModel = specialBonusEditTabContent.DataContext :?> SpecialBonusEditViewModel

        let rec makePowerSet(triggeredIndexList: list<int>) =
            match triggeredIndexList with
            | [] -> [[]]
            | (x::xs) -> 
                let xss = makePowerSet xs
                List.map (fun xs -> x::xs) xss @ xss

        // 声援、タッチ、本命、デート情報保存
        let isTriggeredSkillBonusList = Array.map (fun (c: CardViewWithStrap) -> c.IsTriggeredSkillBonus) <| deckEditViewModel.FrontDeck.ToArray()
            
        let isTriggeredSkillBonusSwitchList = Array.map (fun (c: CardView) -> (c.IsTriggeredSkillBonus, c)) <| deckEditViewModel.BackDeck.ToArray()

        let triggeredSkillBonusSwitchList = 
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then Array.empty
            else Array.filter (fun (c: CardView) -> c.Card.cardType = CardType.Switch) (deckEditViewModel.BackDeck.ToArray())
        
        let triggeredSkillBonusGirlList = Array.append ((deckEditViewModel.FrontDeck.ToArray()) |> Array.map (fun (c: CardViewWithStrap) -> c :> CardView)) triggeredSkillBonusSwitchList
        
        //let frontDeckNum = deckEditViewModel.FrontDeck.Count
        
        let fixedFrontSkillTriggeredGirlNum = 
            let maxFrontFixNum =
                match deckEditViewModel.SelectedEventType with
                | CoolTrio | PopTrio | SweetTrio | Raid ->
                    if deckEditViewModel.SelectedMode = Mode.Defence
                    then
                        3
                    else
                        1
                | Hunters | Charisma | MemorialStory | CoolMega | PopMega | SweetMega -> 1
                | Battle -> 5
            min deckEditViewModel.FrontDeck.Count maxFrontFixNum
        let maxFrontSkillTriggeredGirlNum =
            match deckEditViewModel.SelectedEventType with
            | CoolTrio | PopTrio | SweetTrio | Raid ->
                if deckEditViewModel.SelectedMode = Mode.Defence
                then
                    3
                else
                    5
            | Hunters -> 3 
            | Charisma | MemorialStory | CoolMega | PopMega | SweetMega -> 5
            | Battle -> 5

        let fixedSwitchSkillTriggeredGirlNum =
            let maxSwitchFixNum =
                match deckEditViewModel.SelectedEventType with
                | CoolTrio | PopTrio | SweetTrio | Raid ->
                    if deckEditViewModel.SelectedMode = Mode.Defence
                    then
                        2
                    else
                        0
                | Battle -> 2
                | otherwise -> 0
            min maxSwitchFixNum <| Array.length(triggeredSkillBonusSwitchList)

        let maxSwitchSkillTriggeredGirlNum = 2                
        let triggeredSkillBonusGirlNum = triggeredSkillBonusGirlList.Length
        let mutable expectation: float = 0.0
        //let switchIndexStartPosition = triggeredSkillBonusGirlNum - deckEditViewModel.FrontDeck.Count
        
        // 主センの確定分声援発動
        // 先頭優先
        let mutable frontSkillTriggeredNum = 0
        for frontSkillCardView in deckEditViewModel.FrontDeck do
            if frontSkillTriggeredNum < fixedFrontSkillTriggeredGirlNum
            then
                frontSkillCardView.IsTriggeredSkillBonus <- true  
                frontSkillTriggeredNum <- frontSkillTriggeredNum + 1
            else 0 |> ignore
        // スイッチガールの確定分発動
        // 先頭優先
        let mutable switchSkillTriggeredGirlNum = 0
        for switchSkillCardView in triggeredSkillBonusSwitchList do
            if switchSkillTriggeredGirlNum < fixedSwitchSkillTriggeredGirlNum
            then
                switchSkillCardView.IsTriggeredSkillBonus <- true  
                switchSkillTriggeredGirlNum <- switchSkillTriggeredGirlNum + 1
            else 0 |> ignore

        for frontIndices in makePowerSet([fixedFrontSkillTriggeredGirlNum..deckEditViewModel.FrontDeck.Count-1]) do
            for backIndices in makePowerSet([fixedSwitchSkillTriggeredGirlNum..Array.length(triggeredSkillBonusSwitchList)-1]) do
                // 初期化
                for index in [fixedFrontSkillTriggeredGirlNum..deckEditViewModel.FrontDeck.Count-1] do
                    deckEditViewModel.FrontDeck.[index].IsTriggeredSkillBonus <- false

                for index in [fixedSwitchSkillTriggeredGirlNum..Array.length(triggeredSkillBonusSwitchList)-1] do
                    triggeredSkillBonusSwitchList.[index].IsTriggeredSkillBonus <- false

                let mutable frontSkillCardNum = frontSkillTriggeredNum + 1
                let mutable backSkillCardNum = switchSkillTriggeredGirlNum + 1
                for frontIndex in frontIndices do
                    if frontSkillCardNum < maxFrontSkillTriggeredGirlNum
                    then
                        deckEditViewModel.FrontDeck.[frontIndex].IsTriggeredSkillBonus <- true
                        frontSkillCardNum <- frontSkillCardNum + 1
                    else 0 |> ignore 
                for backIndex in backIndices do
                    if backSkillCardNum < maxSwitchSkillTriggeredGirlNum
                    then
                        triggeredSkillBonusSwitchList.[backIndex].IsTriggeredSkillBonus <- true
                        backSkillCardNum <- backSkillCardNum + 1
            
                CalcBonus.applySkillBonus(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, deckEditViewModel.CardListView)
                for cardView in deckEditViewModel.FrontDeck do
                    let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                    cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                    cardView.CorrectedDefence <- correctedVals.CorrectedDefence
                for cardView in deckEditViewModel.BackDeck do
                    let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                    cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                    cardView.CorrectedDefence <- correctedVals.CorrectedDefence
                
                // 主 確定1 最大声援発揮数5 設定主セン人数5 
                // スイッチ 確定0 最大声援発揮数2 設定スイッチ人数3のとき
                // frontIndicesは{1..4}の部分集合
                // backIndicesは{0..2}の部分集合
                // 最大発揮数オーバーのときも期待値の計算は普通に
                let triggeredProbability = 
                    Math.Pow(settings.SkillBonusSettings.SkillRaisedProbability |> float, List.length(frontIndices) + List.length(backIndices) |> float)
                    * Math.Pow(1.0 - (settings.SkillBonusSettings.SkillRaisedProbability |> float), deckEditViewModel.FrontDeck.Count - fixedFrontSkillTriggeredGirlNum + (Array.length(triggeredSkillBonusSwitchList) - fixedSwitchSkillTriggeredGirlNum) - (List.length(frontIndices) + List.length(backIndices)) |> float)
            
                expectation <- expectation + (calcDamage(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, petitDeckEditViewModel.TotalAttack, petitDeckEditViewModel.TotalDefence, playerParameterViewModel.AttackCost, deckEditViewModel.SelectedEventType, deckEditViewModel.SelectedMode) |> float) * triggeredProbability
       
        for (index, isTriggeredSkillBonus) in Array.indexed isTriggeredSkillBonusList do
            deckEditViewModel.FrontDeck.[index].IsTriggeredSkillBonus <- isTriggeredSkillBonus
        for (isTriggeredSkillBonus, card) in isTriggeredSkillBonusSwitchList do
            deckEditViewModel.BackDeck.[(card.Index |> int) - 1].IsTriggeredSkillBonus <- isTriggeredSkillBonus

        expectation
        
    let refreshDeckEditTab() =
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let petitDeckEditViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        let specialBonusEditViewModel = specialBonusEditTabContent.DataContext :?> SpecialBonusEditViewModel

        match deckEditViewModel.SelectedEventType with
        | EventType.CoolMega | EventType.PopMega | EventType.SweetMega -> 
            //副センバツは計算しない（重いから）
            CalcBonus.applySkillBonus(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, deckEditViewModel.CardListView)
            deckEditViewModel.FrontDeck.ForEach(fun cardView ->
                let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
            deckEditViewModel.BackDeck.ForEach(fun cardView ->
                let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
            deckEditViewModel.CardListView.ForEach(fun cardView ->
                let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
            // 予想ダメージ
            deckEditViewModel.EstimatedDamage <- calcDamage(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, petitDeckEditViewModel.TotalAttack, petitDeckEditViewModel.TotalDefence, playerParameterViewModel.AttackCost, deckEditViewModel.SelectedEventType, deckEditViewModel.SelectedMode)
        | otherwise ->
            CalcBonus.applySkillBonus(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, deckEditViewModel.CardListView)
            
            deckEditViewModel.CardListView.ForEach(fun cardView ->
                 let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                 cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                 cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
            deckEditViewModel.FrontDeck.ForEach(fun cardView ->
                let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
            deckEditViewModel.BackDeck.ForEach(fun cardView ->
                let correctedVals = CalcBonus.calcBonus(cardView, playerParameterViewModel.playerFactory, deckEditViewModel, petitDeckEditViewModel, specialBonusEditViewModel)
                cardView.CorrectedAttack <- correctedVals.CorrectedAttack
                cardView.CorrectedDefence <- correctedVals.CorrectedDefence
            )
        
        if deckEditViewModel.SelectedMode = Mode.Attack
            then 
                deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
                if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
                then
                    deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
                else 
                    deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
                deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
            else 
                deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
                if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
                then
                    deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
                else 
                    deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
                deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
        let BackDeckListItemSource = deckEditTabContent.BackDeckList.ItemsSource
        deckEditTabContent.BackDeckList.ItemsSource <- null
        deckEditTabContent.BackDeckList.ItemsSource <- BackDeckListItemSource
        let FrontDeckGirlListItemsSource = deckEditTabContent.FrontDeckGirlList.ItemsSource
        deckEditTabContent.FrontDeckGirlList.ItemsSource <- null
        deckEditTabContent.FrontDeckGirlList.ItemsSource <- FrontDeckGirlListItemsSource
        let PreciousSceneListBoxItemsSource = deckEditTabContent.PreciousSceneListBox.ItemsSource
        deckEditTabContent.PreciousSceneListBox.ItemsSource <- null
        deckEditTabContent.PreciousSceneListBox.ItemsSource <- PreciousSceneListBoxItemsSource
        let SelectedPreciousSceneListBoxItemsSource = deckEditTabContent.SelectedPreciousSceneListBox.ItemsSource
        deckEditTabContent.SelectedPreciousSceneListBox.ItemsSource <- null
        deckEditTabContent.SelectedPreciousSceneListBox.ItemsSource <- SelectedPreciousSceneListBoxItemsSource
        
        refreshSelectionBonusInfo()
        
        //let SelectionBonusInfoViews = deckEditTabContent.SelectionBonusBox.ItemsSource
        //Console.WriteLine((SelectionBonusInfoViews :?> ResizeArray<SelectionBonusInfoView>).Count)
        //Console.WriteLine(deckEditViewModel.SelectionBonusInfoViews.Count)
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- null
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- SelectionBonusInfoViews

        deckEditViewModel.TotalAttack <- 0
        deckEditViewModel.TotalDefence <- 0
        for card in deckEditViewModel.FrontDeck do
            deckEditViewModel.TotalAttack <- deckEditViewModel.TotalAttack + card.CorrectedAttack
            deckEditViewModel.TotalDefence <- deckEditViewModel.TotalDefence + card.CorrectedDefence
        if (deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega) |> not
        then
            for card in deckEditViewModel.BackDeck do
                deckEditViewModel.TotalAttack <- deckEditViewModel.TotalAttack + card.CorrectedAttack
                deckEditViewModel.TotalDefence <- deckEditViewModel.TotalDefence + card.CorrectedDefence
        else 
            0 |> ignore
        deckEditViewModel.TotalAttack <- deckEditViewModel.TotalAttack + petitDeckEditViewModel.TotalAttack
        deckEditViewModel.TotalDefence <- deckEditViewModel.TotalDefence + petitDeckEditViewModel.TotalDefence
        deckEditViewModel.EstimatedDamage <- calcDamage(deckEditViewModel.FrontDeck, deckEditViewModel.BackDeck, petitDeckEditViewModel.TotalAttack, petitDeckEditViewModel.TotalDefence, playerParameterViewModel.AttackCost, deckEditViewModel.SelectedEventType, deckEditViewModel.SelectedMode)

    window.TabControl.SelectionChanged.Add(fun e ->
        if(window.TabControl.SelectedIndex = 1 && e.OriginalSource :? TabControl) 
            then
                refreshDeckEditTab()
            else 0 |> ignore
    )
    
    // プレイヤー情報セーブ用イベントハンドラー
    playerParameterTabContent.PlayerNameTextBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.AttackCostTextBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.BackDeckNum.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.ClubTypeComboBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.AttributeTypeComboBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.CoolColonTextBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.PopColonTextBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.SweetColonTextBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.WhiteboardCheckBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.TelevisionCheckBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.LockerCheckBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )
    playerParameterTabContent.ClubRoleTypeComboBox.LostFocus.Add(fun e ->
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        playerParameterViewModel.save
    )

    //ぷちデッキ編集用イベント

    petitDeckEditTabContent.InPetitDeck1Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        if petitDeckEditTabContent.PetitGirlListBox.SelectedItems.Count + petitDeckViewModel.PetitGirlDeck1.Count > 10 
            then 
                MessageBox.Show("設定しようとしているぷちが10人を超えています") |> ignore
            else 
                for p in petitDeckEditTabContent.PetitGirlListBox.SelectedItems do
                    petitDeckViewModel.PetitGirlDeck1.Add(p :?> PetitGirlView)
                    petitDeckViewModel.PetitGirlList.Remove(p :?> PetitGirlView) |> ignore
                petitDeckViewModel.save
                petitDeckViewModel.recalculate()
                
                let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
                petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
                petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
                let petitDeck1ItemSource = petitDeckEditTabContent.PetitGirlDeck1.ItemsSource
                petitDeckEditTabContent.PetitGirlDeck1.ItemsSource <- null
                petitDeckEditTabContent.PetitGirlDeck1.ItemsSource <- petitDeck1ItemSource
    )
    petitDeckEditTabContent.OutPetitDeck1Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck1.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck1.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.save
        petitDeckViewModel.recalculate()
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
        let petitDeck1ItemSource = petitDeckEditTabContent.PetitGirlDeck1.ItemsSource
        petitDeckEditTabContent.PetitGirlDeck1.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlDeck1.ItemsSource <- petitDeck1ItemSource
    )
    petitDeckEditTabContent.InPetitDeck2Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        if petitDeckEditTabContent.PetitGirlListBox.SelectedItems.Count + petitDeckViewModel.PetitGirlDeck2.Count > 10 
        then 
            MessageBox.Show("設定しようとしているぷちが10人を超えています") |> ignore
        else 
            for p in petitDeckEditTabContent.PetitGirlListBox.SelectedItems do
                petitDeckViewModel.PetitGirlDeck2.Add(p :?> PetitGirlView)
                petitDeckViewModel.PetitGirlList.Remove(p :?> PetitGirlView) |> ignore
            petitDeckViewModel.save
            petitDeckViewModel.recalculate()
            let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
            petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
            petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
            let petitDeck2ItemSource = petitDeckEditTabContent.PetitGirlDeck2.ItemsSource
            petitDeckEditTabContent.PetitGirlDeck2.ItemsSource <- null
            petitDeckEditTabContent.PetitGirlDeck2.ItemsSource <- petitDeck2ItemSource
    )
    petitDeckEditTabContent.OutPetitDeck2Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck2.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck2.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.save
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        petitDeckViewModel.recalculate()
        let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
        let petitDeck2ItemSource = petitDeckEditTabContent.PetitGirlDeck2.ItemsSource
        petitDeckEditTabContent.PetitGirlDeck2.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlDeck2.ItemsSource <- petitDeck2ItemSource
    )
    petitDeckEditTabContent.InPetitDeck3Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        if petitDeckEditTabContent.PetitGirlListBox.SelectedItems.Count + petitDeckViewModel.PetitGirlDeck3.Count > 10 
        then 
            MessageBox.Show("設定しようとしているぷちが10人を超えています") |> ignore
        else 
            for p in petitDeckEditTabContent.PetitGirlListBox.SelectedItems do
                petitDeckViewModel.PetitGirlDeck3.Add(p :?> PetitGirlView)
                petitDeckViewModel.PetitGirlList.Remove(p :?> PetitGirlView) |> ignore
            petitDeckViewModel.recalculate()
            petitDeckViewModel.save
            let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
            petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
            petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
            let petitDeck3ItemSource = petitDeckEditTabContent.PetitGirlDeck3.ItemsSource
            petitDeckEditTabContent.PetitGirlDeck3.ItemsSource <- null
            petitDeckEditTabContent.PetitGirlDeck3.ItemsSource <- petitDeck3ItemSource
    )
    petitDeckEditTabContent.OutPetitDeck3Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck3.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck3.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.save
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        petitDeckViewModel.recalculate()
        let petitGirlListItemsSource = petitDeckEditTabContent.PetitGirlListBox.ItemsSource
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlListBox.ItemsSource <- petitGirlListItemsSource
        let petitDeck3ItemSource = petitDeckEditTabContent.PetitGirlDeck3.ItemsSource
        petitDeckEditTabContent.PetitGirlDeck3.ItemsSource <- null
        petitDeckEditTabContent.PetitGirlDeck3.ItemsSource <- petitDeck3ItemSource
    )
    
    // デッキ編集用イベントハンドラー
    deckEditTabContent.InFrontDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        
        match deckEditViewModel.SelectedEventType with
        | Charisma ->
            if deckEditTabContent.GirlListBox.SelectedItems.Count + deckEditViewModel.FrontDeck.Count > 10
            then MessageBox.Show("設定しようとしている主センバツガールが10人を超えています") |> ignore
            else 
                for g in deckEditTabContent.GirlListBox.SelectedItems do
                    deckEditViewModel.FrontDeck.Add(CardViewWithStrap((g :?> CardView).Card))
                    deckEditViewModel.CardListView.Remove(g :?> CardView) |> ignore
                deckEditViewModel.save()
                refreshDeckEditTab()
        | otherwise ->
            if deckEditTabContent.GirlListBox.SelectedItems.Count + deckEditViewModel.FrontDeck.Count > 5
            then MessageBox.Show("設定しようとしている主センバツガールが5人を超えています") |> ignore
            else 
                for g in deckEditTabContent.GirlListBox.SelectedItems do
                    deckEditViewModel.FrontDeck.Add(CardViewWithStrap((g :?> CardView).Card))
                    deckEditViewModel.CardListView.Remove(g :?> CardView) |> ignore
                deckEditViewModel.save()
                refreshDeckEditTab()
    )

    deckEditTabContent.OutFrontDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        for g in deckEditTabContent.FrontDeckGirlList.SelectedItems do
            deckEditViewModel.CardListView.Add((g :?> CardViewWithStrap).removeStraps)
            deckEditViewModel.FrontDeck.Remove(g :?> CardViewWithStrap) |> ignore
        deckEditViewModel.save()
        //再計算
        deckEditViewModel.applyFilter()
        refreshDeckEditTab()

    )
    
    deckEditTabContent.GirlListBox.CellEditEnding.Add(fun e ->
        if e.EditAction = DataGridEditAction.Commit
            then 
                let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
                if e.Column.Header.ToString() = "デート設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsDatingCard
                            then // checked
                                for r in deckEditViewModel.CardListView do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsDatingCard <- false
                                selectedCardView.IsDatingCard <- true
                            else 
                                0 |> ignore
                    else
                        0 |> ignore
                if e.Column.Header.ToString() = "本命設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsFavoriteCard
                            then // checked
                                for r in deckEditViewModel.CardListView do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsFavoriteCard <- false
                                selectedCardView.IsFavoriteCard <- true
                            else 
                                0 |> ignore
                    else
                        0 |> ignore
                deckEditViewModel.save()
                refreshDeckEditTab()
            else 0 |> ignore
    )
    deckEditTabContent.FrontDeckGirlList.CellEditEnding.Add(fun e ->
        if e.EditAction = DataGridEditAction.Commit
            then 
                let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
                if e.Column.Header.ToString() = "デート設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsDatingCard
                            then
                                // check入った方の処理
                                for r in deckEditViewModel.CardListView do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsDatingCard <- false
                                selectedCardView.IsDatingCard <- true
                            else 0 |> ignore
                    else
                        0 |> ignore
                if e.Column.Header.ToString() = "本命設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsFavoriteCard
                            then // チェック入った方 
                                for r in deckEditViewModel.CardListView do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsFavoriteCard <- false
                                selectedCardView.IsFavoriteCard <- true
                            else 
                                0 |> ignore
                    else
                        0 |> ignore
                // カリスマ用のストラップ処理
                if deckEditViewModel.SelectedEventType = Charisma
                then
                    for card in deckEditViewModel.FrontDeck do
                        card.Strap2Str <- "0.0"
                        card.Strap3Str <- "0.0"
                        card.Strap4Str <- "0.0"
                else 0 |> ignore
                deckEditViewModel.save()
                refreshDeckEditTab()
            else 0 |> ignore
    )  
    
    deckEditTabContent.BackDeckList.CellEditEnding.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        if e.EditAction = DataGridEditAction.Commit
            then 
                let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
                if e.Column.Header.ToString() = "デート設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsDatingCard
                            then
                                // check入った方の処理
                                for r in deckEditViewModel.CardListView do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsDatingCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsDatingCard <- false
                                selectedCardView.IsDatingCard <- true
                            else 0 |> ignore
                    else
                        0 |> ignore
                if e.Column.Header.ToString() = "本命設定"
                    then
                        let selectedCardView = e.Row.DataContext :?> CardView
                        if selectedCardView.IsFavoriteCard
                            then // チェック入った方 
                                for r in deckEditViewModel.CardListView do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.FrontDeck do
                                    r.IsFavoriteCard <- false
                                for r in deckEditViewModel.BackDeck do
                                    r.IsFavoriteCard <- false
                                selectedCardView.IsFavoriteCard <- true
                            else 
                                0 |> ignore
                    else
                        0 |> ignore

                if e.Column.Header.ToString() = "声援発動"
                then
                    let selectedCardView = e.Row.DataContext :?> CardView
                    if selectedCardView.IsTriggeredSkillBonus 
                        && not (deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega)
                        then // チェック入った方 
                            // スイッチガールでないのに声援にチェックがつくと取り消す
                            if selectedCardView.Card.cardType = CardType.Switch
                            then 
                                0 |> ignore
                            else
                                selectedCardView.IsTriggeredSkillBonus <- false
                        else 
                            0 |> ignore
                else
                    0 |> ignore
                
                deckEditViewModel.save()
                refreshDeckEditTab()
            else 0 |> ignore
    )  

    deckEditTabContent.FrontDeckGirlList.SelectionChanged.Add(fun e ->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel

        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let selectedItemsArray = Array.zeroCreate deckEditTabContent.FrontDeckGirlList.SelectedItems.Count
        deckEditTabContent.FrontDeckGirlList.SelectedItems.CopyTo(selectedItemsArray, 0)
        let selectedGirlNames = Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardViewWithStrap).CardName) <| selectedItemsArray
        let selectedCards = Array.distinctBy (fun (cardView: CardViewWithStrap) -> cardView.CardName) <| (Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardViewWithStrap)) <| selectedItemsArray)
        for line in selectedCards do
            //let cardView = line :?> CardViewWithStrap   
            if deckEditViewModel.FrontDeck.FindAll(fun cardView -> cardView.CardName = line.CardName).Count > (Array.length <| Array.filter (fun girlName -> girlName = line.CardName) selectedGirlNames)
                || deckEditViewModel.BackDeck.Exists(fun cardView -> cardView.CardName = line.CardName)
            then 
                // 主センバツから当該ガールがすべて抜ける予定ではないか、副センバツに当該ガールが存在する場合、センバツボーナスは変化しない
                0 |> ignore
            else
                for selectionBonus in line.Card.girl.selectionBonuses do
                    for listItem in deckEditTabContent.SelectionBonusBox.Items do
                        let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                        if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                        then 
                            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel - 1
                            selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                            selectionBonusInfoView.IsOutOfDeck <- true
                        else 
                            0 |> ignore
    )

    deckEditTabContent.FrontDeckGirlList.GotFocus.Add(fun e ->
        //Console.WriteLine(e.OriginalSource)
        // Focusが当たっているCardViewは取れる
        // でも複数選択のときの処理が不可能（フォーカスが当たるのはひとつだけ）
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        match e.OriginalSource with
        | :? DataGridCell as cell ->
            match cell.DataContext with
            | :? CardViewWithStrap as cardView ->
                if deckEditViewModel.FrontDeck.FindAll(fun frontCardView -> cardView.CardName = frontCardView.CardName).Count > 1
                    || deckEditViewModel.BackDeck.Exists(fun backCardView -> cardView.CardName = backCardView.CardName) 
                then 
                    0 |> ignore // 同一カードが存在
                else
                    for selectionBonus in cardView.Card.girl.selectionBonuses do
                        for listItem in deckEditTabContent.SelectionBonusBox.Items do
                            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                            if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                            then 
                                selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel - 1
                                selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                                selectionBonusInfoView.IsOutOfDeck <- true
                            else 
                                0 |> ignore
            | _ -> 0 |> ignore
        | _ -> 0 |> ignore
        //for line in deckEditTabContent.GirlListBox.SelectedItems do
        //    let cardView = line :?> CardView
        
    )

    deckEditTabContent.FrontDeckGirlList.LostFocus.Add(fun e->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
            selectionBonusInfoView.ConvertToOrdinarySelectionBonusInfo()
            selectionBonusInfoView.IsOutOfDeck <- false
    )

    deckEditTabContent.BackDeckList.SelectionChanged.Add(fun e ->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
        
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let selectedItemsArray = Array.zeroCreate deckEditTabContent.BackDeckList.SelectedItems.Count
        deckEditTabContent.BackDeckList.SelectedItems.CopyTo(selectedItemsArray, 0)
        let selectedGirlNames = Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardView).CardName) <| selectedItemsArray
        let selectedCards = Array.distinctBy (fun (cardView: CardView) -> cardView.CardName) <| (Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardView)) <| selectedItemsArray)
        
        for line in selectedCards do
            //let cardView = line :?> CardView
            if deckEditViewModel.BackDeck.FindAll(fun cardView -> cardView.CardName = line.CardName).Count > (Array.length <| Array.filter (fun girlName -> girlName = line.CardName) selectedGirlNames)
                || deckEditViewModel.FrontDeck.Exists(fun cardView -> cardView.CardName = line.CardName)
            then 
                0 |> ignore
            else
                for selectionBonus in line.Card.girl.selectionBonuses do
                    for listItem in deckEditTabContent.SelectionBonusBox.Items do
                        let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                        if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                        then 
                            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel - 1
                            selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                            selectionBonusInfoView.IsOutOfDeck <- true
                        else 
                            0 |> ignore
    )

    deckEditTabContent.BackDeckList.GotFocus.Add(fun e ->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
        //Console.WriteLine(e.OriginalSource)
        // Focusが当たっているCardViewは取れる
        // でも複数選択のときの処理が不可能（フォーカスが当たるのはひとつだけ）
        match e.OriginalSource with
        | :? DataGridCell as cell ->
            match cell.DataContext with
            | :? CardView as cardView ->
                
        //let cardView = (e.OriginalSource :?> DataGridCell).DataContext :?> CardView
        //for line in deckEditTabContent.GirlListBox.SelectedItems do
        //    let cardView = line :?> CardView
                if deckEditViewModel.BackDeck.FindAll(fun backCardView -> cardView.CardName = backCardView.CardName).Count > 1
                    || deckEditViewModel.FrontDeck.Exists(fun frontCardView -> cardView.CardName = frontCardView.CardName)
                then
                    0 |> ignore
                else
                    for selectionBonus in cardView.Card.girl.selectionBonuses do
                        for listItem in deckEditTabContent.SelectionBonusBox.Items do
                            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                            if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                            then 
                                selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel - 1
                                selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                                selectionBonusInfoView.IsOutOfDeck <- true
                            else 
                                0 |> ignore
            | _ -> 0 |> ignore
        | _ -> 0 |> ignore
    )

    deckEditTabContent.BackDeckList.LostFocus.Add(fun e->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
            selectionBonusInfoView.ConvertToOrdinarySelectionBonusInfo()
            selectionBonusInfoView.IsOutOfDeck <- false
    )
    
    deckEditTabContent.GirlListBox.GotFocus.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel

        deckEditViewModel.SelectionBonusInfoViews.RemoveAll(fun sbiv -> sbiv.CurrentSelectionBonusLevel = 0) |> ignore    
        //Console.WriteLine(e.OriginalSource)
        // Focusが当たっているCardViewは取れる
        // でも複数選択のときの処理が不可能（フォーカスが当たるのはひとつだけ）
        match e.OriginalSource with
        | :? DataGridCell as cell ->
            match cell.DataContext with
            | :? CardView as cardView ->

        //let cardView = (e.OriginalSource :?> DataGridCell).DataContext :?> CardView
        //for line in deckEditTabContent.GirlListBox.SelectedItems do
        //    let cardView = line :?> CardView
                if deckEditViewModel.FrontDeck.Exists(fun frontCard -> frontCard.CardName = cardView.CardName)
                    || deckEditViewModel.BackDeck.Exists(fun backCard -> backCard.CardName = cardView.CardName)
                then
                    0 |> ignore
                else
                    let newSelectionBonusInfoViews = [||] |> ResizeArray
                    for selectionBonus in cardView.Card.girl.selectionBonuses do
                        if deckEditViewModel.SelectionBonusInfoViews.Exists(fun sbiv -> sbiv.SelectionBonus = selectionBonus)
                        then
                            for listItem in deckEditTabContent.SelectionBonusBox.Items do
                                let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                                if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                                then 
                                    selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel + 1
                                    selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                                    selectionBonusInfoView.IsIntoDeck <- true
                                else 
                                    0 |> ignore
                        else 
                            let newSelectionBonusInfoView = SelectionBonusInfoView(selectionBonus, 0, 5)
                            newSelectionBonusInfoView.AfterEditSelectionBonusLevel <- 1
                            newSelectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                            newSelectionBonusInfoView.IsIntoDeck <- true
                            if newSelectionBonusInfoView.SelectionBonus.getSelectionBonusMode.IsAppliable(deckEditViewModel.SelectedMode)
                            then 
                                newSelectionBonusInfoViews.Add(newSelectionBonusInfoView)
                            else 0 |> ignore
                    deckEditViewModel.SelectionBonusInfoViews.AddRange(newSelectionBonusInfoViews)
                    deckEditTabContent.SelectionBonusBox.Items.Refresh()
            | _ -> 0 |> ignore
        | _ -> 0 |> ignore
        //let selectionBonusInfoViews = deckEditTabContent.SelectionBonusBox.ItemsSource
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- null
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- selectionBonusInfoViews
    )

    deckEditTabContent.GirlListBox.SelectionChanged.Add(fun e ->
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
        let newSelectionBonusInfoViews = [||] |> ResizeArray
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.SelectionBonusInfoViews.RemoveAll(fun sbiv -> sbiv.CurrentSelectionBonusLevel = 0) |> ignore    

        let selectedItemsArray = Array.zeroCreate deckEditTabContent.GirlListBox.SelectedItems.Count
        deckEditTabContent.GirlListBox.SelectedItems.CopyTo(selectedItemsArray, 0)
        let selectedGirlNames = Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardView).CardName) <| selectedItemsArray
        let selectedCards = Array.distinctBy (fun (cardView: CardView) -> cardView.CardName) <| (Array.map (fun (selectedItem: obj) -> (selectedItem :?> CardView)) <| selectedItemsArray)

        for cardView in selectedCards do
            //let cardView = line :?> CardView
            if deckEditViewModel.FrontDeck.Exists(fun frontCard -> frontCard.CardName = cardView.CardName) 
                || deckEditViewModel.BackDeck.Exists(fun backCard -> backCard.CardName = cardView.CardName)
            then 
                // 入れようとしてるカードがすでにデッキ内に入っているときは
                // センボを追加しない
                0 |> ignore
            else
                for selectionBonus in cardView.Card.girl.selectionBonuses do
                    if deckEditViewModel.SelectionBonusInfoViews.Exists(fun sbiv -> sbiv.SelectionBonus = selectionBonus)
                    then
                        for listItem in deckEditTabContent.SelectionBonusBox.Items do
                            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
                            if selectionBonusInfoView.SelectionBonus.SelectionBonusName = selectionBonus.SelectionBonusName
                            then 
                                selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.AfterEditSelectionBonusLevel + 1
                                selectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                                selectionBonusInfoView.IsIntoDeck <- true
                            else 
                                0 |> ignore
                    else 
                        let newSelectionBonusInfoView = SelectionBonusInfoView(selectionBonus, 0, 5)
                        newSelectionBonusInfoView.AfterEditSelectionBonusLevel <- 1
                        newSelectionBonusInfoView.ConvertToEditSelectionBonusInfo()
                        newSelectionBonusInfoView.IsIntoDeck <- true
                        if newSelectionBonusInfoView.SelectionBonus.getSelectionBonusMode.IsAppliable(deckEditViewModel.SelectedMode)
                        then 
                            newSelectionBonusInfoViews.Add(newSelectionBonusInfoView)
                        else 0 |> ignore
        deckEditViewModel.SelectionBonusInfoViews.AddRange(newSelectionBonusInfoViews)
        deckEditTabContent.SelectionBonusBox.Items.Refresh()
        //let selectionBonusInfoViews = deckEditTabContent.SelectionBonusBox.ItemsSource
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- null
        //deckEditTabContent.SelectionBonusBox.ItemsSource <- selectionBonusInfoViews
    )

    deckEditTabContent.GirlListBox.LostFocus.Add(fun e->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.SelectionBonusInfoViews.RemoveAll(fun sbiv -> sbiv.CurrentSelectionBonusLevel = 0) |> ignore
        for listItem in deckEditTabContent.SelectionBonusBox.Items do
            let selectionBonusInfoView = listItem :?> SelectionBonusInfoView
            selectionBonusInfoView.AfterEditSelectionBonusLevel <- selectionBonusInfoView.CurrentSelectionBonusLevel
            selectionBonusInfoView.ConvertToOrdinarySelectionBonusInfo()
            selectionBonusInfoView.IsIntoDeck <- false
    )

    deckEditTabContent.EventTypeComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.loadDeck()
        deckEditViewModel.resetFilter()
        refreshDeckEditTab()
        deckEditViewModel.applyFilter()
    )
    deckEditTabContent.BirthdayGirlSettingComboBox.SelectionChanged.Add(fun e ->
        refreshDeckEditTab()
    )

    deckEditTabContent.ModeComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.loadDeck()
        refreshDeckEditTab()
    )
    // 副センバツ編集用イベントハンドラ
    deckEditTabContent.InBackDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
        let maxBackDeckNum = 
            if deckEditViewModel.SelectedEventType = EventType.Charisma 
            then 20
            else playerParameterViewModel.BackDeckNum
        
        if deckEditTabContent.GirlListBox.SelectedItems.Count + deckEditViewModel.BackDeck.Count + deckEditViewModel.SelectedPreciousSceneList.Count > maxBackDeckNum
        then MessageBox.Show("副センバツの枠数の上限を超えています") |> ignore
        else 
            for g in deckEditTabContent.GirlListBox.SelectedItems do
                (g :?> CardView).IsTriggeredSkillBonus <- false
                deckEditViewModel.BackDeck.Add(g :?> CardView)
                deckEditViewModel.CardListView.Remove(g :?> CardView) |> ignore
            refreshDeckEditTab()
            deckEditViewModel.save()
                
    )

    deckEditTabContent.OutBackDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        
        for g in deckEditTabContent.BackDeckList.SelectedItems do
            deckEditViewModel.CardListView.Add(g :?> CardView)
            deckEditViewModel.BackDeck.Remove(g :?> CardView) |> ignore

        deckEditViewModel.applyFilter()
        refreshDeckEditTab()
        deckEditViewModel.save()
    )

    // シーン編集用イベントハンドラ
    deckEditTabContent.SceneInBackDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        
        if deckEditTabContent.PreciousSceneListBox.SelectedItems.Count + deckEditViewModel.SelectedPreciousSceneList.Count > 3
            then MessageBox.Show("設定しようとしているプレシャスシーンが3つを超えています") |> ignore
            else 
                for scene in deckEditTabContent.PreciousSceneListBox.SelectedItems do
                    deckEditViewModel.SelectedPreciousSceneList.Add(scene :?> PreciousSceneView)
                    deckEditViewModel.PreciousSceneListView.Remove(scene :?> PreciousSceneView) |> ignore

                refreshDeckEditTab()
                deckEditViewModel.save()
                
    )

    deckEditTabContent.SceneOutBackDeckButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        
        for scene in deckEditTabContent.SelectedPreciousSceneListBox.SelectedItems do
            deckEditViewModel.PreciousSceneListView.Add(scene :?> PreciousSceneView)
            deckEditViewModel.SelectedPreciousSceneList.Remove(scene :?> PreciousSceneView) |> ignore
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))

        deckEditViewModel.save()
        refreshDeckEditTab()
    )

    deckEditTabContent.CalcDamageExpectationButton.Click.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel      
        deckEditViewModel.DamageExpectation <- calcDamageExpectation()
       
    )
    // ガール検索フィルタ用イベントハンドラ
    deckEditTabContent.AttributeFilterComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.resetFilter()
        deckEditViewModel.applyFilter()
        if deckEditViewModel.SelectedMode = Mode.Attack
        then 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        else 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
        
    )
    deckEditTabContent.CardTypeFilterComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.resetFilter()
        deckEditViewModel.applyFilter()
        if deckEditViewModel.SelectedMode = Mode.Attack
        then 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        else 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
    )
    deckEditTabContent.GirlFilterComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.resetFilter()
        deckEditViewModel.applyFilter()
        if deckEditViewModel.SelectedMode = Mode.Attack
        then 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        else 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
    )
    deckEditTabContent.RarityFilterComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.resetFilter()
        deckEditViewModel.applyFilter()
        if deckEditViewModel.SelectedMode = Mode.Attack
        then 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        else 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
    )
    deckEditTabContent.SelectionBonusFilterComboBox.SelectionChanged.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.resetFilter()
        deckEditViewModel.applyFilter()
        if deckEditViewModel.SelectedMode = Mode.Attack
        then 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Attack.CompareTo(x.Attack))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedAttack.CompareTo(x.CorrectedAttack))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        else 
            deckEditViewModel.BackDeck.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            if deckEditViewModel.SelectedEventType = EventType.CoolMega || deckEditViewModel.SelectedEventType = EventType.PopMega || deckEditViewModel.SelectedEventType = EventType.SweetMega
            then
                deckEditViewModel.CardListView.Sort(fun x y -> y.Defence.CompareTo(x.Defence))
            else 
                deckEditViewModel.CardListView.Sort(fun x y -> y.CorrectedDefence.CompareTo(x.CorrectedDefence))
            deckEditViewModel.PreciousSceneListView.Sort(fun x y -> y.Level.CompareTo(x.Level))
        
        indexing(deckEditViewModel.BackDeck)
        indexing(deckEditViewModel.CardListView)
        let GirlListBoxItemsSource = deckEditTabContent.GirlListBox.ItemsSource
        deckEditTabContent.GirlListBox.ItemsSource <- null
        deckEditTabContent.GirlListBox.ItemsSource <- GirlListBoxItemsSource
    )
    
    //loadでセーブデータがあれば読み込み
    //なければ初期状態から
    
    //window.DataContext <- vm
    let application = Application()
    application.Exit.Add(fun e ->
        let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
        deckEditViewModel.write()
    )
    
    application.Run(window)
    





