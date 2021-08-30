module RelayCommand

open System
open System.Windows.Input

type RelayCommand<'a>(execute : 'a -> unit, ?canExecute : 'a  ->  bool) =
  let event = Event<_,_>()
  interface ICommand with
    member this.CanExecute(param : obj) =
      if canExecute.IsSome then
        canExecute.Value(param :?> 'a)
      else true
    member this.Execute (param : obj) =
      execute(param :?> 'a)
    [< CLIEvent >]
    member this.CanExecuteChanged =
      event.Publish