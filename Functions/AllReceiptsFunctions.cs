using StarMicronics.SMCloudServicesSolution;
using StarMicronics.StarIOExtension;
using System.Drawing;
using System.Windows.Forms;

namespace StarPRNTSDK
{
    public static class AllReceiptsFunctions
    {
        public static byte[] CreateRasterReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt, int width, bool receipt, bool info, bool qrCode)
        {
            if (!receipt && !info && !qrCode)
            {
                return null;
            }

            Bitmap image = localizeReceipt.CreateRasterReceiptImage();

            byte[] urlData = SMCSAllReceipts.UploadBitmap(image);

            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            if (receipt)
            {
                builder.AppendBitmap(image, false);
            }

            byte[] allReceiptsData;
            if (emulation == Emulation.StarGraphic)
            {
                allReceiptsData = SMCSAllReceipts.GenerateAllReceipts(urlData, emulation, info, qrCode, width); // Support to centering in Star Graphic.
            }
            else
            {
                allReceiptsData = SMCSAllReceipts.GenerateAllReceipts(urlData, emulation, info, qrCode);// Non support to centering in Star Graphic.
            }

            builder.AppendRaw(allReceiptsData);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateScaleRasterReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt, int width, bool bothScale, bool receipt, bool info, bool qrCode)
        {
            Bitmap image = localizeReceipt.CreateScaleRasterReceiptImage();

            byte[] urlData = SMCSAllReceipts.UploadBitmap(image);

            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            if (receipt)
            {
                builder.AppendBitmap(image, false, width, bothScale);
            }

            byte[] allReceiptsData;
            if (emulation == Emulation.StarGraphic)
            {
                allReceiptsData = SMCSAllReceipts.GenerateAllReceipts(urlData, emulation, info, qrCode, width); // Support to centering in Star Graphic.
            }
            else
            {
                allReceiptsData = SMCSAllReceipts.GenerateAllReceipts(urlData, emulation, info, qrCode);// Non support to centering in Star Graphic.
            }

            builder.AppendRaw(allReceiptsData);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateTextReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt, int width, bool utf8, bool receipt, bool info, bool qrCode)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            localizeReceipt.AppendTextReceiptData(builder, utf8);

            builder.EndDocument();

            byte[] receiptsData = builder.Commands;

            byte[] urlData = SMCSAllReceipts.UploadData(receiptsData, emulation, localizeReceipt.CharacterCode, width);

            ICommandBuilder printDataBuilder = StarIoExt.CreateCommandBuilder(emulation);

            printDataBuilder.BeginDocument();

            if (receipt)
            {
                localizeReceipt.AppendTextReceiptData(printDataBuilder, utf8);
            }

            byte[] allReceiptsData = SMCSAllReceipts.GenerateAllReceipts(urlData, emulation, info, qrCode);

            printDataBuilder.AppendRaw(allReceiptsData);

            printDataBuilder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            printDataBuilder.EndDocument();

            return printDataBuilder.Commands;
        }

        public static void SMCSAllReceipts_UpdatedStatusResultEvent(object sender, RequestResult e)
        {
            if(!e.ErrorMessage.Equals(""))
            {
                NotifyResult(e);
            }
        }

        public static void SMCSAllReceipts_UploadedBitmapResultEvent(object sender, RequestResult e)
        {
            NotifyResult(e);
        }

        public static void SMCSAllReceipts_UploadedDataResultEvent(object sender, RequestResult e)
        {
            NotifyResult(e);
        }

        public static void SMCloudServices_RegisterResultEvent(object sender, RegisteredResult e)
        {
            bool isRegistered = e.IsRegistered;

            Util.NotifyAllReceiptsIsRegisteredStatusChanged();
        }

        public static void NotifyResult(RequestResult e)
        {
            string resultMessage = null;
            ToolTipIcon icon;

            if (!e.ErrorMessage.Equals(""))
            {
                resultMessage = e.ErrorMessage;
                icon = ToolTipIcon.Error;
            }
            else
            {
                resultMessage = "Status Code : " + e.StatusCode.ToString();
                icon = ToolTipIcon.Info;
            }

            NotifyIconForm notifyIconForm = new NotifyIconForm();
            notifyIconForm.ShowNotify("", resultMessage, icon);
        }
    }
}
