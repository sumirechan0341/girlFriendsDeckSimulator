namespace GirlFriendDeckSimulator
open CalcBonus
open System.Windows
open System

type MainWindowViewModel() =
    member _.OnTargetChanged(sender, args) =
        MessageBox.Show("event fire")