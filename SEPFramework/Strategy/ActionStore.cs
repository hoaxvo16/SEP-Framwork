﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SEPFramework
{
    public class ActionStore
    {
        private Dictionary<string, Action<object[]>> actions = new Dictionary<string, Action<object[]>>();
        private Dictionary<string,RoutedEventHandler> buttonActions = new Dictionary<string,RoutedEventHandler>();

        private List<string> acctionNames;

        public ActionStore()
        {
            acctionNames = new List<string>();
            acctionNames.Add("onRowDelete");
            acctionNames.Add("onRowEdit");
            acctionNames.Add("onAddNew");

        }
        public void AddAction(string actionName,Action<object[]> action)
        {
            if(this.acctionNames.Contains(actionName))
                this.actions.Add(actionName, action);
            else
            {
                throw new Exception($"Action {actionName} does not exit in FormData");
            }
        }

        public void ExecuteAction(string actionName,object[] parameter)
        {
            if (this.actions.ContainsKey(actionName))
            {
             
                this.actions[actionName](parameter);
            }
            else
            {
               //do sth
            }
        }

    }
}
