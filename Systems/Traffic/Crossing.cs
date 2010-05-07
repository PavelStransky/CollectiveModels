using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    public class Crossing: TrafficItem {
        private int length;
        private Street[] incomming;
        private Street[] outgoing;

        private int[] tolerance;

        private int state;
        private int greenTime;

        private int minGreen, maxTolerance;

        private int data, newData;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="incomming">Vstupující ulice</param>
        /// <param name="outgoing">Vystupující ulice</param>
        public Crossing(Street[] incomming, Street[] outgoing) {
            this.length = incomming.Length;

            this.incomming = incomming;
            this.outgoing = outgoing;

            this.tolerance = new int[this.length];

            foreach(Street street in this.incomming)
                street.SetEndCrosing(this);
            foreach(Street street in this.outgoing)
                street.SetBeginningCrossing(this);

            this.minGreen = 10;
            this.maxTolerance = 43;
        }

        /// <summary>
        /// True, pokud je na semaforu vstupujícím do ulice èervená
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
        /// True, pokud je na semaforu vystupujícím z ulice èervená
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
        /// Krok výpoètu
        /// </summary>
        public override void Step() {
            if(this.state >= 0)
                this.newData = Rule(184, incomming[this.state].Last, this.data, outgoing[this.state].First);
        }

        /// <summary>
        /// Dokonèí krok (pøepne semafor)
        /// </summary>
        public override void FinalizeStep() {
            this.data = this.newData;
            this.greenTime++;

            this.ChangeState();
        }

        /// <summary>
        /// Auto na køižovatce
        /// </summary>
        public int Data { get { return this.data; } }

        /// <summary>
        /// Stav køižovatky
        /// </summary>
        public int State { get { return this.state; } }

        /// <summary>
        /// Zmìna stavu køižovatky
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
            if(shortDistance > 0)
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
        /// Pøehodí semafor
        /// </summary>
        /// <param name="newState"></param>
        private void Switch(int newState) {
            if(this.state != newState) {
                this.state = newState;
                for(int i = 0; i < this.length; i++)
                    this.tolerance[i] = 0;
            }
        }
    }
}