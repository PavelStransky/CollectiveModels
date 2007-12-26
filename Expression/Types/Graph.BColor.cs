using System;
using System.Drawing;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression.Functions;

namespace PavelStransky.Expression {
    /// <summary>
    /// Tøída, která zapouzdøuje graf (urèuje jeho obsah, popø. další parametry)
    /// </summary>
    public partial class Graph {
        /// <summary>
        /// Soukromá pomocná tøída k urèení barvy daného bodu
        /// </summary>
        private class BColor {
            private Color colorMin, colorMiddle, colorMax, colorSpecial;
            private bool isDefaultMiddle, isDefaultSpecial;
            private double colorMinValue, colorMiddleValue, colorMaxValue, colorSpecialValue;

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="gv">Parametry dané skupiny grafu</param>
            public BColor(GraphParameterValues gv) {
                this.colorMin = (Color)gv[ParametersIndications.BColorMin];
                this.colorMiddle = (Color)gv[ParametersIndications.BColorMiddle];
                this.colorMax = (Color)gv[ParametersIndications.BColorMax];
                this.colorSpecial = (Color)gv[ParametersIndications.BColorSpecial];

                this.isDefaultMiddle = gv.IsDefault(ParametersIndications.BColorMiddle) & gv.IsDefault(ParametersIndications.BColorMiddleValue);
                this.isDefaultSpecial = gv.IsDefault(ParametersIndications.BColorSpecial) & gv.IsDefault(ParametersIndications.BColorSpecialValue);

                this.colorMinValue = (double)gv[ParametersIndications.BColorMinValue];
                this.colorMiddleValue = (double)gv[ParametersIndications.BColorMiddleValue];
                this.colorMaxValue = (double)gv[ParametersIndications.BColorMaxValue];
                this.colorSpecialValue = (double)gv[ParametersIndications.BColorSpecialValue];
            }

            /// <summary>
            /// Indexer
            /// </summary>
            /// <param name="v">Hodnota</param>
            public Color this[double v] {
                get {
                    Color result;

                    if(v == this.colorSpecialValue && !this.isDefaultSpecial)
                        result = this.colorSpecial;

                    else if(v <= this.colorMinValue)
                        result = this.colorMin;

                    else if(v < this.colorMiddleValue && !this.isDefaultMiddle) {
                        double n = (v - this.colorMinValue) / (this.colorMiddleValue - this.colorMinValue);
                        result = Color.FromArgb(
                            (int)((1 - n) * this.colorMin.R + n * this.colorMiddle.R),
                            (int)((1 - n) * this.colorMin.G + n * this.colorMiddle.G),
                            (int)((1 - n) * this.colorMin.B + n * this.colorMiddle.B));
                    }

                    else if(v < this.colorMaxValue) {
                        if(this.isDefaultMiddle) {
                            double n = (v - this.colorMinValue) / (this.colorMaxValue - this.colorMinValue);
                            result = Color.FromArgb(
                                (int)((1 - n) * this.colorMin.R + n * this.colorMax.R),
                                (int)((1 - n) * this.colorMin.G + n * this.colorMax.G),
                                (int)((1 - n) * this.colorMin.B + n * this.colorMax.B));
                        }
                        else {
                            double n = (v - this.colorMiddleValue) / (this.colorMaxValue - this.colorMiddleValue);
                            result = Color.FromArgb(
                                (int)((1 - n) * this.colorMiddle.R + n * this.colorMax.R),
                                (int)((1 - n) * this.colorMiddle.G + n * this.colorMax.G),
                                (int)((1 - n) * this.colorMiddle.B + n * this.colorMax.B));
                        }
                    }

                    else
                        result = this.colorMax;

                    return result;
                }
            }
        }
    }
}