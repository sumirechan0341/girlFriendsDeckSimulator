namespace GirlFriendDeckSimulator
open AttributeType
open Club
open System.ComponentModel
open Player
open System.Runtime.CompilerServices

type PlayerViewModel() =
    member val ClubTypes = getAllClubTypes
    member val AttributeTypes = getAllAttributeTypes
    member val ClubRoleTypes = getAllClubRoleTypes

    member val PlayerName = "User1" with get, set
    member val CoolColon = 0.0 with get, set
    member val PopColon = 0.0 with get, set    
    member val SweetColon = 0.0 with get, set
    member val ExistWhiteBoard = true with get, set
    member val ExistTelevision = true with get, set
    member val ExistLocker = true with get, set
    member val SelectedClubType = ClubType.NoClub with get, set
    member val SelectedAttributeType = AttributeType.Cool with get, set
    member val SelectedClubRoleType = Role.Member with get, set
    member val AttackCost = 20 with get, set
    member val BackDeckNum = 52 with get, set