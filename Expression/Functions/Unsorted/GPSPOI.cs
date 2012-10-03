using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds the Points of Interest that are near the given path
    /// </summary>
    public class GPSPOI: Fnc {
        public override string Help { get { return Messages.HelpGPS; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PTrack, Messages.PTrackDescription, null, typeof(TArray));
            this.SetParam(1, true, true, false, Messages.PPOI, Messages.PPOIDescription, null, typeof(TArray));
            this.SetParam(2, false, true, true, Messages.PMaxDistance, Messages.PMaxDistanceDescription, 100, typeof(double));
            this.SetParam(3, false, true, true, Messages.PDistanceOnePoint, Messages.PDistanceOnePointDescription, 1000, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray track = arguments[0] as TArray;
            TArray poi = arguments[1] as TArray;
            double maxDistance = (double)arguments[2];
            double distanceOnePoint = (double)arguments[3];

            bool isZ = track.Length > 2;

            Vector trackx = track[0] as Vector;
            Vector tracky = track[1] as Vector;
            Vector trackz = isZ ? track[2] as Vector : null;

            Vector poix = poi[0] as Vector;
            Vector poiy = poi[1] as Vector;
            Vector poiz = isZ ? poi[2] as Vector : null;

            int numPoi = poix.Length;
            int length = trackx.Length;

            // Distances within the track
            Vector trackLength = new Vector(length);
            for(int i = 1; i < length; i++)
                trackLength[i] = trackLength[i - 1] + this.Distance(trackx[i], tracky[i], isZ ? trackz[i] : 0.0, trackx[i - 1], tracky[i - 1], isZ ? trackz[i - 1] : 0.0);

            ArrayList point = new ArrayList();
            ArrayList index = new ArrayList();
            ArrayList distance = new ArrayList();

            for(int i = 0; i < numPoi; i++) {
                Vector distPoi = new Vector(length);

                for(int j = 0; j < length; j++)
                    distPoi[j] = this.Distance(trackx[j], tracky[j], isZ ? trackz[j] : 0.0, poix[i], poiy[i], isZ ? poiz[i] : 0.0);

                int lastFound = -1;
                for(int j = 0; j < length; j++) {
                    if(distPoi[j] < maxDistance) {
                        double min = distPoi[j];
                        int mini = j;
                        int k = j;
                        while(k < length && (distPoi[k] < maxDistance || trackLength[k] - trackLength[mini] < distanceOnePoint)) {
                            if(distPoi[k] < min) {
                                min = distPoi[k];
                                mini = k;
                            }
                            k++;
                        }

                        if(lastFound < 0 || trackLength[mini] - trackLength[lastFound] > distanceOnePoint) {
                            point.Add(i);
                            index.Add(mini);
                            distance.Add(min);
                            lastFound = mini;
                        }
                    }
                }
            }

            int numPoints = point.Count;
            int[] resultPoint = new int[numPoints];
            int[] resultIndex = new int[numPoints];
            double[] resultDistance = new double[numPoints];
            for(int i = 0; i < numPoints; i++) {
                resultPoint[i] = (int)point[i];
                resultIndex[i] = (int)index[i];
                resultDistance[i] = (double)distance[i];
            }

            Array.Sort((int[])resultIndex.Clone(), resultPoint);
            Array.Sort(resultIndex, resultDistance);

            List result = new List();
            result.Add(new TArray(resultIndex));
            result.Add(new TArray(resultPoint));
            result.Add(new TArray(resultDistance));

            return result;
        }

        private double Distance(double x1, double y1, double z1, double x2, double y2, double z2) {
            const double rad = System.Math.PI / 180.0;

            double dlon = (x2 - x1) * rad;
            double dlat = (y2 - y1) * rad;

            double sdlat = System.Math.Sin(dlat / 2.0); sdlat *= sdlat;
            double sdlon = System.Math.Sin(dlon / 2.0); sdlon *= sdlon;

            double a = sdlat + sdlon * System.Math.Cos(y2 * rad) * System.Math.Cos(y1 * rad);
            double c = 2.0 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1.0 - a));
            double d = 6371000.0 * c;

            double dz = z2 - z1;
            return System.Math.Sqrt(dz * dz + d * d);
        }
    }
}
