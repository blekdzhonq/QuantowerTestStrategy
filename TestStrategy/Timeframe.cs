
using System;
using TradingPlatform.BusinessLayer;

namespace TestStrategy
{
    public class Timeframe
    {
        public HistoricalData Data
        {
            get
            {
                if (data is null)
                {
                    data = _symbol.GetHistory(new HistoryRequestParameters()
                    {
                        Symbol = _symbol,
                        FromTime = _startPoint,
                        Aggregation = _aggregation
                    });
                }

                return data;
            }
        }

        public HistoryAggregation Aggregation { get { return _aggregation; } }



        HistoryAggregation _aggregation;

        private HistoricalData data;

        private Symbol _symbol;
        private DateTime _startPoint;
        private Period _basePeriod;

        public Timeframe(Symbol symbol,
            DateTime startPoint,
            TimeframeSetting timeframeSetting)
        {

            _symbol = symbol;
            _startPoint = startPoint;

            HistoryAggregation aggregation = null;

            var timeframeEnum = (TimeframeEnum)timeframeSetting.selectedTimeframe;

            if (timeframeEnum is TimeframeEnum.Ticks)
            {
                aggregation = new HistoryAggregationTickBars(timeframeSetting.TickValue, _symbol.HistoryType);

            }
            else if (timeframeEnum is TimeframeEnum.Time)
            {

                aggregation = new HistoryAggregationTime(
                    new Period((timeframeSetting.TimePeriod == (int)TimePeriodEnum.Minute ? BasePeriod.Minute :
                                timeframeSetting.TimePeriod == (int)TimePeriodEnum.Hour ? BasePeriod.Hour :
                                timeframeSetting.TimePeriod == (int)TimePeriodEnum.Year ? BasePeriod.Year :
                                timeframeSetting.TimePeriod == (int)TimePeriodEnum.Month ? BasePeriod.Month :
                                timeframeSetting.TimePeriod == (int)TimePeriodEnum.Week ? BasePeriod.Week :
                                timeframeSetting.TimePeriod == (int)TimePeriodEnum.Day ? BasePeriod.Day :
                                BasePeriod.Second), timeframeSetting.TimeValue), _symbol.HistoryType);

            }
            else if (timeframeEnum is TimeframeEnum.HeikeinAshi)
            {
                aggregation = new HistoryAggregationHeikenAshi((HeikenAshiSource)timeframeSetting.HaBuildFrom, timeframeSetting.HaValue, _symbol.HistoryType);
            }
            else if (timeframeEnum is TimeframeEnum.RangeBars)
            {
                aggregation = new HistoryAggregationRangeBars(timeframeSetting.RangeTicks, _symbol.HistoryType);
            }
            else if (timeframeEnum is TimeframeEnum.Renko)
            {
                aggregation = new HistoryAggregationRenko(timeframeSetting.RenkoBuildFrom, _symbol.HistoryType,
                    timeframeSetting.RenkoBrickSize, (RenkoStyle)timeframeSetting.RenkoStyle, timeframeSetting.RenkoExtension, timeframeSetting.RenkoInversion);
            }
            else if (timeframeEnum is TimeframeEnum.LineBreak)
            {
                aggregation = new HistoryAggregationLineBreak(timeframeSetting.LineBreakBuildFrom, _symbol.HistoryType, timeframeSetting.LineBreakValue);
            }
            else if (timeframeEnum is TimeframeEnum.Kagi)
            {
                aggregation = new HistoryAggregationKagi(timeframeSetting.KagiBuildFrom, _symbol.HistoryType, timeframeSetting.KagiReversal);
            }
            else if (timeframeEnum is TimeframeEnum.PointsAndFigures)
            {
                aggregation = new HistoryAggregationPointsAndFigures(timeframeSetting.PfBuildFrom, _symbol.HistoryType, timeframeSetting.PfBoxSize,
                    timeframeSetting.PfReversal, (PointsAndFiguresStyle)timeframeSetting.PfStyle);
            }
            else if (timeframeEnum is TimeframeEnum.Volume)
            {
                aggregation = new HistoryAggregationVolume(timeframeSetting.VolumeValue);
            }
            else if (timeframeEnum is TimeframeEnum.Reversal)
            {
                aggregation = new HistoryAggregationReversal(timeframeSetting.ReversalLength, timeframeSetting.ReversalReversalLength);
            }
            else if (timeframeEnum is TimeframeEnum.DeltaBars)
            {
                aggregation = new HistoryAggregationDeltaBars(timeframeSetting.DeltaBarsValue);
            }
            else if (timeframeEnum is TimeframeEnum.PriceChangesCountBars)
            {
                aggregation = new HistoryAggregationPriceChangesCountBars(timeframeSetting.PriceChangesCountBarsTicksCount, _symbol.HistoryType);
            }
            else if (timeframeEnum is TimeframeEnum.SpyMoneyBars)
            {
                aggregation = new HistoryAggregationSpyMoneyBars(timeframeSetting.SpyMoneyBarsValue);


            }

            _aggregation = aggregation;




        }




    }
}
