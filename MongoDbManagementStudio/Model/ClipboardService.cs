using System.Windows;
using MongoDbManagementStudio.Contracts;

namespace MongoDBManagementStudio.Model
{
    public class ClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
