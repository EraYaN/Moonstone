/*-
 * Copyright (c) 2012 Software Development Solutions, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Jamcast.Extensibility.MediaServer;
using Jamcast.Plugins.Spotify.API;

namespace Jamcast.Plugins.Spotify {

    [MediaRequestHandler("530A1876-2C92-449c-87A0-EE18D30D36F3")]
    public class AlbumArtHandler : MediaRequestHandler {

        byte[] _buffer;

        public override RequestInitializationResult Initialize() {
            
            IntPtr albumPtr = (IntPtr)Convert.ToInt32(this.Context.Data[0]);
            
            _buffer = Jamcast.Plugins.Spotify.API.Spotify.GetAlbumArt(albumPtr);

            RequestInitializationResult result = new RequestInitializationResult();
            result.CanProceed = true;
            result.IsConversion = false;
            result.SupportsSeeking = false;
            result.TotalBytes = _buffer.Length;

            return result;

        }

        public override DataPipeBase RetrieveMedia() {

            MemoryStream stream = new MemoryStream(_buffer);
            StreamDataPipe pipe = new StreamDataPipe("Spotify Album Art", stream);            
            return pipe;

        }
    }
}
