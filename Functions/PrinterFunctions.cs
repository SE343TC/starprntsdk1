using StarMicronics.StarIOExtension;
using System.Drawing;

namespace StarPRNTSDK
{
    public class PrinterFunctions
    {
        public static byte[] CreateTextReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt, bool utf8)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            localizeReceipt.AppendTextReceiptData(builder, utf8);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateRasterReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            Bitmap rasterImage = localizeReceipt.CreateRasterReceiptImage();

            builder.AppendBitmap(rasterImage, false);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateScaleRasterReceiptData(Emulation emulation, LocalizeReceipt localizeReceipt, int width, bool bothScale)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            Bitmap rasterImage = localizeReceipt.CreateScaleRasterReceiptImage();

            builder.AppendBitmap(rasterImage, false, width, bothScale);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateCouponData(Emulation emulation, LocalizeReceipt localizeReceipt, int width, BitmapConverterRotation rotation)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            Bitmap rasterImage = localizeReceipt.CreateCouponImage();

            builder.AppendBitmap(rasterImage, false, width, true, rotation);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateTextBlackMarkData(Emulation emulation, LocalizeReceipt localizeReceipt, BlackMarkType type, bool utf8)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            builder.AppendBlackMark(type);

            localizeReceipt.AppendTextLabelData(builder, utf8);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreatePasteTextBlackMarkData(Emulation emulation, LocalizeReceipt localizeReceipt, string pasteText, bool doubleHeight, BlackMarkType type, bool utf8)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            builder.AppendBlackMark(type);

            if (doubleHeight)
            {
                builder.AppendMultipleHeight(2);

                localizeReceipt.AppendPasteTextLabelData(builder, pasteText, utf8);

                builder.AppendMultipleHeight(1);
            }
            else
            {
                localizeReceipt.AppendPasteTextLabelData(builder, pasteText, utf8);
            }

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateTextPageModeData(Emulation emulation, LocalizeReceipt localizeReceipt, Rectangle printRegion, BitmapConverterRotation rotation, bool utf8)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            builder.BeginPageMode(printRegion, rotation);

            localizeReceipt.AppendTextLabelData(builder, utf8);

            builder.EndPageMode();

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }

        public static byte[] CreateFileOpenData(Emulation emulation, string filePath, int paperSize)
        {
            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(emulation);

            builder.BeginDocument();

            Bitmap rasterImage = (Bitmap)Image.FromFile(filePath);

            builder.AppendBitmap(rasterImage, true, paperSize, true);

            builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

            builder.EndDocument();

            return builder.Commands;
        }
    }
}
