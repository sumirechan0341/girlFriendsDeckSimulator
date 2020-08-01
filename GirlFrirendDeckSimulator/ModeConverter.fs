namespace GirlFriendDeckSimulator
open Mode
module ModeConverter =
    let toString(mode) =
        match mode with
        | Attack -> "攻援"
        | Defence -> "守援"
        | AttackAndDefence -> "攻守"

    let fromString(modeStr) =
        match modeStr with
        | "攻援" -> Attack
        | "守援" -> Defence
        | "攻守" -> AttackAndDefence

type ModeConverter() =
    inherit ConverterBase(ModeConverter.toString >> (fun s -> s :> obj) |> convert, ModeConverter.fromString >> (fun m -> m :> obj) |> convert)