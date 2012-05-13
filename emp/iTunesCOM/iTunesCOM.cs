using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;

namespace EMP
{
    public class iTunesCOM
    {
        iTunesApp app = new iTunesApp();
        public iTunesCOM()
        { 

        }
        public string GetState()
        {
            ITPlayerState playerState = app.PlayerState;
            return playerState.ToString();
        }        
    }
}
