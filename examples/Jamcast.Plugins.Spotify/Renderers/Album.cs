﻿/*-
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
using System.Text;

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;
using Jamcast.Plugins.Spotify.API;

namespace Jamcast.Plugins.Spotify.Renderers {

    [ObjectRenderer(ServerObjectType.Album)]
    public class Album : ContainerRenderer {
        
        public override void GetChildren(int startIndex, int count, out int totalMatches) {

            IntPtr[] tracks = Jamcast.Plugins.Spotify.API.Spotify.GetAlbumTracks((IntPtr)this.ObjectData);

            if (tracks == null) {

                totalMatches = 0;
                return;

            }

            totalMatches = tracks.Length;
            count = Math.Min(count, tracks.Length - startIndex);
           
            for (int i = 0; i < count; i++) {

                this.CreateChildObject<TrackObj>(tracks[i + startIndex]);

            }

        }

        public override ServerObject GetMetadata() {

            using (Jamcast.Plugins.Spotify.API.Album album = new Jamcast.Plugins.Spotify.API.Album((IntPtr)this.ObjectData)) {

                //TODO: album art

                return new AlbumContainer(album.Name, album.Artist, null);

            }

        }

    }
    
}
