﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningAssistant.Database.Entities;
using LearningAssistant.Database.Interfaces;
using LearningAssistant.Database.DataAccessImplementations;
using System.Windows.Input;
using LearningAssistant.ViewModels;

namespace LearningAssistant.ViewModels
{
    class UserExplorerViewModel : DetailsBaseViewModel<User>
    {
        public UserExplorerViewModel() : base()
        {

        }

       

        public override async Task RefreshGrid()
        {
            try
            {
                using (IDataAccess mta = new DataAccess())
                {
                    Items = await mta.GetUsers();
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        public override async Task RefreshGrid(IDataAccess mta)
        {
            try
            {
                Items = await mta.GetUsers();
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        public override async void RemoveBut(object obj)
        {
            if (SelectedItem == null)
            {
                OnError("Select something!");
                return;
            }
            try
            {

                ButEnabled = false;
                using (IDataAccess da = new DataAccess())
                {
                    await da.RemoveUser(SelectedItem);
                    await RefreshGrid(da);
                }

                ButEnabled = true;

            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }
    }
 
}
