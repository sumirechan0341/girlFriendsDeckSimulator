open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup
open FsXaml
open System.ComponentModel
open System.Diagnostics
open GirlFriendDeckSimulator.CalcBonus
open GirlFriendDeckSimulator
open GirlFactory
open CardFactory
open CardView
open System.Data
open PetitGirlView
open Microsoft.FSharp.Collections

 type PlayerParameterTab = XAML<"PlayerParameterTab.xaml">
 type DeckEditTab = XAML<"DeckEditTab.xaml">
 type MainWindow = XAML<"MainWindow.xaml">
 type PetitDeckEditTab = XAML<"PetitDeckEditTab.xaml">
 

 let updateBonusRow(e) = 
     //(deckEditTab.DataContext :?> DeckEditViewModel).BirhtdaySettingGirl <- "上条るい"
     //deckEditTab.BirthdayGirlSettingComboBox.SelectedValue <- "上条るい"
     0 |> ignore


[<STAThread>]
[<EntryPoint>]
let main argv =
    //initializeでフォーム生成
    let window = MainWindow()
    let playerParameterTabContent = PlayerParameterTab()
    let deckEditTabContent = DeckEditTab()
    let petitDeckEditTabContent = PetitDeckEditTab()
    

    let playerParameterTab = TabItem()
    playerParameterTab.Header <- "プレイヤー情報"
    playerParameterTab.Content <- playerParameterTabContent

    let deckEditTab = TabItem()
    deckEditTab.Header <- "デッキ編集"
    deckEditTab.Content <- deckEditTabContent

    let petitDeckEditTab = TabItem()
    petitDeckEditTab.Header <- "ぷちデッキ編集"
    petitDeckEditTab.Content <- petitDeckEditTabContent

    window.TabControl.Items.Add(playerParameterTab) |> ignore
    window.TabControl.Items.Add(deckEditTab) |> ignore
    window.TabControl.Items.Add(petitDeckEditTab) |> ignore

    window.TabControl.SelectionChanged.Add(fun e ->
        if(window.TabControl.SelectedIndex = 1 && e.OriginalSource :? TabControl) 
            then
                //let table = (deckEditTabContent.GirlListBox.ItemsSource :?> DataView).Table
                //let CorretedVals = Seq.map calcBonus cardList
                //for i = 1 to table.Rows.Count - 1 do
                //    table.Rows.Item(i).["CorrectedAttack"] <- (Seq.item i CorretedVals).CorrectedAttack
                //    table.Rows.Item(i).["CorrectedDefence"] <- (Seq.item i CorretedVals).CorrectedDefence
                //deckEditTabContent.GirlListBox.ItemsSource <- table.DefaultView
                //deckEditTabContent.GirlListBox.Items.Refresh()
                let playerParameterViewModel = playerParameterTabContent.DataContext :?> PlayerParameterViewModel
                deckEditTabContent.GirlListBox.ItemsSource <- null
                deckEditTabContent.GirlListBox.ItemsSource <- Seq.map(fun c-> CardView(c, playerParameterViewModel)) cardList |> ResizeArray
                deckEditTabContent.GirlListBox.Items.Refresh()
            else 0 |> ignore
    )

    petitDeckEditTabContent.InPetitDeck1Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        if petitDeckEditTabContent.PetitGirlListBox.SelectedItems.Count + petitDeckViewModel.PetitGirlDeck1.Count > 10 
            then 
                MessageBox.Show("設定しようとしているぷちが10人を超えています") |> ignore
            else 
                for p in petitDeckEditTabContent.PetitGirlListBox.SelectedItems do
                    petitDeckViewModel.PetitGirlDeck1.Add(p :?> PetitGirlView)
                    petitDeckViewModel.PetitGirlList.Remove(p :?> PetitGirlView) |> ignore
                petitDeckViewModel.recalculate
                petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
                petitDeckEditTabContent.PetitGirlDeck1.Items.Refresh()
    )
    petitDeckEditTabContent.OutPetitDeck1Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck1.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck1.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        petitDeckViewModel.recalculate
        petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
        petitDeckEditTabContent.PetitGirlDeck1.Items.Refresh()
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
            petitDeckViewModel.recalculate
            petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
            petitDeckEditTabContent.PetitGirlDeck2.Items.Refresh()
    )
    petitDeckEditTabContent.OutPetitDeck2Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck2.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck2.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        petitDeckViewModel.recalculate
        petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
        petitDeckEditTabContent.PetitGirlDeck2.Items.Refresh()
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
            petitDeckViewModel.recalculate
            petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
            petitDeckEditTabContent.PetitGirlDeck3.Items.Refresh()
    )
    petitDeckEditTabContent.OutPetitDeck3Button.Click.Add(fun e ->
        let petitDeckViewModel = petitDeckEditTabContent.DataContext :?> PetitDeckEditViewModel
        for p in petitDeckEditTabContent.PetitGirlDeck3.SelectedItems do
            petitDeckViewModel.PetitGirlList.Add(p :?> PetitGirlView)
            petitDeckViewModel.PetitGirlDeck3.Remove(p :?> PetitGirlView) |> ignore
        petitDeckViewModel.PetitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
        petitDeckViewModel.recalculate
        petitDeckEditTabContent.PetitGirlListBox.Items.Refresh()
        petitDeckEditTabContent.PetitGirlDeck3.Items.Refresh()
    )
    
    
    //playerParameterTabContent.BirthdayGirlSettingComboBox.SelectionChanged.Add(fun e ->
    //    let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
    //    deckEditTabContent.GirlListBox.ItemsSource <- Seq.map(fun card -> (CardView(card, deckEditViewModel.BirhtdaySettingGirl))) cardList
    //    deckEditTabContent.GirlListBox.Items.Refresh()
    //)
    //let OnTargetUpdated(sender, args) =
    //    deckEditTabContent.GirlListBox.Items.Refresh
    
    //window.ContentRendered.Add(fun e ->
    //    let deckEditViewModel = deckEditTabContent.DataContext :?> DeckEditViewModel
    //    let birthdayGirl = deckEditViewModel.BirhtdaySettingGirl
    //    let correctedVals = Seq.map (fun c -> calcBonus(c, birthdayGirl) CardFactory.cardList) 
    //    let correctedAttack = Seq.map (fun vs -> vs.CorrectedAttack) correctedVals
    //    let correctedDefence = Seq.map (fun vs -> vs.CorrectedDefence) correctedVals
        
    //    deckEditTabContent.GirlListBox.Items.Add()
    //)
    //let window2 = DeckEditWindow()
    //loadでセーブデータがあれば読み込み
    //なければ初期状態から
    
    //window.DataContext <- vm
    let application = Application()
    
    application.Run(window)





