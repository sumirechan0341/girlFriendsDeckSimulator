namespace GirlFriendDeckSimulator
open Club
open System.Windows.Data
open System
[<AutoOpen>]
module Converter =
    let nullFunction = fun value _ _ _ -> value
    let convert<'T> f (obj:System.Object) (t:Type) (para:System.Object) (culture:Globalization.CultureInfo)  = (obj :?> 'T) |> f |> box
    [<AbstractClass>]
    type ConverterBase(convertFunction, convertBackFunction) =    
        /// constructor take nullFunction as inputs
        new() = ConverterBase(nullFunction, nullFunction)
    
        // implement the IValueConverter
        interface IValueConverter with
            /// convert a value to new value
            override this.Convert(value, targetType, parameter, culture) =
                this.Convert value targetType parameter culture
    
            /// convert a value back
            override this.ConvertBack(value, targetType, parameter, culture) =
                this.ConvertBack value targetType parameter culture
        
        abstract member Convert : (obj -> Type -> obj -> Globalization.CultureInfo -> obj)
        default this.Convert = convertFunction
    
        abstract member ConvertBack : (obj -> Type -> obj -> Globalization.CultureInfo -> obj)
        default this.ConvertBack = convertBackFunction
        