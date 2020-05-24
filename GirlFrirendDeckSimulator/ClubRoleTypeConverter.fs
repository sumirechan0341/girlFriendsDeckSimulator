namespace GirlFriendDeckSimulator
open Club
open Converter

module ClubRoleTypeConverter =
    let toString(clubRoleType: Role): string =
        match clubRoleType with
        | Role.President -> "部長"
        | Role.VisePresident -> "副部長"
        | Role.AttackCaptain -> "攻キャプテン"
        | Role.DefenceCaptain -> "守キャプテン"
        | Role.Member -> "役職なし"

    let fromString(clubRoleTypeName: string): Role = 
        match clubRoleTypeName with
        | "部長" -> Role.President
        | "副部長" -> Role.VisePresident
        | "攻キャプテン" -> Role.AttackCaptain
        | "守キャプテン" -> Role.DefenceCaptain
        | "役職なし" -> Role.Member

type ClubRoleTypeConverter() =
    inherit ConverterBase(ClubRoleTypeConverter.toString >> (fun s -> s :> obj) |> convert, ClubRoleTypeConverter.fromString >> (fun a -> a :> obj) |> convert)

