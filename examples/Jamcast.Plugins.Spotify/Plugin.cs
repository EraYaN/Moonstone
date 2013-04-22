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

using Jamcast.Extensibility;
using Jamcast.Extensibility.ContentDirectory;
using Jamcast.Plugins.Spotify.API;
using Jamcast.Plugins.Spotify.Renderers;

namespace Jamcast.Plugins.Spotify {

    public class Plugin : IContentDirectoryPlugin {

        internal static string LOG_MODULE = "Spotify";        

        public Type ConfigurationPanelType {

            get { return typeof(SpotifyPanel); }

        }

        public string DisplayName {

            get { return "Spotify"; }
            
        }

        public Type RootObjectRendererType {

            get { return typeof(Root); }

        }        

        public ObjectRenderInfo[] GetPromotedPlaylists() {

            List<ObjectRenderInfo> ret = new List<ObjectRenderInfo>();

            ret.Add(new ObjectRenderInfo(typeof(InboxContainer)));
            ret.Add(new ObjectRenderInfo(typeof(StarredContainer)));

            List<PlaylistContainer.PlaylistInfo> playlists = Jamcast.Plugins.Spotify.API.Spotify.GetAllSessionPlaylists();

            if (playlists != null) {

                foreach(PlaylistContainer.PlaylistInfo playlist in playlists) {

                    using (Playlist p = Jamcast.Plugins.Spotify.API.Spotify.GetPlaylist(playlist.Pointer, false)) {

                        ret.Add(new ObjectRenderInfo(typeof(PlaylistObj), p.Pointer));
                        
                    }

                }

            }

            return ret.ToArray();

        }

        public bool Startup() {

            if (!Configuration.Instance.IsEnabled) {

                Log.Info(Plugin.LOG_MODULE, "Plugin is disabled.");
                return false;

            }

            try {

                Jamcast.Plugins.Spotify.API.Spotify.Initialize();
                
            } catch (Exception ex) {

                Log.Error(LOG_MODULE, "Could not initialize the Spotify interface.", ex);
                return false;

            }

            if(String.IsNullOrEmpty(Configuration.Instance.Username.Trim()) ||
                String.IsNullOrEmpty(Configuration.Instance.Password.Trim()) ||
                Configuration.Instance.ApplicationKey == null)
                return false;            
            
            try {

                if (!Jamcast.Plugins.Spotify.API.Spotify.Login(Configuration.Instance.ApplicationKey, Configuration.Instance.Username, Configuration.Instance.Password)) {

                    Log.Error(LOG_MODULE, "Spotify login failed for user {0}.", Configuration.Instance.Username);
                    return false;

                }

            } catch {

                Log.Error(Plugin.LOG_MODULE, "Spotify login error for user {0}.", Configuration.Instance.Username);
                return false;

            }

            Log.Info(Plugin.LOG_MODULE, "Spotify plugin initialized successfully.");
            return true;

        }

        public void Shutdown() {

            Jamcast.Plugins.Spotify.API.Spotify.ShutDown();
            Log.Debug(Plugin.LOG_MODULE, "Spotify was shut down successfully", null);

        }

    }

}
