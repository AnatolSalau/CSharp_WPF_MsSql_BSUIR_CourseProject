﻿using Sebestoimost.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Sebestoimost.Pages
{
    public partial class PlansList : Page
    {
        public PlansList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            App.db = new dbContext();
            // Предустановки фильтра
            FltrGrid.Visibility = Visibility.Collapsed;
            FltrDateCheck.IsChecked = false;
            FltrDateAt.SelectedDate = new DateTime(2020, 1, 1);
            FltrDateTo.SelectedDate = DateTime.Now;
            FltrNomenclaturesCheck.IsChecked = false;
            FltrNomenclatures.ItemsSource = App.db.Nomenclatures.Where(p => p.NomenclatureTypeId == 1).ToList();
            FltrNomenclatures.SelectedIndex = 0;
            //
            FillData();
        }

        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PlansForm(0));
        }

        private void MenuEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GrdItems.SelectedItem != null)
            {
                var item = GrdItems.SelectedItem as Plan;
                NavigationService.Navigate(new PlansForm(item.Id));
            }
        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GrdItems.SelectedItem != null)
            {
                var item = GrdItems.SelectedItem as Plan;
                if (MessageBox.Show(string.Format("Вы действительно хотите удалить документ №{0} от {1:dd.MM.yyyy}", item.Id, item.Date), "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    App.db.Plans.Remove(item);
                    try
                    {
                        App.db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        App.db.UndoChanges();
                        MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                        App.SetLogText("Ошибка удаления плановой цены\t" + App.user.Name);
                    }
                    FillData();
                }
            }
        }

        private void MenuFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FltrGrid.Visibility == Visibility.Visible)
            {
                FltrGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                FltrGrid.Visibility = Visibility.Visible;
            }
        }

        private void FltrBtn_Click(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void FltrCancel_Click(object sender, RoutedEventArgs e)
        {
            FltrDateCheck.IsChecked = false;
            FltrNomenclaturesCheck.IsChecked = false;
            FillData();
        }

        private void FillData()
        {
            List<Plan> fltrList;
            fltrList = App.db.Plans.ToList();
            //
            if (FltrDateCheck.IsChecked == true && FltrDateAt.SelectedDate <= FltrDateTo.SelectedDate)
            {
                fltrList = fltrList.Where(p => p.Date >= FltrDateAt.SelectedDate && p.Date <= FltrDateTo.SelectedDate).ToList();
            }
            if (FltrNomenclaturesCheck.IsChecked == true)
            {
                var nomenclature = FltrNomenclatures.SelectedItem as Nomenclature;
                fltrList = fltrList.Where(p => p.NomenclatureId == nomenclature.Id).ToList();
            }
            //
            GrdItems.ItemsSource = fltrList;
        }
    }
}
