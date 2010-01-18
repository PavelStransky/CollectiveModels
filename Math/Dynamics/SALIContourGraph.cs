using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, která na základì SALI vytvoøí konturovaný graf 
    /// (Pouze pro speciální øezy rovinou x nebo y)
    /// </summary>
    public class SALIContourGraph: SALI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public SALIContourGraph(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(Messages.EMBadDimension);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precisionT">Pøesnost výsledku trajektorie</param>
        /// <param name="rkMethodT">Metoda k výpoètu trajektorie</param>
        /// <param name="precisionW">Pøesnost výsledku odchylek</param>
        /// <param name="rkMethodW">Metoda k výpoètu odchylek</param>
        public SALIContourGraph(IDynamicalSystem dynamicalSystem, double precisionT, RungeKuttaMethods rkMethodT, double precisionW, RungeKuttaMethods rkMethodW)
            : base(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(Messages.EMBadDimension);
        }

        /// <summary>
        /// Vrátí true, pokud daná trajektorie je podle SALI regulární
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="section">Body øezu (výstup)</param>
        public bool IsRegularPS(Vector initialX, PointVector poincareSection, bool isX) {
            int indexS = isX ? 0 : 1;    // Index promìnné, kterou vede øez
            int indexG1 = isX ? 1 : 0;   // První index pro graf
            int indexG2 = isX ? 3 : 2;   // Druhý index pro graf

            double oldy = initialX[indexS];

            ArrayList crossings = new ArrayList();

            double step = System.Math.Min(this.precisionT, this.precisionW);
            double timeStep = 1.0;
            double t = 0.0;
            double tNext = timeStep;

            MeanQueue queue = new MeanQueue(window);

            this.Init(initialX);

            int result = 0;
            do {
                while(t < tNext){
                    Vector oldx = this.x;

                    double newStep = this.Step(ref step);
                    t += step;
                    step = newStep;

                    double y = this.x[indexS];

                    // Druhá varianta jen v pøípadì, že øežeme X
                    if((y <= 0 && oldy > 0) || (!isX && y > 0 && oldy <= 0)) {
                        Vector v = (oldx - this.x) * (y / (y - oldy)) + oldx;
                        crossings.Add(new PointD(v[indexG1], v[indexG2]));
                    }
                    oldy = y;
                }

                result = this.SALIDecision(t, queue);
                if(result >= 0)
                    break;

                tNext += timeStep;
            } while(true);

            // Pøevod øady na PointVector
            poincareSection.Length = crossings.Count;
            int j = 0;
            foreach(PointD p in crossings)
                poincareSection[j++] = p;

            return (result == 1) ? true : false;
        }

        /// <summary>
        /// Vrátí true, pokud je daný graf celý regulární
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x grafu (interní parametr)</param>
        /// <param name="n2">Rozmìr vx výsledné matice (interní parametr)</param>
        /// <param name="writer">Výpis na konzoli</param>
        public bool IsRegularGraph(double e, int n1, int n2, bool isX, IOutputWriter writer) {
            // Výpoèet mezí
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            int indexS = isX ? 0 : 1;    // Index promìnné, kterou vede øez
            int indexG1 = isX ? 1 : 0;   // První index pro graf
            int indexG2 = isX ? 3 : 2;   // Druhý index pro graf

            // Koeficienty pro rychlý pøepoèet mezi indexy a souøadnicemi n = kx + x0
            double kx = (boundX[2 * indexG1 + 1] - boundX[2 * indexG1]) / (n1 - 1);
            double x0 = boundX[2 * indexG1];
            double ky = (boundX[2 * indexG2 + 1] - boundX[2 * indexG2]) / (n2 - 1);
            double y0 = boundX[2 * indexG2];

            // Poèáteèní podmínky
            Vector ic = new Vector(4);

            int[,] trPassed = new int[n1, n2];
            DateTime startTime = DateTime.Now;

            for(int i = 0; i < n1; i++) {
                if(writer != null)
                    writer.Write('.');

                for(int j = 0; j < n2; j++) {
                    // Na aktuálním bodì už máme spoèítanou trajektorii
                    if(trPassed[i, j] != 0)
                        continue;

                    if(isX) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;
                    }
                    else {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0;
                        ic[3] = double.NaN;
                    }
                    
                    if(this.dynamicalSystem.IC(ic, e)) {
                        PointVector section = new PointVector(0);

                        // Nalezli jsme neregulární trajektorii - konec výpoètu
                        if(!this.IsRegularPS(ic, section, isX)) {
                            writer.WriteLine(string.Format(" {0} (irregular)", SpecialFormat.Format(DateTime.Now - startTime)));
                            return false;
                        }

                        trPassed[i, j]++;

                        for(int k = 0; k < section.Length; k++) {
                            int n1x = (int)((section[k].X - x0) / kx);
                            int n2x = (int)((section[k].Y - y0) / ky);
                            if(n1x < n1 && n2x < n2) 
                                trPassed[n1x, n2x]++;                            
                        }
                    }
                }
            }

            writer.WriteLine(string.Format(" {0} (regular)", SpecialFormat.Format(DateTime.Now - startTime)));
            return true;
        }

        /// <summary>
        /// Vypoèítá a obarví Poincarého øez v promìnných X, Vx
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x výsledné matice</param>
        /// <param name="n2">Rozmìr vx výsledné matice</param>
        public ArrayList Compute(double e, int n1, int n2) {
            return this.Compute(e, n1, n2, false, null);
        }

        /// <summary>
        /// Poèítá po danou energii a pro øez X, Vx
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x výsledné matice</param>
        /// <param name="n2">Rozmìr vx výsledné matice</param>
        /// <param name="writer">Výpis na konzoli</param>
        public ArrayList Compute(double e, int n1, int n2, bool isX, IOutputWriter writer) {
            // Výpoèet mezí
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            int indexS = isX ? 0 : 1;    // Index promìnné, kterou vede øez
            int indexG1 = isX ? 1 : 0;   // První index pro graf
            int indexG2 = isX ? 3 : 2;   // Druhý index pro graf

            // Koeficienty pro rychlý pøepoèet mezi indexy a souøadnicemi n = kx + x0
            double kx = (boundX[2 * indexG1 + 1] - boundX[2 * indexG1]) / (n1 - 1);
            double x0 = boundX[2 * indexG1];
            double ky = (boundX[2 * indexG2 + 1] - boundX[2 * indexG2]) / (n2 - 1);
            double y0 = boundX[2 * indexG2];

            // Poèáteèní podmínky
            Vector ic = new Vector(4);

            int regular = 0;        // Poèet regulárních trajektorií
            int total = 0;          // Celkový poèet trajektorií
            Matrix m = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            DateTime startTime = DateTime.Now;

            for(int i = 0; i < n1; i++) {
                for(int j = 0; j < n2; j++) {
                    if((i * n2 + j + 1) % 100 == 0 && writer != null)
                        writer.Write('.');

                    // Na aktuálním bodì už máme spoèítanou trajektorii
                    if(trPassed[i, j] != 0)
                        continue;

                    if(isX) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;
                    }
                    else {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0;
                        ic[3] = double.NaN;
                    }
                   
                    if(this.dynamicalSystem.IC(ic, e)) {
                        PointVector section = new PointVector(0);
                        int sali = this.IsRegularPS(ic, section, isX) ? 1 : 0;
                        regular += sali;
                        total++;

                        bool[,] actPassed = new bool[n1, n2];

                        // Zaznamenáme i poèáteèní podmínku
                        m[i, j] = sali;
                        trPassed[i, j]++;
                        actPassed[i, j] = true;

                        for(int k = 0; k < section.Length; k++) {
                            int n1x = (int)((section[k].X - x0) / kx);
                            int n2x = (int)((section[k].Y - y0) / ky);
                            if(n1x < n1 && n2x < n2 && !actPassed[n1x, n2x]) {
                                m[n1x, n2x] += sali;
                                trPassed[n1x, n2x]++;
                                actPassed[n1x, n2x] = true;
                            }
                        }
                    }
                }

                if((i + 1) % 10 == 0 && writer != null) {
                    writer.WriteLine(string.Format("{0} ({1}, {2})", SpecialFormat.Format(DateTime.Now - startTime), total, regular));
                }
            }

            // Number of regular points
            double nreg = 0.0;

            // Total number of points
            int ntot = 0;

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++) {
                    if(trPassed[i, j] != 0) {
                        m[i, j] /= trPassed[i, j];
                        ntot++;
                        nreg += m[i, j];
                    }
                    else
                        m[i, j] = -1;
                }

            ArrayList result = new ArrayList();
            result.Add(m);
            result.Add(boundX);
            result.Add(total);
            result.Add(regular);
            result.Add(ntot);
            result.Add(nreg);

            return result;
        }
    }
}
