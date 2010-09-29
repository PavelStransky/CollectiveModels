using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace PavelStransky.Systems {
    public class Randomizer: TrafficItem {
        private ArrayList randomStreets = new ArrayList();
        private Random random = new Random();

        /// <summary>
        /// Pøidá náhodnou ulici
        /// </summary>
        /// <param name="street">Ulice</param>
        public void Add(Street street) {
            this.randomStreets.Add(street);
        }

        /// <summary>
        /// Krok - náhodná výmìna aut na silnicích vstupujících do mìsta
        /// </summary>
        public override void FinalizeStep() {
            int cars = 0;
            foreach(Street street in this.randomStreets) 
                cars += street.RandomCollect();

            int count = this.randomStreets.Count;
            while(cars > 0) {
                int r = this.random.Next(count);
                cars -= (this.randomStreets[r] as Street).RandomAdd();
            }
        }

        public override void Step() { }

        public override int CarNumber() {
            return 0;
        }

        public override void Clear() { }

        public override int Changes { get { return 0; } }
    }
}
