namespace Backtester

open System

module Main = 

    [<EntryPoint>]
    let main argv =
        let portfolio = ["GME", 107; "MSFT", 30; "GOOG", 2] |> Map.ofList
        let f = {FreeCash=13.5; Portfolio=portfolio; Date=(new DateTime (2021, 1, 27))}
        printf "%f" (PortfolioOperations.calculateFrameTotal f)
        0
