﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PinzOutlookAddIn.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PinzOutlookAddIn.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap admin {
            get {
                object obj = ResourceManager.GetObject("admin", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap eye_icon {
            get {
                object obj = ResourceManager.GetObject("eye_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pinz.
        /// </summary>
        internal static string mainTaskPane_title {
            get {
                return ResourceManager.GetString("mainTaskPane_title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uncategorized.
        /// </summary>
        internal static string Outlook_Category_Default {
            get {
                return ResourceManager.GetString("Outlook_Category_Default", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New category.
        /// </summary>
        internal static string Outlook_Category_New {
            get {
                return ResourceManager.GetString("Outlook_Category_New", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Outlook.
        /// </summary>
        internal static string Outlook_Default_Project {
            get {
                return ResourceManager.GetString("Outlook_Default_Project", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New Task.
        /// </summary>
        internal static string Outlook_Task_New {
            get {
                return ResourceManager.GetString("Outlook_Task_New", resourceCulture);
            }
        }
    }
}
