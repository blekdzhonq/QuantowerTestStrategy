﻿// Copyright QUANTOWER LLC. © 2017-2023. All rights reserved.

using System;
using System.Collections.Generic;
using TradingPlatform.BusinessLayer;

namespace TestStrategy
{
    /// <summary>
    /// An example of strategy for working with one symbol. Add your code, compile it and run via Strategy Runner panel in the assigned trading terminal.
    /// Information about API you can find here: http://api.quantower.com
    /// </summary>
	public class TestStrategy : Strategy
    {


        TimeframeSetting timeframeSetting = new();

        Timeframe timeframe;

        HistoricalData botData;


        [InputParameter("Symbol", 10)]
        private Symbol symbol;

        [InputParameter("Account", 20)]
        public Account account;

        [InputParameter("Start Point", 30)]
        public DateTime StartPoint;

        bool isRunning = false;
        private readonly object syncLock = new object();

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

                    // Add your code here


                    timeframe = new(symbol, StartPoint, timeframeSetting);

                    botData = timeframe.Data;

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
            int offset = 1;

            Log($"Time {botData.Time(offset)} " +
                $"Open {botData.Open(offset)} " +
                $"High {botData.High(offset)} " +
                $"Low {botData.Low(offset)} " +
                $"Close {botData.Close(offset)} ");
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
