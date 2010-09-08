using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf 
    /// </summary>
    /// <remarks>Parametry grafu</remarks>
    public partial class Graph {

        /// <summary>
        /// Styly spojnic bodù
        /// </summary>
        public enum LineStyles { Line, Curve, None }

        /// <summary>
        /// Styly bodù
        /// </summary>
        public enum PointStyles { Circle, Square, None, HLines, VLines, FCircle, FSquare }

        /// <summary>
        /// Oznaèení parametrù
        /// </summary>
        public enum ParametersIndications {
            AnimGroup = 1,
            Interval = 2,
            ScrollStep = 3,
            Title = 4,
            TitleFSize = 5,
            TitleColor = 6,
            Shift = 10,

            RWindowL = 20,
            RWindowT = 21,
            RWindowW = 22,
            RWindowH = 23,

            Comment = 30,

            AnimCurve = 100,
            BColor = 101,

            MarginL = 110,
            MarginR = 111,
            MarginT = 112,
            MarginB = 113,

            MinX = 120,
            MaxX = 121,
            MinY = 122,
            MaxY = 123,

            BMinX = 130,
            BMaxX = 131,
            BMinY = 132,
            BMaxY = 133,

            BColorMin = 140,
            BColorMiddle = 141,
            BColorMax = 142,
            BColorSpecial = 143,
            
            BColorMinValue = 145,
            BColorMiddleValue = 146,
            BColorMaxValue = 147,
            BColorSpecialValue = 148,
            
            BColorFnc = 149,

            BLegend = 150,
            BLegendWidth = 151,
            BLegendFSize = 152,
            BLegendFColor = 153,
            BLegendLabel = 154,

            BLegendMinY = 155,
            BLegendMaxY = 156,
        
            BPSizeX = 160,
            BPSizeY = 161,

            GTitle = 170,
            GTitleColor = 171,
            GTitleFSize = 172,

            ATitleX = 200,
            ATitleColorX = 201,
            ATitleFSizeX = 202,
            ALColorX = 210,
            ALWidthX = 211,
            APColorX = 220,
            APStyleX = 221,
            APSizeX = 222,
            AShowT = 230,
            AShowB = 231,
            AShowLabelX = 240,
            ALabelColorX = 241,
            ALabelFSizeX = 242,
            AMinorTicksX = 250,
            AMajorTicksX = 251,
            AShowTicksT = 260,
            AShowTicksB = 261,

            ATitleY = 300,
            ATitleColorY = 301,
            ATitleFSizeY = 302,
            ALColorY = 310,
            ALWidthY = 311,
            APColorY = 320,
            APStyleY = 321,
            APSizeY = 322,
            AShowL = 330,
            AShowR = 331,
            AShowLabelY = 340,
            ALabelColorY = 341,
            ALabelFSizeY = 342,
            AMinorTicksY = 350,
            AMajorTicksY = 351,
            AShowTicksL = 360,
            AShowTicksR = 361,

            ShowGridX = 400,
            GridColorX = 401,
            GridWidthX = 402,
            ShowGridY = 410,
            GridColorY = 411,
            GridWidthY = 412,

            CLegend = 500,
            CLegendWidth = 501,
            CLegendFSize = 502,
            CLegendFColor = 503,

            LColor = 1000,
            LStyle = 1001,
            LWidth = 1002,
            LName = 1003,
            LDash = 1004,

            PColor = 1020,
            PStyle = 1021,
            PSize = 1022,
            PColorFnc = 1023,

            FPColor = 1030,
            FPStyle = 1031,
            FPSize = 1032,

            LPColor = 1040,
            LPStyle = 1041,
            LPSize = 1042,

            Clip = 1050,

            // Tajné parametry
            GroupMaxLength = 10000,
            MatrixAbs = 10001,

            NumGroups = 10002,
            NumCurves = 10003,

            DataBackground = 10004,
            DataCurves = 10005,
            DataErrors = 10006,

            FoundDataMinMax = 10010,
            DataMinX = 10011,
            DataMaxX = 10012,
            DataMinY = 10013,
            DataMaxY = 10014,

            PixelL = 10020,
            PixelT = 10021,
            PixelW = 10022,
            PixelH = 10023,

            PColorFncBuffer = 10100,
            BColorFncBuffer = 10101
        }

        private static GraphParameterDefinitions globalParams;
        private static GraphParameterDefinitions groupParams;
        private static GraphParameterDefinitions curveParams;

        /// <summary>
        /// Globální parametry grafu
        /// </summary>
        public static GraphParameterDefinitions GlobalParams { get { return globalParams; } }

        /// <summary>
        /// Parametry skupiny grafu
        /// </summary>
        public static GraphParameterDefinitions GroupParams { get { return groupParams; } }

        /// <summary>
        /// Parametry køivky grafu
        /// </summary>
        public static GraphParameterDefinitions CurveParams { get { return curveParams; } }

        /// <summary>
        /// Vytvoøení parametrù
        /// </summary>
        static Graph() {
            globalParams = new GraphParameterDefinitions();
            globalParams.Add(ParametersIndications.AnimGroup, Messages.GPAnimGroup, Messages.GPAnimGroupDescription, false);
            globalParams.Add(ParametersIndications.Interval, Messages.GPInterval, Messages.GPIntervalDescription, 1000);
            globalParams.Add(ParametersIndications.ScrollStep, Messages.GPScrollStep, Messages.GPScrollStepDescription, 3);
            globalParams.Add(ParametersIndications.Title, Messages.GPTitle, Messages.GPTitleDescription, string.Empty);
            globalParams.Add(ParametersIndications.TitleFSize, Messages.GPTitleFSize, Messages.GPTitleFSizeDescription, 18);
            globalParams.Add(ParametersIndications.TitleColor, Messages.GPTitleColor, Messages.GPTitleColorDescription, Color.Black);
            globalParams.Add(ParametersIndications.Shift, Messages.GPShift, Messages.GPShiftDescription, false);

            globalParams.Add(ParametersIndications.RWindowL, Messages.GPRWindowL, Messages.GPRWindowLDescription, 0.0);
            globalParams.Add(ParametersIndications.RWindowT, Messages.GPRWindowT, Messages.GPRWindowTDescription, 0.0);
            globalParams.Add(ParametersIndications.RWindowW, Messages.GPRWindowW, Messages.GPRWindowWDescription, 1.0);
            globalParams.Add(ParametersIndications.RWindowH, Messages.GPRWindowH, Messages.GPRWindowHDescription, 1.0);

            globalParams.Add(ParametersIndications.Comment, Messages.GPComment, Messages.GPCommentDescription, string.Empty);

            groupParams = new GraphParameterDefinitions();
            groupParams.Add(ParametersIndications.AnimCurve, Messages.GPAnimCurve, Messages.GPAnimCurveDescription, false);
            groupParams.Add(ParametersIndications.BColor, Messages.GPBColor, Messages.GPBColorDescription, Color.Transparent);
            groupParams.Add(ParametersIndications.MarginL, Messages.GPMarginL, Messages.GPMarginLDescription, 30);
            groupParams.Add(ParametersIndications.MarginR, Messages.GPMarginR, Messages.GPMarginRDescription, 30);
            groupParams.Add(ParametersIndications.MarginT, Messages.GPMarginT, Messages.GPMarginTDescription, 30);
            groupParams.Add(ParametersIndications.MarginB, Messages.GPMarginB, Messages.GPMarginBDescription, 30);
            groupParams.Add(ParametersIndications.MinX, Messages.GPMinX, Messages.GPMinXDescription, 0.0);
            groupParams.Add(ParametersIndications.MaxX, Messages.GPMaxX, Messages.GPMaxXDescription, 0.0);
            groupParams.Add(ParametersIndications.MinY, Messages.GPMinY, Messages.GPMinYDescription, 0.0);
            groupParams.Add(ParametersIndications.MaxY, Messages.GPMaxY, Messages.GPMaxYDescription, 0.0);
            groupParams.Add(ParametersIndications.BMinX, Messages.GPBMinX, Messages.GPBMinXDescription, 0.0);
            groupParams.Add(ParametersIndications.BMaxX, Messages.GPBMaxX, Messages.GPBMaxXDescription, 0.0);
            groupParams.Add(ParametersIndications.BMinY, Messages.GPBMinY, Messages.GPBMinYDescription, 0.0);
            groupParams.Add(ParametersIndications.BMaxY, Messages.GPBMaxY, Messages.GPBMaxYDescription, 0.0);
            groupParams.Add(ParametersIndications.BColorMin, Messages.GPBColorMin, Messages.GPBColorMinDescription, Color.Red);
            groupParams.Add(ParametersIndications.BColorMiddle, Messages.GPBColorMiddle, Messages.GPBColorMiddleDescription, Color.White);
            groupParams.Add(ParametersIndications.BColorMax, Messages.GPBColorMax, Messages.GPBColorMaxDescription, Color.Blue);
            groupParams.Add(ParametersIndications.BColorSpecial, Messages.GPBColorSpecial, Messages.GPBColorSpecialDescription, Color.Black);
            groupParams.Add(ParametersIndications.BColorMinValue, Messages.GPBColorMinValue, Messages.GPBColorMinValueDescription, -1.0);
            groupParams.Add(ParametersIndications.BColorMiddleValue, Messages.GPBColorMiddleValue, Messages.GPBColorMiddleValueDescription, 0.0);
            groupParams.Add(ParametersIndications.BColorMaxValue, Messages.GPBColorMaxValue, Messages.GPBColorMaxValueDescription, 1.0);
            groupParams.Add(ParametersIndications.BColorSpecialValue, Messages.GPBColorSpecialValue, Messages.GPBColorSpecialValueDescription, 0.0);
            groupParams.Add(ParametersIndications.BColorFnc, Messages.GPBColorFnc, Messages.GPBColorFncDescription, string.Empty);
            groupParams.Add(ParametersIndications.BLegend, Messages.GPBLegend, Messages.GPBLegendDescription, false);
            groupParams.Add(ParametersIndications.BLegendFSize, Messages.GPBLegendFSize, Messages.GPBLegendFSizeDescription, 10);
            groupParams.Add(ParametersIndications.BLegendWidth, Messages.GPBLegendWidth, Messages.GPBLegendWidthDescription, 30);
            groupParams.Add(ParametersIndications.BLegendFColor, Messages.GPBLegendFColor, Messages.GPBLegendFColorDescription, Color.Black);
            groupParams.Add(ParametersIndications.BLegendLabel, Messages.GPBLegendLabel, Messages.GPBLegendLabelDescription, true);
            groupParams.Add(ParametersIndications.BLegendMinY, Messages.GPBLegendMinY, Messages.GPBLegendMinYDescription, -1.0);
            groupParams.Add(ParametersIndications.BLegendMaxY, Messages.GPBLegendMaxY, Messages.GPBLegendMaxYDescription, 1.0);
            groupParams.Add(ParametersIndications.BPSizeX, Messages.GPBPSizeX, Messages.GPBPSizeXDescription, 1);
            groupParams.Add(ParametersIndications.BPSizeY, Messages.GPBPSizeY, Messages.GPBPSizeYDescription, 1);
            groupParams.Add(ParametersIndications.GTitle, Messages.GPGTitle, Messages.GPGTitleDescription, string.Empty);
            groupParams.Add(ParametersIndications.GTitleColor, Messages.GPGTitleColor, Messages.GPGTitleColorDescription, Color.Blue);
            groupParams.Add(ParametersIndications.GTitleFSize, Messages.GPGTitleFSize, Messages.GPGTitleFSizeDescription, 15);

            groupParams.Add(ParametersIndications.ATitleX, Messages.GPATitleX, Messages.GPATitleXDescription, string.Empty);
            groupParams.Add(ParametersIndications.ATitleColorX, Messages.GPATitleColorX, Messages.GPATitleColorXDescription, Color.Red);
            groupParams.Add(ParametersIndications.ATitleFSizeX, Messages.GPATitleFSizeX, Messages.GPATitleFSizeXDescription, 12);
            groupParams.Add(ParametersIndications.ALColorX, Messages.GPALColorX, Messages.GPALColorXDescription, Color.Red);
            groupParams.Add(ParametersIndications.ALWidthX, Messages.GPALWidthX, Messages.GPALWidthXDescription, 1);
            groupParams.Add(ParametersIndications.APColorX, Messages.GPAPColorX, Messages.GPAPColorXDescription, Color.Red);
            groupParams.Add(ParametersIndications.APSizeX, Messages.GPAPSizeX, Messages.GPAPSizeXDescription, 5);
            groupParams.Add(ParametersIndications.APStyleX, Messages.GPAPStyleX, Messages.GPAPStyleXDescription, PointStyles.VLines);
            groupParams.Add(ParametersIndications.AShowT, Messages.GPAShowT, Messages.GPAShowTDescription, false);
            groupParams.Add(ParametersIndications.AShowB, Messages.GPAShowB, Messages.GPAShowBDescription, true);
            groupParams.Add(ParametersIndications.AShowLabelX, Messages.GPAShowLabelX, Messages.GPAShowLabelXDescription, true);
            groupParams.Add(ParametersIndications.ALabelColorX, Messages.GPALabelColorX, Messages.GPALabelColorXDescription, Color.Red);
            groupParams.Add(ParametersIndications.ALabelFSizeX, Messages.GPALabelFSizeX, Messages.GPALabelFSizeXDescription, 10);
            groupParams.Add(ParametersIndications.AMinorTicksX, Messages.GPAMinorTicksX, Messages.GPAMinorTicksXDescription, -1);
            groupParams.Add(ParametersIndications.AMajorTicksX, Messages.GPAMajorTicksX, Messages.GPAMajorTicksXDescription, -1.0);
            groupParams.Add(ParametersIndications.AShowTicksT, Messages.GPAShowTicksT, Messages.GPAShowTicksTDescription, true);
            groupParams.Add(ParametersIndications.AShowTicksB, Messages.GPAShowTicksB, Messages.GPAShowTicksBDescription, true);

            groupParams.Add(ParametersIndications.ATitleY, Messages.GPATitleY, Messages.GPATitleYDescription, string.Empty);
            groupParams.Add(ParametersIndications.ATitleColorY, Messages.GPATitleColorY, Messages.GPATitleColorYDescription, Color.Red);
            groupParams.Add(ParametersIndications.ATitleFSizeY, Messages.GPATitleFSizeY, Messages.GPATitleFSizeYDescription, 12);
            groupParams.Add(ParametersIndications.ALColorY, Messages.GPALColorY, Messages.GPALColorYDescription, Color.Red);
            groupParams.Add(ParametersIndications.ALWidthY, Messages.GPALWidthY, Messages.GPALWidthYDescription, 1);
            groupParams.Add(ParametersIndications.APColorY, Messages.GPAPColorY, Messages.GPAPColorYDescription, Color.Red);
            groupParams.Add(ParametersIndications.APSizeY, Messages.GPAPSizeY, Messages.GPAPSizeYDescription, 5);
            groupParams.Add(ParametersIndications.APStyleY, Messages.GPAPStyleY, Messages.GPAPStyleYDescription, PointStyles.HLines);
            groupParams.Add(ParametersIndications.AShowL, Messages.GPAShowL, Messages.GPAShowLDescription, true);
            groupParams.Add(ParametersIndications.AShowR, Messages.GPAShowR, Messages.GPAShowRDescription, false);
            groupParams.Add(ParametersIndications.AShowLabelY, Messages.GPAShowLabelY, Messages.GPAShowLabelYDescription, true);
            groupParams.Add(ParametersIndications.ALabelColorY, Messages.GPALabelColorY, Messages.GPALabelColorYDescription, Color.Red);
            groupParams.Add(ParametersIndications.ALabelFSizeY, Messages.GPALabelFSizeY, Messages.GPALabelFSizeYDescription, 10);
            groupParams.Add(ParametersIndications.AMinorTicksY, Messages.GPAMinorTicksY, Messages.GPAMinorTicksYDescription, -1);
            groupParams.Add(ParametersIndications.AMajorTicksY, Messages.GPAMajorTicksY, Messages.GPAMajorTicksYDescription, -1.0);
            groupParams.Add(ParametersIndications.AShowTicksL, Messages.GPAShowTicksL, Messages.GPAShowTicksLDescription, true);
            groupParams.Add(ParametersIndications.AShowTicksR, Messages.GPAShowTicksR, Messages.GPAShowTicksRDescription, true);

            groupParams.Add(ParametersIndications.ShowGridX, Messages.GPShowGridX, Messages.GPShowGridXDescription, false);
            groupParams.Add(ParametersIndications.GridColorX, Messages.GPGridColorX, Messages.GPGridColorXDescription, Color.LightGray);
            groupParams.Add(ParametersIndications.GridWidthX, Messages.GPGridWidthX, Messages.GPGridWidthXDescription, 1);
            groupParams.Add(ParametersIndications.ShowGridY, Messages.GPShowGridY, Messages.GPShowGridYDescription, false);
            groupParams.Add(ParametersIndications.GridColorY, Messages.GPGridColorY, Messages.GPGridColorYDescription, Color.LightGray);
            groupParams.Add(ParametersIndications.GridWidthY, Messages.GPGridWidthY, Messages.GPGridWidthYDescription, 1);

            groupParams.Add(ParametersIndications.CLegend, Messages.GPCLegend, Messages.GPCLegendDescription, false);
            groupParams.Add(ParametersIndications.CLegendFSize, Messages.GPCLegendFSize, Messages.GPCLegendFSizeDescription, 10);
            groupParams.Add(ParametersIndications.CLegendWidth, Messages.GPCLegendWidth, Messages.GPCLegendWidthDescription, 100);
            groupParams.Add(ParametersIndications.CLegendFColor, Messages.GPCLegendFColor, Messages.GPCLegendFColorDescription, Color.Black);
            
            curveParams = new GraphParameterDefinitions();
            curveParams.Add(ParametersIndications.LColor, Messages.GPLColor, Messages.GPLColorDescription, Color.Blue);
            curveParams.Add(ParametersIndications.LStyle, Messages.GPLStyle, Messages.GPLStyleDescription, LineStyles.Curve);
            curveParams.Add(ParametersIndications.LWidth, Messages.GPLWidth, Messages.GPLWidthDescription, 1);
            curveParams.Add(ParametersIndications.LName, Messages.GPLName, Messages.GPLNameDescription, string.Empty);
            curveParams.Add(ParametersIndications.LDash, Messages.GPLDash, Messages.GPLDashDescription, System.Drawing.Drawing2D.DashStyle.Solid);
            curveParams.Add(ParametersIndications.PColor, Messages.GPPColor, Messages.GPPColorDescription, Color.Brown);
            curveParams.Add(ParametersIndications.PStyle, Messages.GPPStyle, Messages.GPPStyleDescription, PointStyles.FCircle);
            curveParams.Add(ParametersIndications.PSize, Messages.GPPSize, Messages.GPPSizeDescription, 2);
            curveParams.Add(ParametersIndications.PColorFnc, Messages.GPPColorFnc, Messages.GPPColorFncDescription, string.Empty);
            curveParams.Add(ParametersIndications.FPColor, Messages.GPFPColor, Messages.GPFPColorDescription, Color.DarkRed);
            curveParams.Add(ParametersIndications.FPStyle, Messages.GPFPStyle, Messages.GPFPStyleDescription, PointStyles.None);
            curveParams.Add(ParametersIndications.FPSize, Messages.GPFPSize, Messages.GPFPSizeDescription, 2);
            curveParams.Add(ParametersIndications.LPColor, Messages.GPLPColor, Messages.GPLPColorDescription, Color.DarkGreen);
            curveParams.Add(ParametersIndications.LPStyle, Messages.GPLPStyle, Messages.GPLPStyleDescription, PointStyles.None);
            curveParams.Add(ParametersIndications.LPSize, Messages.GPLPSize, Messages.GPLPSizeDescription, 2);
            curveParams.Add(ParametersIndications.Clip, Messages.GPClip, Messages.GPClipDescription, true);
        }
        
        // Okraj v promilích
        private const int defaultMargin = 30;
        private const int defaultMarginAxeL = 80;
        private const int defaultMarginAxeB = 70;
        private const int defaultMarginTitleX = 50;
        private const int defaultMarginTitleY = 50;
    }
}
