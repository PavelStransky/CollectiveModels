using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Rùzné uživatelské formátování øetìzcù
    /// </summary>
    public class SpecialFormat {
        /// <summary>
        /// Formátování èasového intervalu
        /// </summary>
        public static string Format(TimeSpan timeSpan) {
            if(timeSpan.Days == 0) {
                if(timeSpan.Hours == 0) {
                    if(timeSpan.Minutes == 0) {
                        if(timeSpan.Seconds == 0) 
                            return string.Format("{0}s", timeSpan.Milliseconds / 1000.0);                        
                        else 
                            return string.Format("{0}s", System.Math.Round(timeSpan.Seconds + timeSpan.Milliseconds / 1000.0, 1));
                    }
                    else                    
                        return string.Format("{0}m{1}s", timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                    return string.Format("{0}h{1}m", timeSpan.Hours, timeSpan.Minutes);
            }
            else 
                return string.Format("{0}d{1}h{2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
        }

        /// <summary>
        /// Formátování èasového intervalu (celoèíselnì)
        /// </summary>
        public static string FormatInt(TimeSpan timeSpan) {
            if(timeSpan.Days == 0) {
                if(timeSpan.Hours == 0) {
                    if(timeSpan.Minutes == 0) {
                        return string.Format("{0}s", timeSpan.Seconds);
                    }
                    else
                        return string.Format("{0}m{1}s", timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                    return string.Format("{0}h{1}m", timeSpan.Hours, timeSpan.Minutes);
            }
            else
                return string.Format("{0}d{1}h{2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
        }

        /// <summary>
        /// Formátování èasového intervalu
        /// </summary>
        /// <param name="isText">True, pokud chceme i text Doba výpoètu</param>
        /// <returns></returns>
        public static string Format(TimeSpan timeSpan, bool isText) {
            if(isText)
                return string.Format(Messages.CalculationTime, Format(timeSpan));
            else
                return Format(timeSpan);
        }
    }
}
