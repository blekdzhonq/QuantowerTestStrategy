
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TradingPlatform.BusinessLayer;

namespace TestStrategy
{
    public class TimeframeSetting
    {

        public int selectedTimeframe = 1;

        public int TickValue { get; set; } = 500;
        public int TimeValue { get; set; } = 5;
        public int TimePeriod { get; set; } = 1;
        public int HaBuildFrom { get; set; } = 0;
        public int HaValue { get; set; } = 1;
        public int RangeTicks { get; set; } = 10;
        public Period RenkoBuildFrom { get; set; } = Period.TICK1;
        public int RenkoBrickSize { get; set; } = 10;
        public int RenkoStyle { get; set; } = 0;
        public int RenkoInversion { get; set; } = 100;
        public int RenkoExtension { get; set; } = 100;
        public Period LineBreakBuildFrom { get; set; } = Period.MIN15;
        public int LineBreakValue { get; set; } = 3;
        public Period KagiBuildFrom { get; set; } = Period.MIN15;
        public int KagiReversal { get; set; } = 10;
        public Period PfBuildFrom { get; set; } = Period.MIN1;
        public int PfBoxSize { get; set; } = 5;
        public int PfReversal { get; set; } = 3;
        public int PfStyle { get; set; } = 0;
        public int VolumeValue { get; set; } = 1000;
        public int ReversalLength { get; set; } = 13;
        public int ReversalReversalLength { get; set; } = 5;
        public int DeltaBarsValue { get; set; } = 1000;
        public int PriceChangesCountBarsTicksCount { get; set; } = 100;
        public int SpyMoneyBarsValue { get; set; } = 20;



        public List<SettingItem> GetSettings(int startSortIndex, string tfName, string groupName, int groupIdx, string parentName)
        {
            List<SettingItem> settings = new List<SettingItem>();

            var ticksOption = new SelectItem("Ticks", 0);
            var timeOption = new SelectItem("Time", 1);
            var heikeinAshiOption = new SelectItem("Heiken Ashi", 2);
            var rangeBarsOption = new SelectItem("Range Bars", 3);
            var renkoOption = new SelectItem("Renko", 4);
            var lineBreakOption = new SelectItem("Line Break", 5);
            var kagiOption = new SelectItem("Kagi", 6);
            var pointsAndFiguresOption = new SelectItem("Points & Figures", 7);
            var volumeOption = new SelectItem("Volume", 8);
            var reversalOption = new SelectItem("Reversal", 9);
            var deltaBarsOption = new SelectItem("Delta Bars", 10);
            var priceChangesCountBarsOption = new SelectItem("Price Changes count bars", 11);
            var spyMoneyBarsOption = new SelectItem("Spymoney bars", 12);


            var group = new SettingItemSeparatorGroup(groupName, groupIdx);//new SettingItemSeparatorGroup("Timeframe", 0);

            settings.Add(new SettingItemSelectorLocalized(tfName, new SelectItem("", selectedTimeframe),
                new List<SelectItem>
                {
                        ticksOption,
                        timeOption,
                        heikeinAshiOption,
                        rangeBarsOption,
                        renkoOption,
                        lineBreakOption,
                        kagiOption,
                        pointsAndFiguresOption,
                        volumeOption,
                        reversalOption,
                        deltaBarsOption,
                        priceChangesCountBarsOption,
                        spyMoneyBarsOption,
                })
            {
                Text = tfName,
                SortIndex = startSortIndex + 1,
                SeparatorGroup = group,

                Relation = new SettingItemRelationVisibility(parentName, true),

            });


            //Ticks
            settings.Add(new SettingItemInteger("tickValue", TickValue, startSortIndex + 2)
            {
                Text = "Tick Value",
                SortIndex = startSortIndex + 2,
                Relation = new SettingItemRelationVisibility(tfName, ticksOption),
                Minimum = 1,
                Maximum = Int32.MaxValue,
                SeparatorGroup = group,
            });



            settings.Add(new SettingItemInteger("timeValue", TimeValue, startSortIndex + 3)
            {
                Text = "Time Value",
                SortIndex = startSortIndex + 3,
                Relation = new SettingItemRelationVisibility(tfName, timeOption),
                Minimum = 1,
                Maximum = Int32.MaxValue,
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemSelectorLocalized("timePeriod", new SelectItem("", TimePeriod),
                Enum.GetValues(typeof(TimePeriodEnum))
                .Cast<TimePeriodEnum>()
                .Select(t => new SelectItem(t.ToString(), (int)t))
                .ToList())
            {
                Text = "Time Period",
                SortIndex = startSortIndex + 4,
                Relation = new SettingItemRelationVisibility(tfName, timeOption),
                SeparatorGroup = group,
            });


            settings.Add(new SettingItemInteger("haValue", HaValue, startSortIndex + 5)
            {
                Text = "HA Value",
                SortIndex = startSortIndex + 5,
                Relation = new SettingItemRelationVisibility(tfName, heikeinAshiOption),
                Minimum = 1,
                Maximum = Int32.MaxValue,
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemSelectorLocalized("haBuildFrom", new SelectItem("", HaBuildFrom),
                Enum.GetValues(typeof(HeikenAshiSource))
                .Cast<HeikenAshiSource>()
                .Select(t => new SelectItem(t.ToString(), (int)t))
                .ToList())
            {
                Text = "HA Build From",
                SortIndex = startSortIndex + 6,
                Relation = new SettingItemRelationVisibility(tfName, heikeinAshiOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("rangeTicks", RangeTicks, startSortIndex + 7)
            {
                Text = "Range Ticks",
                SortIndex = startSortIndex + 7,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, rangeBarsOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemPeriod("renkoBuildFrom", RenkoBuildFrom, startSortIndex + 8)
            {
                Text = "Renko Build From",
                SortIndex = startSortIndex + 8,
                Relation = new SettingItemRelationVisibility(tfName, renkoOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("renkoBrickSize", RenkoBrickSize, startSortIndex + 9)
            {
                Text = "Brick Size",
                SortIndex = startSortIndex + 9,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, renkoOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemSelectorLocalized("renkoStyle", new SelectItem("", RenkoStyle),
                Enum.GetValues(typeof(RenkoStyle))
                .Cast<RenkoStyle>()
                .Select(t => new SelectItem(t.ToString(), (int)t))
                .ToList())
            {
                Text = "Renko Style",
                SortIndex = startSortIndex + 10,
                Relation = new SettingItemRelationVisibility(tfName, renkoOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("renkoExtension", RenkoExtension, startSortIndex + 11)
            {
                Text = "Renko Extension",
                SortIndex = startSortIndex + 11,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, renkoOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("renkoInversion", RenkoInversion, startSortIndex + 12)
            {
                Text = "Renko Inversion",
                SortIndex = startSortIndex + 12,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, renkoOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemPeriod("lineBreakBuildFrom", LineBreakBuildFrom, startSortIndex + 13)
            {
                Text = "Line Break Build From",
                SortIndex = startSortIndex + 13,
                Relation = new SettingItemRelationVisibility(tfName, lineBreakOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("lineBreakValue", LineBreakValue, startSortIndex + 14)
            {
                Text = "Line break",
                SortIndex = startSortIndex + 14,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, lineBreakOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemPeriod("kagiBuildFrom", KagiBuildFrom, startSortIndex + 15)
            {
                Text = "Kagi Build From",
                SortIndex = startSortIndex + 15,
                Relation = new SettingItemRelationVisibility(tfName, kagiOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("kagiReversal", KagiReversal, startSortIndex + 16)
            {
                Text = "Kagi Reversal",
                SortIndex = startSortIndex + 16,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, kagiOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemPeriod("pfBuildFrom", PfBuildFrom, startSortIndex + 17)
            {
                Text = "PF Build From",
                SortIndex = startSortIndex + 17,
                Relation = new SettingItemRelationVisibility(tfName, pointsAndFiguresOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("pfBoxSize", PfBoxSize, startSortIndex + 18)
            {
                Text = "Box Size",
                SortIndex = startSortIndex + 18,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, pointsAndFiguresOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("pfReversal", PfReversal, startSortIndex + 19)
            {
                Text = "PF Reversal",
                SortIndex = startSortIndex + 19,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, pointsAndFiguresOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemSelectorLocalized("pfStyle", new SelectItem("", PfStyle),
                Enum.GetValues(typeof(PointsAndFiguresStyle))
                .Cast<PointsAndFiguresStyle>()
                .Select(t => new SelectItem(t.ToString(), (int)t))
                .ToList())
            {
                Text = "PF Style",
                SortIndex = startSortIndex + 20,
                Relation = new SettingItemRelationVisibility(tfName, pointsAndFiguresOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("volumeValue", VolumeValue, startSortIndex + 21)
            {
                Text = "Volume Value",
                SortIndex = startSortIndex + 21,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, volumeOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("reversalLength", ReversalLength, startSortIndex + 22)
            {
                Text = "Length",
                SortIndex = startSortIndex + 22,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, reversalOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("reversalReversalLength", ReversalReversalLength, startSortIndex + 23)
            {
                Text = "Reversal Length",
                SortIndex = startSortIndex + 23,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, reversalOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("deltaBarsValue", DeltaBarsValue, startSortIndex + 24)
            {
                Text = "Delta Value",
                SortIndex = startSortIndex + 24,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, deltaBarsOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("priceChangesCountBarsTicksCount", PriceChangesCountBarsTicksCount, startSortIndex + 25)
            {
                Text = "Ticks Count",
                SortIndex = startSortIndex + 25,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, priceChangesCountBarsOption),
                SeparatorGroup = group,
            });

            settings.Add(new SettingItemInteger("spyMoneyBarsValue", SpyMoneyBarsValue, startSortIndex + 26)
            {
                Text = "Bars period value",
                SortIndex = startSortIndex + 25,
                Minimum = 1,
                Maximum = Int32.MaxValue,
                Relation = new SettingItemRelationVisibility(tfName, spyMoneyBarsOption),
                SeparatorGroup = group,
            });



            return settings;
        }

        public void SetSettings(IList<SettingItem> value, string tfName)
        {
            if (value.TryGetValue(tfName, out int selectedTimeframe))
                this.selectedTimeframe = selectedTimeframe;

            if (value.TryGetValue("tickValue", out int tickValue))
                TickValue = tickValue;

            if (value.TryGetValue("timeValue", out int timeValue))
                TimeValue = timeValue;

            if (value.TryGetValue("timePeriod", out int timePeriod))
                TimePeriod = timePeriod;

            if (value.TryGetValue("haBuildFrom", out int haValue))
                HaValue = haValue;

            if (value.TryGetValue("haBuildFrom", out int haBuildFrom))
                HaBuildFrom = haBuildFrom;

            if (value.TryGetValue("rangeTicks", out int rangeTicks))
                RangeTicks = rangeTicks;

            if (value.TryGetValue("renkoBuildFrom", out Period renkoBuildFrom))
                RenkoBuildFrom = renkoBuildFrom;

            if (value.TryGetValue("renkoBrickSize", out int renkoBrickSize))
                RenkoBrickSize = renkoBrickSize;

            if (value.TryGetValue("renkoStyle", out int renkoStyle))
                RenkoStyle = renkoStyle;

            if (value.TryGetValue("renkoExtension", out int renkoExtension))
                RenkoExtension = renkoExtension;

            if (value.TryGetValue("renkoInversion", out int renkoInversion))
                RenkoInversion = renkoInversion;

            if (value.TryGetValue("lineBreakBuildFrom", out Period lineBreakBuildFrom))
                LineBreakBuildFrom = lineBreakBuildFrom;

            if (value.TryGetValue("lineBreakValue", out int lineBreakValue))
                LineBreakValue = lineBreakValue;

            if (value.TryGetValue("kagiBuildFrom", out Period kagiBuildFrom))
                KagiBuildFrom = kagiBuildFrom;

            if (value.TryGetValue("kagiReversal", out int kagiReversal))
                KagiReversal = kagiReversal;

            if (value.TryGetValue("pfBuildFrom", out Period pfBuildFrom))
                PfBuildFrom = pfBuildFrom;

            if (value.TryGetValue("pfBoxSize", out int pfBoxSize))
                PfBoxSize = pfBoxSize;

            if (value.TryGetValue("pfReversal", out int pfReversal))
                PfReversal = pfReversal;

            if (value.TryGetValue("pfStyle", out int pfStyle))
                PfStyle = pfStyle;

            if (value.TryGetValue("volumeValue", out int volumeValue))
                VolumeValue = volumeValue;

            if (value.TryGetValue("reversalLength", out int reversalLength))
                ReversalLength = reversalLength;

            if (value.TryGetValue("reversalReversalLength", out int reversalReversalLength))
                ReversalReversalLength = reversalReversalLength;

            if (value.TryGetValue("priceChangesCountBarsTicksCount", out int priceChangesCountBarsTicksCount))
                PriceChangesCountBarsTicksCount = priceChangesCountBarsTicksCount;

            if (value.TryGetValue("spyMoneyBarsValue", out int spyMoneyBarsValue))
                SpyMoneyBarsValue = spyMoneyBarsValue;
        }

    }
}
