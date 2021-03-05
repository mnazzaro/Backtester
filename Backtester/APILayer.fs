namespace Backtester

open System
open System.Net.Http
open FSharp.Json

module APILayer = 

    let client = new HttpClient()

    type QueryParams = 
        {
            api_key: string;
            period_type: string;
            frequency_type: string;
            frequency: string;
            start_date: string;
            end_date: string;
            need_extended_hours: string
        }

    type HistoricalData = 
        { 
            [<JsonField("candles")>]
            Candles: Candle list; 
            [<JsonField("symbol")>]
            Symbol: string; 
            [<JsonField("empty")>]
            Empty: bool
        }

        
    let QueryParamsToString qp =
        $"apikey={qp.api_key}&periodType={qp.period_type}&frequencyType={qp.frequency_type}&frequency={qp.frequency}&startDate={qp.start_date}&endDate={qp.end_date}&needExtendedHoursData={qp.need_extended_hours}"

    let GetInstrumentHistoricalData symbol start_date end_date = 
        let uri_builder = new UriBuilder($"https://api.tdameritrade.com/v1/marketdata/%s{symbol}/pricehistory")
        let qp = 
            {
                api_key = "VNYD9APYA0VKPQWBJP7UV3TCGWGGPBUE";
                period_type = "year";
                frequency_type = "daily";
                frequency = "1";
                start_date = (string (DateUtil.ToTDDateTime start_date));
                end_date = (string (DateUtil.ToTDDateTime end_date));
                need_extended_hours = "false";
            }
        uri_builder.Query <- QueryParamsToString qp

        let res = async {
            let! response = client.GetAsync uri_builder.Uri |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore 
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return content
        }

        res |> Async.RunSynchronously |> Json.deserialize<HistoricalData>


        

