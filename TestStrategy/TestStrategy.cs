// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using TradingPlatform.BusinessLayer;

namespace TestStrategy
{
    /// <summary>
    /// An example of strategy for working with one symbol. Add your code, compile it and run via Strategy Runner panel in the assigned trading terminal.
    /// Information about API you can find here: http://api.quantower.com
    /// </summary>
	public class TestStrategy : Strategy
    {

        int orderQuantity = 1;
        private DateTime lastProcessedBarTime = DateTime.MinValue;

        TimeframeSetting timeframeSetting = new();

        Timeframe timeframe;

        HistoricalData botData;

        bool isRunning = false;
        private readonly object syncLock = new object();

        Indicator fastEMA;
        Indicator slowEMA;


        [InputParameter("Symbol", 10)]
        private Symbol symbol;

        [InputParameter("Account", 20)]
        public Account account;

        [InputParameter("Start Point", 30)]
        public DateTime StartPoint;

        [InputParameter("Profit Target Ticks", 40, 1, Int32.MaxValue)]
        public int profitTargetTicks = 10;

        [InputParameter("Stop Loss Ticks", 50, 1, Int32.MaxValue)]
        public int stopLossTicks = 10;

        [InputParameter("fast ema period", 60, 1, Int32.MaxValue)]
        public int fastEmaPeriod = 9;

        [InputParameter("slow ema period", 70, 1, Int32.MaxValue)]
        public int slowEmaPeriod = 50;



        public override IList<SettingItem> Settings
        {
            get
            {
                var settings = base.Settings;

                settings.AddRange(timeframeSetting.GetSettings(2, "Bot Timeframe", "Timeframe", 0, ""));

                return settings;
            }
            set
            {
                base.Settings = value;

                timeframeSetting.SetSettings(value, "Bot Timeframe");

            }
        }

        public override string[] MonitoringConnectionsIds => new string[] { this.symbol?.ConnectionId };

        public TestStrategy()
            : base()
        {
            // Defines strategy's name and description.
            this.Name = "TestStrategy";
            this.Description = "My strategy's annotation";


            StartPoint = new DateTime(
                Core.TimeUtils.DateTimeUtcNow.AddDays(-3).Year,
                Core.TimeUtils.DateTimeUtcNow.AddDays(-3).Month,
                Core.TimeUtils.DateTimeUtcNow.AddDays(-3).Day,
                21,
                59,
                0,
                DateTimeKind.Utc);

        }

        /// <summary>
        /// This function will be called after creating a strategy
        /// </summary>
        protected override void OnCreated()
        {
            // Add your code here

        }

        /// <summary>
        /// This function will be called after running a strategy
        /// </summary>
        protected override void OnRun()
        {

            lock (syncLock)
            {
                try
                {
                    isRunning = true;


                    if (symbol == null || account == null || symbol.ConnectionId != account.ConnectionId)
                    {
                        Log("Incorrect input parameters... Symbol or Account are not specified or they have diffent connectionID.", StrategyLoggingLevel.Error);
                        return;
                    }

                    this.symbol = Core.GetSymbol(this.symbol?.CreateInfo());

                    if (this.symbol != null)
                    {
                        this.symbol.NewQuote += SymbolOnNewQuote;
                        this.symbol.NewLast += SymbolOnNewLast;
                    }
                    timeframe = new(Core, symbol, StartPoint, timeframeSetting);

                    botData = timeframe.Data;

                    Log($"Bot tf is {botData.Aggregation.ToString()}");

                    botData.AddIndicator(fastEMA = Core.Indicators.BuiltIn.EMA(fastEmaPeriod, PriceType.Close));
                    botData.AddIndicator(slowEMA = Core.Indicators.BuiltIn.EMA(slowEmaPeriod, PriceType.Close));

                    botData.NewHistoryItem += OnNewHistoryItem;


                    



                }
                    finally
                    {
                        isRunning = false;
                    }
                }
        }

        private void OnNewHistoryItem(object sender, HistoryEventArgs e)
        {
              
            if (botData == null || botData.Count < 3)
                return;

            int offset = 1; // last closed bar
            int offset1 = offset + 1; // prior bar

            DateTime barTime = botData.Time(offset);

            if (barTime <= lastProcessedBarTime)
                return;

            lastProcessedBarTime = barTime;

            double open = botData.Open(offset);
            double high = botData.High(offset);
            double low = botData.Low(offset);
            double close = botData.Close(offset);
            double fastEMAValue = fastEMA.GetValue(offset);
            double slowEMAValue = slowEMA.GetValue(offset);

            Log($"BAR {barTime:yyyy-MM-dd HH:mm:ss}  O:{open} H:{high} L:{low} C:{close} Fast EMA:{fastEMAValue} Slow EMA:{slowEMAValue}");

            bool longSignal = fastEMA.GetValue(offset) > slowEMA.GetValue(offset) && fastEMA.GetValue(offset1) <= slowEMA.GetValue(offset1);
            bool shortSignal = fastEMA.GetValue(offset) < slowEMA.GetValue(offset) && fastEMA.GetValue(offset1) >= slowEMA.GetValue(offset1);

            Position currentPosition = GetCurrentPosition();

            if (currentPosition == null)
            {
                if (longSignal)
                {
                    Log("LONG signal detected. Sending BUY market order.");
                    PlaceMarketOrder(Side.Buy);
                }
                else if (shortSignal)
                {
                    Log("SHORT signal detected. Sending SELL market order.");
                    PlaceMarketOrder(Side.Sell);
                }
            }
        }

        private Position GetCurrentPosition()
        {
            if (this.symbol == null || this.account == null)
                return null;

            foreach (var pos in Core.Instance.Positions)
            {
                if (pos.Account == this.account && pos.Symbol == this.symbol)
                    return pos;
            }

            return null;
        }

        private void PlaceMarketOrder(Side side)
        {
            if (orderQuantity <= 0)
            {
                Log("OrderQuantity must be > 0", StrategyLoggingLevel.Error);
                return;
            }

            var request = new PlaceOrderRequestParameters()
            {
                Account = this.account,
                Symbol = this.symbol,
                Side = side,
                Quantity = orderQuantity,
                OrderTypeId = OrderType.Market,
                StopLoss = SlTpHolder.CreateSL(stopLossTicks, PriceMeasurement.Offset),
                TakeProfit = SlTpHolder.CreateTP(profitTargetTicks, PriceMeasurement.Offset),
            };

            var result = Core.Instance.PlaceOrder(request);

            if (result.Status == TradingOperationResultStatus.Failure)
            {
                Log($"PlaceOrder failed: {result.Message}", StrategyLoggingLevel.Error);
            }

        }

        /// <summary>
        /// This function will be called after stopping a strategy
        /// </summary>
        protected override void OnStop()
        {
            lock (syncLock)
            {
                if (!isRunning)
                {
                    if (this.symbol != null)
                    {
                        this.symbol.NewQuote -= SymbolOnNewQuote;
                        this.symbol.NewLast -= SymbolOnNewLast;
                    }

                    // Add your code here


                    if (botData is not null)
                        botData.NewHistoryItem -= OnNewHistoryItem;
                }
            }


            
        }

        /// <summary>
        /// This function will be called after removing a strategy
        /// </summary>
        protected override void OnRemove()
        {
            this.symbol = null;
            this.account = null;
            // Add your code here

            lock (syncLock)
            {
                if (!isRunning)
                {
                    if (botData is not null)
                        botData.NewHistoryItem -= OnNewHistoryItem;
                }
            }

        }

        /// <summary>
        /// Use this method to provide run time information about your strategy. You will see it in StrategyRunner panel in trading terminal
        /// </summary>
        protected override List<StrategyMetric> OnGetMetrics()
        {
            List<StrategyMetric> result = base.OnGetMetrics();

            // An example of adding custom strategy metrics:
            // result.Add("Opened buy orders", "2");
            // result.Add("Opened sell orders", "7");

            return result;
        }

        private void SymbolOnNewQuote(Symbol symbol, Quote quote)
        {
            // Add your code here
        }

        private void SymbolOnNewLast(Symbol symbol, Last last)
        {
            // Add your code here
        }




    }
}
