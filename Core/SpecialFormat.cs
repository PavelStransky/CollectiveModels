using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// R�zn� u�ivatelsk� form�tov�n� �et�zc�
    /// </summary>
    public class SpecialFormat {
        /// <summary>
        /// Form�tov�n� �asov�ho intervalu
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
        /// Form�tov�n� �asov�ho intervalu (celo��seln�)
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
        /// Form�tov�n� �asov�ho intervalu
        /// </summary>
        /// <param name="isText">True, pokud chceme i text Doba v�po�tu</param>
        /// <returns></returns>
        public static string Format(TimeSpan timeSpan, bool isText) {
            if(isText)
                return string.Format(Messages.CalculationTime, Format(timeSpan));
            else
                return Format(timeSpan);
        }
    }
}
