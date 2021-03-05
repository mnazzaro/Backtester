namespace Backtester

open System

module PortfolioOperations = 

    exception SellError of string
    exception BuyError of string

    let buy frame symbol shares share_price = 
        
        if frame.FreeCash >= (float shares) * share_price then
            let ret_free_cash = frame.FreeCash - ((float shares) * share_price)
            if Map.containsKey symbol frame.Portfolio then
                let ret_map = Map.add symbol (frame.Portfolio.[symbol] + shares) frame.Portfolio
                { frame with FreeCash = ret_free_cash; Portfolio = ret_map }
            else 
                let ret_map = Map.add symbol shares frame.Portfolio
                { frame with FreeCash = ret_free_cash; Portfolio = ret_map }
        else raise (BuyError($"You don't have enough capital to buy %d{shares} shares of %s{symbol}"))
        
    let sell frame symbol shares share_price=

        let ret_map = match frame.Portfolio.TryFind symbol with
                      | Some value -> match (value - shares) with 
                                      | 0 -> frame.Portfolio.Remove symbol
                                      | x when x > 0 -> frame.Portfolio.Add (symbol, x)
                                      | _ -> raise (SellError($"Not enough shares of %s{symbol}"))
                      | None -> raise (SellError($"You don't have %s{symbol} in your portfolio"))
        let ret_free_cash = frame.FreeCash + ((float shares) * share_price)
        { frame with FreeCash = ret_free_cash; Portfolio = ret_map}


    let calculateFrameTotal frame = 
        frame.Portfolio
        |> Map.toList
        |> List.map (fun (symbol, shares) -> ((APILayer.GetInstrumentHistoricalData symbol frame.Date frame.Date), shares))
        |> List.map (fun (data, shares) -> data.Candles.Head.Close * (float shares))
        |> List.fold (fun x y -> x + y) frame.FreeCash 

    let rec calculateTransactionList transactions frame = 
        match transactions with 
        | [] -> frame
        | h::t -> ((function | Buy position -> buy frame position.Symbol position.Shares position.SharePrice 
                             | Sell position -> sell frame position.Symbol position.Shares position.SharePrice) h) |> calculateTransactionList t        
