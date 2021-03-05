namespace Backtester

open System
open FSharp.Json

type Position = { Symbol: string; Shares: int; SharePrice: float }

type Frame = 
    { 
        FreeCash: float; 
        Portfolio: Map<string, int>; 
        Date: DateTime 
    }

type Operation = | Buy of Position | Sell of Position

type Transaction = { OpType: Operation; Value: float; Date: DateTime}

type Candle = 
    { 
        [<JsonField("open")>]
        Open: float; 
        [<JsonField("high")>]
        High: float; 
        [<JsonField("low")>]
        Low: float; 
        [<JsonField("close")>]
        Close: float; 
        [<JsonField("volume")>]
        Volume: int; 
        [<JsonField("datetime")>]
        DateTime: int64 
    }


    

