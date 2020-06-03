namespace GirlFriendDeckSimulator
open Converter
open Club
module ClubTypeConverter =
    let toString clubType: string =
        match clubType with
        | ClubType.Committee -> "委員会&団体"
        | ClubType.SportsClub -> "運動部"
        | ClubType.IndividualSportsClub -> "運動部(個人競技)"
        | ClubType.GoHomeClub -> "帰宅部"
        | ClubType.StudyClub -> "研究会"
        | ClubType.CultureClub -> "文化部"
        | ClubType.MusicClub -> "文化部(音楽系)"
        | ClubType.JapaneseCultureClub -> "文化部(日本)"
        | ClubType.NoClub -> "未所属"

    let fromString(clubTypeName): ClubType = 
        match clubTypeName with
        | "委員会&団体" -> ClubType.Committee
        | "運動部" -> ClubType.SportsClub
        | "運動部(個人競技)" -> ClubType.IndividualSportsClub
        | "帰宅部" -> ClubType.GoHomeClub
        | "研究会" -> ClubType.StudyClub
        | "文化部" -> ClubType.CultureClub
        | "文化部(音楽系)" -> ClubType.MusicClub
        | "文化部(日本)" -> ClubType.JapaneseCultureClub
        | "未所属" -> ClubType.NoClub

module FacilityConverter =
    let toString facility: string =
        match facility with
        | Facility.WhiteBoard -> "ホワイトボード"
        | Facility.Television -> "テレビ"
        | Facility.Locker -> "ロッカー"

    let fromString(facilityStr): Facility = 
        match facilityStr with
        | "ホワイトボード" | "ﾎﾜｲﾄﾎﾞｰﾄﾞ" -> Facility.WhiteBoard
        | "テレビ" | "ﾃﾚﾋﾞ" -> Facility.Television
        | "ロッカー" | "ﾛｯｶｰ" -> Facility.Locker


type ClubTypeConverter() =
    inherit ConverterBase(ClubTypeConverter.toString >> (fun s -> s :> obj) |> convert, ClubTypeConverter.fromString >> (fun c -> c :> obj) |> convert)

