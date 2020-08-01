namespace GirlFriendDeckSimulator
open Converter

module EventTypeConverter =
    let toString(ev) =
        match ev with
        | EventType.Raid -> "レイド"
        | EventType.CoolTrio -> "COOLトリオ"
        | EventType.PopTrio -> "POPトリオ"
        | EventType.SweetTrio -> "SWEETトリオ"
        | EventType.Hunters -> "ハンターズ"
        | EventType.CoolMega -> "COOLメガ"
        | EventType.PopMega -> "POPメガ"
        | EventType.SweetMega -> "SWEETメガ"
        | EventType.MemorialStory -> "メモリアルストーリ―"
        | EventType.Charisma -> "カリスマ"
        | EventType.Battle -> "通常バトル"

    let fromString(evStr) =
        match evStr with
        | "レイド" -> EventType.Raid
        | "COOLトリオ" -> EventType.CoolTrio
        | "POPトリオ" -> EventType.PopTrio
        | "SWEETトリオ" -> EventType.SweetTrio
        | "ハンターズ" -> EventType.Hunters
        | "COOLメガ" -> EventType.CoolMega
        | "POPメガ" -> EventType.PopMega
        | "SWEETメガ" -> EventType.SweetMega
        | "メモリアルストーリ―" -> EventType.MemorialStory
        | "カリスマ" -> EventType.Charisma
        | "通常バトル" -> EventType.Battle

type EventTypeConverter() =
    inherit ConverterBase(EventTypeConverter.toString >> (fun s -> s :> obj) |> convert, EventTypeConverter.fromString >> (fun e -> e :> obj) |> convert)