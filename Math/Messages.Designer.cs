﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4952
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PavelStransky.Math {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PavelStransky.Math.Messages", typeof(Messages).Assembly);
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
        ///   Looks up a localized string similar to Unknown &apos;{0}&apos; comparison operator..
        /// </summary>
        internal static string EMBadComparisonOperator {
            get {
                return ResourceManager.GetString("EMBadComparisonOperator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bad number of degrees of freedom..
        /// </summary>
        internal static string EMBadDimension {
            get {
                return ResourceManager.GetString("EMBadDimension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bessel series failed to converge.
        /// </summary>
        internal static string EMBesselNotConverge {
            get {
                return ResourceManager.GetString("EMBesselNotConverge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The lengths of vectors must be the same as their number..
        /// </summary>
        internal static string EMGSLengths {
            get {
                return ResourceManager.GetString("EMGSLengths", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of vectors: {0}\nLength of vector {1}: {2}.
        /// </summary>
        internal static string EMGSLengthsDetail {
            get {
                return ResourceManager.GetString("EMGSLengthsDetail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to X is too large in BesselFunction; try asymptotic expansion.
        /// </summary>
        internal static string EMLargeXBessel {
            get {
                return ResourceManager.GetString("EMLargeXBessel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The trajectory with given initial conditions does not cross the plane of the Poincaré section..
        /// </summary>
        internal static string EMNoCross {
            get {
                return ResourceManager.GetString("EMNoCross", resourceCulture);
            }
        }
    }
}
