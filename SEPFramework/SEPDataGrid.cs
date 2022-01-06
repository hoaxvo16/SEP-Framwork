using SEPFramework.Interface;
using SEPFramework.Memento;
using SEPFramework.Observer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SEPFramework
{
    public class SEPDataGrid<T>:ISubscriber<T>
    {

        //Atributes

        protected DataGrid UIElement;
        protected ObserverDataSource<T> data;
        protected ActionStore actionStore = new ActionStore();
        protected CareTaker<T> careTaker = new CareTaker<T>();



        //Getter, Setter

        public DataGrid GetUIElement()
        {
            return UIElement;
        }
        public void SetDataList(List<T> dataList)
        {
            this.data = new ObserverDataSource<T>(dataList);
            UpdateCareTaker();
            this.UIElement.ItemsSource = dataList;
            this.data.Subscribe(this);
        }

        public void AddAction(string actionName, Action<object[]> function)
        {
            this.actionStore.AddAction(actionName, function);
        }

        public void AddColumn(DataGridTemplateColumn col)
        {
            this.UIElement.Columns.Add(col);
        }

        //Constructor
        public SEPDataGrid()
        {
            this.UIElement = new DataGrid();
            this.UIElement.IsReadOnly = true;
            this.UIElement.MouseDoubleClick += DataGridMouseDoubleClick;
        }

        //Render
        public void Render(Panel container)
        {

            container.Children.Add(UIElement);
        }


        //Event handler


        //Click Event

        private void DataGridMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedItem = data[UIElement.SelectedIndex];
            var editForm = new EditForm();
            editForm.Init(selectedItem, FinishUpdate);

        }

        public void AddNewButtonClick(object sender, RoutedEventArgs e)
        {
            var item = this.data[0];
            var addForm = new AddNewForm();
            addForm.Init(item, FinishAddNew);
        }

        public void DeleteItemClick(object sender, RoutedEventArgs e)
        {
            var isAbort = false;
            var parameters = new object[2]{data[UIElement.SelectedIndex],isAbort};
            this.actionStore.ExecuteAction("onRowDelete", parameters);
            if ((bool)parameters[1] == false)
            {
                this.data.RemoveData(data[UIElement.SelectedIndex]);
                UpdateCareTaker();
            }
        }

        public void EditButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.data[UIElement.SelectedIndex];

            var editForm = new EditForm();
        
            editForm.Init(selectedItem, FinishUpdate);
        }


  

        private void FinishAddNew(object newData)
        {
            var isAbort = false;
            var parameters = new object[2] { newData, isAbort };
            this.actionStore.ExecuteAction("onAddNew", parameters);
            if (!(bool)parameters[1]) {
                this.data.AddNewData((T)newData);
                UpdateCareTaker();
            }
        }

        private void FinishUpdate(object result)
        {
            var isAbort = false;
            var parameters = new object[2] { result, isAbort };
            this.actionStore.ExecuteAction("onRowEdit", parameters);
            if (!(bool)parameters[1])
            {
                this.data.UpdateData((T)result, UIElement.SelectedIndex);
                UpdateCareTaker();
            }
            
        }

        //Undo redo
        public void UndoClick(object sender, RoutedEventArgs e)
        {

            var prev = this.careTaker.Undo();
            if (prev != null)
                this.data.SetDataSource(prev.GetSate());

        }

        public void RedoClick(object sender, RoutedEventArgs e)
        {
            var next = this.careTaker.Redo();
            if (next != null)
                this.data.SetDataSource(next.GetSate());
        }

        //Update history


        public void UpdateCareTaker()
        {
            this.careTaker.AddMemento(new Memento<T>(data.GetDataSource()));
        }



        //Public method for user


      

        public void RemoveIfPropertyEqual(string propertyName, object value)
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < data.Count(); i++)
            {
                var propertyValue = data[i].GetType().GetProperty(propertyName).GetValue(data[i], null);
                if (propertyValue == value)
                {
                    indexList.Add(i);
                }
            }

            foreach (int id in indexList)
            {
                data.RemoveAt(id);
            }
        }

        public void Update(List<T> data)
        {
            this.UIElement.ItemsSource = data;
            this.UIElement.Items.Refresh();
        }
    }
}
