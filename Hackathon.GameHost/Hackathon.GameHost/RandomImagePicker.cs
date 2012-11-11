using System;
using Hackathon.GameHost.Domain;

namespace Hackathon.GameHost
{
    public class RandomImagePicker : IImagePicker
    {
        #region Fields

        private readonly string[] images =
            new[]
                {
                    "100Amerihealth.JPG",
                    "142Ameritas.jpg",
                    "142RelianceStandard_bw.jpg",
                    "142TheStandard_color_tag.jpg",
                    "186PriorityHealth.jpg",
                    "247_ods.gif",
                    "263LovelaceHP.jpg",
                    "29BCBSFlorida.gif",
                    "31UnitedHealthcare_Black.jpg",
                    "358_NPP.jpg",
                    "358_UHN.jpg",
                    "36BCBSTennessee.gif",
                    "37PacificSource.jpg",
                    "46_PHP_KC.jpg",
                    "49_ConnectionDental.jpg",
                    "49_PPOUSA.jpg",
                    "51Aetna.gif",
                    "53HealthNet.gif",
                    "85Excellus.jpg",
                    "Asuris.jpg",
                    "bc1hrgbpbbkw.jpg",
                    "BCBS.jpg",
                    "BCBSM_blue.jpg",
                    "BCBSM_blue2.jpg",
                    "BCBSM_Logo_Transparent.gif",
                    "bcnepa_80.jpg",
                    "CDPHP_3405_rgb_.jpg",
                    "IPN-Seal.jpg",
                    "IPNLogo.jpg",
                    "MagnaCare.GIF",
                    "MC-logo-new-4inches-PMS294.jpg",
                    "MM FOC Graphic (RGB_150dpi).jpg",
                    "omega_logo.jpg",
                    "PSHP logo_fin_v_4c .jpg",
                    "PSHP logo_fin_v_4c.jpg",
                    "RegenceBlueCrossBlueShield.jpg",
                    "RegenceBlueShield.jpg",
                };

        #endregion

        public ImageData Pick()
        {
            var random = new Random();
            var index = random.Next(images.Length);

            var url = "https://strenuus.com/N360/images/custom/" + images[index];

            return new ImageData {Url = url};
        }
    }
}
