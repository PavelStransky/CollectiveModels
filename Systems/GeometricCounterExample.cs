using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class GeometricCounterExample: IExportable {

        int type;

        public GeometricCounterExample(int type) {
            this.type = type;
        }

        public GeometricCounterExample(Core.Import import) {
            IEParam param = new IEParam(import);
            this.type = (int)param.Get(0);
        }

        public double V(double x, double y) {
            switch(this.type) {
                case 0: {
                        return x - y + x * x + y * y + 4.0 * x * y + x * x * x * x + y * y * y * y;
                    }
                case 1: {
                        return -1.0 / (1.0 + System.Math.Exp(0.5 * (x * x + y * y)));
                    }
                case 2: {
                        double yz = y * y - 1.0;
                        return yz * yz * yz + x * x * x * x;
                    }
                case 3: {
                        double yz = y * y - 1.0;
                        return yz * yz * yz + x * x * x * x + x * y;
                    }
                case 4: {
                    double rd = System.Math.Sqrt(x * x + y * y) - x0_4;
                    return rd * (5.0 + rd * (1.0 + rd * (4.0 + rd)));
                    }
                case 5: {
                    return x * (3.0 + x * (1.0 + x * (3.0 + x))) + y * (3.0 + y * (1.0 + y * (3.0 + y)));
                    }
            }
            return 0.0;
        }

        public double E(Vector v) {
            double x = v[0];
            double y = v[1];
            double px = v[2];
            double py = v[3];

            return 0.5 * (px * px + py * py) + V(x, y);
        }

        public Matrix VMatrix(double e, double x, double y) {
            Matrix result = new Matrix(2);

            switch(this.type) {
                case 0: {
                        double vx = 1.0 + 2.0 * x + 4.0 * x * x * x + 4.0 * y;
                        double vy = -1.0 + 4.0 * x + 2.0 * y + 4.0 * y * y * y;

                        double vxx = 2.0 + 12.0 * x * x;
                        double vxy = 4.0;
                        double vyy = 2.0 * 12 * y * y;

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }
                case 1: {
                        double ex = System.Math.Exp(0.5 * (x * x + y * y));
                        double j = 1.0 + ex;

                        double vx = x * ex / (j * j);
                        double vy = y * ex / (j * j);

                        double vxx = (ex * ex * (1.0 - x * x) + ex * (1.0 + x * x)) / (j * j * j);
                        double vxy = (ex - ex * ex) * x * y / (j * j * j);
                        double vyy = (ex * ex * (1.0 - y * y) + ex * (1.0 + y * y)) / (j * j * j);

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }
                case 2: {
                        double yz = y * y - 1.0;

                        double vx = 4.0 * x * x * x;
                        double vy = 6.0 * y * yz * yz;

                        double vxx = 12.0 * x * x;
                        double vxy = 0.0;
                        double vyy = 6.0 * (1 - 6.0 * y * y + 5.0 * y * y * y * y);

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }
                case 3: {
                        double yz = y * y - 1.0;

                        double vx = 4.0 * x * x * x + y;
                        double vy = 6.0 * y * yz * yz + x;

                        double vxx = 12.0 * x * x;
                        double vxy = 1.0;
                        double vyy = 6.0 * (1 - 6.0 * y * y + 5.0 * y * y * y * y);

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }
                case 4: {
                        double r = System.Math.Sqrt(x * x + y * y);
                        double rd = r - x0_4;

                        double vr = (5.0 + rd * (2.0 + rd * (12.0 + 4.0 * rd))) / r;
                        double vx = vr * x;
                        double vy = vr * y;

                        double vrr1 = 2.0 * (1.0 + rd * (12.0 + 6.0 * rd)) / (r*r);
                        double vrr2 = vr / (r*r);

                        double vxx = vrr1 * x * x + vrr2 * y * y;
                        double vxy = (vrr1 - vrr2) * x * y;
                        double vyy = vrr1 * y * y + vrr2 * x * x;

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }

                case 5: {
                        double vx = 3.0 + x * (2.0 + x * (9.0 + 4.0 * x));
                        double vy = 3.0 + y * (2.0 + y * (9.0 + 4.0 * y));

                        double vxx = 2.0 + x * (18.0 + 12.0 * x);
                        double vxy = 0;
                        double vyy = 2.0 + y * (18.0 + 12.0 * y);

                        double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

                        result[0, 0] = (a * vx * vx + vxx);
                        result[0, 1] = (a * vx * vy + vxy);
                        result[1, 0] = result[0, 1];
                        result[1, 1] = (a * vy * vy + vyy);
                        break;
                    }
            }

            return result;
        }

        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.type, "Type");
            param.Export(export);
            
        }

        private const double x0_4 = 2.973233745465326;
    }
}
