using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StarPRNTSDK
{
    public partial class App : Application
    {
        public App()
        {
            SharedInformationManager.SelectedModelManager = new SelectedModelManager();

            SharedInformationManager.ReceiptInformationManager = new ReceiptInformationManager();

            SharedInformationManager.DisplayFunctionManager = new DisplayFunctionManager();

            SharedInformationManager.AllReceiptsInfoManager = new AllReceiptsInfoManager();

            SharedInformationManager.RestorePreviousInfo();
        }
    }
}
