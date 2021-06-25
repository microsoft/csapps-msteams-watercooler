

namespace WaterCoolerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserData
    {
        /// <summary>
        /// Gets or sets the isFirstLogin.
        /// </summary>
        public bool IsFirstLogin { get; set; }

        /// <summary>
        /// Gets or sets the TermsofUseText.
        /// </summary>
        public string TermsofUseText { get; set; }

        /// <summary>
        /// Gets or sets the TermsofUseUrl.
        /// </summary>
        public string TermsofUseUrl { get; set; }
    }
}
