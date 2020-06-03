namespace GirlFriendDeckSimulator
open Card
open CalcBonus
open System.Data
module CardView =
    //let createDataTableFromCards(cards: seq<Card>): DataTable =
    //    let table = DataTable()
    //    table.Columns.Add("CardName") |> ignore
    //    table.Columns.Add("Attack") |> ignore
    //    table.Columns.Add("Defence") |> ignore
    //    table.Columns.Add("CorrectedAttack") |> ignore
    //    table.Columns.Add("CorrectedDefence") |> ignore
    //    table.Columns.Add("Attribute") |> ignore
    //    for card in cards do 
    //        let row = table.NewRow()
    //        row.["CardName"] <- "[" + card.eventName + "]" + card.girl.name
    //        row.["Attack"] <- card.attack
    //        row.["Defence"] <- card.defence
    //        let correctedVal = calcBonus(card)
    //        row.["CorrectedAttack"] <- correctedVal.CorrectedAttack
    //        row.["CorrectedDefence"] <- correctedVal.CorrectedDefence
    //        row.["Attribute"] <- card.attribute
    //        table.Rows.Add(row)
    //    table

    type CardView (c: Card, p) =
        let correctedVals = calcBonus(c, p)
        member val CardName = 
            (match c.eventName with
            | None -> c.girl.name
            | Some(evName) -> "[" + evName + "]" + c.girl.name) with get, set
        member val Attack = c.attack with get, set
        member val Defence = c.defence with get, set
        member val Attribute = c.attribute with get, set
        member val CorrectedAttack = correctedVals.CorrectedAttack with get, set
        member val CorrectedDefence = correctedVals.CorrectedDefence with get, set
        member val IsFavoriteGirl = false with get, set
        
        new (c) = CardView(c, PlayerParameterViewModel())


