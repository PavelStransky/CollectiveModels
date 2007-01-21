using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    public class CreateGraph : FnGraph {
        public override string Name { get { return name; } }
        private const string name = "creategraph";
    }
}
