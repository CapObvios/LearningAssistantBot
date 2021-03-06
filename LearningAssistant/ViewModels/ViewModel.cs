﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using LearningAssistant.Classes;
using LearningAssistant.Interfaces;
using Factory = LearningAssistant.Classes.Factory;

namespace LearningAssistant.ViewModels
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            _nav = Factory.GetNavigator;        
            ButtonStartClick = new Command(StartBut);
            ButtonStopClick = new Command(StopBut);
            ButtonNewAssignmentClick = new Command(NABut);
            ButtonOverviewDeadlinesClick = new Command(OverviewDeadlines);
            ButtonOverviewHomeTasksClick = new Command(OverviewHomeTasks);
            ButtonOverviewUsersClick = new Command(OverviewUsers);
            ButtonSendClick = new Command(SendMessage);
            ButtonInfoClick = new Command((o)=> _nav.ShowInfo());
            ButtonExitClick = new Command((o) => App.Current.Shutdown());
        }

        INavigator _nav;
        
        public ICommand ButtonStartClick { get; set; }
        public ICommand ButtonStopClick { get; set; }
        public ICommand ButtonNewAssignmentClick { get; set; }
        public ICommand ButtonOverviewHomeTasksClick { get; set; }
        public ICommand ButtonOverviewDeadlinesClick { get; set; }
        public ICommand ButtonOverviewUsersClick { get; set; }
        public ICommand ButtonSendClick { get; set; }
        public ICommand ButtonInfoClick { get; set; }
        public ICommand ButtonExitClick { get; set; }

        private string _status = "inactive";

        public string StatusLabel
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("StatusLabel");
            }
        }

        private string _bn = "Learning Assistant";

        public string BotName
        {
            get { return _bn; }
            set
            {
                _bn = value;
                OnPropertyChanged("BotName");
            }
        }

        private string _un = "Unknown";

        public string UserName
        {
            get { return _un; }
            set
            {
                _un = value;
                OnPropertyChanged("UserName");
            }
        }


        private bool _startbe = true;

        public bool StartButEnabled
        {
            get { return _startbe; }
            set
            {
                _startbe = value;
                OnPropertyChanged("StartButEnabled");
            }
        }

        private bool _stopbe = false;

        public bool StopButEnabled
        {
            get { return _stopbe; }
            set
            {
                _stopbe = value;
                OnPropertyChanged("StopButEnabled");
            }
        }

        private string _mes;

        public string Message
        {
            get { return _mes; }
            set
            {
                _mes = value;
                OnPropertyChanged("Message");
            }
        }

        public void BotError()
        {                       
            _nav.ErrorCaught("bot could not process requests");
            Factory.GetBot.OnError -= BotError;
            StatusLabel = "inactive";
            StartButEnabled = true;
            StopButEnabled = false;
        }       

        public void SendMessage(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                Factory.GetBot.SendBulkMessage(Message);
            }
            Message = string.Empty;
        }

        public void OverviewUsers(object obj)
        {
            _nav.NavigateTo("UE");
        }

        public void OverviewHomeTasks(object obj)
        {
            _nav.NavigateTo("HTE");
        }

        public void OverviewDeadlines(object obj)
        {
            _nav.NavigateTo("DE");
        }

        public async void StartBut(object obj)
        {
            StartButEnabled = false;
            try
            {
                Factory.GetBot.OnError += BotError;
                Factory.GetBot.StartProcessing();
                await Factory.GetBot.GetBotName();
                if (Factory.GetBot.IsActive)
                {
                    StatusLabel = "active";
                    StopButEnabled = true;
                    UserName = $"@{Factory.GetBot.BotUsername.ToLower()}";
                    BotName = Factory.GetBot.BotName;
                }
                else
                {
                    StatusLabel = "inactive";
                    StartButEnabled = true;
                }
            }
            catch(Exception ex)
            {
                OnError(ex.Message);
                StartButEnabled = true;
                StopButEnabled = false;
            }
        }

        public void StopBut(object obj)
        {            
            StopButEnabled = false;
            try {
                Factory.GetBot.CancelProcessing();
                if (Factory.GetBot.IsActive)
                {
                    StatusLabel = "active";
                    StopButEnabled = true;
                }
                else
                {
                    StatusLabel = "inactive";
                    StartButEnabled = true;
                }
            }
            catch(Exception ex)
            {
                StartButEnabled = false;
                StopButEnabled = true;
                OnError(ex.Message);
            }
        }

        public void OnError(string ex)
        {
            _nav.ErrorCaught(ex);
        }

        public void NABut(object obj)
        {
            _nav.NavigateTo("AdditionalWindow");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
