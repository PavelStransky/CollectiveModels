using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z matice, kterou vrátí SALIG, vypoèítá freg
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

                // Poèty trajektorií
                int regularT = 0;
                int totalT = 0;

                // Poèty bodù
                int regularP = 0;
                int chaoticP = 0;
                int totalP = 0;

                // Celková regularita
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

        private const string help = "Vyhodnotí matici získanou pomocí funkce SALIG a vrátí vektor se složkami: (freg; celkový poèet bodù; poèet bodù kinematicky dostupné oblasti; poèet zcela regulárních bodù; poèet zcela chaotických bodù; celkový poèet trajektorií; poèet regulárních trajektorií";
        private const string parameters = "Dynamický systém; poèáteèní podmínky (x, y, vx, vy) (Vector) [; pøesnost výpoètu (double)]]";
    }
}
