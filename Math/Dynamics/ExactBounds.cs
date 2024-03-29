using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    public class ExactBounds {
        /// <summary>
        /// Zp�esn�n� mez� pro v�po�et
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        /// <param name="numIterations">Po�et iterac� pro zp�esn�n�</param>
        public static Vector ComputeExactBounds(IDynamicalSystem dynamicalSystem, double e, int n1, int n2, int numIterations) {
            // Z�kladn� p�ibli�n� meze
            Vector boundX = dynamicalSystem.Bounds(e);

            // Doln� meze n�sob�me 1.2, pokud jsou z�porn�, 0.8, pokud jsou kladn�
            for(int i = 0; i < boundX.Length; i += 2) 
                if(boundX[i] > 0.0)
                    boundX[i] *= 0.8;
                else
                    boundX[i] *= 1.2;

            // Horn� meze obr�cen�
            for(int i = 1; i < boundX.Length; i += 2)
                if(boundX[i] > 0.0)
                    boundX[i] *= 1.2;
                else
                    boundX[i] *= 0.8;

            // N�kolikr�t iterujeme pro zp�esn�n� mez� (meze X, Vx)
            for(int iteration = 0; iteration < numIterations; iteration++) {
                // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
                double kx = (boundX[1] - boundX[0]) / (n1 - 1);
                double x0 = boundX[0];
                double ky = (boundX[5] - boundX[4]) / (n2 - 1);
                double y0 = boundX[4];

                // Po��te�n� podm�nky
                Vector ic = new Vector(4);

                // Hled�n� optim�ln� oblasti
                bool foundIC = false;

                // Sm�r 1
                for(int i = 1; i < n1; i++) {
                    for(int j = 1; j < n2; j++) {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                        ic[3] = double.NaN;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[0] = kx * (i - 2) + x0;
                        break;
                    }
                }

                kx = (boundX[1] - boundX[0]) / (n1 - 1);
                x0 = boundX[0];

                foundIC = false;

                // Sm�r 4
                for(int j = 1; j < n2; j++) {
                    for(int i = 1; i < n1; i++) {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                        ic[3] = double.NaN;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[4] = ky * (j - 2) + y0;
                        break;
                    }
                }

                ky = (boundX[5] - boundX[4]) / (n2 - 1);
                y0 = boundX[4];

                foundIC = false;

                // Sm�r 3
                for(int i = n2 - 2; i >= 0; i--) {
                    for(int j = 1; j < n2; j++) {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                        ic[3] = double.NaN;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[1] = kx * (i + 1) + x0;
                        break;
                    }
                }

                kx = (boundX[1] - boundX[0]) / (n1 - 1);
                x0 = boundX[0];

                foundIC = false;

                // Sm�r 2
                for(int j = n2 - 2; j >= 0; j--) {
                    for(int i = 1; i < n1; i++) {
                        ic[0] = kx * i + x0;
                        ic[1] = 0.0;
                        ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                        ic[3] = double.NaN;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[5] = ky * (j + 1) + y0;
                        break;
                    }
                }
            }

            // N�kolikr�t iterujeme pro zp�esn�n� mez� (meze Y, Vy)
            for(int iteration = 0; iteration < numIterations; iteration++) {
                // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
                double kx = (boundX[3] - boundX[2]) / (n1 - 1);
                double x0 = boundX[2];
                double ky = (boundX[7] - boundX[6]) / (n2 - 1);
                double y0 = boundX[6];

                // Po��te�n� podm�nky
                Vector ic = new Vector(4);

                // Hled�n� optim�ln� oblasti
                bool foundIC = false;

                // Sm�r 1
                for(int i = 1; i < n1; i++) {
                    for(int j = 1; j < n2; j++) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[2] = kx * (i - 2) + x0;
                        break;
                    }
                }

                kx = (boundX[3] - boundX[2]) / (n1 - 1);
                x0 = boundX[2];

                foundIC = false;

                // Sm�r 4
                for(int j = 1; j < n2; j++) {
                    for(int i = 1; i < n1; i++) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[6] = ky * (j - 2) + y0;
                        break;
                    }
                }

                ky = (boundX[7] - boundX[6]) / (n2 - 1);
                y0 = boundX[6];

                foundIC = false;

                // Sm�r 3
                for(int i = n2 - 2; i >= 0; i--) {
                    for(int j = 1; j < n2; j++) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[3] = kx * (i + 1) + x0;
                        break;
                    }
                }

                kx = (boundX[3] - boundX[2]) / (n1 - 1);
                x0 = boundX[2];

                foundIC = false;

                // Sm�r 2
                for(int j = n2 - 2; j >= 0; j--) {
                    for(int i = 1; i < n1; i++) {
                        ic[0] = 0.0;
                        ic[1] = kx * i + x0;
                        ic[2] = double.NaN;
                        ic[3] = ky * j + y0;

                        if(dynamicalSystem.IC(ic, e)) {
                            foundIC = true;
                            break;
                        }
                    }
                    if(foundIC) {
                        boundX[7] = ky * (j + 1) + y0;
                        break;
                    }
                }
            }

            return dynamicalSystem.CheckBounds(boundX);
        }
    }
}
