using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    public class Street: TrafficItem {
        private int length;
        private int[] data, newData;
        private int sensorDistance, shortDistance, shortDistanceStopped;

        private Crossing beginningCrossing, endCrossing;
        private Random random = new Random();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">D�lka ulice</param>
        public Street(int length) {
            this.length = length;
            this.data = new int[this.length];

            this.sensorDistance = length / 2;
            this.shortDistance = 2;
            this.shortDistanceStopped = 2;
        }

        /// <summary>
        /// Krok
        /// </summary>
        public override void Step() {
            this.newData = new int[this.length];

            if(this.beginningCrossing.OutgoingRed(this))
                this.newData[0] = Rule(136, this.beginningCrossing.Data, this.data[0], this.data[1]);
            else
                this.newData[0] = Rule(184, this.beginningCrossing.Data, this.data[0], this.data[1]);

            for(int i = 1; i < this.length - 1; i++)
                this.newData[i] = Rule(184, this.data[i - 1], this.data[i], this.data[i + 1]);

            if(this.endCrossing.IncommingRed(this))
                this.newData[this.length - 1] = Rule(252, this.data[this.length - 2], this.data[this.length - 1], this.endCrossing.Data);
            else
                this.newData[this.length - 1] = Rule(184, this.data[this.length - 2], this.data[this.length - 1], this.endCrossing.Data);
        }

        /// <summary>
        /// Nastav� po��te�n� semafor
        /// </summary>
        public void SetBeginningCrossing(Crossing crossing) {
            this.beginningCrossing = crossing;
        }

        /// <summary>
        /// Nastav� koncov� semafor
        /// </summary>
        public void SetEndCrosing(Crossing crossing) {
            this.endCrossing = crossing;
        }

        /// <summary>
        /// Prvn� pozice ulice
        /// </summary>
        public int First { get { return this.data[0]; } }

        /// <summary>
        /// Posledn� pozice ulice
        /// </summary>
        public int Last { get { return this.data[this.length - 1]; } }

        /// <summary>
        /// Vr�t� prvek na dan� pozici
        /// </summary>
        public int Get(int i) {
            return this.data[i];
        }

        /// <summary>
        /// Vr�t� prvek na dan� pozici od konce ulice
        /// </summary>
        public int GetReverse(int i) {
            return this.data[this.length - i - 1];
        }

        /// <summary>
        /// Dokon�� krok
        /// </summary>
        public override void FinalizeStep() {
            this.data = this.newData;
        }

        /// <summary>
        /// Po�et aut za detektorem
        /// </summary>
        public int WatchingDistanceCars() {
            int result = 0;

            for(int i = this.length - this.sensorDistance; i < this.length; i++)
                result += this.data[i];

            return result;
        }

        /// <summary>
        /// Po�et aut t�sn� p�ed k�i�ovatkou
        /// </summary>
        public int ShortDistanceCars() {
            int result = 0;

            for(int i = this.length - this.shortDistance; i < this.length; i++)
                result += this.data[i];

            return result;
        }

        /// <summary>
        /// Po�et aut, kter� stoj� na za��tku ulice
        /// </summary>
        public bool ShortDistanceStopped() {
            for(int i = 1; i < this.shortDistanceStopped; i++)
                if(this.data[i - 1] == 1 && this.data[i] == 1)
                    return true;

            return false;
        }

        /// <summary>
        /// Vyma�e v�echna auta z ulice
        /// </summary>
        public void Clear() {
            for(int i = 0; i < this.length; i++)
                this.data[i] = 0;
        }

        /// <summary>
        /// P�id� auto na n�hodn� generovanou pozici
        /// </summary>
        /// <returns>True, pokud se p�id�n� poda�ilo; False, pokud na n�hodn� pozici ji� auto bylo</returns>
        public bool AddCar() {
            int r = this.random.Next(this.length);
            if(this.data[r] == 0) {
                this.data[r] = 1;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Na dan� ulici nageneruje auta
        /// </summary>
        /// <param name="number">Po�et aut</param>
        public void GenerateNumber(int number) {
            this.Clear();

            int n = 0;

            while(n < number) {
                int r = this.random.Next(this.length);
                if(this.data[r] == 0) {
                    this.data[r] = 1;
                    n++;
                }
            }
        }

        public void Generate(double probability) {
            for(int i = 0; i < this.length; i++)
                if(this.random.NextDouble() < probability)
                    this.data[i] = 1;
                else
                    this.data[i] = 0;
        }
    }
}