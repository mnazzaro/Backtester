namespace Backtester

open System

module DateUtil =

    let epoch_date = new DateTime(1969, 12, 31, 0, 0, 0)

    let ToTDDateTime (date: DateTime) = 
        let delta = date.Subtract epoch_date
        int64 delta.TotalMilliseconds
        



        

