﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5420
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PavelStransky.Systems {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PavelStransky.Systems.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid mean energy!.
        /// </summary>
        internal static string EMBadMeanEnergy {
            get {
                return ResourceManager.GetString("EMBadMeanEnergy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mean energy {0} must be within the interval &lt;0; {1}&gt;..
        /// </summary>
        internal static string EMBadMeanEnergyDetail {
            get {
                return ResourceManager.GetString("EMBadMeanEnergyDetail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid index of quantum number {0}..
        /// </summary>
        internal static string EMBadQNIndex {
            get {
                return ResourceManager.GetString("EMBadQNIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Current QuantumGCM object is being calculated. New calculation cannot be begun..
        /// </summary>
        internal static string EMComputing {
            get {
                return ResourceManager.GetString("EMComputing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The eigenvectors of the system have not been computed..
        /// </summary>
        internal static string EMNoEigenVectors {
            get {
                return ResourceManager.GetString("EMNoEigenVectors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Spectrum has not been calculated yet..
        /// </summary>
        internal static string EMNotComputed {
            get {
                return ResourceManager.GetString("EMNotComputed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This function is not implemented..
        /// </summary>
        internal static string EMNotImplemented {
            get {
                return ResourceManager.GetString("EMNotImplemented", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diagonalization ARPACK.
        /// </summary>
        internal static string MDiagonalizationARPACK {
            get {
                return ResourceManager.GetString("MDiagonalizationARPACK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diagonalization LAPACK.dsbevx.
        /// </summary>
        internal static string MDiagonalizationDSBEVX {
            get {
                return ResourceManager.GetString("MDiagonalizationDSBEVX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diagonalization LAPACK.dsyev .
        /// </summary>
        internal static string MDiagonalizationDSYEV {
            get {
                return ResourceManager.GetString("MDiagonalizationDSYEV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to , NO eigenvectors.
        /// </summary>
        internal static string MDiagonalizationEVNo {
            get {
                return ResourceManager.GetString("MDiagonalizationEVNo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  AND eigenvectors.
        /// </summary>
        internal static string MDiagonalizationEVYes {
            get {
                return ResourceManager.GetString("MDiagonalizationEVYes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diagonalization Jacobi....
        /// </summary>
        internal static string MDiagonalizationJacobi {
            get {
                return ResourceManager.GetString("MDiagonalizationJacobi", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} has started the diagonalization of {1}.
        /// </summary>
        internal static string MDiagonalizationStart {
            get {
                return ResourceManager.GetString("MDiagonalizationStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Calculation of the Hamiltonian matrix....
        /// </summary>
        internal static string MHMCalculation {
            get {
                return ResourceManager.GetString("MHMCalculation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nonzero {0} of {1} elements..
        /// </summary>
        internal static string MNonzeroElements {
            get {
                return ResourceManager.GetString("MNonzeroElements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  ({0} eigenvalues{1})....
        /// </summary>
        internal static string MNumEV {
            get {
                return ResourceManager.GetString("MNumEV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Symmetric band matrix ({0} x {1}).
        /// </summary>
        internal static string MSBMatrixDimension {
            get {
                return ResourceManager.GetString("MSBMatrixDimension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Symmetric matrix ({0} x {0}).
        /// </summary>
        internal static string MSMatrixDimension {
            get {
                return ResourceManager.GetString("MSMatrixDimension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Symmetric sparse matrix ({0} x {0}), {1} nonzero elements.
        /// </summary>
        internal static string MSSMatrixDimension {
            get {
                return ResourceManager.GetString("MSSMatrixDimension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trace of the matrix: {0}.
        /// </summary>
        internal static string MTrace {
            get {
                return ResourceManager.GetString("MTrace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trace of Hamiltonian matrix....
        /// </summary>
        internal static string MTraceHM {
            get {
                return ResourceManager.GetString("MTraceHM", resourceCulture);
            }
        }
    }
}
