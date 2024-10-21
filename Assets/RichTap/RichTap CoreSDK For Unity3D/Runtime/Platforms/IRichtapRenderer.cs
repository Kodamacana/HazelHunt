using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Platforms
{
    public interface IRichtapRenderer
    {

        #region Basic infomation about device.
        string DeviceName();
        string Description();
        string Version();
        #endregion

        #region Renderer setting
        bool Init();

        bool Quit();
        #endregion


        #region Renderer rendering
        bool IsRichtapAvailable();

        void RenderRichtapEffect();
        #endregion
    }
}
