using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
