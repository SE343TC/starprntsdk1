using StarMicronics.StarIOExtension;

namespace StarPRNTSDK
{
    public static class ScaleFunctions
    {
        public static byte[] CreateZeroClear()
        {
            IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS10);
            //IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS12);
            //IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS20);

            builder.AppendZeroClear();

            return builder.PassThroughCommands;
        }

        public static byte[] CreateUnitChange()
        {
            IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS10);
            //IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS12);
            //IScaleCommandBuilder builder = StarIoExt.CreateScaleCommandBuilder(ScaleModel.APS20);

            builder.AppendUnitChange();

            return builder.PassThroughCommands;
        }
    }
}
