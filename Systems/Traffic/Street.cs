using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class Street: TrafficItem, IExportable {
        private int length;
        private int[] data, newData;
        private int sensorDistance, shortDistance, shortDistanceStopped;
        private int changes;

        private Crossing beginningCrossing, endCrossing;
        private Random random = new Random();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">Délka ulice</param>
        public Street(int lengthMin, int lengthMax) {
            this.length = random.Next(lengthMin, lengthMax + 1);
            this.data = new int[this.length];

            // Defaultní hodnoty pro parametry systému
            this.sensorDistance = this.length / 2;
            this.shortDistance = 5;
            this.shortDistanceStopped = 2;
        }

        /// <summary>
        /// Nastaví parametry ulice
        /// </summary>
        /// <param name="sensorDistance">Vzdálenost sensoru od køižovatky</param>
        /// <param name="shortDistance">Blízkost køižovatky - pøijíždìjící smìr</param>
        /// <param name="shortDistanceStopped">Blízkost køižovatky - stojící auta za køižovatkou</param>
        public void SetParams(int sensorDistance, int shortDistance, int shortDistanceStopped) {
            if(sensorDistance >= 0)
                this.sensorDistance = sensorDistance;

            if(shortDistance >= 0)
                this.shortDistance = shortDistance;

            if(shortDistanceStopped >= 0)
                this.shortDistanceStopped = shortDistanceStopped;
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
        /// Vybere auta z druhé pozice ulice a tyto pozice vyprázdní
        /// </summary>
        public int RandomCollect() {
            int result = this.data[1];
            this.data[1] = 0;
            return result;
        }

        /// <summary>
        /// Pokusí se pøidat na druhou pozici ulice auto; pokud tam už auto je, nic nepøidá a vrátí 0
        /// </summary>
        /// <returns>Poèet pøidaných aut</returns>
        public int RandomAdd() {
            if(this.data[1] == 0) {
                this.data[1] = 1;
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Nastaví poèáteèní semafor
        /// </summary>
        public void SetBeginningCrossing(Crossing crossing) {
            this.beginningCrossing = crossing;
        }

        /// <summary>
        /// Nastaví koncový semafor
        /// </summary>
        public void SetEndCrosing(Crossing crossing) {
            this.endCrossing = crossing;
        }

        /// <summary>
        /// První pozice ulice
        /// </summary>
        public int First { get { return this.data[0]; } }

        /// <summary>
        /// Støed ulice
        /// </summary>
        public int Middle { get { return this.data[this.length / 2]; } }

        /// <summary>
        /// Poslední pozice ulice
        /// </summary>
        public int Last { get { return this.data[this.length - 1]; } }

        /// <summary>
        /// Délka ulice
        /// </summary>
        public int Length { get { return this.length; } }

        /// <summary>
        /// Vrátí prvek na dané pozici
        /// </summary>
        public int Get(int i) {
            if(i < this.length)
                return this.data[i];
            else
                return -1;
        }

        /// <summary>
        /// Poèet aut na dané ulici
        /// </summary>
        public override int CarNumber() {
            int result = 0;
            for(int i = 0; i < this.length; i++)
                result += this.data[i];
            return result;
        }

        /// <summary>
        /// Vrátí prvek na dané pozici od konce ulice
        /// </summary>
        public int GetReverse(int i) {
            if(i < this.length)
                return this.data[this.length - i - 1];
            else
                return -1;
        }

        /// <summary>
        /// Dokonèí krok
        /// </summary>
        public override void FinalizeStep() {
            this.changes = 0;
            for(int i = 0; i < this.length; i++)
                if(this.data[i] != this.newData[i])
                    this.changes++;
            this.data = this.newData;
        }

        /// <summary>
        /// Poèet zmìn v posledním kroku
        /// </summary>
        public override int Changes { get { return this.changes; } }

        /// <summary>
        /// Poèet aut za detektorem
        /// </summary>
        public int WatchingDistanceCars() {
            int result = 0;

            for(int i = this.length - this.sensorDistance; i < this.length; i++)
                result += this.data[i];

            return result;
        }

        /// <summary>
        /// Poèet aut tìsnì pøed køižovatkou
        /// </summary>
        public int ShortDistanceCars() {
            int result = 0;

            for(int i = this.length - this.shortDistance; i < this.length; i++)
                result += this.data[i];

            return result;
        }

        /// <summary>
        /// Poèet aut, která stojí na zaèátku ulice
        /// </summary>
        public bool ShortDistanceStopped() {
            for(int i = 1; i < this.shortDistanceStopped; i++)
                if(this.data[i - 1] == 1 && this.data[i] == 1)
                    return true;

            return false;
        }

        /// <summary>
        /// Vymaže všechna auta z ulice
        /// </summary>
        public override void Clear() {
            for(int i = 0; i < this.length; i++)
                this.data[i] = 0;
        }

        /// <summary>
        /// Pøidá auto na náhodnì generovanou pozici
        /// </summary>
        /// <returns>True, pokud se pøidání podaøilo; False, pokud na náhodné pozici již auto bylo</returns>
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
        /// Na dané ulici nageneruje auta
        /// </summary>
        /// <param name="number">Poèet aut</param>
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
 
        #region Implementace IExportable
        /// <summary>
        /// Uloží Traffic tøídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.length, "Length");
            param.Add(this.sensorDistance, "Sensor distance");
            param.Add(this.shortDistance, "Short distance");
            param.Add(this.shortDistanceStopped, "Short distance of stopped cars");

            // Auta
            for(int i = 0; i < this.length; i++)
                param.Add(this.data[i]);

            param.Export(export);
        }

        /// <summary>
        /// Naète Street tøídu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Street(Core.Import import) {
            IEParam param = new IEParam(import);

            this.length = (int)param.Get();
            this.sensorDistance = (int)param.Get(this.length / 2);
            this.shortDistance = (int)param.Get(2);
            this.shortDistanceStopped = (int)param.Get(2);

            this.data = new int[this.length];
            for(int i = 0; i < this.length; i++)
                this.data[i] = (int)param.Get();
        }
        #endregion
    }
}