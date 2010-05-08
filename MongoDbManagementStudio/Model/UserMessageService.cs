using System.Windows;
using MongoDbManagementStudio.Contracts;

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
