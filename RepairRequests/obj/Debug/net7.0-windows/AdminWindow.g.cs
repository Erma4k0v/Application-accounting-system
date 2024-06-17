﻿#pragma checksum "..\..\..\AdminWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "119CFF4607EDC17562426E1ABD9712FA2559E6FB"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace RepairRequests {
    
    
    /// <summary>
    /// AdminWindow
    /// </summary>
    public partial class AdminWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearchRequestNumber;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearchEquipment;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearchFaultType;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearchClientName;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtExecutorLogin;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtRequestNumberForExecutor;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtCompletedRequests;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtAverageExecutionTime;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\AdminWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtFaultTypeStatistics;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RepairRequests;component/adminwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AdminWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtSearchRequestNumber = ((System.Windows.Controls.TextBox)(target));
            
            #line 15 "..\..\..\AdminWindow.xaml"
            this.txtSearchRequestNumber.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\AdminWindow.xaml"
            this.txtSearchRequestNumber.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtSearchEquipment = ((System.Windows.Controls.TextBox)(target));
            
            #line 16 "..\..\..\AdminWindow.xaml"
            this.txtSearchEquipment.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\AdminWindow.xaml"
            this.txtSearchEquipment.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtSearchFaultType = ((System.Windows.Controls.TextBox)(target));
            
            #line 17 "..\..\..\AdminWindow.xaml"
            this.txtSearchFaultType.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\AdminWindow.xaml"
            this.txtSearchFaultType.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtSearchClientName = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\AdminWindow.xaml"
            this.txtSearchClientName.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\AdminWindow.xaml"
            this.txtSearchClientName.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 19 "..\..\..\AdminWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Search_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtExecutorLogin = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\..\AdminWindow.xaml"
            this.txtExecutorLogin.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\AdminWindow.xaml"
            this.txtExecutorLogin.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 7:
            this.txtRequestNumberForExecutor = ((System.Windows.Controls.TextBox)(target));
            
            #line 21 "..\..\..\AdminWindow.xaml"
            this.txtRequestNumberForExecutor.GotFocus += new System.Windows.RoutedEventHandler(this.TextBox_GotFocus);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\AdminWindow.xaml"
            this.txtRequestNumberForExecutor.LostFocus += new System.Windows.RoutedEventHandler(this.TextBox_LostFocus);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 22 "..\..\..\AdminWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddExecutor_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 23 "..\..\..\AdminWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeResponsible_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.txtCompletedRequests = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.txtAverageExecutionTime = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.txtFaultTypeStatistics = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

