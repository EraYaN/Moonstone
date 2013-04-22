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
using System.Text;
using System.Threading;

using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Extensibility.Metadata;
using Jamcast.Plugins.Spotify.API;

using libspotifydotnet;

namespace Jamcast.Plugins.Spotify.Renderers {

    [ObjectRenderer(ServerObjectType.GenericContainer)]
    class Playlists : ContainerRenderer {
        
        public override void GetChildren(int startIndex, int count, out int totalMatches) {

            List<PlaylistContainer.PlaylistInfo> children = null;

            if (this.ObjectData.ToString().Equals("Playlists", StringComparison.InvariantCultureIgnoreCase)) {

                children = Jamcast.Plugins.Spotify.API.Spotify.GetPlaylists(null);            

            } else {
                
                children = Jamcast.Plugins.Spotify.API.Spotify.GetPlaylists((PlaylistContainer.PlaylistInfo)this.ObjectData);

            }

            if (children == null || children.Count == 0) {

                totalMatches = 0;
                return;

            }

            totalMatches = children.Count;
            count = Math.Min(children.Count - startIndex, count);

            for (int i = 0; i < count; i++) {

                if (children[i + startIndex].PlaylistType == libspotify.sp_playlist_type.SP_PLAYLIST_TYPE_PLAYLIST) {

                    using (Playlist p = Jamcast.Plugins.Spotify.API.Spotify.GetPlaylist(children[i + startIndex].Pointer, false)) {

                        this.CreateChildObject<PlaylistObj>(p.Pointer);

                    }

                } else {

                    this.CreateChildObject<Playlists>(children[i + startIndex]);

                }

            }
            
        }

        public override ServerObject GetMetadata() {

            if (this.ObjectData.Equals("Playlists")) {

                return new GenericContainer("Playlists");

            } else {

                return new GenericContainer(((PlaylistContainer.PlaylistInfo)this.ObjectData).Name);

            }

        }

    }
    
}
