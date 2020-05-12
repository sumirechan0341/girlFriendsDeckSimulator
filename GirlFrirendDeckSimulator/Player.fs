namespace GirlFriendDeckSimulator
open AttributeType
open Club
module Player = 
    type Player = { 
        PlayerName: string;
        AttrType: AttributeType;
        ClubType: ClubType;
        AttackCost: int;
        CoolColon: float;
        PopColon: float; 
        SweetColon: float; 
        ExistWhiteBoard: bool; 
        ExistTelevision: bool; 
        ExistLocker: bool;
        AssignedRole: Role
    }
    

