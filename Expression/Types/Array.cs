using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// Øada - typová kontrola do objektu ArrayList
    /// </summary>
    public class TArray: ICloneable, IExportable, IEnumerable, ISortable {
        private Array data;
        private Type type;

        /// <summary>
        /// Testovací øada ve tvaru (000; 001; 010; ...)
        /// </summary>
        /// <param name="i">Délky øady</param>
        public static TArray TestArray(int[] i) {
            TArray result = new TArray(typeof(int), i);
            int rank = result.Rank;

            result.ResetEnumerator();
            int[] index = (int[])result.startEnumIndex.Clone();

            do {
                StringBuilder s = new StringBuilder();
                for(int r = 0; r < rank; r++)
                    s.Append(index[r] + 1);
                result[index] = int.Parse(s.ToString());
            }
            while(TArray.MoveNext(rank, index, result.startEnumIndex, result.endEnumIndex));

            return result;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="type">Typ øady</param>
        /// <param name="lengths">Délky øady</param>
        public TArray(Type type, params int[] lengths) {
            this.data = Array.CreateInstance(type, lengths);
            this.type = type;
            this.ResetEnumerator();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="i">Celoèíselné hodnoty</param>
        public TArray(params int[] i) {
            this.type = typeof(int);
            this.data = (int[])i.Clone();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="d">Hodnoty double</param>
        public TArray(params double[] i) {
            this.type = typeof(double);
            this.data = (double[])i.Clone();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Øada</param>
        public TArray(Array a) {
            this.data = (Array)a.Clone();

            foreach(object o in this.data) {
                this.type = o.GetType();
                break;
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a">Øada</param>
        public TArray(TArray a) {
            this.data = (Array)a.data.Clone();
            this.type = a.type;
        }

        /// <summary>
        /// Poèet rozmìrù øady
        /// </summary>
        public int Rank { get { return this.data.Rank; } }

        /// <summary>
        /// Je øada jednorozmìrná?
        /// </summary>
        public bool Is1D { get { return this.Rank == 1; } }

        /// <summary>
        /// Je øada dvourozmìrná?
        /// </summary>
        public bool Is2D { get { return this.Rank == 2; } }

        /// <summary>
        /// Typ prvku
        /// </summary>
        public Type GetItemType() {
            return this.type;
        }

        /// <summary>
        /// Dimenze øady
        /// </summary>
        public int[] Lengths {
            get {
                int rank = this.Rank;
                int[] result = new int[rank];
                for(int r = 0; r < rank; r++)
                    result[r] = this.data.GetLength(r);
                return result;
            }
        }

        /// <summary>
        /// Vrátí délku jednorozmìrné øady
        /// </summary>
        public int Length {
            get {
                this.Check1D();
                return this.GetLength(0);
            }
        }

        /// <summary>
        /// Checks whether the array is 1D
        /// </summary>
        public void Check1D() {
            if(!this.Is1D)
                throw new TArrayException(Messages.EMNot1D,
                    string.Format(Messages.EMRankDetail, this.Rank));
        }

        /// <summary>
        /// Vrátí délky pole jako string
        /// </summary>
        /// <param name="separator">Oddìlovaè</param>
        public string LengthsString(string separator) {
            int[] lengths = this.Lengths;
            int rank = lengths.Length;

            if(rank == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            result.Append(lengths[0]);

            for(int i = 1; i < rank; i++) {
                result.Append(separator);
                result.Append(lengths[i]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Vrátí délky pole jako string, jednotlivá èísla budou oddìlena èárkami
        /// </summary>
        /// <param name="separator">Oddìlovaè</param>
        public string LengthsString() {
            return this.LengthsString(", ");
        }

        /// <summary>
        /// Poèet prvkù dané øady
        /// </summary>
        public int GetNumElements() {
            int[] lengths = this.Lengths;
            int result = 1;
            for(int i = 0; i < lengths.Length; i++)
                result *= lengths[i];
            return result;
        }

        /// <summary>
        /// Vrátí rozmìr v zadaném ranku
        /// </summary>
        /// <param name="rank">Rank</param>
        public int GetLength(int rank) {
            return this.data.GetLength(rank);
        }

        /// <summary>
        /// Indexer pro více indexù
        /// </summary>
        /// <param name="index">Indexy</param>
        public object this[params int[] index] {
            get {
                if(this.Rank == index.Length)
                    return this.data.GetValue(index);
                else
                    throw new TArrayException(errorMessageBadRank, string.Format(errorMessageBadRankDetail, this.Rank, index.Length));
            }
            set {
                if(this.Rank == index.Length)
                    this.data.SetValue(value, index);
                else
                    throw new TArrayException(errorMessageBadRank, string.Format(errorMessageBadRankDetail, this.Rank, index.Length));
            }
        }

        /// <summary>
        /// Posun na další prvek
        /// </summary>
        /// <returns>True, pokud se posun povedl, false, pokud ne</returns>
        public static bool MoveNext(int rank, int[] index, int[] startIndex, int[] endIndex) {
            for(int r = rank - 1; r >= 0; r--) {
                index[r]++;
                if(index[r] > endIndex[r])
                    index[r] = startIndex[r];
                else
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True, pokud jsou mají øady stejné dimenze
        /// </summary>
        /// <param name="a1">První øada</param>
        /// <param name="a2">Druhá øada</param>
        public static bool IsEqualDimension(TArray a1, TArray a2) {
            int[] l1 = a1.Lengths;
            int[] l2 = a2.Lengths;

            if(l1.Length != l2.Length)
                return false;

            int l = l1.Length;
            for(int i = 0; i < l; i++)
                if(l1[i] != l2[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Basic getting function
        /// </summary>
        private object GetFunction(object o) {
            return o;
        }

        /// <summary>
        /// Vrátí podmnožinu zadané øady
        /// </summary>
        /// <param name="inIndex">Vstupní indexy v jednotlivých rozmìrech (mùžou být duplicity)</param>
        public object GetSubArray(int[][] inIndex) {
            int length = inIndex.Length;

            bool[] shrink = new bool[length];
            for(int i = 0; i < length; i++)
                shrink[i] = false;

            return this.GetSubArray(inIndex, shrink);
        }

        /// <summary>
        /// Vrátí podmnožinu zadané øady a provede kontrakci pøes zadané indexy
        /// </summary>
        /// <param name="inIndex">Vstupní indexy v jednotlivých rozmìrech (mùžou být duplicity)</param>
        /// <param name="shrink">Dimensions in which the result array will be shrinked</param>
        public object GetSubArray(int[][] inIndex, bool[] shrink) {
            return this.GetSubArray(inIndex, shrink, this.GetFunction);
        }
        
        /// <summary>
        /// Vrátí podmnožinu zadané øady a provede kontrakci pøes zadané indexy
        /// </summary>
        /// <param name="inIndex">Vstupní indexy v jednotlivých rozmìrech (mùžou být duplicity)</param>
        /// <param name="shrink">Dimensions in which the result array will be shrinked</param>
        public object GetSubArray(int[][] inIndex, bool[] shrink, Indexer.GetFunction getFn) {
            int rank = this.Rank;

            inIndex = this.AddMissingIndexes(inIndex);

            bool[] oldShrink = shrink;
            shrink = new bool[rank];
            for(int r = 0; r < oldShrink.Length; r++)
                shrink[r] = oldShrink[r];
            for(int r = oldShrink.Length; r < rank; r++)
                shrink[r] = false;

            // Rozmìr nové øady (rozmìr pùvodní - kontrakce)
            int rRank = rank;
            for(int r = 0; r < rank; r++)
                if(shrink[r])
                    rRank--;

            // All indexes are shrinked
            if(rRank == 0) {
                int[] indexS = new int[rank];
                for(int r = 0; r < rank; r++)
                    indexS[r] = inIndex[r][0];

                return getFn(this[indexS]);
            }

            int[] lengths = new int[rRank];
            int rr = 0;
            for(int r = 0; r < rank; r++) {
                if(shrink[r]) {
                    int l = inIndex[r].Length;
                    if(l != 1)
                        throw new TArrayException(
                            string.Format(errorMessageShrink, r),
                            string.Format(errorMessageShrinkDetail, l));
                }
                else
                    lengths[rr++] = inIndex[r].Length;
            }

            TArray result = null;
            
            int []startIndex = new int[rRank];
            int[] endIndex = (int[])lengths.Clone();
            for(int r = 0; r < rRank; r++)
                endIndex[r]--;
            int []index = new int[rRank];

            int[] sourceIndexI = new int[rank];
            int[] sourceIndex = new int[rank];

            do {
                bool zeroArray = false;
                for(int r = 0; r < rank; r++)
                    if(inIndex[r].Length == 0) {
                        zeroArray = true;
                        break;
                    }
                    else
                        sourceIndex[r] = inIndex[r][sourceIndexI[r]];

                if(!zeroArray) {
                    object o = getFn(this[sourceIndex]);
                    if(result == null)
                        result = new TArray(o.GetType(), lengths);
                    result[index] = o;
                }
                else
                    result = new TArray(this.GetItemType(), lengths);

                for(int r = rank - 1; r >= 0; r--) {
                    sourceIndexI[r]++;
                    if(sourceIndexI[r] >= inIndex[r].Length)
                        sourceIndexI[r] = 0;
                    else
                        break;
                }

            } while(TArray.MoveNext(rRank, index, startIndex, endIndex));

            return result;
        }

        /// <summary>
        /// Pøidá do øady indexy chybìjících rozmìrù
        /// </summary>
        /// <param name="index">Vstupní indexy</param>
        /// <returns>Výstupní indexy</returns>
        private int[][] AddMissingIndexes(int[][] index) {
            int rank = this.Rank;
            int length = index.Length;

            if(rank < length)
                throw new TArrayException(errorMessageBadRank, string.Format(errorMessageBadRankDetail, rank, length));

            int[][] result = index;

            if(rank > length) {
                result = new int[rank][];
                for(int r = 0; r < length; r++)
                    result[r] = index[r];
                for(int r = length; r < rank; r++) {
                    int l = this.GetLength(r);
                    int[] ind = new int[l];
                    for(int i = 0; i < l; i++)
                        ind[i] = i;
                    result[r] = ind;
                }
            }

            return result;
        }

        /// <summary>
        /// Nastaví hodnotu øady pro zadané indexy
        /// </summary>
        /// <param name="inIndex">Vstupní indexy v jednotlivých rozmìrech (mùžou být duplicity)</param>
        /// <param name="value">Pøiøazovaná hodnota</param>
        public void SetValue(int[][] inIndex, Atom.AssignmentFunction assignFn) {
            int rank = this.Rank;

            inIndex = this.AddMissingIndexes(inIndex);

            int[] sourceIndexI = new int[rank];
            int[] sourceIndex = new int[rank];

            bool finish = false;

            do {
                for(int r = 0; r < rank; r++)
                    sourceIndex[r] = inIndex[r][sourceIndexI[r]];

                this[sourceIndex] = assignFn(this[sourceIndex]);

                for(int r = rank - 1; r >= 0; r--) {
                    sourceIndexI[r]++;
                    if(sourceIndexI[r] >= inIndex[r].Length) {
                        sourceIndexI[r] = 0;
                        if(r == 0)
                            finish = true;
                    }
                    else
                        break;
                }

            } while(!finish);
        }

        /// <summary>
        /// Pøetypování obyèejné øady na TArray
        /// </summary>
        /// <param name="a">Obyèejná øada</param>
        public static implicit operator TArray(Array a) {
            return new TArray(a);
        }

        /// <summary>
        /// Pøetypování na vektor
        /// </summary>
        /// <param name="array">Øada</param>
        public static explicit operator Vector(TArray array) {
            array.Check1D();

            if(array.type == typeof(double))
                return new Vector((double[])array.data);
            else if(array.type == typeof(int))
                return new Vector((int[])array.data);
            else
                throw new TArrayException(errorMessageBadType, string.Format(errorMessageBadTypeDetail, typeof(double).FullName, array.type.FullName));
        }

        /// <summary>
        /// Pøevede danou øadu na øadu vektorù
        /// </summary>
        public TArray CovertToVectors() {
            if(this.type != typeof(int) && this.type != typeof(double))
                throw new TArrayException(errorMessageBadType, string.Format(errorMessageBadTypeDetail, typeof(double).FullName, this.type.FullName));

            int rank = this.Rank;
            int r = rank - 1;

            int[] startIndex = new int[r];
            int[] endIndex = new int[r];
            int[] lengths = new int[r];

            for(int i = 0; i < r; i++) {
                int l = this.data.GetLength(i);
                lengths[i] = l;
                startIndex[i] = 0;
                endIndex[i] = l - 1;
            }

            TArray result = new TArray(typeof(Vector), lengths);

            this.ResetEnumerator();
            this.enumType = TArrayEnumType.Vector;

            int[] index = (int[])result.startEnumIndex.Clone();

            foreach(Vector v in this) {
                result[index] = v;
                if(!TArray.MoveNext(r, index, startIndex, endIndex))
                    break;
            }

            return result;
        }

        /// <summary>
        /// Všechny prvky øady smrskne do øady jednorozmìrné
        /// </summary>
        public TArray Deflate() {
            int length = this.GetNumElements();
            TArray result = new TArray(this.type, length);

            int i = 0;
            this.ResetEnumerator();
            foreach(object o in this)
                result[i++] = o;

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží obsah øady do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            int rank = this.Rank;
            int[] lengths = this.Lengths;
            string typeName = this.type.FullName;

            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;

                b.Write(rank);
                for(int i = 0; i < rank; i++)
                    b.Write(lengths[i]);

                b.Write(typeName);
            }
            else {
                // Textovì
                StreamWriter t = export.T;

                t.WriteLine(rank);
                for(int i = 0; i < rank; i++) {
                    if(i != 0)
                        t.Write('\t');
                    t.Write(lengths[i]);
                }
                t.WriteLine();

                t.WriteLine(typeName);
            }

            this.ResetEnumerator();
            foreach(object o in this)
                export.Write(typeName, o);
        }

        /// <summary>
        /// Naète obsah øady ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public TArray(Core.Import import) {
            int rank = 0;
            string typeName = string.Empty;
            int[] lengths;

            // Stará verze (seznam)
            if(import.VersionNumber < 5) {
                rank = 1;
                lengths = new int[1];
                if(import.Binary) {
                    // Binárnì
                    BinaryReader b = import.B;
                    lengths[0] = b.ReadInt32();
                    typeName = b.ReadString();
                }
                else {
                    // Textovì
                    StreamReader t = import.T;
                    lengths[0] = int.Parse(t.ReadLine());
                    typeName = t.ReadLine();
                }
            }
                // Nejnovìjší verze
            else {
                if(import.Binary) {
                    // Binárnì
                    BinaryReader b = import.B;
                    rank = b.ReadInt32();
                    lengths = new int[rank];
                    for(int r = 0; r < rank; r++)
                        lengths[r] = b.ReadInt32();

                    typeName = b.ReadString();
                }
                else {
                    // Textovì
                    StreamReader t = import.T;

                    rank = int.Parse(t.ReadLine());
                    lengths = new int[rank];
                    string[] line = t.ReadLine().Split('\t');

                    for(int r = 0; r < rank; r++)
                        lengths[r] = int.Parse(line[r]);

                    typeName = t.ReadLine();
                }
            }
            int[] startIndex = new int[rank];
            int[] endIndex = (int[])lengths.Clone();
            for(int r = 0; r < rank; r++)
                endIndex[r]--;
            int[] index = new int[rank];

            if(lengths.Length <= 0 || lengths[0] <= 0) {
                this.type = Type.GetType(typeName);
                this.data = Array.CreateInstance(this.type, lengths);
            }
            else {
                object o = import.Read(typeName);
                this.type = o.GetType();
                this.data = Array.CreateInstance(this.type, lengths);
                this[index] = o;

                while(TArray.MoveNext(rank, index, startIndex, endIndex))
                    this[index] = import.Read(typeName);
            }
            this.ResetEnumerator();
        }
        #endregion

        #region Implementace IEnumerable, IEnumerator
        public enum TArrayEnumType { Value, Vector, Matrix }

        private int[] startEnumIndex;
        private int[] endEnumIndex;
        private TArrayEnumType enumType;

        /// <summary>
        /// Poèáteèní index pro enumerátor
        /// </summary>
        public int[] StartEnumIndex { get { return this.startEnumIndex; } }

        /// <summary>
        /// Koncový index pro enumerátor
        /// </summary>
        public int[] EndEnumIndex { get { return this.endEnumIndex; } }

        /// <summary>
        /// Pøi vytvoøení øady nastaví enumerátory
        /// </summary>
        public void ResetEnumerator() {
            int rank = this.Rank;
            this.enumType = TArrayEnumType.Value;
            this.startEnumIndex = new int[rank];

            this.endEnumIndex = this.Lengths;
            for(int i = 0; i < rank; i++)
                this.endEnumIndex[i]--;
        }

        /// <summary>
        /// Nastaví enumerátor TArray
        /// </summary>
        /// <param name="enumType">Typ enumerátoru</param>
        public void SetEnumerator(TArrayEnumType enumType) {
            if(enumType == TArrayEnumType.Vector && this.type != typeof(double) && this.type != typeof(int))
                throw new ListException(errorMessageBadType, string.Format(errorMessageBadTypeDetail, typeof(double), this.type));

            if(enumType == TArrayEnumType.Matrix) {
                if(this.type != typeof(double) && this.type != typeof(int))
                    throw new ListException(errorMessageBadType, string.Format(errorMessageBadTypeDetail, typeof(double), this.type));
                if(this.Rank < 2)
                    throw new ListException(errorMessageBadRank);
            }

            this.enumType = enumType;
        }

        /// <summary>
        /// Vrátí aktuální tøídu jako Enumerátor
        /// </summary>
        public IEnumerator GetEnumerator() {
            return new TArrayEnumerator(this);
        }

        /// <summary>
        /// Tøída implementující enumerátor
        /// </summary>
        private class TArrayEnumerator: IEnumerator {
            private TArray array;
            private int[] startEnumIndex;
            private int[] endEnumIndex;
            private int[] enumIndex;
            private bool reseted;
            private TArrayEnumType enumType = TArrayEnumType.Value;
            private int rank;

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="array">Objekt TArray</param>
            public TArrayEnumerator(TArray array) {
                this.array = array;
                this.enumType = array.enumType;

                this.rank = array.Rank;
                if(this.enumType == TArrayEnumType.Vector)
                    this.rank -= 1;
                else if(this.enumType == TArrayEnumType.Matrix)
                    this.rank -= 2;

                this.startEnumIndex = new int[rank];
                for(int r = 0; r < rank; r++)
                    this.startEnumIndex[r] = array.startEnumIndex[r];

                this.endEnumIndex = new int[rank];
                for(int r = 0; r < rank; r++)
                    this.endEnumIndex[r] = array.endEnumIndex[r];

                this.enumIndex = (int[])this.startEnumIndex.Clone();
                this.reseted = true;
            }

            public void Reset() {
                this.enumIndex = (int[])this.startEnumIndex.Clone();
                this.reseted = true;
            }

            public bool MoveNext() {
                if(reseted) {
                    reseted = false;

                    if(this.endEnumIndex.Length == 1 && this.endEnumIndex[0] < 0)
                        return false;
                    else
                        return true;
                }

                return TArray.MoveNext(this.rank, this.enumIndex, this.startEnumIndex, this.endEnumIndex);
            }

            public object Current {
                get {
                    if(this.enumType == TArrayEnumType.Vector)
                        return (Vector)(TArray)this.array[this.enumIndex];
                    else
                        return this.array[this.enumIndex];
                }
            }
        }
        #endregion

        #region Implementace ICloneable
        public object Clone() {
            return new TArray(this);
        }
        #endregion

        #region ISortable Members
        public object Sort() {
            this.Check1D();

            TArray result = this.Clone() as TArray;
            Array.Sort(result.data);
            return result;
        }

        public object SortDesc() {
            TArray result = this.Sort() as TArray;
            Array.Reverse(result.data);
            return result;
        }

        public object Sort(ISortable keys) {
            this.Check1D();

            TArray result = this.Clone() as TArray;
            Array.Sort(keys.GetKeys(), result.data);
            return result;
        }

        public object SortDesc(ISortable keys) {
            TArray result = this.Sort(keys) as TArray;
            Array.Reverse(result.data);
            return result;
        }

        public Array GetKeys() {
            this.Check1D();

            return this.data;
        }
        #endregion

        /// <summary>
        /// Øetìzec
        /// </summary>
        public override string ToString() {
            StringBuilder result = new StringBuilder();
            this.ResetEnumerator();
            foreach(object o in this)
                if(o == null)
                    result.AppendLine("null");
                else
                    result.AppendLine(o.ToString());

            return result.ToString();
        }

        private const string errorMessageBadType = "For the considered operation the array does not have the correct type.";
        private static string errorMessageBadTypeDetail = "Requested type: {0}" + Environment.NewLine + "Current type: {1}";

        private const string errorMessageBadRank = "The rank of the Array is invalid.";
        private static string errorMessageBadRankDetail = "Rank is: {0}" + Environment.NewLine + "Number of indexes: {1}";

        private const string errorMessageShrink = "It is not possible to shrink in dimension {0}.";
        private static string errorMessageShrinkDetail = "There must be only one element." + Environment.NewLine + "Number of elements: {0}";
    }
}
