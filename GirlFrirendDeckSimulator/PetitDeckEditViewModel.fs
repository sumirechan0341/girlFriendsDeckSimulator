namespace GirlFriendDeckSimulator
open PetitGirlFactory
open PetitGirlView
open PetitGirl
open System.ComponentModel
open AttributeType
open FSharp.Data
open System.IO
open PetitGirlFactory
open SelectionBonus
type PetitDeckEditViewModel(deck1: PetitGirlView[], deck2: PetitGirlView[], deck3: PetitGirlView[]) =
    let ev = Event<_, _>()
    let mutable petitGirlList: ResizeArray<PetitGirlView> = Seq.map (PetitGirlView) (petitGirlList: seq<PetitGirl>) |> ResizeArray
    let mutable petitGirlDeck1: ResizeArray<PetitGirlView> = deck1 |> ResizeArray
    let mutable petitGirlDeck2: ResizeArray<PetitGirlView> = deck2 |> ResizeArray
    let mutable petitGirlDeck3: ResizeArray<PetitGirlView> = deck3 |> ResizeArray
    let mutable totalAttack = 0
    let mutable totalDefence = 0
    let mutable activationCheerEffects = [] |> ResizeArray
    let mutable activatedSelectionBonus: ResizeArray<SelectionBonus> = [||] |> ResizeArray
    let calculate =
        let calcDifference (originalValue: int) (bonusPercentage: float) = 
            (originalValue |> float) * bonusPercentage / 100.0 |> ceil |> int
        let mutable attack1 = 0
        let mutable defence1 = 0
        let mutable attack2 = 0
        let mutable defence2 = 0
        let mutable attack3 = 0
        let mutable defence3 = 0
        let mutable activationSkillEffects1: list<option<PetitSkillEffect>> = []
        let mutable activationSkillEffects2: list<option<PetitSkillEffect>> = []
        let mutable activationSkillEffects3: list<option<PetitSkillEffect>> = []
        let newActivationCheerEffects = [] |> ResizeArray
        let newActivatedSelectionBonus = [||] |> ResizeArray

        let applySkillEffectBonus(p: PetitGirl, activationSkillEffects) =
            let mutable attackBonusPercentage = 0.0
            let mutable defenceBonusPercentage = 0.0
            for skillEffect in activationSkillEffects do
                match skillEffect with
                | None -> 0 |> ignore
                | Some(skillEffect) -> 
                    match skillEffect.petitSkillType with
                    | AttributeSkillType(skillAttr, mode) ->
                        match skillAttr with
                        | SkillAttributeType.Cool -> 
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Cool
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Cool
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Cool
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.Pop ->
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Pop
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Pop
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Pop
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.Sweet ->
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Sweet
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Sweet
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Sweet
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.All ->
                            match mode with
                            | Mode.Attack ->
                                attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                            | Mode.Defence -> 
                                defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum 
                            | Mode.AttackAndDefence ->
                                attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
            {|attackBonusPercentage = attackBonusPercentage; defenceBonusPercentage = defenceBonusPercentage|}
        
        for i = 0 to petitGirlDeck1.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck1.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck1.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects1 <- petitGirlDeck1.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects1
                else 
                    0 |> ignore

        for p in petitGirlDeck1 do
            attack1 <- attack1 + p.petitGirl.attack
            defence1 <- defence1 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects1)
            attack1 <- attack1 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence1 <- defence1 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage

        for i = 0 to petitGirlDeck2.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck2.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck2.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects2 <- petitGirlDeck2.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects2
                else 
                    0 |> ignore

        for p in petitGirlDeck2 do
            attack2 <- attack2 + p.petitGirl.attack
            defence2 <- defence2 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects2)
            attack2 <- attack2 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence2 <- defence2 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage

        for i = 0 to petitGirlDeck3.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck3.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck3.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects3 <- petitGirlDeck3.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects3
                else 
                    0 |> ignore

        for p in petitGirlDeck3 do
            attack3 <- attack3 + p.petitGirl.attack
            defence3 <- defence3 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects3)
            attack3 <- attack3 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence3 <- defence3 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage
        
        {|
            totalAttack = attack1 + attack2 + attack3;
            totalDefence = defence1 + defence2 + defence3;
            activationCheerEffects = newActivationCheerEffects;
            activatedSelectionBonus = newActivatedSelectionBonus
        |}
        
    do
        for petitGirl in petitGirlDeck1 do
            petitGirlList.Remove(petitGirl) |> ignore
        for petitGirl in petitGirlDeck2 do
            petitGirlList.Remove(petitGirl) |> ignore
        for petitGirl in petitGirlDeck3 do
            petitGirlList.Remove(petitGirl) |> ignore
        let vals = calculate
        totalAttack <- vals.totalAttack
        totalDefence <- vals.totalDefence
        activationCheerEffects <- vals.activationCheerEffects
        activatedSelectionBonus <- vals.activatedSelectionBonus
        petitGirlList.Sort(fun x y -> if x.Attribute.CompareTo(y.Attribute) <> 0 then x.Attribute.CompareTo(y.Attribute) else y.Attack.CompareTo(x.Attack))
    new() = PetitDeckEditViewModel([||], [||], [||])
            
    member this.PetitGirlList 
        with get() = petitGirlList
        and set(newPetitGirlList) =
            petitGirlList <- newPetitGirlList
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PetitGirlDeck1
        with get() = petitGirlDeck1
        and set(newPetitGirlDeck) = 
            petitGirlDeck1 <- newPetitGirlDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PetitGirlDeck2
        with get() = petitGirlDeck2
        and set(newPetitGirlDeck) = 
            petitGirlDeck2 <- newPetitGirlDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))
    member this.PetitGirlDeck3
        with get() = petitGirlDeck3
        and set(newPetitGirlDeck) = 
            petitGirlDeck3 <- newPetitGirlDeck
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.TotalAttack
        with get() = totalAttack
        and set(newTotalAttack) =
            totalAttack <- newTotalAttack
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.TotalDefence
        with get() = totalDefence
        and set(newTotalDefence) =
            totalDefence <- newTotalDefence
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.ActivationCheerEffects
        with get() = activationCheerEffects
        and set(newActivationCheerEffect) =
            activationCheerEffects <- newActivationCheerEffect
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.ActivatedSelectionBonus
        with get() = activatedSelectionBonus
        and set(newActivatedSelectionBonus) =
            activatedSelectionBonus <- newActivatedSelectionBonus
            ev.Trigger(this, PropertyChangedEventArgs(""))

    member this.recalculate() =
        // setterを通さないと変更が通知されない
        let calcDifference (originalValue: int) (bonusPercentage: float) = 
            (originalValue |> float) * bonusPercentage / 100.0 |> ceil |> int
        let mutable attack1 = 0
        let mutable defence1 = 0
        let mutable attack2 = 0
        let mutable defence2 = 0
        let mutable attack3 = 0
        let mutable defence3 = 0
        let mutable activationSkillEffects1: list<option<PetitSkillEffect>> = []
        let mutable activationSkillEffects2: list<option<PetitSkillEffect>> = []
        let mutable activationSkillEffects3: list<option<PetitSkillEffect>> = []
        let newActivationCheerEffects = [] |> ResizeArray
        let newActivatedSelectionBonus = [||] |> ResizeArray
        let applySkillEffectBonus(p: PetitGirl, activationSkillEffects) =
            let mutable attackBonusPercentage = 0.0
            let mutable defenceBonusPercentage = 0.0
            for skillEffect in activationSkillEffects do
                match skillEffect with
                | None -> 0 |> ignore
                | Some(skillEffect) -> 
                    match skillEffect.petitSkillType with
                    | AttributeSkillType(skillAttr, mode) ->
                        match skillAttr with
                        | SkillAttributeType.Cool -> 
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Cool
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Cool
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Cool
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.Pop ->
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Pop
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Pop
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Pop
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.Sweet ->
                            match mode with
                            | Mode.Attack ->
                                if p.attribute = AttributeType.Sweet
                                    then attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.Defence -> 
                                if p.attribute = AttributeType.Sweet
                                    then defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                            | Mode.AttackAndDefence ->
                                if p.attribute = AttributeType.Sweet
                                    then 
                                        attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                        defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
                                    else 0 |> ignore
                        | SkillAttributeType.All ->
                            match mode with
                            | Mode.Attack ->
                                attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                            | Mode.Defence -> 
                                defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum 
                            | Mode.AttackAndDefence ->
                                attackBonusPercentage <- attackBonusPercentage + skillEffect.effectNum
                                defenceBonusPercentage <- defenceBonusPercentage + skillEffect.effectNum
            {|attackBonusPercentage = attackBonusPercentage; defenceBonusPercentage = defenceBonusPercentage|}
        
        for i = 0 to this.PetitGirlDeck1.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck1.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck1.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects1 <- petitGirlDeck1.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects1
                else 
                    0 |> ignore

        for p in this.PetitGirlDeck1 do
            attack1 <- attack1 + p.petitGirl.attack
            defence1 <- defence1 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects1)
            attack1 <- attack1 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence1 <- defence1 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage

        for i = 0 to this.PetitGirlDeck2.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck2.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck2.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects2 <- petitGirlDeck2.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects2
                else 
                    0 |> ignore

        for p in this.PetitGirlDeck2 do
            attack2 <- attack2 + p.petitGirl.attack
            defence2 <- defence2 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects2)
            attack2 <- attack2 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence2 <- defence2 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage

        for i = 0 to this.PetitGirlDeck3.Count-1 do
            if i = 0 
                then 
                    newActivationCheerEffects.AddRange(petitGirlDeck3.Item(i).petitGirl.petitCheerEffects)
                    newActivatedSelectionBonus.AddRange(petitGirlDeck3.[i].petitGirl.selectionBonus)
                else 
                    0 |> ignore
            if i < 4 
                then 
                    activationSkillEffects3 <- petitGirlDeck3.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects3
                else 
                    0 |> ignore

        for p in this.PetitGirlDeck3 do
            attack3 <- attack3 + p.petitGirl.attack
            defence3 <- defence3 + p.petitGirl.defence
            let bonus = applySkillEffectBonus(p.petitGirl, activationSkillEffects3)
            attack3 <- attack3 + calcDifference p.petitGirl.attack bonus.attackBonusPercentage
            defence3 <- defence3 + calcDifference p.petitGirl.defence bonus.defenceBonusPercentage
        
        this.TotalAttack <- attack1 + attack2 + attack3
        this.TotalDefence <- defence1 + defence2 + defence3
        this.ActivationCheerEffects <- newActivationCheerEffects
        this.ActivatedSelectionBonus <- newActivatedSelectionBonus
        //for i = 0 to petitGirlDeck1.Count-1 do
        //    let key = petitGirlDeck1.Item(i).petitGirl.attribute
        //    deck1Attack <- deck1Attack.Add(key, deck1Attack.Item(key) + petitGirlDeck1.Item(i).petitGirl.attack)
        //    deck1Defence <- deck1Defence.Add(key, deck1Defence.Item(key) + petitGirlDeck1.Item(i).petitGirl.defence)
        //    if i = 0 
        //        then 
        //            activationCheerEffects.AddRange(petitGirlDeck1.Item(i).petitGirl.petitCheerEffects)
        //        else 
        //            0 |> ignore
        //    if i < 4 
        //        then 
        //            activationSkillEffects1 <- petitGirlDeck1.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects1
        //        else 
        //            0 |> ignore
        
        //for skillEffect in activationSkillEffects1 do
        //    match skillEffect with
        //    | None -> 0 |> ignore
        //    | Some(skillEffect) -> 
        //        match skillEffect.petitSkillType with
        //        | AttributeSkillType(skillAttr, mode) ->
        //            match skillAttr with
        //            | SkillAttributeType.Cool -> 
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //            | SkillAttributeType.Pop ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //            | SkillAttributeType.Sweet ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //            | SkillAttributeType.All ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack1 <- bonusAttack1 + calcDifference (deck1Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence1 <- bonusDefence1 + calcDifference (deck1Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //System.Console.WriteLine(bonusDefence1)
        //for i = 0 to petitGirlDeck2.Count-1 do
        //    let key = petitGirlDeck2.Item(i).petitGirl.attribute
        //    deck2Attack <- deck2Attack.Add(key, deck2Attack.Item(key) + petitGirlDeck2.Item(i).petitGirl.attack)
        //    deck2Defence <- deck2Defence.Add(key, deck2Defence.Item(key) + petitGirlDeck2.Item(i).petitGirl.defence)
        //    if i = 0 
        //        then 
        //            activationSkillEffects2 <- petitGirlDeck2.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects2
        //            activationCheerEffects.AddRange(petitGirlDeck2.Item(i).petitGirl.petitCheerEffects)
        //        else 
        //            0 |> ignore
        //    if 0 < i && i < 4 
        //        then 
        //            activationSkillEffects2 <- petitGirlDeck2.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects2
        //        else 
        //            0 |> ignore

        //for skillEffect in activationSkillEffects2 do
        //    match skillEffect with
        //    | None -> 0 |> ignore
        //    | Some(skillEffect) -> 
        //        match skillEffect.petitSkillType with
        //        | AttributeSkillType(skillAttr, mode) ->
        //            match skillAttr with
        //            | SkillAttributeType.Cool -> 
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //            | SkillAttributeType.Pop ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //            | SkillAttributeType.Sweet ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //            | SkillAttributeType.All ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack2 <- bonusAttack2 + calcDifference (deck2Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence2 <- bonusDefence2 + calcDifference (deck2Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        

        //for i = 0 to petitGirlDeck3.Count-1 do
        //    let key = petitGirlDeck3.Item(i).petitGirl.attribute
        //    deck3Attack <- deck3Attack.Add(key, deck3Attack.Item(key) + petitGirlDeck3.Item(i).petitGirl.attack)
        //    deck3Defence <- deck3Defence.Add(key, deck3Defence.Item(key) + petitGirlDeck3.Item(i).petitGirl.defence)
        //    if i = 0 
        //        then 
        //            activationSkillEffects3 <- petitGirlDeck3.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects3
        //            activationCheerEffects.AddRange(petitGirlDeck3.Item(i).petitGirl.petitCheerEffects)
        //        else 
        //            0 |> ignore
        //    if 0 < i && i < 4 
        //        then 
        //            activationSkillEffects3 <- petitGirlDeck3.Item(i).petitGirl.petitSkillEffect :: activationSkillEffects3
        //        else 
        //            0 |> ignore

        //for skillEffect in activationSkillEffects3 do
        //    match skillEffect with
        //    | None -> 0 |> ignore
        //    | Some(skillEffect) -> 
        //        match skillEffect.petitSkillType with
        //        | AttributeSkillType(skillAttr, mode) ->
        //            match skillAttr with
        //            | SkillAttributeType.Cool -> 
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //            | SkillAttributeType.Pop ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //            | SkillAttributeType.Sweet ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //            | SkillAttributeType.All ->
        //                match mode with
        //                | Mode.Attack -> 
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.Defence -> 
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                | Mode.AttackAndDefence ->
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusAttack3 <- bonusAttack3 + calcDifference (deck3Attack.Item(AttributeType.Sweet)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Cool)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Pop)) skillEffect.effectNum
        //                    bonusDefence3 <- bonusDefence3 + calcDifference (deck3Defence.Item(AttributeType.Sweet)) skillEffect.effectNum

        
        //this.TotalAttack <- 
        //    deck1Attack.Item(AttributeType.Cool) + 
        //    deck1Attack.Item(AttributeType.Pop) + 
        //    deck1Attack.Item(AttributeType.Sweet) + 
        //    bonusAttack1 +
        //    deck2Attack.Item(AttributeType.Cool) + 
        //    deck2Attack.Item(AttributeType.Pop) + 
        //    deck2Attack.Item(AttributeType.Sweet) + 
        //    bonusAttack2 +
        //    deck3Attack.Item(AttributeType.Cool) + 
        //    deck3Attack.Item(AttributeType.Pop) + 
        //    deck3Attack.Item(AttributeType.Sweet) + 
        //    bonusAttack3

        //this.TotalDefence <- 
        //    deck1Defence.Item(AttributeType.Cool) + 
        //    deck1Defence.Item(AttributeType.Pop) + 
        //    deck1Defence.Item(AttributeType.Sweet) + 
        //    bonusDefence1 +
        //    deck2Defence.Item(AttributeType.Cool) + 
        //    deck2Defence.Item(AttributeType.Pop) + 
        //    deck2Defence.Item(AttributeType.Sweet) + 
        //    bonusDefence2 +
        //    deck3Defence.Item(AttributeType.Cool) + 
        //    deck3Defence.Item(AttributeType.Pop) + 
        //    deck3Defence.Item(AttributeType.Sweet) + 
        //    bonusDefence3

    member this.save =
        let jsonData = JsonValue.Record[|
            "petitDeck1", JsonValue.Array <| Array.map(fun (petitGilrView: PetitGirlView) -> petitGilrView.petitGirl.petitGirlJson) (petitGirlDeck1.ToArray());
            "petitDeck2", JsonValue.Array <| Array.map(fun (petitGilrView: PetitGirlView) -> petitGilrView.petitGirl.petitGirlJson) (petitGirlDeck2.ToArray());
            "petitDeck3", JsonValue.Array <| Array.map(fun (petitGilrView: PetitGirlView) -> petitGilrView.petitGirl.petitGirlJson) (petitGirlDeck3.ToArray())
        |]
        let writer = File.CreateText(".\PetitDeck.json")
        jsonData.WriteTo(writer, JsonSaveOptions.None)
        writer.Close()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged = ev.Publish
module PetitDeckEditViewModel =
    type PetitDeckElements = JsonProvider<""" 
        {"petitDeck1": [],
         "petitDeck2": [],
         "petitDeck3": []} """>
    let loadPetitDeckEditViewModel =
        if File.Exists(".\PetitDeck.json")
            then
                let petitDeckElements = PetitDeckElements.Parse(File.ReadAllText(".\PetitDeck.json"))
                let deck1 =
                    Array.map(fun (petitGirl: Runtime.BaseTypes.IJsonDocument) -> petitGirlFactoryFromJson(petitGirl.JsonValue.ToString()) |> PetitGirlView) petitDeckElements.PetitDeck1
                let deck2 =
                    Array.map(fun (petitGirl: Runtime.BaseTypes.IJsonDocument) -> petitGirlFactoryFromJson(petitGirl.JsonValue.ToString()) |> PetitGirlView) petitDeckElements.PetitDeck2
                let deck3 =
                    Array.map(fun (petitGirl: Runtime.BaseTypes.IJsonDocument) -> petitGirlFactoryFromJson(petitGirl.JsonValue.ToString()) |> PetitGirlView) petitDeckElements.PetitDeck3
                PetitDeckEditViewModel(deck1, deck2, deck3) |> Some
            else None