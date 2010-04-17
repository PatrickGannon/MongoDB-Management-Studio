using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MongoDBManagementStudio.Model
{
    public class UserMessageService : IUserMessageService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
