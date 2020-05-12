namespace GirlFriendDeckSimulator
open AttributeType
open Club
open System
open SelectionBonus
open Grade
module Girl =
    type Girl = {
        name: string
        club: ClubType
        grade: Grade
        classRoom: option<string> // 〇年〇組
        selectionBonuses: SelectionBonus[]
        birthDay: DateTime
    }
    