using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    internal interface FileSelectionWindowInterface
    {
        void SetDirectoryText(string text); 
        void EnableConfirmButton();
        void SetInitializationParams(string filePath, bool isNewDB);
        void ShowError(string message); // Shows error messages to user
        void CloseWindow(); 
        string GetFilePath();
    }
}
