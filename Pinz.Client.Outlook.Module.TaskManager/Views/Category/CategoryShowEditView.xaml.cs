﻿using Com.Pinz.Client.Outlook.Module.TaskManager.Models;
using Com.Pinz.Client.Outlook.Service.Model;
using Prism.Regions;
using System.Windows.Controls;

namespace Com.Pinz.Client.Outlook.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for CategoryShowEditView.xaml
    /// </summary>
    public partial class CategoryShowEditView : UserControl
    {
        public CategoryShowEditView(CategoryShowEditModel model)
        {
            InitializeComponent();

            this.DataContext = model;

            RegionContext.GetObservableContext(this).PropertyChanged += (s, e) =>
            {
                if (RegionContext.GetObservableContext(this).Value != null)
                    model.Category = RegionContext.GetObservableContext(this).Value as OutlookCategory;
            };
        }
    }
}