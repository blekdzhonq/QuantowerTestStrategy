using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStrategy
{

    

    public enum TimeframeEnum
    {
        [Description("Ticks")]
        Ticks,
        [Description("Time")]
        Time,
        [Description("Heikein Ashi")]
        HeikeinAshi,
        [Description("Range Bars")]
        RangeBars,
        [Description("Renko")]
        Renko,
        [Description("Line Break")]
        LineBreak,
        [Description("Kagi")]
        Kagi,
        [Description("Points & Figures")]
        PointsAndFigures,
        [Description("Volume")]
        Volume,
        [Description("Reversal")]
        Reversal,
        [Description("Delta Bars")]
        DeltaBars,
        [Description("Price Changes count bars")]
        PriceChangesCountBars,
        [Description("Spy Money bars")]
        SpyMoneyBars
    };

    public enum TimePeriodEnum
    {
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year

    }


    

}
