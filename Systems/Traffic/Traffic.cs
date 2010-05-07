using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class Traffic {
        public enum TrafficTopology {
            Cyclic,
            SingleMoebius,
            SimpleMoebius
        }

        public enum ICType {
            Total,
            Street,
            Probability
        }

        private Random random = new Random();

        private int streetLengthX, streetLengthY;
        private TrafficTopology topology;

        private int streetsX, streetsY;

        private Street[,] vertical, horizontal;
        private Crossing[,] crossing;
        private ArrayList trafficItems;

        public Traffic(int streetsX, int streetsY, int streetLengthX, int streetLengthY, TrafficTopology topology) {
            this.streetsX = streetsX;
            this.streetsY = streetsY;
            this.streetLengthX = streetLengthX;
            this.streetLengthY = streetLengthY;

            this.topology = topology;
               
            this.Create();
        }

        /// <summary>
        /// Vytvoøí systém køižovatek
        /// </summary>
        private void Create() {
            this.vertical = new Street[this.streetsX, this.streetsY];
            this.horizontal = new Street[this.streetsX, this.streetsY];
            this.crossing = new Crossing[this.streetsX, this.streetsY];

            this.trafficItems = new ArrayList();

            // Ulice
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    this.horizontal[i, j] = new Street(streetLengthX);
                    this.vertical[i, j] = new Street(streetLengthY);

                    this.trafficItems.Add(this.horizontal[i, j]);
                    this.trafficItems.Add(this.vertical[i, j]);
                }

            // Køižovatky
            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    Street[] incomming = new Street[2];
                    Street[] outgoing = new Street[2];

                    Street s1 = null, s2 = null;

                    incomming[0] = this.horizontal[i, j];

                    if(i % 2 == 0) { // Smìr pravo - levý
                        if(j + 1 < this.streetsY)
                            outgoing[0] = this.horizontal[i, j + 1];
                        else {
                            if(this.topology == TrafficTopology.Cyclic)
                                outgoing[0] = this.horizontal[i, 0];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[0] = this.vertical[0, this.streetsY - i - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius) 
                                    outgoing[0] = this.horizontal[i + 1, j];
                        }
                    }
                    else {
                        if(j - 1 >= 0)
                            outgoing[0] = this.horizontal[i, j - 1];
                        else {
                            if(this.topology == TrafficTopology.Cyclic)
                                outgoing[0] = this.horizontal[i, this.streetsY - 1];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[0] = this.vertical[this.streetsX - 1, this.streetsY - i - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius) {
                                if(i + 1 < this.streetsX)
                                    outgoing[0] = this.horizontal[i + 1, j];
                                else
                                    outgoing[0] = this.vertical[i, j];
                            }
                        }
                    }

                    incomming[1]= this.vertical[i, j];
                    if(j % 2 == 0) { // Smìr zdola - nahoru
                        if(i - 1 >= 0)
                            outgoing[1] = this.vertical[i - 1, j];
                        else {
                            if(this.topology == TrafficTopology.Cyclic)
                                outgoing[1] = this.vertical[this.streetsX - 1, j];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[1] = this.horizontal[this.streetsX - j - 1, this.streetsY - 1];
                            else if(this.topology == TrafficTopology.SingleMoebius)
                                outgoing[1] = this.vertical[i, j + 1];
                        }
                    }
                    else {
                        if(i + 1 < this.streetsX)
                            outgoing[1] = this.vertical[i + 1, j];
                        else {
                            if(this.topology == TrafficTopology.Cyclic)
                                outgoing[1] = this.vertical[0, j];
                            else if(this.topology == TrafficTopology.SimpleMoebius)
                                outgoing[1] = this.horizontal[this.streetsX - j - 1, 0];
                            else if(this.topology == TrafficTopology.SingleMoebius) {
                                if(j + 1 < this.streetsY)
                                    outgoing[1] = this.vertical[i, j + 1];
                                else
                                    outgoing[1] = this.horizontal[0, 0];
                            }
                        }
                    }

                    this.crossing[i, j] = new Crossing(incomming, outgoing);
                    this.trafficItems.Add(this.crossing[i, j]);
                }
        }

        /// <summary>
        /// Nageneruje poèáteèní podmínky
        /// </summary>
        public void InitialCondition(double icx, double icy, ICType type) {
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
        /// Provede krok a vrátí stav jako matici
        /// </summary>
        public Matrix GetMatrix() {
            Matrix result = new Matrix((this.streetsX + 1) * (this.streetLengthX + 1), (this.streetsY + 1) * (this.streetLengthY + 1));
            result.Fill(-1.0);

            for(int i = 0; i < this.streetsX; i++)
                for(int j = 0; j < this.streetsY; j++) {
                    int offsetXh = (i + 1) * (this.streetLengthX + 1) - 1;
                    int offsetYh = (j + (i % 2 == 0 ? 0 : 1)) * (this.streetLengthY + 1);
                    Street sh = this.horizontal[i, j];
                    for(int h = 0; h < this.streetLengthY; h++)
                        if(i % 2 == 0)
                            result[offsetXh, offsetYh + h] = sh.Get(h);
                        else
                            result[offsetXh, offsetYh + h] = sh.GetReverse(h);

                    int offsetXv = (i + (j % 2 == 0 ? 1 : 0)) * (this.streetLengthX + 1);
                    int offsetYv = (j + 1) * (this.streetLengthY + 1) - 1;
                    Street sv = this.vertical[i, j];
                    for(int v = 0; v < this.streetLengthX; v++)
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
        /// Krok výpoètu
        /// </summary>
        public void Step() {
            foreach(TrafficItem item in this.trafficItems)
                item.Step();

            foreach(TrafficItem item in this.trafficItems)
                item.FinalizeStep();
        }

        /// <summary>
        /// Výpoèet po èas T
        /// </summary>
        public void Run(int time) {
            for(int t = 0; t < time; t++)
                this.Step();
        }
    }
}
