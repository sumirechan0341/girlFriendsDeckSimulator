namespace GirlFriendDeckSimulator
open System
module Club =
    type ClubType =
        | Committee = 0
        | SportsClub = 1
        | IndividualSportsClub = 2
        | GoHomeClub = 3
        | StudyClub = 4
        | CultureClub = 5
        | MusicClub = 6
        | JapaneseCultureClub = 7
        | NoClub = 8

    let getAllClubTypes = Enum.GetValues(typeof<ClubType>)

    type Facility = 
        | WhiteBoard = 'C'
        | Television = 'P'
        | Locker = 'S'

    type Role =
        | President = 0
        | VisePresident = 1
        | AttackCaptain = 2
        | DeffenceCaptain = 3
        | Member = 4

    let getAllClubRoleTypes = Enum.GetValues(typeof<Role>)

    