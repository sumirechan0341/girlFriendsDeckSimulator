namespace GirlFriendDeckSimulator
open System.Windows.Controls
open System
open System.Globalization
   
type ColonFormRule() =
    inherit ValidationRule()
    member val MinVal = 0.0
    member val MaxVal = 25.0
    override this.Validate(value: Object, cultureInfo: CultureInfo): ValidationResult = 
        try 
            if (string value).Length > 0
            then 
                let colonValue = float(string value)
                if colonValue < this.MinVal || colonValue > this.MaxVal
                    then ValidationResult(false, "有効な数字ではありません。0~25までの値を指定してください")
                    else ValidationResult.ValidResult
            else 
                ValidationResult(false, "数値を入力してください")
        with 
        | (e: Exception) -> ValidationResult(false, "数値を入力してください")
                
            