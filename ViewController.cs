using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using UIKit;

namespace iosapp
{
    public partial class ViewController : UIViewController
    {
        List<TaskItem> unfilteredData = new List<TaskItem>();
        List<TaskItem> data = new List<TaskItem>();
        RestService restService;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            restService = new RestService();

            GetTasks();

            var tableSource = new TableSource(data);
            tableSource.TaskDeleted += (sender, e) => DeleteTask(e.TaskItem);
            tableSource.TaskCompleted += (sender, e) => CompleteTask(e.TaskItem);

            taskListTableView.Source = tableSource;

            SearchBar.SearchButtonClicked += (sender, e) => UpdateFilter(SearchBar.Text);
            SearchBar.TextChanged += (sender, e) => {
                if (SearchBar.Text == "") {
                    UpdateFilter();
                } 
            };

            View.AddSubview(taskListTableView);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void NewTask_TouchUpInside(UIButton sender)
        {
            var dialog = UIAlertController.Create("Add", "Enter new task", UIAlertControllerStyle.Alert);

            dialog.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
            {
                var newTask = new TaskItem(((UITextField)dialog.TextFields.GetValue(0)).Text);
                AddTask(newTask);
            }));

            dialog.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
            dialog.AddTextField(textField => textField.Placeholder = "Task");

            PresentViewController(dialog, true, null);
        }

        public async void GetTasks()
        {
            var json = await restService.GetTasksAsync();
            json.ForEach((TaskItem task) => unfilteredData.Add(task));
            UpdateFilter();
        }

        public async void AddTask(TaskItem item)
        {
            var json = await restService.PostTaskAsync(item);
            data.Add(json);
            taskListTableView.ReloadData();
        }

        public async void DeleteTask(TaskItem item)
        {
            await restService.DeleteTaskAsync(item);
        }

        public async void CompleteTask(TaskItem item)
        {
            var task = unfilteredData.Find(x => x.id == item.id);
            task.completed = true;
            data.Find(x => x.id == item.id).completed = true;
            await restService.UpdateTaskAsync(task);
            taskListTableView.ReloadData();
        }

        public void UpdateFilter(string filterText)
        {
            data.RemoveAll((task) => true);
            unfilteredData.ForEach((task) =>
            {
                if (task.text.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    data.Add(task);
                }
            });
            taskListTableView.ReloadData();
        }

        public void UpdateFilter()
        {
            data.RemoveAll((task) => true);
            unfilteredData.ForEach((task) => data.Add(task));
            taskListTableView.ReloadData();
        }
    }
}
