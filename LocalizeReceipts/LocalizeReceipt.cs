using StarMicronics.StarIOExtension;
using System.Drawing;

namespace StarPRNTSDK
{
    public abstract class LocalizeReceipt
    {
        public ReceiptInformationManager ReceiptInformationManager { get; set; }

        public CharacterCode CharacterCode { get; protected set; }

        public virtual Font RasterReceiptFont { get; protected set; }

        public PaperSize PaperSize
        {
            get
            {
                return ReceiptInformationManager.PaperSize;
            }
        }

        public static LocalizeReceipt CreateLocalizeReceipt(ReceiptInformationManager receiptInformationManager)
        {
            LocalizeReceipt localizeReceipts = null;

            switch (receiptInformationManager.LanguageType)
            {
                case Language.LanguageType.English:
                    localizeReceipts = new EnglishReceipt();
                    break;

                case Language.LanguageType.Japanese:
                    localizeReceipts = new JapaneseReceipt();
                    break;

                case Language.LanguageType.French:
                    localizeReceipts = new FrenchReceipt();
                    break;

                case Language.LanguageType.Portuguese:
                    localizeReceipts = new PortugueseReceipt();
                    break;

                case Language.LanguageType.Spanish:
                    localizeReceipts = new SpanishReceipt();
                    break;

                case Language.LanguageType.German:
                    localizeReceipts = new GermanReceipt();
                    break;

                case Language.LanguageType.Russian:
                    localizeReceipts = new RussianReceipt();
                    break;

                case Language.LanguageType.SimplifiedChinese:
                    localizeReceipts = new SimplifiedChineseReceipt();
                    break;

                case Language.LanguageType.TraditionalChinese:
                    localizeReceipts = new TraditionalChineseReceipt();
                    break;
            }

            localizeReceipts.ReceiptInformationManager = receiptInformationManager;

            return localizeReceipts;
        }

        public void AppendTextReceiptData(ICommandBuilder builder, bool utf8)
        {
            switch (PaperSize.Type)
            {
                default:
                case PaperSize.PaperSizeType.TwoInch:
                    Append2inchTextReceiptData(builder, utf8);
                    break;

                case PaperSize.PaperSizeType.ThreeInch:
                    switch (SharedInformationManager.GetSelectedEmulation())
                    {
                        case Emulation.EscPos:
                        case Emulation.EscPosMobile:
                            AppendEscPos3inchTextReceiptData(builder, utf8);
                            break;

                        case Emulation.StarDotImpact:
                            AppendDotImpact3inchTextReceiptData(builder, utf8);
                            break;

                        default:
                            Append3inchTextReceiptData(builder, utf8);
                            break;
                    }
                    break;

                case PaperSize.PaperSizeType.FourInch:
                    Append4inchTextReceiptData(builder, utf8);
                    break;
            }
        }

        public Bitmap CreateRasterReceiptImage()
        {
            Bitmap bitmap;

            switch (PaperSize.Type)
            {
                default:
                case PaperSize.PaperSizeType.TwoInch:
                    bitmap = Create2inchRasterImage();
                    break;

                case PaperSize.PaperSizeType.ThreeInch:
                    switch (SharedInformationManager.GetSelectedEmulation())
                    {
                        case Emulation.EscPos:
                        case Emulation.EscPosMobile:
                            bitmap = CreateEscPos3inchRasterImage();
                            break;

                        case Emulation.StarDotImpact:
                            bitmap = CreateCouponImage();
                            break;

                        default:
                            bitmap = Create3inchRasterImage();
                            break;
                    }

                    break;

                case PaperSize.PaperSizeType.FourInch:
                    bitmap = Create4inchRasterImage();
                    break;
            }

            return bitmap;
        }

        /// <summary>
        /// Get source image to scale.
        /// 3inch → 2inch
        /// 4inch → 3inch
        /// 3inch → 4inch
        /// </summary>
        public Bitmap CreateScaleRasterReceiptImage()
        {
            PaperSize.Scale = true;

            Bitmap rasterImage = CreateRasterReceiptImage();

            PaperSize.Scale = false;

            return rasterImage;
        }

        public Bitmap Create2inchRasterImage()
        {
            string sourceString = Create2inchBitmapSourceText();

            Bitmap rasterImage = CreateBitmapFromString(sourceString);

            return rasterImage;
        }

        public Bitmap Create3inchRasterImage()
        {
            string sourceString = Create3inchBitmapSourceText();

            Bitmap rasterImage = CreateBitmapFromString(sourceString);

            return rasterImage;
        }

        public Bitmap CreateEscPos3inchRasterImage()
        {
            string sourceString = CreateEscPos3inchBitmapSourceText();

            Bitmap rasterImage = CreateBitmapFromString(sourceString);

            return rasterImage;
        }

        public Bitmap Create4inchRasterImage()
        {
            string sourceString = Create4inchBitmapSourceText();

            Bitmap rasterImage = CreateBitmapFromString(sourceString);

            return rasterImage;
        }

        private Bitmap CreateBitmapFromString(string sourceString)
        {
            Font printFont = RasterReceiptFont;

            StringFormat format = new StringFormat();

            float yPos = 0;
            int count = 0;
            float leftMargin = 0;
            float topMargin = 0;

            SizeF bitmapSize = CaluculateBitmapSize(sourceString, printFont);
            Bitmap bitmap = new Bitmap((int)bitmapSize.Width, (int)(bitmapSize.Height + topMargin));
            Graphics graphics = Graphics.FromImage(bitmap);

            string[] lines = sourceString.Split('\n');
            foreach (string line in lines)
            {
                yPos = topMargin + (count * printFont.GetHeight(graphics));
                graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, format);
                count++;
            }

            graphics.Dispose();
            printFont.Dispose();

            return bitmap;
        }

        private static SizeF CaluculateBitmapSize(string sourceString, Font printFont)
        {
            SizeF stringSize = new SizeF();
            float width = 0;
            float height = 0;
            Bitmap bitmap = new Bitmap(2000, 2000);
            Graphics graphics = Graphics.FromImage(bitmap);

            int count = 0;

            string[] lines = sourceString.Split('\n');
            foreach (string line in lines)
            {
                stringSize = graphics.MeasureString(line, printFont, 2000);
                if (stringSize.Width > width)
                {
                    width = stringSize.Width;
                }

                height = count * printFont.GetHeight(graphics);
                count++;
            }

            return new SizeF(width, height);
        }

        public string CreateRasterImageText()
        {
            string rasterImageText;

            switch (PaperSize.Type)
            {
                default:
                case PaperSize.PaperSizeType.TwoInch:
                    rasterImageText = Create2inchBitmapSourceText();
                    break;

                case PaperSize.PaperSizeType.ThreeInch:
                    switch (SharedInformationManager.GetSelectedEmulation())
                    {
                        case Emulation.EscPos:
                        case Emulation.EscPosMobile:
                        case Emulation.StarDotImpact:
                            rasterImageText = CreateEscPos3inchBitmapSourceText();
                            break;

                        default:
                            rasterImageText = Create3inchBitmapSourceText();
                            break;
                    }
                    break;

                case PaperSize.PaperSizeType.FourInch:
                    rasterImageText = Create4inchBitmapSourceText();
                    break;
            }

            return rasterImageText;
        }

        public abstract void Append2inchTextReceiptData(ICommandBuilder builder, bool utf8);

        public abstract void Append3inchTextReceiptData(ICommandBuilder builder, bool utf8);

        public abstract void Append4inchTextReceiptData(ICommandBuilder builder, bool utf8);

        public abstract string Create2inchRasterReceiptText();

        public abstract string Create3inchRasterReceiptText();

        public abstract string Create4inchRasterReceiptText();

        public abstract string Create2inchBitmapSourceText();

        public abstract string Create3inchBitmapSourceText();

        public abstract string CreateEscPos3inchBitmapSourceText();

        public abstract string Create4inchBitmapSourceText();

        public abstract Bitmap CreateCouponImage();

        public abstract void AppendEscPos3inchTextReceiptData(ICommandBuilder builder, bool utf8);

        public abstract void AppendDotImpact3inchTextReceiptData(ICommandBuilder builder, bool utf8);

        public abstract void AppendTextLabelData(ICommandBuilder builder, bool utf8);

        public abstract string CreatePasteTextLabelString();

        public abstract void AppendPasteTextLabelData(ICommandBuilder builder, string pasteText, bool utf8);
    }
}
