namespace GirlFriendDeckSimulator
open AttributeType
open Club
module Player = 
    type Player = { 
        PlayerName: string;
        AttributeType: AttributeType;
        ClubType: ClubType;
        AttackCost: int;
        CoolColon: float;
        PopColon: float; 
        SweetColon: float; 
        ExistWhiteboard: bool; 
        ExistTelevision: bool; 
        ExistLocker: bool;
        AssignedRole: Role;
        BackDeckNum: int
    }
    

