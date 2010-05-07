using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates Traffic class
    /// </summary>
    public class FnTraffic: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTraffic; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, false, true, false, Messages.PSizeX, Messages.PSizeXDescription, 10, typeof(int));
            this.SetParam(1, false, true, false, Messages.PSizeY, Messages.PSizeYDescription, 10, typeof(int));
            this.SetParam(2, false, true, false, Messages.PStreetLengthX, Messages.PStreetLengthXDescription, 15, typeof(int));
            this.SetParam(3, false, true, false, Messages.PStreetLengthY, Messages.PStreetLengthYDescription, 15, typeof(int));
            this.SetParam(4, false, true, false, Messages.PTrafficTopology, Messages.PTrafficTopologyDescription, "cyclic", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int sizeX = (int)arguments[0];
            int sizeY = (int)arguments[1];
            int lengthX = (int)arguments[2];
            int lengthY = (int)arguments[3];

            Traffic.TrafficTopology topology = (Traffic.TrafficTopology)Enum.Parse(typeof(Traffic.TrafficTopology), (string)arguments[4], true);

            return new Traffic(sizeX, sizeY, lengthX, lengthY, topology);
        }

        private const string name = "traffic";
    }
}