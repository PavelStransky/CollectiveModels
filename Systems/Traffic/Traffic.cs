using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class Traffic: IExportable {
        public enum TrafficTopology {
            Cyclic,
            SingleMoebius,
            SimpleMoebius,
            Random
        }

        public enum ICType {
            Total,
            Street,
            Probability
        }

        private Random random = new Random();

        private int streetLengthXMin, streetLengthXMax, streetLengthYMin, streetLengthYMax;
        private TrafficTopology topology;

        private int streetsX, streetsY;

        private Street[,] vertical, horizontal;
        private Crossing[,] crossing;
        private Randomizer randomizer;
        private ArrayList trafficItems;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="streetsX">Po�et ulic ve sm�ru X</param>
        /// <param name="streetsY">Po�et ulic ve sm�ru Y</param>
        /// <param name="streetLengthXMin">D�lka ulice X (minimum)</param>
        /// <param name="streetLengthXMax">D�lka ulice X (maximum)</param>
        /// <param name="streetLengthYMin">D�lka ulice Y (minimum)</param>
        /// <param name="streetLengthYMax">D�lka ulice Y (maximum)</param>
        /// <param name="topology">Topologie</param>
        public Traffic(int streetsX, int streetsY, int streetLengthXMin, int streetLengthXMax, int streetLengthYMin, int streetLengthYMax, TrafficTopology topology) {
            this.streetsX = streetsX;
            this.streetsY = streetsY;
            this.streetLengthXMax = streetLengthXMax;
            this.streetLengthXMin = streetLengthXMin;
            this.streetLengthYMax = streetLengthYMax;
            this.streetLengthYMin = streetLengthYMin;

            this.topology = topology;

            this.Create();
        }

        /// <summary>
        /// Nastav� parametry dopravn�ho syst�mu
        /// </summary>
        /// <param name="sensorDistance">Vzd�lenost sensoru od k�i�ovatky</param>
        /// <param name="shortDistance">Bl�zkost k�i�ovatky - p�ij�d�j�c� sm�r</param>
        /// <param name="shortDistanceStopped">Bl�zkost k�i�ovatky - stoj�c� auta za k�i�ovatkou</param>
        /// <param name="minGreen">Minim�ln� doba zelen�</param>
        /// <param name="maxTolerance">Maxim�ln� tolerance (pravidlo 1)</param>
        /// <param name="cutPlatton">Maxim�ln� po�et aut v bl�zk� vzd�lenosti (�ez�n� �ady, pravidlo 3)</param>
        public void SetParams(int sensorDistance, int shortDistance, int shortDistanceStopped, int minGreen, int maxTolerance, int cutPlatoon) {
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.horizontal[i, j].SetParams(sensorDistance, shortDistance, shortDistanceStopped);
                    this.vertical[i, j].SetParams(sensorDistance, shortDistance, shortDistanceStopped);
                    this.crossing[i, j].SetParams(minGreen, maxTolerance, cutPlatoon);
                }
        }

        /// <summary>
        /// Vytvo�� syst�m k�i�ovatek
        /// </summary>
        private void Create() {
            this.vertical = new Street[this.streetsX, this.streetsY];
            this.horizontal = new Street[this.streetsX, this.streetsY];
            this.crossing = new Crossing[this.streetsX, this.streetsY];

            this.trafficItems = new ArrayList();

            // Ulice
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.horizontal[i, j] = new Street(streetLengthXMin, streetLengthXMax);
                    this.vertical[i, j] = new Street(streetLengthYMin, streetLengthYMax);

                    this.trafficItems.Add(this.horizontal[i, j]);
                    this.trafficItems.Add(this.vertical[i, j]);
                }

            // K�i�ovatky
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.crossing[i, j] = new Crossing(2);
                    this.trafficItems.Add(this.crossing[i, j]);
                }

            // Topologie
            this.SetTopology();

            if(this.topology == TrafficTopology.Random)
                this.trafficItems.Add(this.randomizer);
        }

        /// <summary>
        /// Nastav� topologii ulic
        /// </summary>
        private void SetTopology() {
            this.randomizer = new Randomizer();

            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    Street[] incomming = new Street[2];
                    Street[] outgoing = new Street[2];

                    incomming[0] = this.horizontal[i, j];

                    if(i % 2 == 0) { // Sm�r pravo - lev�
                        if(j + 1 < this.streetsY)
                            outgoing[0] = this.horizontal[i, j + 1];
                        else {
                            if(this.topology == TrafficTopology.Cyclic || this.topology == TrafficTopology.Random)
                                outgoing[0] = this.horizontal[i, 0];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[0] = this.vertical[0, this.streetsY - i - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius)
                                outgoing[0] = this.horizontal[i + 1, j];
                            this.randomizer.Add(outgoing[0]);
                        }
                    }
                    else {
                        if(j - 1 >= 0)
                            outgoing[0] = this.horizontal[i, j - 1];
                        else {
                            if(this.topology == TrafficTopology.Cyclic || this.topology == TrafficTopology.Random)
                                outgoing[0] = this.horizontal[i, this.streetsY - 1];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[0] = this.vertical[this.streetsX - 1, this.streetsY - i - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius) {
                                if(i + 1 < this.streetsX)
                                    outgoing[0] = this.horizontal[i + 1, j];
                                else
                                    outgoing[0] = this.vertical[i, j];
                            }
                            this.randomizer.Add(outgoing[0]);
                        }
                    }

                    incomming[1] = this.vertical[i, j];
                    if(j % 2 == 0) { // Sm�r zdola - nahoru
                        if(i - 1 >= 0)
                            outgoing[1] = this.vertical[i - 1, j];
                        else {
                            if(this.topology == TrafficTopology.Cyclic || this.topology == TrafficTopology.Random)
                                outgoing[1] = this.vertical[this.streetsX - 1, j];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[1] = this.horizontal[this.streetsX - j - 1, this.streetsY - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius)
                                outgoing[1] = this.vertical[i, j + 1];
                            this.randomizer.Add(outgoing[1]);
                        }
                    }
                    else {
                        if(i + 1 < this.streetsX)
                            outgoing[1] = this.vertical[i + 1, j];
                        else {
                            if(this.topology == TrafficTopology.Cyclic || this.topology == TrafficTopology.Random)
                                outgoing[1] = this.vertical[0, j];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[1] = this.horizontal[this.streetsX - j - 1, 0];
                            else if(this.topology == TrafficTopology.SingleMoebius) {
                                if(j + 1 < this.streetsY)
                                    outgoing[1] = this.vertical[i, j + 1];
                                else
                                    outgoing[1] = this.horizontal[0, 0];
                            }
                            this.randomizer.Add(outgoing[1]);
                        }
                    }

                    this.crossing[i, j].SetStreets(incomming, outgoing);
                }
        }

        /// <summary>
        /// Nageneruje po��te�n� podm�nky
        /// </summary>
        public void InitialCondition(double icx, double icy, ICType type) {
            // Nulov�n� k�i�ovatek
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++)
                    this.crossing[i, j].Clear();

            switch(type) {
                case ICType.Probability: {
                        for(int i = 0; i < this.streetsX; i++)
                            for(int j = 0; j < this.streetsY; j++) {
                                this.horizontal[i, j].Generate(icy);
                                this.vertical[i, j].Generate(icx);
                            }
                        break;
                    }

                case ICType.Street: {
                        int icix = (int)icx;
                        int iciy = (int)icy;
                        for(int i = 0; i < this.streetsX; i++)
                            for(int j = 0; j < this.streetsY; j++) {
                                this.horizontal[i, j].GenerateNumber(iciy);
                                this.vertical[i, j].GenerateNumber(icix);
                            }
                        break;
                    }

                case ICType.Total: {
                        for(int i = 0; i < this.streetsX; i++)
                            for(int j = 0; j < this.streetsY; j++) {
                                this.horizontal[i, j].Clear();
                                this.vertical[i, j].Clear();
                            }

                        int n = 0;
                        int ici = (int)icx;

                        while(n < ici) {
                            int rx = this.random.Next(this.streetsX);
                            int ry = this.random.Next(this.streetsY);
                            Street s = this.random.Next(2) == 0 ? this.horizontal[rx, ry] : this.vertical[rx, ry];
                            if(s.AddCar())
                                n++;
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Celkov� po�et aut
        /// </summary>
        public int CarNumber() {
            int result = 0;
            foreach(TrafficItem item in this.trafficItems)
                result += item.CarNumber();
            return result;
        }

        /// <summary>
        /// Celkov� po�et pozic na ulic�ch
        /// </summary>
        /// <returns></returns>
        private int StreetPlace() {
            int result = 0;
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++)
                    result += this.horizontal[i, j].Length + this.vertical[i, j].Length;
            return result;
        }

        /// <summary>
        /// Celkov� po�et k�i�ovatek
        /// </summary>
        private int CrossingNumber() {
            return this.streetsX * this.streetsY;
        }

        /// <summary>
        /// Po�et zm�n v posledn�m kroku
        /// </summary>
        /// <remarks>M�lo by b�t sud� ��slo</remarks>
        public int Changes() {
            int result = 0;
            foreach(TrafficItem item in this.trafficItems)
                result += item.Changes;
            return result;
        }

        /// <summary>
        /// Provede krok a vr�t� stav jako matici
        /// </summary>
        public Matrix GetMatrix() {
            Matrix result = new Matrix((this.streetsX + 1) * (this.streetLengthXMax + 1), (this.streetsY + 1) * (this.streetLengthYMax + 1));
            result.Fill(-1.0);

            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    int offsetXh = (i + 1) * (this.streetLengthXMax + 1) - 1;
                    int offsetYh = (j + (i % 2 == 0 ? 0 : 1)) * (this.streetLengthYMax + 1);
                    Street sh = this.horizontal[i, j];
                    for(int h = 0; h < this.streetLengthYMax; h++)
                        if(i % 2 == 0)
                            result[offsetXh, offsetYh + h] = sh.Get(h);
                        else
                            result[offsetXh, offsetYh + h] = sh.GetReverse(h);

                    int offsetXv = (i + (j % 2 == 0 ? 1 : 0)) * (this.streetLengthXMax + 1);
                    int offsetYv = (j + 1) * (this.streetLengthYMax + 1) - 1;
                    Street sv = this.vertical[i, j];
                    for(int v = 0; v < this.streetLengthXMax; v++)
                        if(j % 2 == 0)
                            result[offsetXv + v, offsetYv] = sv.GetReverse(v);
                        else
                            result[offsetXv + v, offsetYv] = sv.Get(v);

                    Crossing c = this.crossing[i, j];
                    result[offsetXh, offsetYv] = c.Data;

                    if(c.State == 0) {
                        if(i % 2 == 0)
                            result[offsetXh + 1, offsetYv - 1] = 1;
                        else
                            result[offsetXh - 1, offsetYv + 1] = 1;
                    }
                    else {
                        if(j % 2 == 0)
                            result[offsetXh + 1, offsetYv + 1] = 1;
                        else
                            result[offsetXh - 1, offsetYv - 1] = 1;
                    }
                }

            return result;
        }

        /// <summary>
        /// Kroky v�po�tu
        /// </summary>
        /// <param name="time">Doba v�po�tu</param>
        public void Step(int time) {
            for(int t = 0; t < time; t++) {
                foreach(TrafficItem item in this.trafficItems)
                    item.Step();

                foreach(TrafficItem item in this.trafficItems)
                    item.FinalizeStep();
            }
        }

        /// <summary>
        /// V�po�et po �as T
        /// </summary>
        /// <param name="time">Doba v�po�tu</param>
        /// <param name="boundary">Okrajov� podm�nky pro u��znut� matice</param>
        public ArrayList Run(int time, int boundary) {
            ArrayList result = new ArrayList();

            Vector velocity = new Vector(time);
            Matrix lights = new Matrix((this.streetsX - 2 * boundary) * (this.streetsY - 2 * boundary), time);
            Matrix streetVertical = new Matrix((this.streetsX - 2 * boundary) * (this.streetsY - 2 * boundary), time);
            Matrix streetHorizontal = new Matrix((this.streetsX - 2 * boundary) * (this.streetsY - 2 * boundary), time);

            for(int t = 0; t < time; t++) {
                foreach(TrafficItem item in this.trafficItems)
                    item.Step();
                foreach(TrafficItem item in this.trafficItems)
                    item.FinalizeStep();

                velocity[t] = this.Changes() / 2;

                int k = 0;
                for(int i = boundary; i < this.streetsX - boundary; i++)
                    for(int j = boundary; j < this.streetsY - boundary; j++) {
                        streetVertical[k, t] = this.vertical[i, j].Middle;
                        streetHorizontal[k, t] = this.horizontal[i, j].Middle;
                        lights[k, t] = this.crossing[i, j].State;
                        k++;
                    }
            }

            result.Add(velocity);
            result.Add(lights);
            result.Add(this.CarNumber());
            result.Add(this.StreetPlace());
            result.Add(this.CrossingNumber());
            result.Add(streetHorizontal);
            result.Add(streetVertical);
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� Traffic t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.streetsX, "Streets X");
            param.Add(this.streetsY, "Streets Y");
            param.Add(this.streetLengthXMin, "Length of the streets X (min)");
            param.Add(this.streetLengthYMin, "Length of the streets Y (min)");
            param.Add(this.topology.ToString(), "Topology");

            // Ulice
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    param.Add(this.horizontal[i, j]);
                    param.Add(this.vertical[i, j]);
                }

            // K�i�ovatky
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++)
                    param.Add(this.crossing[i, j]);

            param.Add(this.streetLengthXMax, "Length of the streets X (max)");
            param.Add(this.streetLengthYMax, "Length of the streets Y (max)");

            param.Export(export);
        }

        /// <summary>
        /// Na�te GCM t��du ze souboru textov�
        /// </summary>
        /// <param name="import">Import</param>
        public Traffic(Core.Import import) {
            IEParam param = new IEParam(import);

            this.streetsX = (int)param.Get();
            this.streetsY = (int)param.Get();
            this.streetLengthXMin = (int)param.Get();
            this.streetLengthYMin = (int)param.Get();

            this.topology = (TrafficTopology)Enum.Parse(typeof(TrafficTopology), (string)param.Get(), true);

            this.vertical = new Street[this.streetsX, this.streetsY];
            this.horizontal = new Street[this.streetsX, this.streetsY];
            this.crossing = new Crossing[this.streetsX, this.streetsY];
            this.trafficItems = new ArrayList();

            // Ulice
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.horizontal[i, j] = (Street)param.Get();
                    this.vertical[i, j] = (Street)param.Get();

                    this.trafficItems.Add(this.horizontal[i, j]);
                    this.trafficItems.Add(this.vertical[i, j]);
                }

            // K�i�ovatky
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.crossing[i, j] = (Crossing)param.Get();
                    this.trafficItems.Add(this.crossing[i, j]);
                }

            this.streetLengthXMax = (int)param.Get(this.streetLengthXMin);
            this.streetLengthYMax = (int)param.Get(this.streetLengthYMin);

            this.SetTopology();

            if(this.topology == TrafficTopology.Random)
                this.trafficItems.Add(this.randomizer);
        }
        #endregion

        /// <summary>
        /// P�evod na �et�zec
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();

            s.Append(string.Format("Street mesh = ({0}, {1})\n", this.streetsX, this.streetsY));
            s.Append(string.Format("Street length = ("));
            if(this.streetLengthXMin != this.streetLengthXMax)
                s.Append(string.Format("({0}, {1})", this.streetLengthXMin, this.streetLengthXMax));
            else
                s.Append(this.streetLengthXMin);
            s.Append(", ");
            if(this.streetLengthYMin != this.streetLengthYMax)
                s.Append(string.Format("({0}, {1})", this.streetLengthYMin, this.streetLengthYMax));
            else
                s.Append(this.streetLengthYMin);            
            s.Append(")\n");
            s.Append(string.Format("Topology: {0}\n\n", this.topology.ToString()));

            int streetPlace = this.StreetPlace();
            int crossings = this.CrossingNumber();
            int cars = this.CarNumber();
            int places = streetPlace + crossings;

            s.Append(string.Format("Number of street places = {0}\n", streetPlace));
            s.Append(string.Format("Number of crossings = {0}\n", crossings));
            s.Append(string.Format("Total number of places = {0}\n", places));
            s.Append(string.Format("Number of cars = {0}\n", cars));
            s.Append(string.Format("Density = {0}\n", (double)cars / (double)places));

            return s.ToString();            
        }
    }
}
