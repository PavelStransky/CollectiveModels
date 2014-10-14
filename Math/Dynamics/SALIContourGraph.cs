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
        /// Vrátí èíslo mezi 0 a 1 podle regularity trajektorie
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="sali">Závislost SALI na èase (výstup)</param>
        /// <param name="section">Body øezu (výstup)</param>
        public double SALIPoincareSection(Vector initialX, PointVector poincareSection, PointVector sali) {
            return this.SALIPoincareSection(initialX, poincareSection, sali, 1, 0, false);
        }
        
        /// <summary>
        /// Vrátí èíslo mezi 0 a 1 podle regularity trajektorie
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="section">Body øezu (výstup)</param>
        /// <param name="sali">Závislost SALI na èase (výstup)</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        public double SALIPoincareSection(Vector initialX, PointVector poincareSection, PointVector sali, int sectionS) {
            return this.SALIPoincareSection(initialX, poincareSection, sali, sectionS, 0, false);
        }

        /// <summary>
        /// Vrátí èíslo mezi 0 a 1 podle regularity trajektorie
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="section">Body øezu (výstup)</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        /// <param name="oneOrientation">True, pokud poèítáme jen jednu orientaci prùchodu rovinou</param>
        public double SALIPoincareSection(Vector initialX, PointVector poincareSection, PointVector sali, int sectionS, bool oneOrientation) {
            return this.SALIPoincareSection(initialX, poincareSection, sali, sectionS, 0, oneOrientation);
        }
        
        /// <summary>
        /// Vrátí èíslo mezi 0 a 1 podle regularity trajektorie a výpoèet skonèí nejdøíve po dosažení numPointsSection prùchodù rovinou
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="section">Body øezu (výstup)</param>
        /// <param name="sali">Závislost SALI na èase (výstup)</param>
        /// <param name="numPointsSection">Minimální poèet bodù øezu</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        public double SALIPoincareSection(Vector initialX, PointVector poincareSection, PointVector sali, int sectionS, int numPointsSection) {
            return this.SALIPoincareSection(initialX, poincareSection, sali, sectionS, numPointsSection, false);
        }
        
        /// <summary>
        /// Vrátí èíslo mezi 0 a 1 podle regularity trajektorie a výpoèet skonèí nejdøíve po dosažení numPointsSection prùchodù rovinou
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="poincareSection">Body øezu (výstup)</param>
        /// <param name="sali">Závislost SALI na èase (výstup)</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        /// <param name="numPointsSection">Minimální poèet bodù øezu</param>
        /// <param name="oneOrientation">True, pokud poèítáme jen jednu orientaci prùchodu rovinou</param>
        public double SALIPoincareSection(Vector initialX, PointVector poincareSection, PointVector sali, int sectionS, int numPointsSection, bool oneOrientation) {
            int indexG1 = sectionS == 0 || sectionS == 2 ? 1 : 0;   // První index pro graf
            int indexG2 = sectionS == 0 || sectionS == 2 ? 3 : 2;   // Druhý index pro graf

            double oldy = initialX[sectionS];

            ArrayList crossings = new ArrayList();
            ArrayList salia = new ArrayList();

            double step = System.Math.Min(this.precisionT, this.precisionW);
            double timeStep = 1.0;
            double t = 0.0;
            double tNext = timeStep;
            bool postProcessed = false;

            MeanQueue queue = new MeanQueue(window);

            this.Init(initialX);

            double result = 0.0;
            // Zatím k nièemu, mohlo by sloužit k pøenormovávání SALI (pro velmi rychlý, nebo naopak pomalý pohyb)
            Vector lengths = new Vector(this.X.Length);
            int numPoints = 0;

            do {
                while(t < tNext){
                    Vector oldx = this.X;

                    double newStep = this.Step(ref step);
                    t += step;
                    step = newStep;

                    lengths += (this.X - oldx).Abs();

                    double y = this.X[sectionS];

                    if(!postProcessed)
                    // Druhá varianta jen v pøípadì, že øežeme X
                    if((y <= 0 && oldy > 0) || (!oneOrientation && y > 0 && oldy <= 0)) {
                        Vector v = (oldx - this.X) * (oldy / (y - oldy)) + oldx;
                        crossings.Add(new PointD(v[indexG1], v[indexG2]));
                        numPoints++;
                    }
                    oldy = y;

                    postProcessed = this.dynamicalSystem.PostProcess(this.X);
                }

                salia.Add(new PointD(t, this.AlignmentIndex()));
                result = this.SALIDecision(t, queue);

                tNext += timeStep;
            } while(result < 0 || numPoints < numPointsSection);

            // Pøevod øady na PointVector
            poincareSection.Length = crossings.Count + 1;
            int j = 0;
            foreach(PointD p in crossings)
                poincareSection[j++] = p;

            poincareSection[j] = new PointD(lengths.MaxIndex(), lengths.Max() / t);

            sali.Length = salia.Count;
            j = 0;
            foreach(PointD p in salia)
                sali[j++] = p;

            return result;
        }

        /// <summary>
        /// Vrátí true, pokud je daný graf celý regulární
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x grafu (interní parametr)</param>
        /// <param name="n2">Rozmìr vx výsledné matice (interní parametr)</param>
        /// <param name="writer">Výpis na konzoli</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        public bool IsRegularGraph(double e, int n1, int n2, int sectionS, IOutputWriter writer) {
            // Výpoèet mezí
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            int indexG1 = sectionS == 0 || sectionS == 2 ? 1 : 0;   // První index pro graf
            int indexG2 = sectionS == 0 || sectionS == 2 ? 3 : 2;   // Druhý index pro graf

            int nan = 0;
            if(sectionS == 0)
                nan = 2;
            else if(sectionS == 2)
                nan = 0;
            else if(sectionS == 1)
                nan = 3;

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

                    ic[sectionS] = 0.0;
                    ic[indexG1] = kx * i + x0;
                    ic[indexG2] = ky * j + y0;
                    ic[nan] = double.NaN;
                   
                    if(this.dynamicalSystem.IC(ic, e)) {
                        PointVector section = new PointVector(0);
                        PointVector sali = new PointVector(0);

                        // Nalezli jsme neregulární trajektorii - konec výpoètu
                        if(this.SALIPoincareSection(ic, section, sali, sectionS) < 0.5) {
                            writer.WriteLine(string.Format(" {0} (irregular)", SpecialFormat.Format(DateTime.Now - startTime)));
                            return false;
                        }

                        trPassed[i, j]++;

                        for(int k = 0; k < section.Length - 1; k++) {
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
            return this.Compute(e, n1, n2, 1, false, null);
        }

        /// <summary>
        /// Poèítá po danou energii a pro øez X, Vx
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x výsledné matice</param>
        /// <param name="n2">Rozmìr vx výsledné matice</param>
        /// <param name="writer">Výpis na konzoli</param>
        /// <param name="sectionS">Index promìnné, kterou vede øez</param>
        /// <param name="oneOrientation">True, pokud poèítáme jen jednu orientaci prùchodu rovinou</param>
        public ArrayList Compute(double e, int n1, int n2, int sectionS, bool oneOrientation, IOutputWriter writer) {
            // Výpoèet mezí
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            int indexG1 = sectionS == 0 || sectionS == 2 ? 1 : 0;   // První index pro graf
            int indexG2 = sectionS == 0 || sectionS == 2 ? 3 : 2;   // Druhý index pro graf

            int nan = 0;
            if(sectionS == 0)
                nan = 2;
            else if(sectionS == 2)
                nan = 0;
            else if(sectionS == 1)
                nan = 3;

            // Koeficienty pro rychlý pøepoèet mezi indexy a souøadnicemi n = kx + x0
            double kx = (boundX[2 * indexG1 + 1] - boundX[2 * indexG1]) / n1;
            double x0 = boundX[2 * indexG1];
            double ky = (boundX[2 * indexG2 + 1] - boundX[2 * indexG2]) / n2;
            double y0 = boundX[2 * indexG2];

            double regular = 0;     // Skóre regularity trajektorií
            int total = 0;          // Celkový poèet trajektorií
            Matrix m = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            DateTime startTime = DateTime.Now;

            ArrayList sections = new ArrayList();
            ArrayList salis = new ArrayList();
            ArrayList salits = new ArrayList();
            ArrayList ics = new ArrayList();
            ArrayList maxLength = new ArrayList();

            for(int i = 0; i < n1; i++) {
                for(int j = 0; j < n2; j++) {
                    if((i * n2 + j + 1) % 100 == 0 && writer != null)
                        writer.Write('.');

                    // Na aktuálním bodì už máme spoèítanou trajektorii
                    if(trPassed[i, j] != 0)
                        continue;

                    // Poèáteèní podmínky
                    Vector ic = new Vector(4);
                    ic[sectionS] = 0.0;
                    ic[indexG1] = kx * (i + 0.5) + x0;
                    ic[indexG2] = ky * (j + 0.5) + y0;
                    ic[nan] = double.NaN;
                   
                    if(this.dynamicalSystem.IC(ic, e)) {
                        PointVector section = new PointVector(0);
                        PointVector salit = new PointVector(0);

                        double sali = this.SALIPoincareSection(ic, section, salit, sectionS, oneOrientation);
                        regular += sali;
                        total++;

                        bool[,] actPassed = new bool[n1, n2];

                        // Délka trajektorie - uchována neintuitivnì v poli section jako poslední prvek
                        int mi = (int)section.LastItem.X;
                        maxLength.Add(section.LastItem.Y * (boundX[2 * mi + 1] - boundX[2 * mi]));
                        section.Length--;

                        sections.Add(section);
                        salis.Add(sali);
                        salits.Add(salit);
                        ics.Add(ic);

                        // Zaznamenáme i poèáteèní podmínku
                        m[i, j] = sali;
                        trPassed[i, j]++;
                        actPassed[i, j] = true;

                        for(int k = 0; k < section.Length; k++) {
                            int n1x = (int)((section[k].X - x0) / kx);
                            int n2x = (int)((section[k].Y - y0) / ky);
                            if(n1x >= 0 && n2x >= 0 && n1x < n1 && n2x < n2 && !actPassed[n1x, n2x]) {
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
            result.Add(m);          // 0
            result.Add(boundX);     // 1
            result.Add(total);      // 2
            result.Add(regular);    // 3
            result.Add(ntot);       // 4
            result.Add(nreg);       // 5

            Vector saliv = new Vector(salis.Count);
            int l = 0;
            foreach(double sali in salis)
                saliv[l++] = sali;
            result.Add(saliv);      // 6

            Vector maxLengthv = new Vector(maxLength.Count);
            l = 0;
            foreach(double max in maxLength)
                maxLengthv[l++] = max;
            result.Add(maxLengthv); // 7

            result.Add(ics);        // 8
            result.Add(sections);   // 9
            result.Add(salits);     // 10

            return result;
        }
    }
}
