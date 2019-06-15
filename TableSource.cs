using System;
using System.Collections.Generic;
using UIKit;

namespace iosapp
{
    public class TableSource : UITableViewSource
    {
        List<TaskItem> TableItems;
        string cellIdentifier = "TableCell";

        public event EventHandler<TaskEventArgs> TaskDeleted;
        public event EventHandler<TaskEventArgs> TaskCompleted;

        public TableSource(List<TaskItem> items)
        {
            TableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            if (cell == null) {
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);
            }
            cell.TextLabel.Text = TableItems[indexPath.Row].text;
            if (TableItems[indexPath.Row].completed) {
                cell.BackgroundColor = UIColor.Green;
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            TaskCompleted.Invoke(this, new TaskEventArgs(TableItems[indexPath.Row]));
            tableView.DeselectRow(indexPath, true);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
        {
            switch (editingStyle) {
                case UITableViewCellEditingStyle.Delete:
                    TaskDeleted.Invoke(this, new TaskEventArgs(TableItems[indexPath.Row]));

                    TableItems.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new Foundation.NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
            }
        }

        public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return true;
        }

        public override string TitleForDeleteConfirmation(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return "Remove";
        }
    }

    public class TaskEventArgs : EventArgs {
        private readonly TaskItem _taskItem;

        public TaskEventArgs(TaskItem taskItem) {
            _taskItem = taskItem;
        }

        public TaskItem TaskItem {
            get { return _taskItem; }
        }
    }
}
