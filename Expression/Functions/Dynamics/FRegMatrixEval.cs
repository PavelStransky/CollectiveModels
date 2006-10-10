using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z matice, kterou vr�t� SALIG, vypo��t� freg
    /// </summary>
    public class FRegMatrixEval : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is Matrix) {
                Matrix m = item as Matrix;

                int lengthX = m.LengthX;
                int lengthY = m.LengthY;

                // Po�ty trajektori�
                int regularT = 0;
                int totalT = 0;

                // Po�ty bod�
                int regularP = 0;
                int chaoticP = 0;
                int totalP = 0;

                // Celkov� regularita
                double total = 0;

                for(int i = 0; i < lengthX; i++)
                    for(int j = 0; j < lengthY; j++) {
                        double d = m[i, j];

                        if(d < 0) {
                            if(totalT == 0)
                                totalT = -(int)d;
                            else if(regularT == 0)
                                regularT = -(int)d;
                        }
                        else {
                            if(d == 0.0)
                                chaoticP++;
                            else if(d == 1.0)
                                regularP++;

                            totalP++;
                            total += d;
                        }
                    }

                Vector result = new Vector(6);
                result[0] = total / totalP;
                result[1] = lengthX * lengthY;
                result[2] = totalP;
                result[3] = regularP;
                result[4] = chaoticP;
                result[5] = totalT;
                result[6] = regularT;

                return result;
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
        }

        private const RungeKuttaMethods defaultRKMethod = RungeKuttaMethods.Normal;

        private const string help = "Vyhodnot� matici z�skanou pomoc� funkce SALIG a vr�t� vektor se slo�kami: (freg; celkov� po�et bod�; po�et bod� kinematicky dostupn� oblasti; po�et zcela regul�rn�ch bod�; po�et zcela chaotick�ch bod�; celkov� po�et trajektori�; po�et regul�rn�ch trajektori�";
        private const string parameters = "Dynamick� syst�m; po��te�n� podm�nky (x, y, vx, vy) (Vector) [; p�esnost v�po�tu (double)]]";
    }
}
