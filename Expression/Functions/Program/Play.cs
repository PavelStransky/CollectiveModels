using System;
using System.Collections;
using System.IO;
using System.Media;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Plays a given vector
    /// </summary>
    public class Play: Fnc {
        public override string Help { get { return Messages.HelpPlay; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSound, Messages.PSoundDescription, null, typeof(Vector), typeof(TArray), typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.PSoundParams, Messages.PSoundParamsDescription, new Vector(0), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray data = null;
            if(arguments[0] is Vector) {
                data = new TArray(typeof(Vector), 1);
                data[0] = arguments[0] as Vector;
            }
            else if(arguments[0] is PointVector) {
                data = new TArray(typeof(PointVector), 1);
                data[0] = arguments[0] as PointVector;
            }
            else
                data = arguments[0] as TArray;

            Vector parameters = arguments[1] as Vector;

            MemoryStream m = new MemoryStream();
            BinaryWriter b = new BinaryWriter(m);
            FnExport.ExportWave(b, data, parameters);

            m.Seek(0, SeekOrigin.Begin);
            SoundPlayer player = new SoundPlayer(m);
            player.PlaySync();
            
            return null;
        }
    }
}
