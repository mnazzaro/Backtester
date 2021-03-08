namespace Backtester

open System

module PortfolioOperations = 

    exception SellError of string
    exception BuyError of string

    let buy frame transaction = 
        
        let { Symbol=symbol; Shares=shares;  SharePrice=share_price } = (function | Buy position | Sell position -> position) transaction.OpType
        if frame.FreeCash >= (float shares) * share_price then
            let ret_free_cash = frame.FreeCash - ((float shares) * share_price)
            if Map.containsKey symbol frame.Portfolio then
                let ret_map = Map.add symbol (frame.Portfolio.[symbol] + shares) frame.Portfolio
                { FreeCash = ret_free_cash; Portfolio = ret_map; Date = transaction.Date }
            else 
                let ret_map = Map.add symbol shares frame.Portfolio
                { FreeCash = ret_free_cash; Portfolio = ret_map; Date = transaction.Date }
        else raise (BuyError($"You don't have enough capital to buy %d{shares} shares of %s{symbol}"))
        
    let sell frame transaction =
        
        let { Symbol=symbol; Shares=shares;  SharePrice=share_price } = (function | Buy position | Sell position -> position) transaction.OpType
        let ret_map = match frame.Portfolio.TryFind symbol with
                      | Some value -> match (value - shares) with 
                                      | 0 -> frame.Portfolio.Remove symbol
                                      | x when x > 0 -> frame.Portfolio.Add (symbol, x)
                                      | _ -> raise (SellError($"Not enough shares of %s{symbol}"))
                      | None -> raise (SellError($"You don't have %s{symbol} in your portfolio"))
        let ret_free_cash = frame.FreeCash + ((float shares) * share_price)
        { FreeCash = ret_free_cash; Portfolio = ret_map; Date=transaction.Date}


    let calculateFrameTotal frame = 
        frame.Portfolio
        |> Map.toList
        |> List.map (fun (symbol, shares) -> ((APILayer.GetInstrumentHistoricalData symbol frame.Date frame.Date), shares))
        |> List.map (fun (data, shares) -> data.Candles.Head.Close * (float shares))
        |> List.fold (fun x y -> x + y) frame.FreeCash 

    let rec calculateTransactionList transactions frame = 
        match transactions with 
        | [] -> frame
        | h::t -> ((function | Buy position -> buy frame h 
                             | Sell position -> sell frame h) h.OpType) |> calculateTransactionList t        
