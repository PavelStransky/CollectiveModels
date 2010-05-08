using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class Crossing: TrafficItem, IExportable {
        private int length;
        private Street[] incomming;
        private Street[] outgoing;

        private int[] tolerance;

        private int state;
        private int greenTime;

        private int minGreen, maxTolerance, cutPlatoon;

        private int data, newData, changed;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="incomming">Vstupuj�c� ulice</param>
        /// <param name="outgoing">Vystupuj�c� ulice</param>
        public Crossing(Street[] incomming, Street[] outgoing)
            : this(incomming.Length) {
            this.SetStreets(incomming, outgoing);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">Po�et k��en�</param>
        public Crossing(int length) {
            this.length = length;
            this.tolerance = new int[this.length];

            this.minGreen = 10;
            this.maxTolerance = 43;
            this.cutPlatoon = 2;
        }

        /// <summary>
        /// Nastav� vstupuj�c� a vystupuj�c� ulice
        /// </summary>
        /// <param name="incomming">Vstupuj�c� ulice</param>
        /// <param name="outgoing">Vystupuj�c� ulice</param>
        public void SetStreets(Street[] incomming, Street[] outgoing) {
            this.incomming = incomming;
            this.outgoing = outgoing;

            foreach(Street street in this.incomming)
                street.SetEndCrosing(this);
            foreach(Street street in this.outgoing)
                street.SetBeginningCrossing(this);
        }

        /// <summary>
        /// Nastav� parametry k�i�ovatky
        /// </summary>
        /// <param name="minGreen">Minim�ln� doba zelen� (pravidlo 2)</param>
        /// <param name="maxTolerance">Maxim�ln� tolerance (pravidlo 1)</param>
        /// <param name="cutPlatton">Maxim�ln� po�et aut v bl�zk� vzd�lenosti (�ez�n� �ady, pravidlo 3)</param>
        public void SetParams(int minGreen, int maxTolerance, int cutPlatton) {
            if(minGreen >= 0)
                this.minGreen = minGreen;

            if(maxTolerance >= 0)
                this.maxTolerance = maxTolerance;

            if(cutPlatoon >= 0)
                this.cutPlatoon = cutPlatoon;
        }

        /// <summary>
        /// True, pokud je na semaforu vstupuj�c�m do ulice �erven�
        /// </summary>
        /// <param name="street">Ulice</param>
        public bool IncommingRed(Street street) {
            for(int i = 0; i < this.length; i++)
                if(this.incomming[i] == street)
                    if(i == this.state)
                        return false;
                    else
                        return true;
            return true;
        }

        /// <summary>
        /// True, pokud je na semaforu vystupuj�c�m z ulice �erven�
        /// </summary>
        /// <param name="street">Ulice</param>
        public bool OutgoingRed(Street street) {
            for(int i = 0; i < this.length; i++)
                if(this.outgoing[i] == street)
                    if(i == this.state)
                        return false;
                    else
                        return true;
            return true;
        }

        /// <summary>
        /// Po�et aut na k�i�ovatce
        /// </summary>
        public override int CarNumber() {
            return this.data;
        }

        /// <summary>
        /// Krok v�po�tu
        /// </summary>
        public override void Step() {
            if(this.state >= 0)
                this.newData = Rule(184, incomming[this.state].Last, this.data, outgoing[this.state].First);
        }

        /// <summary>
        /// Dokon�� krok (p�epne semafor)
        /// </summary>
        public override void FinalizeStep() {
            if(this.data != this.newData)
                this.changed = 1;
            else
                this.changed = 0;

            this.data = this.newData;
            this.greenTime++;

            this.ChangeState();
        }

        /// <summary>
        /// Vyma�e auta z k�i�ovatky
        /// </summary>
        public override void Clear() {
            this.data = 0;
        }

        /// <summary>
        /// Po�et zm�n
        /// </summary>
        public override int Changes { get { return this.changed; } }

        /// <summary>
        /// Auto na k�i�ovatce
        /// </summary>
        public int Data { get { return this.data; } }

        /// <summary>
        /// Stav k�i�ovatky
        /// </summary>
        public int State { get { return this.state; } }

        /// <summary>
        /// Zm�na stavu k�i�ovatky
        /// </summary>
        private void ChangeState() {
            int[] watchingDistanceCars = new int[this.length];
            bool[] shortDistanceStopped = new bool[this.length];

            for(int i = 0; i < this.length; i++) {
                watchingDistanceCars[i] = this.incomming[i].WatchingDistanceCars();
                shortDistanceStopped[i] = this.outgoing[i].ShortDistanceStopped();
                this.tolerance[i] += watchingDistanceCars[i];
            }

            // Crossing occupied
            if(this.data == 1)
                return;

            int directionToGo = this.state;
            int maxTolerance = 0;

            // Rule 5, 6 - blocked outgoing street
            if(this.state < 0 || shortDistanceStopped[this.state]) {
                directionToGo = -1;
                maxTolerance = 0;

                for(int i = 0; i < this.length; i++)
                    if(!shortDistanceStopped[i] && watchingDistanceCars[i] != 0) {
                        if(this.tolerance[i] > maxTolerance) {
                            maxTolerance = this.tolerance[i];
                            directionToGo = i;
                        }
                    }

                this.Switch(directionToGo);
                return;
            }

            // Rule 4 - no cars in the green street
            if(watchingDistanceCars[this.state] == 0) {
                directionToGo = this.state;
                maxTolerance = 0;

                for(int i = 0; i < this.length; i++) {
                    if(i == this.state)
                        continue;

                    if(!shortDistanceStopped[i] && watchingDistanceCars[i] != 0) {
                        if(this.tolerance[i] > maxTolerance) {
                            maxTolerance = this.tolerance[i];
                            directionToGo = i;
                        }
                    }
                }

                this.Switch(directionToGo);
                return;
            }

            // Rule 2 - minimal time of the green light
            if(this.greenTime <= this.minGreen)
                return;

            int shortDistance = this.incomming[this.state].ShortDistanceCars();

            // Rule 3 - number of the incomming cars in the vicinity of the crossroad
            if(shortDistance <= this.cutPlatoon)
                return;

            // Rule 1
            directionToGo = this.state;
            maxTolerance = 0;

            for(int i = 0; i < this.length; i++) {
                if(i == this.state)
                    continue;

                if(!shortDistanceStopped[i] && watchingDistanceCars[i] != 0) {
                    if(this.tolerance[i] > maxTolerance) {
                        maxTolerance = this.tolerance[i];
                        directionToGo = i;
                    }
                }
            }

            if(maxTolerance >= this.maxTolerance)
                this.Switch(directionToGo);
        }

        /// <summary>
        /// P�ehod� semafor
        /// </summary>
        /// <param name="newState"></param>
        private void Switch(int newState) {
            if(this.state != newState) {
                this.state = newState;
                for(int i = 0; i < this.length; i++)
                    this.tolerance[i] = 0;
            }
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� Traffic t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.length, "Length");
            param.Add(this.state, "State of the traffic lights");
            param.Add(this.data, "Car in the crossing");
            param.Add(this.greenTime, "Green time");
            param.Add(this.minGreen, "Minimum green time");
            param.Add(this.maxTolerance, "Maximum tolerance");

            // Tolerance
            for(int i = 0; i < this.length; i++)
                param.Add(this.tolerance[i]);

            param.Add(this.cutPlatoon, "Cut platoon");

            param.Export(export);
        }

        /// <summary>
        /// Na�te Crossing t��du ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Crossing(Core.Import import) {
            IEParam param = new IEParam(import);

            this.length = (int)param.Get();
            this.state = (int)param.Get();
            this.data = (int)param.Get();
            this.greenTime = (int)param.Get();
            this.minGreen = (int)param.Get();
            this.maxTolerance = (int)param.Get();

            // Tolerance
            this.tolerance = new int[this.length];
            for(int i = 0; i < this.length; i++)
                this.tolerance[i] = (int)param.Get();

            this.cutPlatoon = (int)param.Get(2);
        }
        #endregion
    }
}